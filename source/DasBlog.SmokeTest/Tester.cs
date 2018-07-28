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
			const string methodName = nameof(Login_WithBlankPassword_Fails);
			pages.Login.Goto();
			if (!pages.Login.IsDisplayed())
			{
				Results.Add(methodName, false, "failed to find login page");
				return;
			}

			pages.Login.LoginButton.Click();
			if (!pages.Login.Password.Text.ToLower().Contains("the password field is required"))
			{
				Results.Add(methodName, false, "failed to find missing password message");
				return;
			}
			Results.Add(methodName, true);
		}
		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
