using System.Collections.Generic;
using System.Threading;
using DasBlog.SmokeTest.Dom;
using DasBlog.SmokeTest.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

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
			Click_OnNavBarItem_ShowsPage();
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

		private void Click_OnNavBarItem_ShowsPage()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Login.Goto()),
				new TestStep(() => pages.NavBar.IsDisplayed()),
				new TestStep(() => pages.NavBar[AppConstants.CategoryId] != null),
				new TestStep(() => pages.NavBar[AppConstants.CategoryId].Click()),
				new TestStep(() => pages.Category.IsDisplayed())
				// TODO other pages
			};
			testExecutor.Execute(testSteps, Results);
		}
		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
