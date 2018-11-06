using System.Collections.Generic;
using System.Threading;
using DasBlog.Tests.Automation.Dom;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.SmokeTest
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
			Goto_UnauthorizedPage_ShowsLoginPage();
			Thread.Sleep(1000);
		}

		private void Login_WithBlankPassword_Fails()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new ActionStep(() => pages.LoginPage.Goto())
				, new VerificationStep(() => pages.LoginPage.IsDisplayed())
				, new VerificationStep(() => pages.LoginPage.PasswordTextBox != null)
				, new ActionStep(() => pages.LoginPage.PasswordTextBox.SetText(string.Empty))
				, new VerificationStep(() => pages.LoginPage.LoginButton != null)
				, new ActionStep(() => pages.LoginPage.LoginButton.Click())
				, new VerificationStep(() => pages.LoginPage.PasswordValidation.Text.ToLower().Contains("the password field is required"))
				, new VerificationStep(() => pages.LoginPage.IsDisplayed())
			};
			testExecutor.Execute(testSteps, Results);
		}

		private void Click_OnNavBarItem_ShowsPage()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.HomePage.Goto()),
				new TestStep(() => pages.NavBar.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.CategoryNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.CategoryNavId].Click()),
				new TestStep(() => pages.CategoryPage.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.ArchiveNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.ArchiveNavId].Click()),
				new TestStep(() => pages.ArchivePage.IsDisplayed()),
				
				new TestStep(() => pages.NavBar[AppConstants.HomeNavId] != null),
				new TestStep(() => pages.NavBar[AppConstants.HomeNavId].Click()),
				new TestStep(() => pages.HomePage.IsDisplayed())

			};
			testExecutor.Execute(testSteps, Results);
		}

		private void Goto_UnauthorizedPage_ShowsLoginPage()
		{
			List<TestStep> testSteps = new List<TestStep>
			{
				new TestStep(() => pages.UsersPage.Goto()),
				new TestStep(() => pages.LoginPage.IsDisplayed()),

				new TestStep(() => pages.HomePage.Goto()),
				new TestStep(() => pages.PostMaintenancePage.Goto()),
				new TestStep(() => pages.LoginPage.IsDisplayed()),

				new TestStep(() => pages.HomePage.Goto()),
				new TestStep(() => pages.ActivityPage.Goto()),
				new TestStep(() => pages.LoginPage.IsDisplayed())
			};
			testExecutor.Execute(testSteps, Results);
		}
		public void Dispose()
		{
			browser?.Dispose();
		}
	}
}
