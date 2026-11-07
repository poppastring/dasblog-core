using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Site;
using DasBlog.Web;
using DasBlog.Web.Controllers;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Test.Integration
{
	// In-process MVC/Razor fixture with temporary writable paths; bypasses Program bootstrap.
	public class ThemeActivationRenderingTests
	{
		private const string PostId = "46fa37da-5bc5-4ec4-a989-6e5a8e6a1d0c";
		private const string PostTitle = "Theme activation regression post";
		private readonly ITestOutputHelper output;

		public ThemeActivationRenderingTests(ITestOutputHelper output) => this.output = output;

		[Theory]
		[InlineData("darkly", false)]
		[InlineData("darkly", true)]
		[InlineData("dasblog", false)]
		[InlineData("dasblog", true)]
		[InlineData("flamingo", false)]
		[InlineData("flamingo", true)]
		[InlineData("flatly", false)]
		[InlineData("flatly", true)]
		[InlineData("kindofblue", false)]
		[InlineData("kindofblue", true)]
		[InlineData("median", false)]
		[InlineData("median", true)]
		public async Task SetActive_ImmediateHomeAndXmlReload_Returns200WithSelectedTheme(string theme, bool summary)
		{
			var sourceRoot = FindSourceRoot();
			var temporaryRoot = Path.Combine(Path.GetTempPath(), "dasblog-theme-reproduction-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(temporaryRoot);
			try
			{
				var initialTheme = theme == "darkly" ? "dasblog" : "darkly";
				PrepareContentRoot(sourceRoot, temporaryRoot, initialTheme, summary);
				using var configuration = (ConfigurationRoot)new ConfigurationBuilder()
					.SetBasePath(temporaryRoot)
					.AddXmlFile("Config/site.config", optional: false, reloadOnChange: true)
					.AddXmlFile("Config/site.Development.config", optional: false, reloadOnChange: true)
					.AddXmlFile("Config/meta.config", optional: false, reloadOnChange: true)
					.Build();
				var errors = new ExceptionLoggerProvider();
				var paths = new DasBlogPathResolver(temporaryRoot, "Development", configuration);
				Assert.Equal(Path.Combine(temporaryRoot, "Config", "site.Development.config"), paths.SiteConfigFilePath);
				Assert.StartsWith(temporaryRoot + Path.DirectorySeparatorChar, paths.ContentFolderPath);
				Assert.StartsWith(temporaryRoot + Path.DirectorySeparatorChar, paths.LogFolderPath);

				using var server = new TestServer(new WebHostBuilder()
					.UseContentRoot(temporaryRoot)
					.UseWebRoot("wwwroot")
					.UseEnvironment("Development")
					.UseSetting(WebHostDefaults.ApplicationKey, typeof(Program).Assembly.GetName().Name)
					.ConfigureAppConfiguration((_, builder) => builder.AddConfiguration(configuration))
					.ConfigureServices((context, services) =>
					{
						services.AddDasBlogConfiguration(configuration, paths);
						// No outbound telemetry or scheduled site-email work in this reproducer.
						services.Configure<TelemetryConfiguration>(options => options.DisableTelemetry = true);
						services.AddDasBlogSecurity(configuration);
						services.AddDasBlogWebServices(configuration);
						services.AddControllersWithViews().AddApplicationPart(typeof(HomeController).Assembly);
						services.AddDasBlogDataServices();
						services.AddDasBlogManagers();
						services.AddDasBlogServices(context.HostingEnvironment);
						services.AddLogging(logging => logging.ClearProviders().AddProvider(errors));
					})
					.Configure(app =>
					{
						app.UseRouting();
						app.UseAuthentication();
						app.UseAuthorization();
						app.UseHttpContext();
						app.UseDasBlogEndpoints();
					}));
				using var client = server.CreateClient();
				client.BaseAddress = new Uri("http://localhost");
				var monitor = server.Services.GetRequiredService<IOptionsMonitor<SiteConfig>>();
				var settings = server.Services.GetRequiredService<IDasBlogSettings>();
				Assert.Equal(initialTheme, monitor.CurrentValue.Theme);
				await AssertHome(client, initialTheme, summary, "baseline");

				var beforeActivation = monitor.CurrentValue;
				using (var scope = server.Services.CreateScope())
				{
					var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
					var controller = ActivatorUtilities.CreateInstance<ThemesController>(scope.ServiceProvider);
					controller.ControllerContext = new ControllerContext { HttpContext = httpContext, RouteData = new RouteData(), ActionDescriptor = new ControllerActionDescriptor() };
					controller.TempData = scope.ServiceProvider.GetRequiredService<ITempDataDictionaryFactory>().GetTempData(httpContext);
					// Invoke the exact production action; bypass only authentication/antiforgery transport.
					var redirect = Assert.IsType<RedirectToActionResult>(controller.SetActive(theme));
					Assert.Equal("Index", redirect.ActionName);
					Assert.False(controller.TempData.ContainsKey("ErrorMessage"),
						"ACTIVATION/SAVE FAILURE: " + controller.TempData["ErrorMessage"] + Environment.NewLine + string.Join(Environment.NewLine, errors.Exceptions));
					Assert.Equal($"Active theme set to '{theme}'.", controller.TempData["SuccessMessage"]);
				}

				// No delay, polling, cache clearing, or explicit reload before the first navigation.
				await AssertHome(client, theme, summary, "immediately after SetActive");
				Assert.Equal(theme, settings.SiteConfiguration.Theme);
				var saved = XDocument.Load(paths.SiteConfigFilePath);
				Assert.Equal(theme, saved.Root.Element("Theme").Value);
				Assert.Equal("0", saved.Root.Element("ValidCommentTags").Attribute("name").Value);

				// Explicit reload verifies XML rebinding without depending on watcher timing.
				configuration.Reload();
				Assert.NotSame(beforeActivation, monitor.CurrentValue);
				Assert.Equal(theme, monitor.CurrentValue.Theme);
				Assert.Equal(summary, monitor.CurrentValue.ShowItemSummaryInAggregatedViews);
				Assert.Equal("0", monitor.CurrentValue.ValidCommentTags[0].Name);
				Assert.Same(monitor.CurrentValue, settings.SiteConfiguration);
				await AssertHome(client, theme, summary, "after XML provider reload");
				output.WriteLine($"PASS: {initialTheme} -> {theme}, summary={summary}; baseline/immediate/reloaded home=200; persisted theme and rebound options verified.");
			}
			finally
			{
				// Only this test's unique temporary root is ever deleted.
				Directory.Delete(temporaryRoot, recursive: true);
			}
		}

		private async Task AssertHome(HttpClient client, string theme, bool summary, string stage)
		{
			output.WriteLine($"Rendering {stage}: theme={theme}, summary={summary}");
			using var response = await client.GetAsync("/");
			var html = await response.Content.ReadAsStringAsync();
			Assert.True(response.StatusCode == HttpStatusCode.OK,
				$"{stage}: expected 200, got {(int)response.StatusCode}; body: {html}");
			Assert.Contains("<html", html);
			Assert.Contains(PostTitle, html);
			Assert.Contains("INTRO_MARKER", html);
			// Actual selected theme stylesheet, not just the mutated options object.
			Assert.Contains(theme == "dasblog" ? "/Themes/dasblog/custom.css" : $"/theme/{theme}/custom.css", html);
			if (summary) Assert.DoesNotContain("FULL_BODY_MARKER", html);
			else Assert.Contains("FULL_BODY_MARKER", html);
		}

		private static string FindSourceRoot()
		{
			for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory != null; directory = directory.Parent)
			{
				if (File.Exists(Path.Combine(directory.FullName, "DasBlog.Web", "DasBlog.Web.csproj")))
					return directory.FullName;
			}
			throw new DirectoryNotFoundException("The source-tree reproducer requires source/DasBlog.Web and UnitTests/TestContent.");
		}

		private static void PrepareContentRoot(string sourceRoot, string root, string theme, bool summary)
		{
			var web = Path.Combine(sourceRoot, "DasBlog.Web");
			CopyTree(Path.Combine(web, "Views"), Path.Combine(root, "Views"));
			CopyTree(Path.Combine(web, "Themes"), Path.Combine(root, "Themes"));
			foreach (var directory in new[] { "Config", "content", "content/binary", "logs", "wwwroot" })
				Directory.CreateDirectory(Path.Combine(root, directory));

			// Read-only copy of stock defaults; do not use any environment/developer config.
			var site = XDocument.Load(Path.Combine(web, "Config", "site.config"));
			void Set(string name, string value) => site.Root.SetElementValue(name, value);
			Set("Root", "http://localhost/");
			Set("Theme", theme);
			Set("ContentDir", "content");
			Set("LogDir", "logs");
			Set("BinariesDir", "content/binary");
			Set("EnableStartPageCaching", "false");
			Set("PostPinnedToHomePage", PostId);
			Set("ShowItemSummaryInAggregatedViews", summary ? "true" : "false");
			Set("EnableDailyReportEmail", "false");
			Set("SendCommentsByEmail", "false");
			Set("EnableRewritingHashtagsToCategoryLinks", "false");
			Set("EnableRewritingBareLinksToEmbeddings", "false");
			Set("EnableRewritingBareLinksToIcons", "false");
			Set("AkismetAPIKey", "");
			site.Save(Path.Combine(root, "Config", "site.config"));
			// Pre-exist so SaveSiteConfig exercises AtomicFileWriter's File.Replace path.
			site.Save(Path.Combine(root, "Config", "site.Development.config"));
			File.Copy(Path.Combine(web, "Config", "meta.config"), Path.Combine(root, "Config", "meta.config"));
			File.Copy(Path.Combine(web, "Config", "siteSecurity.config"), Path.Combine(root, "Config", "siteSecurity.Development.config"));

			// Reuse the canonical runtime fixture's shape, with deterministic inert content.
			var day = XDocument.Load(Path.Combine(sourceRoot, "DasBlog.Tests", "UnitTests", "TestContent", "2003-07-31.dayentry.xml"));
			XNamespace ns = "urn:newtelligence-com:dasblog:runtime:data";
			var entries = day.Root.Element(ns + "Entries");
			var post = entries.Elements(ns + "Entry").Single(entry => entry.Element(ns + "EntryId").Value == PostId);
			post.SetElementValue(ns + "Title", PostTitle);
			post.SetElementValue(ns + "Content", "<p>INTRO_MARKER " + string.Join(" ", Enumerable.Repeat("fixture", 120)) + " FULL_BODY_MARKER</p>");
			post.SetElementValue(ns + "AllowComments", "false");
			entries.ReplaceNodes(post);
			day.Save(Path.Combine(root, "content", "2003-07-31.dayentry.xml"));
		}

		// Capture exceptions from the real activation and rendering services.
		private sealed class ExceptionLoggerProvider : ILoggerProvider
		{
			public ConcurrentQueue<Exception> Exceptions { get; } = new ConcurrentQueue<Exception>();
			public ILogger CreateLogger(string categoryName) => new ExceptionLogger(Exceptions);
			public void Dispose() { }

			private sealed class ExceptionLogger : ILogger
			{
				private readonly ConcurrentQueue<Exception> exceptions;
				public ExceptionLogger(ConcurrentQueue<Exception> exceptions) => this.exceptions = exceptions;
				public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
				public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;
				public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
				{
					if (exception != null) exceptions.Enqueue(exception);
				}
			}
		}
		private static void CopyTree(string source, string destination)
		{
			Directory.CreateDirectory(destination);
			foreach (var file in Directory.EnumerateFiles(source))
				File.Copy(file, Path.Combine(destination, Path.GetFileName(file)));
			foreach (var directory in Directory.EnumerateDirectories(source))
				CopyTree(directory, Path.Combine(destination, Path.GetFileName(directory)));
		}
	}
}
