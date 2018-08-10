using System;
using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Dom;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace DasBlog.Tests.SmokeTest.Selenium
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

		public ButtonPageElement GetButtonById(string id)
		{
			return GetElementById<ButtonPageElement>(id);
		}

		public SpanPageElement GetElementById(string id)
		{
			return GetElementById<SpanPageElement>(id);
		}

		public LinkPageElement GetLinkById(string id)
		{
			return GetElementById<LinkPageElement>(id);
		}

		public AnyPageElement GetAnyElementById(string id)
		{
			return GetElementById<AnyPageElement>(id);
		}

		public PE GetElementById<PE>(string id) where PE : PageElement, new()
		{
			try
			{
				var el = driver.FindElement(By.Id(id));
				if (el != null)
				{
					PE pe = new PE();
					pe.WebElement = el;
					return pe;
				}
				else
				{
					return null;
				}
			}
			catch (NoSuchElementException e)
			{
				return null;
			}
		}
	}
}
