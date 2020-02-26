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
	public class SeleniumTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
	{
		public HttpClient Client { get; }
		public SeleniumServerFactory<Startup> Server { get; }
		public IWebDriver Browser { get; }
		public ILogs Logs { get; }

		public SeleniumTests(SeleniumServerFactory<Startup> server)
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
		public void LoadTheMainPageAndCheckTitle()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);
			Assert.StartsWith("Hanselminutes Technology Podcast - Fresh Air and Fresh Perspectives for Developers", Browser.Title);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void ThereIsAnH1()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri);

			var headerSelector = By.TagName("h1");
			Assert.Equal("HANSELMINUTES PODCAST by Scott Hanselman", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void KevinScottTest()
		{
			Skip.If(AreWe.InDockerOrBuildServer);
			Browser.Navigate().GoToUrl(Server.RootUri + "/631/how-do-you-become-a-cto-with-microsofts-cto-kevin-scott");

			var headerSelector = By.TagName("h2");
			Assert.Equal("How do you become a CTO - with Microsoft's CTO Kevin Scott", Browser.FindElement(headerSelector).Text);
		}

		[SkippableFact(typeof(OpenQA.Selenium.WebDriverException))]
		public void KevinScottTestThenGoHome()
		{
			Skip.If(AreWe.InDockerOrBuildServer, "In Docker!");
			Browser.Navigate().GoToUrl(Server.RootUri + "/631/how-do-you-become-a-cto-with-microsofts-cto-kevin-scott");

			var headerSelector = By.TagName("h1");
			var link = Browser.FindElement(headerSelector);
			link.Click();
			Assert.Equal(Browser.Url.TrimEnd('/'), Server.RootUri); //WTF
		}

		public void Dispose()
		{
			if (Browser != null)
				Browser.Dispose();
		}
	}
}
