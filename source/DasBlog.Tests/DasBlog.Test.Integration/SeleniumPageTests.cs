using System;
using System.Net.Http;
using System.Linq;
using DasBlog.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;
using System.Threading;

namespace DasBlog.Test.Integration
{
	[Trait("Category", "SkipWhenLiveUnitTesting")]
	public class SeleniumPageTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
	{
		public HttpClient Client { get; }
		public SeleniumServerFactory<Startup> Server { get; }
		public IWebDriver Browser { get; }
		public ILogs Logs { get; }

		public SeleniumPageTests(SeleniumServerFactory<Startup> server)
		{
			Console.WriteLine("In Docker?" + AreWe.InDockerOrBuildServer);
			if (AreWe.InDockerOrBuildServer) return;
			Server = server;
			Client = server.CreateClient(); //weird side effecty thing here. This shouldn't be required but it is.

			var opts = new ChromeOptions();
			//opts.AddArgument("--headless");
			opts.SetLoggingPreference(OpenQA.Selenium.LogType.Browser, LogLevel.All);

			var driver = new RemoteWebDriver(opts);
			Browser = driver;
			// Logs = new RemoteLogs(driver); //TODO: Still not bringing the logs over yet?
		}

		[SkippableFact(typeof(WebDriverException))]
		public void LoadTheMainPageAndCheckPageTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);
			Assert.StartsWith("My DasBlog!", Browser.Title);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void FrontPageH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);

			var headerSelector = By.TagName("h2");
			Assert.Equal("Welcome to DasBlog Core", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void WelcomePostCheckPageBrowserTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");
			Assert.StartsWith("Welcome to DasBlog Core", Browser.Title);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void WelcomePostH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var headerSelector = By.TagName("h2");
			Assert.Equal("Welcome to DasBlog Core", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToWelcomePostThenGoHome()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var headerSelector = By.LinkText("Home");
			var link = Browser.FindElement(headerSelector);
			link.Click();
			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri);
		}


		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToPageOneAndBack()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri);

			var olderpostSelector = By.LinkText("<< Older Posts");
			var link = Browser.FindElement(olderpostSelector);
			link.Click();
			Assert.Equal(Server.RootUri + "/page/1", Browser.Url.TrimEnd('/'));

			var newerpostSelector = By.LinkText("Newer Posts >>");
			var link2 = Browser.FindElement(newerpostSelector);
			link2.Click();
			Assert.Equal(Server.RootUri + "/page/0", Browser.Url.TrimEnd('/'));
		}


		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToCategory()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/category");
			Assert.StartsWith("Category - My DasBlog!", Browser.Title);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToSpecificCategoryNavigateToPost()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/category/dasblog-core");
			Assert.StartsWith("Category - My DasBlog!", Browser.Title);

			var headerSelector = By.TagName("h4");
			Assert.Equal("Dasblog Core (1)", Browser.FindElement(headerSelector).Text);

			var titleSelector = By.LinkText("Welcome to DasBlog Core");
			Browser.FindElement(titleSelector).Click();

			Assert.StartsWith("Welcome to DasBlog Core", Browser.Title);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToSpecificArchiveDate()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2021/11/2");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var postSelector = By.PartialLinkText("Welcome to DasBlog Core");
			var link = Browser.FindElement(postSelector);
			link.Click();

			Assert.Equal(Server.RootUri + "/welcome-to-dasblog-core", Browser.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToArchiveUseBackCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var navSelector = By.LinkText("<<");
			var link = Browser.FindElement(navSelector);
			link.Click();

			Assert.Equal(Server.RootUri + "/archive/2020/1", Browser.Url.TrimEnd('/'));
		}


		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToArchiveUseForwardCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var navSelector = By.LinkText(">>");
			var link = Browser.FindElement(navSelector);
			link.Click();

			Assert.Equal(Server.RootUri + "/archive/2020/3", Browser.Url.TrimEnd('/'));
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToRSSFeed()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			Browser.Navigate().GoToUrl(Server.RootUri + "/feed/rss");

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/feed/rss");
		}


		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToPostAndCreateCommentDeleteComment()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			// Add comment
			const string commentname = "First Name";

			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			SendKeysToElement("Name", commentname);

			SendKeysToElement("Email", "someemail@someplace.com");

			SendKeysToElement("HomePage", "https://www.github.com/poppastring/dasblog-core");

			SendKeysToElement("CheesyQuestionAnswered", "7");

			SendKeysToElement("Content", "A comment about this blog post");

			var navSelector = By.Id("SaveContentButton");
			var link = Browser.FindElement(navSelector);
			link.Click();

			Assert.Equal(Server.RootUri + "/welcome-to-dasblog-core/comments#comments-start", Browser.Url.TrimEnd('/'));

			var elementid = By.ClassName("dbc-comment-user-homepage-name");
			Assert.Equal(commentname, Browser.FindElement(elementid).Text); ;


			LoginToSite();

			// Delete this comment
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var deleteSelector = By.LinkText("Delete this comment");
			var deletelink = Browser.FindElements(deleteSelector);
			var deletecount = deletelink.Count;
			deletelink[0].Click();	

			Browser.SwitchTo().Alert().Accept();

			Browser.Navigate().GoToUrl(Server.RootUri);

			Thread.Sleep(2000);

			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			deleteSelector = By.LinkText("Delete this comment");
			var deletelinks = Browser.FindElements(deleteSelector);
			Assert.True(deletecount - 1 == deletelinks.Count);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void NavigateToPostAndCreateCommentManageCommentsPage()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			// Add comment
			const string commentname = "Second Name";

			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			SendKeysToElement("Name", commentname);

			SendKeysToElement("Email", "otheremail@someplace.com");

			SendKeysToElement("HomePage", "https://www.github.com/poppastring/dasblog-core");

			SendKeysToElement("CheesyQuestionAnswered", "7");

			SendKeysToElement("Content", "Another comment on this blog post");

			var navSelector = By.Id("SaveContentButton");
			var link = Browser.FindElement(navSelector);
			link.Click();

			LoginToSite();

			// Navigate to comment management
			Browser.Navigate().GoToUrl(Server.RootUri + "/admin/manage-comments");

			Assert.Equal(Server.RootUri + "/admin/manage-comments", Browser.Url.TrimEnd('/'));

			var postSelector = By.PartialLinkText("Delete this comment");
			var link2 = Browser.FindElements(postSelector);
			var deletecount = link2.Count;
			link2[0].Click();

			Browser.SwitchTo().Alert().Accept();

			// Navigate to comment management
			Browser.Navigate().GoToUrl(Server.RootUri + "/admin/manage-comments");

			var deleteSelector = By.LinkText("Delete this comment");
			var deletelink = Browser.FindElements(deleteSelector);
			var deletecount2 = deletelink.Count;

			Assert.True(deletecount - deletecount2 == 1, "Comment was not deleted");
		}

			[SkippableFact(typeof(WebDriverException))]
		public void NavigateToLoginPageLoginThenLogout()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			Browser.Navigate().GoToUrl(Server.RootUri + "/post/create");

			Assert.Equal(Server.RootUri + "/account/login?ReturnUrl=%2Fpost%2Fcreate", Browser.Url.TrimEnd('/'), true, true, true);

			LoginToSite();

			Browser.Navigate().GoToUrl(Server.RootUri + "/post/create");

			Assert.Equal(Server.RootUri + "/post/create", Browser.Url.TrimEnd('/') );

			Browser.Navigate().GoToUrl(Server.RootUri + "/account/logout");

			try
			{
				var createpostSelector = By.Id("CreatePostLink");
				var createpostlink = Browser.FindElement(createpostSelector);
			}
			catch(Exception)
			{
				Assert.StartsWith("My DasBlog!", Browser.Title);
			}
		}

		[SkippableFact(typeof(WebDriverException))]
		public void LoginCreateAPostThenEditPostThenDeletAPost()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			// Fill out post
			Browser.Navigate().GoToUrl(Server.RootUri + "/post/create");

			SendKeysToElement("BlogTitle", "A New Post");

			// SendKeysToElement("tinymce", "We certainly hope this works...");

			SendKeysToElement("BlogNewCategoryName", "Test Category");

			// Add a new category
			var newCategorySelector = By.Id("NewCategorySubmit");
			var newcategorylink = Browser.FindElement(newCategorySelector);
			newcategorylink.Click();

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/post/create");

			var blogpostSelector = By.Id("BlogPostCreateSubmit");
			var blogpostSubmitlink = Browser.FindElement(blogpostSelector);
			blogpostSubmitlink.Click();

			// Check new post
			Browser.Navigate().GoToUrl(Server.RootUri);

			Browser.Navigate().GoToUrl(Server.RootUri + "/a-new-post");

			Assert.StartsWith("A New Post", Browser.Title);

			// Check new category
			Browser.Navigate().GoToUrl(Server.RootUri + "/category/test-category");
			var titleSelector = By.LinkText("A New Post");
			Browser.FindElement(titleSelector).Click();
			Assert.StartsWith("A New Post", Browser.Title);


			//Navigate back to the main page and post
			Browser.Navigate().GoToUrl(Server.RootUri);
			Browser.Navigate().GoToUrl(Server.RootUri + "/a-new-post");
			Assert.StartsWith("A New Post", Browser.Title);


			//Edit the post title
			var editpostSelector = By.LinkText("Edit this post");
			var editpostSubmitlink = Browser.FindElement(editpostSelector);
			editpostSubmitlink.Click();

			SendKeysToElement("BlogTitle", " Now Edit");
			blogpostSelector = By.Id("BlogPostEditSubmit");
			blogpostSubmitlink = Browser.FindElement(blogpostSelector);
			blogpostSubmitlink.Click();

			//Check the new post title
			Browser.Navigate().GoToUrl(Server.RootUri);
			Browser.Navigate().GoToUrl(Server.RootUri + "/a-new-post-now-edit");
			Assert.StartsWith("A New Post Now Edit", Browser.Title);

			//Delete this post
			var deletepostSelector = By.LinkText("Delete this post");
			var deletepostSubmitlink = Browser.FindElement(deletepostSelector);
			deletepostSubmitlink.Click();

			Browser.SwitchTo().Alert().Accept();

			// logout
			Browser.Navigate().GoToUrl(Server.RootUri + "/account/logout");

			var titledeleteSelector = By.LinkText("A New Post Now Edit");
			var deletedLink = Browser.FindElements(titledeleteSelector);

			Assert.True(deletedLink.Count == 0);
		}

		[SkippableFact(typeof(WebDriverException))]
		public void LoginNavigateSiteAdmin()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			Browser.Navigate().GoToUrl(Server.RootUri + "/admin/settings");

			var siteRoot = By.Id("SiteConfig_Root");
			var roottext = Browser.FindElement(siteRoot);

			Assert.Contains("https://localhost:5001/", roottext.GetAttribute("Value"));	
		}

		[SkippableFact(typeof(WebDriverException))]
		public void LoginNavigateUserAdmin()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			Browser.Navigate().GoToUrl(Server.RootUri + "/users/myemail@myemail.com");

			var emailAddress = By.Name("EmailAddress");
			var address = Browser.FindElement(emailAddress);

			Assert.Contains("myemail@myemail.com", address.GetAttribute("Value"));
		}

		[SkippableFact(typeof(WebDriverException))]
		public void LoginNavigateActivity()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			Browser.Navigate().GoToUrl(Server.RootUri + "/activity/list");

			var forwardLink = By.LinkText(">|");
			var forward = Browser.FindElement(forwardLink);
			forward.Click();

			var tablecolumn = By.ClassName("dbc-activity-table-column");
			var tablecolumns = Browser.FindElements(tablecolumn);

			Assert.True(tablecolumns.Count > 0);

		}


		private void LoginToSite()
		{
			Browser.Navigate().GoToUrl(Server.RootUri + "/account/login");

			SendKeysToElement("Email", "myemail@myemail.com");

			SendKeysToElement("Password", "admin");

			var navSelector = By.Id("LoginButton");
			var link = Browser.FindElement(navSelector);
			link.Click();
		}

		private void SendKeysToElement(string element, string keystosend)
		{
			var elementid = By.Id(element);
			var eleementlink = Browser.FindElement(elementid);
			eleementlink.SendKeys(keystosend);
		}

		public void Dispose()
		{
			if (Browser != null)
				Browser.Dispose();
		}
	}
}
