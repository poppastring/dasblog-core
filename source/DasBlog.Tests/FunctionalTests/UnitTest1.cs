using System;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Xunit;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.FunctionalTests
{
	public class BrowserOptionsAccessor : IOptions<BrowserOptions>
	{
		public BrowserOptionsAccessor(BrowserOptions opts)
		{
			Value = opts;
		}
		public BrowserOptions Value { get; }
	}
	public class AutomateThePlanetTestsXUnit
	{
		[Fact]
		public void ProtoTest()
		{
			IBrowser browser = new Browser(
			  new BrowserOptionsAccessor(new BrowserOptions
			  {
				  HomeUrl =  "http://localhost:5050/",
				  Driver = "firefox"
			  }));
			browser.Init();
			browser.Goto("/Login");
		}
/*
		[Fact]
		public void TestWithFirefoxDriver()
		{
			using (var driver = new FirefoxDriver())
			{
				driver.Navigate().GoToUrl(@"https://automatetheplanet.com/multiple-files-page-objects-item-templates/");
				var link = driver.FindElement(By.PartialLinkText("TFS Test API"));
				var jsToBeExecuted = $"window.scroll(0, {link.Location.Y});";
				((IJavaScriptExecutor)driver).ExecuteScript(jsToBeExecuted);
				var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
				var clickableElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("TFS Test API")));
//				clickableElement.Click();
			}
		}
*/
/*
		// failing on AppVeyor due to version mismatch - we expect AppVeyor to upgrade
		[Fact]
		public void TestWithChromeDriver()
		{
			using (var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)))
			{
				driver.Navigate().GoToUrl(@"https://automatetheplanet.com/multiple-files-page-objects-item-templates/");
				var link = driver.FindElement(By.PartialLinkText("TFS Test API"));
				var jsToBeExecuted = $"window.scroll(0, {link.Location.Y});";
				((IJavaScriptExecutor)driver).ExecuteScript(jsToBeExecuted);
				var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
				var clickableElement = wait.Until(ExpectedConditions.ElementToBeClickable(By.PartialLinkText("TFS Test API")));
				clickableElement.Click();
			}
		}
*/
	}
}
