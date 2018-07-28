using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using DasBlog.SmokeTest.Common;
using static DasBlog.SmokeTest.Common.Utils;
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
		private ITestExecutor testExecutor;

		public Tester(IBrowser browser, ITestExecutor testExecutor)
		{
			this.browser = browser;
			pages = new Pages(browser);
			this.testExecutor = testExecutor;
		}

		public void Test()
		{
			browser.Init();
			Login_WithBlankPassword_Fails();
			Thread.Sleep(10000);
		}

		private void Login_WithBlankPassword_Fails()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Login.Goto())
				, new TestStep(() => pages.Login.IsDisplayed())
				, new TestStep(() => pages.Login.LoginButton != null)
				, new TestStep(() => pages.Login.LoginButton.Click())
				, new TestStep(() => pages.Login.Password.Text.ToLower().Contains("the password field is required"))
				, new TestStep(() => pages.Login.IsDisplayed())
			};
			testExecutor.Execute(testSteps, Results);
		}


		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
