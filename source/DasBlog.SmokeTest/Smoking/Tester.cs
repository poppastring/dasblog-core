using System.Collections.Generic;
using System.Threading;
using DasBlog.SmokeTest.Dom;
using DasBlog.SmokeTest.Selenium.Interfaces;
using DasBlog.SmokeTest.Smoking.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.SmokeTest.Smoking
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
			Goto_UnauthorizedPage_LoginPage();
			Thread.Sleep(1000);
		}

		private void Login_WithBlankPassword_Fails()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Login.Goto())
				, new TestStep(() => pages.Login.IsDisplayed())
				, new TestStep(() => pages.Login.LoginButton != null)
				, new TestStep(() => pages.Login.LoginButton.Click())
				, new TestStep(() => pages.Login.PasswordValidation.Text.ToLower().Contains("the password field is required"))
				, new TestStep(() => pages.Login.IsDisplayed())
			};
			testExecutor.Execute(testSteps, Results);
		}

		private void Click_OnNavBarItem_ShowsPage()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Home.Goto()),
				new TestStep(() => pages.NavBar.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.CategoryNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.CategoryNavId].Click()),
				new TestStep(() => pages.Category.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.ArchiveNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.ArchiveNavId].Click()),
				new TestStep(() => pages.Archive.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.HomeNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.HomeNavId].Click()),
				new TestStep(() => pages.Home.IsDisplayed())

			};
			testExecutor.Execute(testSteps, Results);
		}

		private void Goto_UnauthorizedPage_LoginPage()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.Users.Goto()),
				new TestStep(() => pages.Login.IsDisplayed()),

				new TestStep(() => pages.Home.Goto()),
				new TestStep(() => pages.PostMaintenance.Goto()),
				new TestStep(() => pages.Login.IsDisplayed()),

				new TestStep(() => pages.Home.Goto()),
				new TestStep(() => pages.Activity.Goto()),
				new TestStep(() => pages.Login.IsDisplayed())
			};
			testExecutor.Execute(testSteps, Results);
		}
		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
