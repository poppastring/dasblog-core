﻿using System;
using System.Net.Http;
using DasBlog.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Xunit;

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
			Logs = new RemoteLogs(driver); //TODO: Still not bringing the logs over yet?
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void LoadTheMainPageAndCheckPageTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);
			Assert.StartsWith("My DasBlog!", Browser.Title);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void FrontPageH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);

			var headerSelector = By.TagName("h2");
			Assert.Equal("Welcome to DasBlog Core", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void WelcomePostCheckPageBrowserTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");
			Assert.StartsWith("Welcome to DasBlog Core", Browser.Title);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void WelcomePostH2PostTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var headerSelector = By.TagName("h2");
			Assert.Equal("Welcome to DasBlog Core", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToWelcomePostThenGoHome()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var headerSelector = By.LinkText("Home");
			var link = Browser.FindElement(headerSelector);
			link.Click();
			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri);
		}


		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToPageOneAndBack()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri);

			var olderpostSelector = By.LinkText("<< Older Posts");
			var link = Browser.FindElement(olderpostSelector);
			link.Click();
			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/page/1");

			var newerpostSelector = By.LinkText("Newer Posts >>");
			var link2 = Browser.FindElement(newerpostSelector);
			link2.Click();
			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/page/0");
		}


		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToCategory()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/category");
			Assert.StartsWith("Category - My DasBlog!", Browser.Title);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
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

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToSpecificArchiveDate()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2020/2/27");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var postSelector = By.LinkText("Welcome To DasBlog Core");
			var link = Browser.FindElement(postSelector);
			link.Click();

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/welcome-to-dasblog-core");
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToArchiveUseBackCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var navSelector = By.LinkText("❮");
			var link = Browser.FindElement(navSelector);
			link.Click();

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/archive/2020/1");
		}


		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToArchiveUseForwardCalendarControls()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/archive/2020/2");
			Assert.StartsWith("Archive - My DasBlog!", Browser.Title);

			var navSelector = By.LinkText("❯");
			var link = Browser.FindElement(navSelector);
			link.Click();

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/archive/2020/3");
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToRSSFeed()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			Browser.Navigate().GoToUrl(Server.RootUri + "/feed/rss");

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/feed/rss");
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToPostAndCreateComment()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

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

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/welcome-to-dasblog-core/comments#comments-start");

			var elementid = By.ClassName("dbc-comment-user-homepage-name");
			Assert.Equal(commentname, Browser.FindElement(elementid).Text);
		}


		// Approve Comment
		// Delete Comment

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToPostAndCreateCommentApproveCommentDeleteComment()
		{
			NavigateToPostAndCreateComment();

			LoginToSite();

			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var approveSelector = By.LinkText("Approve this comment");
			var approvelink = Browser.FindElement(approveSelector);
			approvelink.Click();

			Browser.SwitchTo().Alert().Accept();

			// Check the comment is approved
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var approvedSelector = By.LinkText("Comment Approved");
			var approvedlink = Browser.FindElement(approvedSelector);

			// Delete this comment
			Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

			var deleteSelector = By.LinkText("Delete this comment");
			var deletelink = Browser.FindElement(deleteSelector);
			deletelink.Click();

			Browser.SwitchTo().Alert().Accept();

			try
			{
				Browser.Navigate().GoToUrl(Server.RootUri + "/welcome-to-dasblog-core");

				deleteSelector = By.LinkText("Delete this comment");
				deletelink = Browser.FindElement(deleteSelector);
			}
			catch (Exception ex)
			{
				Assert.StartsWith("Welcome to DasBlog Core", Browser.Title);
			}

		}



		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void NavigateToLoginPageLoginThenLogout()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			var createpostSelector = By.Id("CreatePostLink");
			var createpostlink = Browser.FindElement(createpostSelector);
			createpostlink.Click();

			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri + "/post/create");

			Browser.Navigate().GoToUrl(Server.RootUri + "/account/logout");

			try
			{
				createpostSelector = By.Id("CreatePostLink");
				createpostlink = Browser.FindElement(createpostSelector);
			}
			catch(Exception ex)
			{
				Assert.StartsWith("My DasBlog!", Browser.Title);
			}
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void LoginCreateAPostThenEditPostThenDeletAPost()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");

			LoginToSite();

			// Fill out post
			Browser.Navigate().GoToUrl(Server.RootUri + "/post/create");

			SendKeysToElement("BlogTitle", "A New Post");

			SendKeysToElement("mytextarea", "We certainly hope this works...");

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
			Browser.Navigate().GoToUrl(Server.RootUri + "/a-new-post");

			Assert.StartsWith("A New Post", Browser.Title);

			// Check new category
			Browser.Navigate().GoToUrl(Server.RootUri + "/category/test-category");
			var titleSelector = By.LinkText("A New Post");
			Browser.FindElement(titleSelector).Click();

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
			Browser.Navigate().GoToUrl(Server.RootUri + "/a-new-post-now-edit");
			Assert.StartsWith("A New Post Now Edit", Browser.Title);

			//Delete this post
			var deletepostSelector = By.LinkText("Delete this post");
			var deletepostSubmitlink = Browser.FindElement(deletepostSelector);
			deletepostSubmitlink.Click();

			Browser.SwitchTo().Alert().Accept();

			// logout
			Browser.Navigate().GoToUrl(Server.RootUri + "/account/logout");

			try
			{
				var titledeleteSelector = By.LinkText("A New Post Now Edit");
				Browser.FindElement(titledeleteSelector).Click();
			}
			catch (Exception ex)
			{
				Assert.StartsWith("My DasBlog!", Browser.Title);
			}
		}

		// Site Admin
		// Users
		//	Activity


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