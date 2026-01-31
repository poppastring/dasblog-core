using System;
using System.Net.Http;
using System.Threading.Tasks;
using DasBlog.Web;
using Microsoft.Playwright;
using Xunit;

namespace DasBlog.Test.Integration
{
	[Trait("Category", "SkipWhenLiveUnitTesting")]
	public class PlaywrightPageTests : IClassFixture<PlaywrightServerFactory<Startup>>, IAsyncLifetime
	{
		public HttpClient Client { get; }
		public PlaywrightServerFactory<Startup> Server { get; }
		private IPlaywright _playwright;
		private IBrowser _browser;
		public IPage Page { get; private set; }

		public PlaywrightPageTests(PlaywrightServerFactory<Startup> server)
		{
			Console.WriteLine("In Docker?" + AreWe.InDockerOrBuildServer);
			if (AreWe.InDockerOrBuildServer) return;
			Server = server;
			Client = server.CreateClient();
		}

		public async Task InitializeAsync()
		{
			if (AreWe.InDockerOrBuildServer) return;

			_playwright = await Playwright.CreateAsync();
			_browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
			{
				Headless = true
			});
			Page = await _browser.NewPageAsync();
			Page.SetDefaultTimeout(30000);
		}

		public async Task DisposeAsync()
		{
			if (Page != null)
				await Page.CloseAsync();
			if (_browser != null)
				await _browser.CloseAsync();
			_playwright?.Dispose();
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task LoadTheMainPageAndCheckPageTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri);
			Assert.StartsWith("My DasBlog!", await Page.TitleAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task FrontPageH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri);

