using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.TagHelpers.RichEdit;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Playwright;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class ImageBrowserUiTests
	{
		[SkippableTheory]
		[InlineData(200, false)]
		[InlineData(200, true)]
		[InlineData(400, false)]
		[InlineData(401, false)]
		[InlineData(422, false)]
		[InlineData(413, false)]
		[InlineData(502, false)]
		public async Task AddFolder_ServerResponse_ShowsSuccessOrHelpfulError(int status, bool nested)
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
			var page = await browser.NewPageAsync();
			var succeeds = status != 400 && status != 401;
			var created = false;
			var uploaded = false;
			string uploadBody = null;
			string uploadUrl = null;
			string requestBody = null;
			Func<IRoute, Task> routeHandler = async route =>
			{
				if (route.Request.Url.Contains("/api/image/upload"))
				{
					uploadUrl = route.Request.Url;
					uploadBody = route.Request.PostData;
					uploaded = status == 200;
					await route.FulfillAsync(status == 413 || status == 502
						? new RouteFulfillOptions { Status = status, ContentType = "text/html", Body = "<!DOCTYPE html><title>Error</title>" }
						: new RouteFulfillOptions { ContentType = "application/json", Body = uploaded
							? "{\"success\":true,\"files\":[\"/content/binary/New-folder/photo.png\"]}"
							: "{\"success\":false,\"message\":\"The upload directory is not writable.\"}" });
				}
				else if (route.Request.Url.Contains("/api/image/delete-folder"))
				{
					Assert.Contains("__RequestVerificationToken=test-token", route.Request.PostData);
					created = false;
					await route.FulfillAsync(new RouteFulfillOptions { ContentType = "application/json", Body = "{\"success\":true,\"message\":\"Folder deleted: New-folder\"}" });
				}
				else if (route.Request.Url.Contains("/api/image/create-folder"))
				{
					requestBody = route.Request.PostData;
					created = succeeds;
					await route.FulfillAsync(succeeds
						? new RouteFulfillOptions { ContentType = "application/json", Body = "{\"success\":true,\"data\":{\"code\":220}}" }
						: new RouteFulfillOptions { Status = status, ContentType = "text/html", Body = "<!DOCTYPE html><title>Error</title>" });
				}
				else if (route.Request.Url.Contains("/api/image/list"))
				{
					var inNestedFolder = Uri.UnescapeDataString(route.Request.Url).Contains("Open-Live-Writer");
					var path = inNestedFolder ? "/Open-Live-Writer/" : "/";
					var folders = inNestedFolder ? (created ? "[\"..\",\"New-folder\"]" : "[\"..\"]") : (created && !nested ? "[\"Open-Live-Writer\",\"New-folder\"]" : "[\"Open-Live-Writer\"]");
					if (Uri.UnescapeDataString(route.Request.Url).Contains("New-folder"))
					{
						path += "New-folder/";
						folders = "[\"..\"]";
					}
					var files = uploaded ? "[{\"file\":\"photo.png\",\"name\":\"photo.png\",\"type\":\"image\",\"thumb\":\"photo.png\",\"isImage\":true}]" : "[]";
					await route.FulfillAsync(new RouteFulfillOptions { ContentType = "application/json", Body = "{\"success\":true,\"data\":{\"sources\":[{\"name\":\"default\",\"baseurl\":\"/content/binary/\",\"path\":\"" + path + "\",\"files\":" + files + ",\"folders\":" + folders + "}],\"code\":220}}" });
				}
				else
				{
					await route.FulfillAsync(new RouteFulfillOptions { ContentType = "text/html", Body = "<form><textarea id='editor'></textarea><input name='__RequestVerificationToken' value='test-token'></form>" });
				}
			};
			await page.RouteAsync("http://image-test.local/**", routeHandler);
			await page.RouteAsync("http://configured-site.local/**", routeHandler);
			await page.GotoAsync("http://image-test.local/");
			await page.AddScriptTagAsync(new PageAddScriptTagOptions { Url = "https://cdn.jsdelivr.net/npm/jodit@4/es2021/jodit.min.js" });
			await InitializeEditorAsync(page);
			await page.EvaluateAsync("() => { window.dasBlogEditor.filebrowser.open(); }");
			if (nested)
			{
				await page.GetByText("Open-Live-Writer", new PageGetByTextOptions { Exact = true }).DblClickAsync();
				await page.GetByText("..", new PageGetByTextOptions { Exact = true }).WaitForAsync();
			}
			await page.GetByText("Add folder", new PageGetByTextOptions { Exact = true }).ClickAsync();
			await page.Locator("input[placeholder='type dir name']").FillAsync("New-folder");
			await page.RunAndWaitForResponseAsync(
				() => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Ok", Exact = true }).ClickAsync(),
				response => response.Url.Contains("/api/image/create-folder"));
			if (succeeds)
			{
				await page.GetByText("New-folder", new PageGetByTextOptions { Exact = true }).DblClickAsync();
				await page.WaitForFunctionAsync("() => window.dasBlogEditor.filebrowser.state.currentPath.includes('/New-folder')");
				await page.RunAndWaitForResponseAsync(
					() => page.Locator("input[type=file]").SetInputFilesAsync(new FilePayload
					{
						Name = "photo.png",
						MimeType = "image/png",
						Buffer = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aS1sAAAAASUVORK5CYII=")
					}),
					response => response.Url.Contains("/api/image/upload"));
				if (uploaded)
				{
					await page.GetByText("photo.png", new PageGetByTextOptions { Exact = true }).WaitForAsync();
					await page.EvaluateAsync("() => window.dasBlogEditor.filebrowser.close()");
					await page.RunAndWaitForResponseAsync(
						() => page.EvaluateAsync("() => { const transfer = new DataTransfer(); transfer.items.add(new File([new Uint8Array([1])], 'photo.png', {type: 'image/png'})); window.dasBlogEditor.editor.dispatchEvent(new DragEvent('drop', {bubbles: true, cancelable: true, dataTransfer: transfer})); }"),
						response => response.Url.Contains("/api/image/upload"));
					await page.Locator(".jodit-wysiwyg figure img[src='/content/binary/New-folder/photo.png']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Attached });
					Assert.Contains("/content/binary/New-folder/photo.png", await page.EvaluateAsync<string>("() => window.dasBlogEditor.value"));
				}
				else
				{
					var message = status switch
					{
						413 => "The upload exceeds the server size limit. Try a smaller image.",
						502 => "Image upload failed (HTTP 502). Refresh the page and try again.",
						_ => "The upload directory is not writable."
					};
					await page.GetByText(message).WaitForAsync();
					Assert.Equal(0, await page.GetByText("photo.png", new PageGetByTextOptions { Exact = true }).CountAsync());
					await page.GetByText("..", new PageGetByTextOptions { Exact = true }).ClickAsync();
					await page.GetByText("New-folder", new PageGetByTextOptions { Exact = true }).WaitForAsync();
					await page.Locator("a[data-name='New-folder']").GetByRole(AriaRole.Button, new LocatorGetByRoleOptions { Name = "Delete", Exact = true }).ClickAsync();
					await page.RunAndWaitForResponseAsync(
						() => page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = "Yes", Exact = true }).ClickAsync(),
						response => response.Url.Contains("/api/image/delete-folder"));
					await page.Locator("a[data-name='New-folder']").WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Detached });
				}
				Assert.Equal("http://image-test.local/api/image/upload", uploadUrl);
				Assert.Contains("name=\"files\"; filename=\"photo.png\"", uploadBody);
				Assert.Contains(nested ? "/Open-Live-Writer/New-folder" : "/New-folder", uploadBody);
			}
			else
			{
				var message = status == 401 ? "Your session may have expired. Refresh the page and sign in again." : "Folder creation failed (HTTP 400). Refresh the page and try again.";
				await page.GetByText(message).WaitForAsync();
				Assert.Equal(0, await page.GetByText("New-folder", new PageGetByTextOptions { Exact = true }).CountAsync());
			}
			Assert.NotNull(requestBody);
			Assert.Contains("name=New-folder", requestBody);
			Assert.Contains(nested ? "path=%2FOpen-Live-Writer%2F" : "path=%2F", requestBody);
			Assert.Contains("__RequestVerificationToken=test-token", requestBody);
		}

		[SkippableFact]
		public async Task OpenImageBrowser_StoredDeletedFolder_StartsAtBinaryRoot()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
			var page = await browser.NewPageAsync();
			await page.RouteAsync("http://image-test.local/**", async route =>
			{
				if (route.Request.Url.Contains("/api/image/list"))
				{
					await route.FulfillAsync(Uri.UnescapeDataString(route.Request.Url).Contains("deleted-folder")
						? new RouteFulfillOptions { Status = 404, ContentType = "application/json", Body = "{\"success\":false,\"message\":\"The selected directory does not exist\"}" }
						: new RouteFulfillOptions { ContentType = "application/json", Body = "{\"success\":true,\"data\":{\"sources\":[{\"name\":\"default\",\"path\":\"/\",\"baseurl\":\"/content/binary/\",\"files\":[],\"folders\":[\"Open-Live-Writer\"]}]}}" });
				}
				else
				{
					await route.FulfillAsync(new RouteFulfillOptions { ContentType = "text/html", Body = "<form><textarea id='editor'></textarea></form>" });
				}
			});
			await page.GotoAsync("http://image-test.local/");
			await page.AddScriptTagAsync(new PageAddScriptTagOptions { Url = "https://cdn.jsdelivr.net/npm/jodit@4/es2021/jodit.min.js" });
			await page.EvaluateAsync("() => { const previous = Jodit.make('#editor'); previous.filebrowser.storage.set('currentPath', '/deleted-folder/').set('currentSource', 'default'); previous.destruct(); }");
			await InitializeEditorAsync(page);
			Assert.Equal(string.Empty, await page.EvaluateAsync<string>("() => window.dasBlogEditor.filebrowser.state.currentPath"));
			await page.EvaluateAsync("() => window.dasBlogEditor.filebrowser.open()");
			await page.GetByText("Open-Live-Writer", new PageGetByTextOptions { Exact = true }).WaitForAsync();
			Assert.Equal(0, await page.GetByText("The selected directory does not exist", new PageGetByTextOptions { Exact = true }).CountAsync());
			await page.GetByText("Open-Live-Writer", new PageGetByTextOptions { Exact = true }).ClickAsync();
			Assert.Contains("Open-Live-Writer", await page.EvaluateAsync<string>("() => window.dasBlogEditor.filebrowser.state.currentPath"));
			await page.EvaluateAsync("() => { window.dasBlogEditor.filebrowser.close(); return window.dasBlogEditor.filebrowser.open(); }");
			Assert.Equal(string.Empty, await page.EvaluateAsync<string>("() => window.dasBlogEditor.filebrowser.state.currentPath"));
			Assert.Equal("default", await page.EvaluateAsync<string>("() => window.dasBlogEditor.filebrowser.state.currentSource"));
		}

		internal static async Task InitializeEditorAsync(IPage page)
		{
			var builder = new JoditBuilder(new TestUrlResolver());
			var helper = new RichEditScriptsTagHelper(builder) { ControlId = "editor" };
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
			var output = new TagHelperOutput("script", new TagHelperAttributeList(), (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
			builder.ProcessScripts(helper, context, output);
			var markup = output.PostElement.GetContent();
			var start = markup.IndexOf("<script>", StringComparison.Ordinal) + "<script>".Length;
			var script = markup.Substring(start, markup.LastIndexOf("</script>", StringComparison.Ordinal) - start);
			await page.EvaluateAsync("() => { " + script + " document.dispatchEvent(new Event('DOMContentLoaded')); }");
		}

		private sealed class TestUrlResolver : IUrlResolver
		{
			public string GetBaseUrl() => "http://configured-site.local";
			public string RssUrl => throw new NotSupportedException();
			public string PingBackUrl => throw new NotSupportedException();
			public string CategoryUrl => throw new NotSupportedException();
			public string ArchiveUrl => throw new NotSupportedException();
			public string MicroSummaryUrl => throw new NotSupportedException();
			public string RsdUrl => throw new NotSupportedException();
			public string ShortCutIconUrl => throw new NotSupportedException();
			public string ThemeCssUrl => throw new NotSupportedException();
			public string RelativeToRoot(string relative) => throw new NotSupportedException();
			public string GetPermaLinkUrl(string entryId) => throw new NotSupportedException();
			public string GetPermaTitle(string titleurl) => throw new NotSupportedException();
			public string GetTrackbackUrl(string entryId) => throw new NotSupportedException();
			public string GetEntryCommentsRssUrl(string entryId) => throw new NotSupportedException();
			public string GetCommentViewUrl(string entryId) => throw new NotSupportedException();
			public string GetCategoryViewUrl(string category) => throw new NotSupportedException();
			public string GetCategoryViewUrlName(string category) => throw new NotSupportedException();
			public string GetRssCategoryUrl(string category) => throw new NotSupportedException();
			public string GeneratePostUrl(Entry entry) => throw new NotSupportedException();
		}
	}
}
