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
		public TestResults Results { get; } = new TestResults();
		private readonly Pages pages;
		public Tester(IBrowser browser)
		{
			this.browser = browser;
			pages = new Pages(browser);
		}

		public void Test()
		{
			browser.Init();
			Login_WithBlankPassword_Fails();
			Thread.Sleep(10000);
		}

		private void Login_WithBlankPassword_Fails()
		{
			Results.Add("Login_WithBlankPassword_Fails", false, "test not implemented");
		}
		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
