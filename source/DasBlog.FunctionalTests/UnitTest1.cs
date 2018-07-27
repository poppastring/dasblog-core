using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace DasBlog.FunctionalTests
{
	public class AutomateThePlanetTestsXUnit
	{
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
//				Thread.Sleep(100000);
			}
		}
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
