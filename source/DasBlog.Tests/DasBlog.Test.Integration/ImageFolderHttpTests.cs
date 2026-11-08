using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Web.Controllers;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Playwright;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class ImageFolderHttpTests
	{
		[SkippableTheory]
		[InlineData(false)]
		[InlineData(true)]
		public async Task ImageBrowser_RealHttpRequests_PersistUploadsAndDeleteEmptyFolders(bool useBrowser)
		{
			Skip.If(useBrowser && AreWe.InDockerOrBuildServer);
			var root = Path.Combine(Path.GetTempPath(), "dasblog-image-http-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(root);
			try
			{
				var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> { ["BinariesDir"] = root }).Build();
				var resolver = new DasBlogPathResolver(root, "Test", configuration);
				var binaryService = BinaryDataServiceFactory.GetService(root, new Uri("http://localhost/content/binary/"),
					LoggingDataServiceFactory.GetService(root), CdnManagerFactory.GetService(string.Empty, string.Empty));
				using var server = new TestServer(new WebHostBuilder()
					.ConfigureServices(services =>
					{
						services.AddControllersWithViews().AddApplicationPart(typeof(BlogPostController).Assembly).AddControllersAsServices();
						services.AddAntiforgery();
						services.AddAuthorization();
						services.AddTransient(_ => new BlogPostController(null, null, null, null, null, null, null, new RuntimeBinaryManager(binaryService),
							NullLogger<BlogPostController>.Instance, null, null, null, resolver));
					})
					.Configure(app =>
					{
						app.UseStatusCodePagesWithReExecute("/error");
						app.UseRouting();
						app.Use(async (context, next) =>
						{
							context.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test") }, "test"));
							await next();
						});
						app.UseAuthorization();
						app.UseEndpoints(endpoints =>
						{
							endpoints.MapGet("/token", async context =>
							{
								var token = context.RequestServices.GetRequiredService<IAntiforgery>().GetAndStoreTokens(context).RequestToken;
								await context.Response.WriteAsync(token);
							});
							endpoints.Map("/error", async context =>
							{
								context.Response.ContentType = "text/html";
								await context.Response.WriteAsync("<!DOCTYPE html><title>Error</title>");
							});
							endpoints.MapControllers();
						});
					}));
				using var client = server.CreateClient();
				var tokenResponse = await client.GetAsync("/token");
				var token = await tokenResponse.Content.ReadAsStringAsync();
				var cookie = string.Join("; ", tokenResponse.Headers.GetValues("Set-Cookie"));
				using var request = new HttpRequestMessage(HttpMethod.Post, "/api/image/create-folder");
				request.Headers.Add("Cookie", cookie.Split(';')[0]);
				request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/",
					["name"] = "new-folder",
					["__RequestVerificationToken"] = token
				});
				var response = await client.SendAsync(request);
				var body = await response.Content.ReadAsStringAsync();
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
				using var created = JsonDocument.Parse(body);
				Assert.True(created.RootElement.GetProperty("success").GetBoolean());
				Assert.True(Directory.Exists(Path.Combine(root, "new-folder")));

				using var upload = new HttpRequestMessage(HttpMethod.Post, "/api/image/upload");
				upload.Headers.Add("Cookie", cookie.Split(';')[0]);
				using var multipart = new MultipartFormDataContent();
				var image = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aS1sAAAAASUVORK5CYII=");
				multipart.Add(new ByteArrayContent(image), "files", "photo.png");
				multipart.Add(new StringContent("/new-folder"), "path");
				multipart.Add(new StringContent(token), "__RequestVerificationToken");
				upload.Content = multipart;
				using var uploadedResponse = await client.SendAsync(upload);
				var uploadedBody = await uploadedResponse.Content.ReadAsStringAsync();
				Assert.Equal(HttpStatusCode.OK, uploadedResponse.StatusCode);
				using var uploaded = JsonDocument.Parse(uploadedBody);
				Assert.True(uploaded.RootElement.GetProperty("success").GetBoolean(), uploadedBody);
				Assert.Equal("http://localhost/content/binary/new-folder/photo.png", uploaded.RootElement.GetProperty("files")[0].GetString());
				Assert.Equal(image, await File.ReadAllBytesAsync(Path.Combine(root, "new-folder", "photo.png")));
				client.DefaultRequestHeaders.Add("Cookie", cookie.Split(';')[0]);
				using var listed = JsonDocument.Parse(await client.GetStringAsync("/api/image/list?path=/new-folder"));
				Assert.Equal("photo.png", listed.RootElement.GetProperty("data").GetProperty("sources")[0].GetProperty("files")[0].GetProperty("name").GetString());

				using var nonEmptyDelete = await client.PostAsync("/api/image/delete-folder", new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/", ["name"] = "new-folder", ["__RequestVerificationToken"] = token
				}));
				Assert.Equal(HttpStatusCode.Conflict, nonEmptyDelete.StatusCode);
				Assert.True(File.Exists(Path.Combine(root, "new-folder", "photo.png")));
				Directory.CreateDirectory(Path.Combine(root, "empty"));
				using var invalidDelete = await client.PostAsync("/api/image/delete-folder", new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/", ["name"] = "empty"
				}));
				Assert.Equal(HttpStatusCode.BadRequest, invalidDelete.StatusCode);
				Assert.True(Directory.Exists(Path.Combine(root, "empty")));
				using var emptyDelete = await client.PostAsync("/api/image/delete-folder", new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/", ["name"] = "empty", ["__RequestVerificationToken"] = token
				}));
				Assert.Equal(HttpStatusCode.OK, emptyDelete.StatusCode);
				Assert.False(Directory.Exists(Path.Combine(root, "empty")));
				using var invalidImageDelete = await client.PostAsync("/api/image/delete", new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/new-folder", ["name"] = "photo.png"
				}));
				Assert.Equal(HttpStatusCode.BadRequest, invalidImageDelete.StatusCode);
				Assert.True(File.Exists(Path.Combine(root, "new-folder", "photo.png")));
				using var imageDelete = await client.PostAsync("/api/image/delete", new FormUrlEncodedContent(new Dictionary<string, string>
				{
					["path"] = "/new-folder", ["name"] = "photo.png", ["__RequestVerificationToken"] = token
				}));
				Assert.Equal(HttpStatusCode.OK, imageDelete.StatusCode);
				Assert.False(File.Exists(Path.Combine(root, "new-folder", "photo.png")));

				if (useBrowser)
				{
					using var playwright = await Playwright.CreateAsync();
					await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
					var page = await browser.NewPageAsync();
					await page.RouteAsync("http://image-test.local/**", async route =>
					{
						var url = new Uri(route.Request.Url);
						if (!url.AbsolutePath.StartsWith("/api/image/", StringComparison.Ordinal))
						{
							await route.FulfillAsync(new RouteFulfillOptions { ContentType = "text/html", Body = $"<form><textarea id='editor'></textarea><input name='__RequestVerificationToken' value='{token}'></form>" });
							return;
						}

						using var forwarded = new HttpRequestMessage(new HttpMethod(route.Request.Method), url.PathAndQuery);
						if (route.Request.PostDataBuffer != null)
						{
							forwarded.Content = new ByteArrayContent(route.Request.PostDataBuffer);
							forwarded.Content.Headers.TryAddWithoutValidation("Content-Type", route.Request.Headers["content-type"]);
						}
						using var actualResponse = await client.SendAsync(forwarded);
						await route.FulfillAsync(new RouteFulfillOptions
						{
							Status = (int)actualResponse.StatusCode,
							ContentType = actualResponse.Content.Headers.ContentType?.ToString(),
							BodyBytes = await actualResponse.Content.ReadAsByteArrayAsync()
						});
					});
					await page.GotoAsync("http://image-test.local/");
					await page.AddScriptTagAsync(new PageAddScriptTagOptions { Url = "https://cdn.jsdelivr.net/npm/jodit@4/es2021/jodit.min.js" });
					await ImageBrowserUiTests.InitializeEditorAsync(page);
					await page.EvaluateAsync("() => window.dasBlogEditor.filebrowser.open()");
					await page.GetByText("Add folder", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.Locator("input[placeholder='type dir name']").FillAsync("Browser folder");
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Ok", Exact = true }).ClickAsync();
					await page.GetByText("Browser folder", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.WaitForFunctionAsync("() => window.dasBlogEditor.filebrowser.state.currentPath.includes('Browser folder')");
					var chooser = await page.RunAndWaitForFileChooserAsync(() => page.Locator("input[type=file]").ClickAsync());
					await chooser.SetFilesAsync(new FilePayload { Name = "photo.png", MimeType = "image/png", Buffer = image });
					await page.GetByText("photo.png", new PageGetByTextOptions { Exact = true }).WaitForAsync();
					Assert.Equal(image, await File.ReadAllBytesAsync(Path.Combine(root, "Browser folder", "photo.png")));
					await page.GetByText("..", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.Locator("a[data-name='Browser folder']").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete", Exact = true }).ClickAsync();
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Yes", Exact = true }).ClickAsync();
					await page.GetByText("The folder is not empty. Remove its contents before deleting it.").WaitForAsync();
					Assert.True(File.Exists(Path.Combine(root, "Browser folder", "photo.png")));
					await page.GetByText("Browser folder", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.GetByText("photo.png", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Remove file", Exact = true }).ClickAsync();
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Cancel", Exact = true }).ClickAsync();
					Assert.True(File.Exists(Path.Combine(root, "Browser folder", "photo.png")));
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Remove file", Exact = true }).ClickAsync();
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Yes", Exact = true }).ClickAsync();
					await page.GetByText("photo.png", new PageGetByTextOptions { Exact = true }).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
					Assert.False(File.Exists(Path.Combine(root, "Browser folder", "photo.png")));
					await page.Locator("input[type=file]").SetInputFilesAsync(new FilePayload { Name = "context.png", MimeType = "image/png", Buffer = image });
					await page.GetByText("context.png", new PageGetByTextOptions { Exact = true }).ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
					var contextDelete = await page.RunAndWaitForResponseAsync(
						() => page.GetByText("Delete", new PageGetByTextOptions { Exact = true }).ClickAsync(),
						response => response.Url.Contains("/api/image/delete"));
					Assert.True(contextDelete.Ok, await contextDelete.TextAsync());
					await page.GetByText("context.png", new PageGetByTextOptions { Exact = true }).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
					Assert.False(File.Exists(Path.Combine(root, "Browser folder", "context.png")));
					await page.GetByText("..", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.Locator("a[data-name='new-folder']").WaitForAsync();
					await page.GetByText("Add folder", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.Locator("input[placeholder='type dir name']").FillAsync("Browser empty");
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Ok", Exact = true }).ClickAsync();
					await page.Locator("a[data-name='Browser empty']").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete", Exact = true }).ClickAsync();
					await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Yes", Exact = true }).ClickAsync();
					await page.Locator("a[data-name='Browser empty']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
					Assert.False(Directory.Exists(Path.Combine(root, "Browser empty")));
				}
			}
			finally
			{
				BinaryDataServiceFactory.RemoveService(root);
				Directory.Delete(root, true);
			}
		}

		private sealed class RuntimeBinaryManager(IBinaryDataService service) : IFileSystemBinaryManager
		{
			public string SaveFile(Stream inputFile, string fileName) => service.SaveFile(inputFile, ref fileName);
			public bool SaveSiteConfig(SiteConfig config) => throw new NotSupportedException();
			public bool SaveMetaConfig(MetaTags config) => throw new NotSupportedException();
		}
	}
}
