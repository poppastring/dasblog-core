using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Site;
using DasBlog.Web;
using DasBlog.Web.Controllers;
using HtmlAgilityPack;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class AppPasswordsRenderingTests
	{
		[Fact]
		public async Task Create_MultiplePasswords_GenerateAndRevokeDialogsRemainIndependent()
		{
			const string email = "author@example.com";
			var root = Path.Combine(Path.GetTempPath(), "dasblog-app-password-test-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(root);
			try
			{
				ThemeActivationRenderingTests.PrepareContentRoot(ThemeActivationRenderingTests.FindSourceRoot(), root, "dasblog", false);
				var securityPath = Path.Combine(root, "Config", "siteSecurity.Development.config");
				var security = XDocument.Load(securityPath);
				security.Root.Element("Users").Element("User").SetElementValue("EmailAddress", email);
				security.Save(securityPath);

				using var configuration = (ConfigurationRoot)new ConfigurationBuilder()
					.SetBasePath(root)
					.AddXmlFile("Config/site.config", optional: false, reloadOnChange: true)
					.AddXmlFile("Config/site.Development.config", optional: false, reloadOnChange: true)
					.AddXmlFile("Config/meta.config", optional: false, reloadOnChange: true)
					.Build();
				var paths = new DasBlogPathResolver(root, "Development", configuration);
				using var server = new TestServer(new WebHostBuilder()
					.UseContentRoot(root)
					.UseWebRoot("wwwroot")
					.UseEnvironment("Development")
					.UseSetting(WebHostDefaults.ApplicationKey, typeof(Program).Assembly.GetName().Name)
					.ConfigureAppConfiguration((_, builder) => builder.AddConfiguration(configuration))
					.ConfigureServices((context, services) =>
					{
						services.AddDasBlogConfiguration(configuration, paths);
						services.Configure<TelemetryConfiguration>(options => options.DisableTelemetry = true);
						services.AddDasBlogSecurity(configuration);
						services.AddDasBlogWebServices(configuration);
						services.AddControllersWithViews().AddApplicationPart(typeof(AppPasswordsController).Assembly);
						services.AddDasBlogDataServices();
						services.AddDasBlogManagers();
						services.AddDasBlogServices(context.HostingEnvironment);
					})
					.Configure(app =>
					{
						app.UseRouting();
						app.UseAuthentication();
						app.Use(async (context, next) =>
						{
							context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
							{
								new Claim(ClaimTypes.NameIdentifier, email),
								new Claim(ClaimTypes.Name, email)
							}, "test"));
							await next();
						});
						app.UseAuthorization();
						app.UseHttpContext();
						app.UseDasBlogEndpoints();
					}));
				using var client = server.CreateClient();
				client.BaseAddress = new Uri("http://localhost");
				var url = "/admin/authors/" + email + "/app-passwords";

				using var initial = await client.GetAsync(url);
				Assert.Equal(HttpStatusCode.OK, initial.StatusCode);
				var initialHtml = await initial.Content.ReadAsStringAsync();
				AssertDialogs(initialHtml, Array.Empty<string>());
				var document = new HtmlDocument();
				document.LoadHtml(initialHtml);
				var token = document.DocumentNode.SelectSingleNode("//form[@id='CreateAppPasswordForm']//input[@name='__RequestVerificationToken']")?.GetAttributeValue("value", null);
				Assert.False(string.IsNullOrEmpty(token));
				var cookies = initial.Headers.GetValues("Set-Cookie").Select(value => value.Split(';')[0]);
				client.DefaultRequestHeaders.Add("Cookie", string.Join("; ", cookies));

				foreach (var name in new[] { "Laptop", "Desktop" })
				{
					using var post = await client.PostAsync(url + "/create", new FormUrlEncodedContent(new Dictionary<string, string>
					{
						["__RequestVerificationToken"] = token,
						["NewPasswordName"] = name
					}));
					Assert.Equal(HttpStatusCode.Redirect, post.StatusCode);
					var saved = XDocument.Load(securityPath).Root.Element("Users").Element("User").Element("AppPasswords").Elements("AppPassword").ToList();
					Assert.Contains(saved, password => (string)password.Attribute("name") == name);
					using var page = await client.GetAsync(url);
					Assert.Equal(HttpStatusCode.OK, page.StatusCode);
					AssertDialogs(await page.Content.ReadAsStringAsync(), saved.Select(password => (string)password.Attribute("id")).ToArray());
				}

				var passwords = XDocument.Load(securityPath).Root.Element("Users").Element("User").Element("AppPasswords").Elements("AppPassword").ToList();
				Assert.Equal(2, passwords.Count);
				Assert.NotEqual((string)passwords[0].Attribute("hash"), (string)passwords[1].Attribute("hash"));
				var revokedId = (string)passwords[0].Attribute("id");
				var remainingId = (string)passwords[1].Attribute("id");
				using var revoke = await client.PostAsync(url + "/revoke/" + revokedId, new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["__RequestVerificationToken"] = token
				}));
				Assert.Equal(HttpStatusCode.Redirect, revoke.StatusCode);
				var remaining = XDocument.Load(securityPath).Root.Element("Users").Element("User").Element("AppPasswords").Elements("AppPassword").ToList();
				Assert.Equal(remainingId, (string)Assert.Single(remaining).Attribute("id"));
				using var afterRevoke = await client.GetAsync(url);
				Assert.Equal(HttpStatusCode.OK, afterRevoke.StatusCode);
				AssertDialogs(await afterRevoke.Content.ReadAsStringAsync(), new[] { remainingId });
			}
			finally
			{
				Directory.Delete(root, recursive: true);
			}
		}

		private static void AssertDialogs(string html, string[] ids)
		{
			var document = new HtmlDocument();
			document.LoadHtml(html);
			Assert.Single(document.DocumentNode.SelectNodes("//*[@id='confirmGenerateModal']") ?? new HtmlNodeCollection(null));
			Assert.NotNull(document.DocumentNode.SelectSingleNode("//button[@data-bs-target='#confirmGenerateModal']"));
			Assert.Contains("getElementById('hiddenGenerateButton')", html);
			foreach (var id in ids)
			{
				Assert.Single(document.DocumentNode.SelectNodes($"//*[@id='confirmRevokeModal_{id}']") ?? new HtmlNodeCollection(null));
				Assert.NotNull(document.DocumentNode.SelectSingleNode($"//button[@data-bs-target='#confirmRevokeModal_{id}']"));
				Assert.Contains($"getElementById('hiddenRevokeButton_{id}')", html);
			}
		}
	}
}
