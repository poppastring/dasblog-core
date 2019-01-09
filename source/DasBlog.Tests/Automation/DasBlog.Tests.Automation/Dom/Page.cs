using System;
using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Automation.Dom
{
	public class Page
	{
		protected readonly IBrowser browser;
		protected readonly string path;						// relative to the root e.g. "category" or "account/login"
		protected readonly string pageTestId;
		public Page(IBrowser browser, string path, string pageTestId)
		{
			this.browser = browser;
			this.path = path;
			this.pageTestId = pageTestId;
		}
		public void Goto()
		{
			browser.Goto(path);
		}

		public virtual bool IsDisplayed()
		{
			try
			{
				var elem = browser.GetDivById(pageTestId);
				if (elem == null)
				{
					browser.Logger.LogInformation( browser.GetPageSource());
				}

				return elem != null;
			}
			catch (OpenQA.Selenium.NoSuchElementException e)
			{
				_ = e;
				return false;
			}
		}
	}
}
