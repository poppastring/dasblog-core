using System;
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
			Browser.Navigate().GoToUrl(Server.RootUri + "/category/dasblogcore");
			Assert.StartsWith("Category - My DasBlog!", Browser.Title);

			var headerSelector = By.TagName("h4");
			Assert.Equal("dasblog-core (1)", Browser.FindElement(headerSelector).Text);

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


		// Create Comments

		// Login

		// Approve Comment

		// Delete Comment

		// Create category

		// Create Post

		// Edit Post

		// Delete Post


		public void Dispose()
		{
			if (Browser != null)
				Browser.Dispose();
		}
	}
}
