using System;
using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Dom;
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

		public void Init()
		{
			System.Diagnostics.Debug.Assert(driver == null);
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

		public string GetTitle()
		{
			return driver.Title;
		}

		public ButtonElement GetButtonById(string id)
		{
			var el = driver.FindElement(By.Id(id));
			if (el != null)
			{
				return new ButtonElement(el);
			}
			else
			{
				return null;
			}
		}

		public SpanElement GetElementById(string id)
		{
			var el = driver.FindElement(By.Id(id));
			if (el != null)
			{
				return new SpanElement(el);
			}
			else
			{
				return null;
			}
		}

		public LinkElement GetLinkById(string id)
		{
			var el = driver.FindElement(By.Id(id));
			if (el != null)
			{
				return new LinkElement(el);
			}
			else
			{
				return null;
			}
		}
	}
}
