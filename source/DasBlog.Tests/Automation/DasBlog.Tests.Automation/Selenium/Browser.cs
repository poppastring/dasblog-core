using System;
using System.Diagnostics;
using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.Automation.Dom;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.Automation.Selenium
{
	public partial class Browser : IBrowser
	{
		private readonly string homeUrl;
		private IWebDriver driver;
		private readonly string driverId;
		private ILogger<Browser> logger;

		public Browser(IOptions<BrowserOptions> optionsAccessor, ILogger<Browser> logger)
		{
			this.logger = logger;
			this.homeUrl = optionsAccessor.Value.HomeUrl;
			driverId = optionsAccessor.Value.Driver;
		}

		public void Init()
		{
			System.Diagnostics.Debug.Assert(driver == null);
			switch (driverId)
			{
				case Constants.FirefoxDriverId:
					driver = CreateFireFoxWebDriver();
					break;
				default:
					throw new Exception("firefox is the only supported browser at present");
			}
		}

		private IWebDriver CreateFireFoxWebDriver()
		{
			IWebDriver candidateDriver = null;
			const int NUM_TRIALS = 3;
			for (int trial = 0; trial < NUM_TRIALS + 1; trial++)
			{
				try
				{
					candidateDriver = new FirefoxDriver();
					return candidateDriver;
				}
				catch (WebDriverException wde)
				{
					if (trial >= NUM_TRIALS)
					{
						throw;
					}
					candidateDriver?.Dispose();
					KillDriverExe();
					logger.LogInformation($"Attempt to start web driver faailed attempt {trial+1} of {NUM_TRIALS}");
				}
			}

			return null;
		}

		private void KillDriverExe()
		{
			foreach (var process in Process.GetProcessesByName("geckodriver.exe"))
			{
				process.Kill();
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

		public string GetUrl()
		{
			return driver.Url;
		}

		public string GetPageSource()
		{
			return driver.PageSource;
		}

		public ILogger<Browser> Logger => logger;

		public ButtonPageElement GetButtonById(string id)
		{
			return GetElementById<ButtonPageElement>(id);
		}

		public SpanPageElement GetElementById(string id)
		{
			return GetElementById<SpanPageElement>(id);
		}

		public TextBoxPageElement GetTextBoxElementById(string id)
		{
			return GetElementById<TextBoxPageElement>(id);
		}

		public LinkPageElement GetLinkById(string id)
		{
			return GetElementById<LinkPageElement>(id);
		}

		public AnyPageElement GetAnyElementById(string id)
		{
			return GetElementById<AnyPageElement>(id);
		}

		public DivPageElement GetDivById(string id)
		{
			return GetElementById<DivPageElement>(id);
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
					pe.Id = id;
					return pe;
				}
				else
				{
					return null;
				}
			}
			catch (NoSuchElementException e)
			{
				_ = e;
				return null;
			}
		}

//		Much thanks to:
//		https://stackoverflow.com/questions/2646195/how-to-check-if-an-element-is-visible-with-webdriver/52261060#52261060
		public bool IsElementVisible(PageElement pe)
		{
			return driver.ExecuteJavaScript<bool>(
				isVisibleScript.Replace("element-id", pe.Id));
		}
	}
}
