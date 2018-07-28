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
		private readonly string driverId;

		public Browser(IOptions<BrowserOptions> optionsAccessor)
		{
			this.homeUrl = optionsAccessor.Value.HomeUrl;
			driverId = optionsAccessor.Value.Driver;
		}

		private void Init()
		{
			if (driver != null)
			{
				return;
			}
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
			Init();
			driver.Navigate().GoToUrl(homeUrl + path);
			var wait = new WebDriverWait(driver, TimeSpan.FromMinutes(1));
		}

		public void Dispose()
		{
			driver?.Dispose();
		}
	}
}
