using System;
using DasBlog.SmokeTest.Interfaces;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace DasBlog.SmokeTest
{
	internal class Browser : IBrowser
	{
		private readonly string homeUrl;
		private IWebDriver driver;

		public Browser(IOptions<BrowserOptions> optionsAccessor)
		{
			this.homeUrl = optionsAccessor.Value.HomeUrl;
			Init(optionsAccessor.Value.Driver);
		}

		private void Init(string driverId)
		{
			switch (driverId)
			{
				case Constants.FirefoxDriverId:
					driver = new FirefoxDriver();
					break;
				default:
					throw new Exception("firefox is the only supported browser at present");
			}
		}

		public void Goto(string path)
		{
			driver.Navigate().GoToUrl(homeUrl + path);
			var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
		}

		public void Dispose()
		{
			driver?.Dispose();
		}
	}
}
