using System;
using System.Threading;
using DasBlog.SmokeTest.Dom;
using DasBlog.SmokeTest.Interfaces;
using OpenQA.Selenium.Firefox;

namespace DasBlog.SmokeTest
{
	internal class Tester : ITester
	{
		private readonly IBrowser browser;
		public Tester(IBrowser browser)
		{
			this.browser = browser;
		}

		public void Test()
		{
			Pages pages = new Pages(browser);
			pages.Home.Goto();
			Thread.Sleep(10000);
		}

		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
