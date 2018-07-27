using System;
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
			browser.Home();
		}

		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