			var headerElement = Page.Locator("h2").First;
			Assert.Equal("Welcome to DasBlog Core", await headerElement.TextContentAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task WelcomePostCheckPageBrowserTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");
			Assert.StartsWith("Welcome to DasBlog Core", await Page.TitleAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task WelcomePostH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			var headerElement = Page.Locator("h2").First;
			Assert.Equal("Welcome to DasBlog Core", await headerElement.TextContentAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToWelcomePostThenGoHome()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			var link = Page.GetByRole(AriaRole.Link, new() { Name = "Home" });
			await link.ClickAsync();
			Assert.Equal(Page.Url.TrimEnd('/'), Server.RootUri);
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToPageOneAndBack()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri);

			var link = Page.GetByRole(AriaRole.Link, new() { Name = "<< Older Posts" });
			await link.ClickAsync();
			Assert.Equal(Server.RootUri + "/page/1", Page.Url.TrimEnd('/'));

			var link2 = Page.GetByRole(AriaRole.Link, new() { Name = "Newer Posts >>" });
			await link2.ClickAsync();
			Assert.Equal(Server.RootUri + "/page/0", Page.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToCategory()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/category");
			Assert.StartsWith("Category - My DasBlog!", await Page.TitleAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToSpecificCategoryNavigateToPost()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/category/dasblog-core");
			Assert.StartsWith("Category - My DasBlog!", await Page.TitleAsync());

			var headerElement = Page.Locator("h4").First;
			Assert.Equal("Dasblog Core (1)", await headerElement.TextContentAsync());

			var titleLink = Page.GetByRole(AriaRole.Link, new() { Name = "Welcome to DasBlog Core" });
			await titleLink.ClickAsync();

			Assert.StartsWith("Welcome to DasBlog Core", await Page.TitleAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToSpecificArchiveDate()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/archive/2021/11/2");
			Assert.StartsWith("Archive - My DasBlog!", await Page.TitleAsync());

			var link = Page.GetByRole(AriaRole.Link, new() { NameRegex = new System.Text.RegularExpressions.Regex("Welcome to DasBlog Core") });
			await link.ClickAsync();

			Assert.Equal(Server.RootUri + "/welcome-to-dasblog-core", Page.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToArchiveUseBackCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", await Page.TitleAsync());

			var link = Page.GetByRole(AriaRole.Link, new() { Name = "<<" });
			await link.ClickAsync();

			Assert.Equal(Server.RootUri + "/archive/2020/1", Page.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToArchiveUseForwardCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			await Page.GotoAsync(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", await Page.TitleAsync());

			var link = Page.GetByRole(AriaRole.Link, new() { Name = ">>" });
			await link.ClickAsync();

			Assert.Equal(Server.RootUri + "/archive/2020/3", Page.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToRSSFeed()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await Page.GotoAsync(Server.RootUri + "/feed/rss");

			Assert.Equal(Page.Url.TrimEnd('/'), Server.RootUri + "/feed/rss");
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToPostAndCreateCommentDeleteComment()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			const string commentname = "First Name";

			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			await FillInputById("Name", commentname);
			await FillInputById("Email", "someemail@someplace.com");
			await FillInputById("HomePage", "https://www.github.com/poppastring/dasblog-core");
			await FillInputById("CheesyQuestionAnswered", "7");
			await FillInputById("Content", "A comment about this blog post");

			var saveButton = Page.Locator("#SaveContentButton");
			await saveButton.ClickAsync();

			Assert.Equal(Server.RootUri + "/welcome-to-dasblog-core/comments#comments-start", Page.Url.TrimEnd('/'));

			var commentElement = Page.Locator(".dbc-comment-user-homepage-name");
			Assert.Equal(commentname, await commentElement.First.TextContentAsync());

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			var deleteLinks = Page.GetByRole(AriaRole.Link, new() { Name = "Delete this comment" });
			var deletecount = await deleteLinks.CountAsync();

			Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
			await deleteLinks.First.ClickAsync();

			await Page.GotoAsync(Server.RootUri);
			await Task.Delay(2000);
			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			var deleteLinksAfter = Page.GetByRole(AriaRole.Link, new() { Name = "Delete this comment" });
			Assert.True(deletecount - 1 == await deleteLinksAfter.CountAsync());
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToPostAndCreateCommentManageCommentsPage()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			const string commentname = "Second Name";

			await Page.GotoAsync(Server.RootUri + "/welcome-to-dasblog-core");

			await FillInputById("Name", commentname);
			await FillInputById("Email", "otheremail@someplace.com");
			await FillInputById("HomePage", "https://www.github.com/poppastring/dasblog-core");
			await FillInputById("CheesyQuestionAnswered", "7");
			await FillInputById("Content", "Another comment on this blog post");

			var saveButton = Page.Locator("#SaveContentButton");
			await saveButton.ClickAsync();

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/admin/manage-comments");

			Assert.Equal(Server.RootUri + "/admin/manage-comments", Page.Url.TrimEnd('/'));

			var deleteLinks = Page.GetByRole(AriaRole.Link, new() { NameRegex = new System.Text.RegularExpressions.Regex("Delete this comment") });
			var deletecount = await deleteLinks.CountAsync();

			Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
			await deleteLinks.First.ClickAsync();

			await Page.GotoAsync(Server.RootUri + "/admin/manage-comments");

			var deleteLinksAfter = Page.GetByRole(AriaRole.Link, new() { Name = "Delete this comment" });
			var deletecount2 = await deleteLinksAfter.CountAsync();

			Assert.True(deletecount - deletecount2 == 1, "Comment was not deleted");
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task NavigateToLoginPageLoginThenLogout()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await Page.GotoAsync(Server.RootUri + "/post/create");

			Assert.Equal(Server.RootUri + "/account/login?ReturnUrl=%2Fpost%2Fcreate", Page.Url.TrimEnd('/'), true, true, true);

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/post/create");

			Assert.Equal(Server.RootUri + "/post/create", Page.Url.TrimEnd('/'));

			await Page.GotoAsync(Server.RootUri + "/account/logout");

			try
			{
				var createpostLink = Page.Locator("#CreatePostLink");
				await createpostLink.WaitForAsync(new LocatorWaitForOptions { Timeout = 1000 });
			}
			catch (Exception)
			{
				Assert.StartsWith("My DasBlog!", await Page.TitleAsync());
			}
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task LoginCreateAPostThenEditPostThenDeletAPost()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/admin/post/create");

			await FillInputById("BlogTitle", "A New Post");
			await FillInputById("BlogNewCategoryName", "Test Category");

			var newCategoryButton = Page.Locator("#NewCategorySubmit");
			await newCategoryButton.ClickAsync();

			Assert.Equal(Page.Url.TrimEnd('/'), Server.RootUri + "/admin/post/create");

			// Wait for "Test Category" label to appear in the categories list
			await Page.GetByText("Test Category", new() { Exact = true }).WaitForAsync(new() { Timeout = 5000 });

			// Check the checkbox for "Test Category" using XPath  
			// Find the label containing exactly "Test Category", then get its preceding checkbox sibling
			var categoryCheckbox = Page.Locator("//label[normalize-space(text())='Test Category']/preceding-sibling::input[@type='checkbox'][@class='form-check-input me-1']");
			await categoryCheckbox.CheckAsync();

			// Click the visible Create Post button to open the modal
			var createPostButton = Page.Locator("button:has-text('Create Post')").First;
			await createPostButton.ClickAsync();

			// Click the confirm button in the modal
			var confirmButton = Page.Locator("#confirmCreateModalButton");
			await confirmButton.ClickAsync();

			// Wait for the form submission to complete - wait for either redirect to edit page OR success alert
			await Task.WhenAny(
				Page.WaitForURLAsync("**/admin/post/*/edit", new() { Timeout = 5000 }),
				Page.Locator(".alert-success").WaitForAsync(new() { Timeout = 5000 })
			);
			await Task.Delay(500); // Small delay to ensure page is stable

			await Page.GotoAsync(Server.RootUri);
			await Page.GotoAsync(Server.RootUri + "/a-new-post");

			Assert.StartsWith("A New Post", await Page.TitleAsync());

			await Page.GotoAsync(Server.RootUri + "/category/test-category");
			var titleLink = Page.GetByRole(AriaRole.Link, new() { Name = "A New Post" });
			await titleLink.ClickAsync();
			Assert.StartsWith("A New Post", await Page.TitleAsync());

			await Page.GotoAsync(Server.RootUri);
			await Page.GotoAsync(Server.RootUri + "/a-new-post");
			Assert.StartsWith("A New Post", await Page.TitleAsync());

			var editpostLink = Page.GetByRole(AriaRole.Link, new() { Name = "Edit this post" });
			await editpostLink.ClickAsync();

			await FillInputById("BlogTitle", " Now Edit");

			// Click the visible Save button to open the modal
			var saveButton = Page.Locator("button:has-text('Save')").First;
			await saveButton.ClickAsync();

			// Click the confirm button in the modal
			var confirmSaveButton = Page.Locator("#confirmSaveModalButton");
			await confirmSaveButton.ClickAsync();

			// Wait for the save to complete - wait for success alert to appear
			await Task.WhenAny(
				Page.Locator(".alert-success").WaitForAsync(new() { Timeout = 5000 }),
				Task.Delay(5000)
			);
			await Task.Delay(500); // Small delay to ensure page is stable

			await Page.GotoAsync(Server.RootUri);
			await Page.GotoAsync(Server.RootUri + "/a-new-post-now-edit");
			Assert.StartsWith("A New Post Now Edit", await Page.TitleAsync());

			var deletepostLink = Page.GetByRole(AriaRole.Link, new() { Name = "Delete this post" });

			Page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
			await deletepostLink.ClickAsync();

			await Page.GotoAsync(Server.RootUri + "/account/logout");

			var deletedLinks = Page.GetByRole(AriaRole.Link, new() { Name = "A New Post Now Edit" });
			Assert.True(await deletedLinks.CountAsync() == 0);
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task LoginNavigateSiteAdmin()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/admin/settings");

			var siteRoot = Page.Locator("#SiteConfig_Root");
			var rootValue = await siteRoot.GetAttributeAsync("Value");

			Assert.Contains("https://localhost:5001/", rootValue);
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task LoginNavigateUserAdmin()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/users/myemail@myemail.com");

			var emailAddress = Page.Locator("[name='EmailAddress']");
			var address = await emailAddress.GetAttributeAsync("Value");

			Assert.Contains("myemail@myemail.com", address);
		}

		[SkippableFact(typeof(PlaywrightException))]
		public async Task LoginNavigateActivity()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			await LoginToSite();

			await Page.GotoAsync(Server.RootUri + "/activity/list");

			var forwardLink = Page.GetByRole(AriaRole.Link, new() { Name = ">|" });
			await forwardLink.ClickAsync();

			var tablecolumns = Page.Locator(".dbc-activity-table-column");
			Assert.True(await tablecolumns.CountAsync() > 0);
		}

		private async Task LoginToSite()
		{
			await Page.GotoAsync(Server.RootUri + "/account/login");

			await FillInputById("EmailLogin", "myemail@myemail.com");
			await FillInputById("PasswordLogin", "admin");

			var loginButton = Page.Locator("#LoginButton");
			await loginButton.ClickAsync();
		}

		private async Task FillInputById(string elementId, string text)
		{
			var element = Page.Locator($"#{elementId}");
			await element.FillAsync(text);
		}
	}
}
