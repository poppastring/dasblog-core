using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;

namespace DasBlog.Test.Integration
{
	[Trait("Category", "SkipWhenLiveUnitTesting")]
	public class TagHelperMigrationSmokeTests : IClassFixture<PlaywrightServerFactory<DasBlog.Web.Program>>, IAsyncLifetime
	{
		private const string PostUrl = "/welcome-to-dasblog-core";

		public PlaywrightServerFactory<DasBlog.Web.Program> Server { get; }
		private IPlaywright _playwright;
		private IBrowser _browser;
		public IPage Page { get; private set; }

		public TagHelperMigrationSmokeTests(PlaywrightServerFactory<DasBlog.Web.Program> server)
		{
			if (AreWe.InDockerOrBuildServer) return;
			Server = server;
			Server.EnsureStarted();
		}

		public async Task InitializeAsync()
		{
			if (AreWe.InDockerOrBuildServer) return;

			_playwright = await Playwright.CreateAsync();
			_browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
			Page = await _browser.NewPageAsync();
			Page.SetDefaultTimeout(30000);
		}

		public async Task DisposeAsync()
		{
			if (Page != null) await Page.CloseAsync();
			if (_browser != null) await _browser.CloseAsync();
			_playwright?.Dispose();
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task PostPage_HasOpenGraphTags()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri + PostUrl);

			await AssertMetaPropertyExists("og:title", "<open-graph />");
			await AssertMetaPropertyExists("og:type", "<open-graph />");
			await AssertMetaPropertyExists("og:url", "<open-graph />");
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task PostPage_HasTwitterCardTags()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri + PostUrl);

			await AssertMetaNameExists("twitter:card", "<twitter-card />");
			await AssertMetaNameExists("twitter:title", "<twitter-card />");
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task PostPage_HasJsonLdBlogPostingSchema()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri + PostUrl);

			var script = Page.Locator("head script[type='application/ld+json']").First;
			var count = await Page.Locator("head script[type='application/ld+json']").CountAsync();
			Assert.True(count > 0,
				$"Expected <script type='application/ld+json'> in <head> on '{PostUrl}' (rendered by <blog-posting-schema />). Found 0.");

			var json = await script.TextContentAsync();
			Assert.False(string.IsNullOrWhiteSpace(json),
				"<blog-posting-schema /> emitted an empty <script type='application/ld+json'>.");

			JsonDocument doc;
			try
			{
				doc = JsonDocument.Parse(json);
			}
			catch (JsonException ex)
			{
				Assert.Fail($"<blog-posting-schema /> emitted invalid JSON-LD: {ex.Message}. Payload: {json}");
				return;
			}

			using (doc)
			{
				Assert.True(doc.RootElement.TryGetProperty("@type", out var typeProp),
					$"JSON-LD payload missing '@type' property. Payload: {json}");
				Assert.Equal("BlogPosting", typeProp.GetString());
			}
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task HomePage_HasCollapsibleCommentsToggle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);

			// The 'Add a comment' collapse toggle is only emitted when ViewData[Constants.ShowPageControl]
			// is true. HomeController sets that to true; BlogPostController (single-post permalink) sets it
			// to false. So we exercise <vc:collapse-comment-block /> via the home page list view.
			await Page.GotoAsync(Server.RootUri + "/");

			var toggle = Page.Locator("a[data-bs-toggle='collapse']", new() { HasText = "Add a comment" });
			var toggleCount = await toggle.CountAsync();
			Assert.True(toggleCount > 0,
				"Expected at least one 'Add a comment' collapse toggle on the home page " +
				"(rendered by <vc:collapse-comment-block /> when ShowPageControl is true). " +
				"Looked for: a[data-bs-toggle='collapse'] with text 'Add a comment'. Found 0.");

			// The toggle's data-bs-target must reference a .collapse container that's also present in the DOM.
			var target = await toggle.First.GetAttributeAsync("data-bs-target");
			Assert.False(string.IsNullOrWhiteSpace(target),
				"<vc:collapse-comment-block /> emitted a toggle anchor without a data-bs-target attribute.");

			var collapseContainerCount = await Page.Locator($"div.collapse{target}").CountAsync();
			Assert.True(collapseContainerCount > 0,
				$"Expected a 'div.collapse{target}' container matching the toggle's data-bs-target on the home page " +
				"(rendered by <vc:collapse-comment-block />). Found 0.");
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task AdminPage_DefaultCredentialsWarning_IsSuppressedInDevelopment()
		{
			Skip.If(AreWe.InDockerOrBuildServer);

			await LoginToSite();

			// Admin pages hard-code the dasblog theme layout, which contains <default-credentials-warning />.
			// DefaultCredentialsCheck.IsUsingDefaults() intentionally returns false in the Development
			// environment (the env this fixture runs under), so the warning banner must NOT render.
			// This test guards against accidental removal of that dev-skip, and implicitly verifies the
			// new <default-credentials-warning /> TagHelper is correctly registered: if it weren't, the
			// admin layout would fail to render and we'd never reach the /admin/settings page below.
			await Page.GotoAsync(Server.RootUri + "/admin/settings");

			// Confirm we actually landed on the admin settings page (and weren't redirected to login etc.).
			Assert.Contains("/admin/settings", Page.Url);

			var banner = Page.Locator(".alert.alert-danger", new() { HasText = "Security Warning" });
			var count = await banner.CountAsync();
			Assert.True(count == 0,
				"Expected <default-credentials-warning /> to be suppressed in the Development environment " +
				"(DefaultCredentialsCheck.IsUsingDefaults() returns false when IHostEnvironment.IsDevelopment() is true). " +
				$"Found {count} 'Security Warning' alert(s) on /admin/settings. " +
				"Either the dev-skip was removed, or the test fixture is no longer running in Development.");
		}

		private async Task AssertMetaPropertyExists(string property, string source)
		{
			var count = await Page.Locator($"head meta[property='{property}']").CountAsync();
			Assert.True(count > 0,
				$"Expected <meta property='{property}'> in <head> on '{PostUrl}' (rendered by {source}). Found 0.");
		}

		private async Task AssertMetaNameExists(string name, string source)
		{
			var count = await Page.Locator($"head meta[name='{name}']").CountAsync();
			Assert.True(count > 0,
				$"Expected <meta name='{name}'> in <head> on '{PostUrl}' (rendered by {source}). Found 0.");
		}

		private async Task LoginToSite()
		{
			await Page.GotoAsync(Server.RootUri + "/account/login");
			await Page.Locator("#EmailLogin").FillAsync("myemail@myemail.com");
			await Page.Locator("#PasswordLogin").FillAsync("admin");
			await Page.Locator("#LoginButton").ClickAsync();
		}
	}
}
