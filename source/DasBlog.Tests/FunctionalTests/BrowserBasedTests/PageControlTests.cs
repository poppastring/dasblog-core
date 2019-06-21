using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	[Trait("Category", "SkipWhenLiveUnitTesting")]

	public class PageControlTests : BrowserBasedTestsBase
	{
		public PageControlTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
			: base(testOutputHelper, browserTestPlatform)
		{
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void Browse_OnHomePage_ShowsPageControlNextOnly()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Pages.HomePage.Goto()),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NextPageLink != null),
					new VerificationStep(() => platform.Pages.HomePage.PreviousPageLink == null),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void Browse_Page1_ShowsPageControlNextAndPRev()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("page/1")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NextPageLink != null),
					new VerificationStep(() => platform.Pages.HomePage.PreviousPageLink != null),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void Browse_EmptyPage_ShowsPageControlPRevOnly()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("page/100")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NextPageLink == null),
					new VerificationStep(() => platform.Pages.HomePage.PreviousPageLink != null),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void ClickOlderPosts_OnPage1_GoesToPage2()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("page/1")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NextPageLink != null),
					new ActionStep(() => platform.Pages.HomePage.NextPageLink.Click()),
					new VerificationStep(() => CheckBrowserUrlPageNumber(platform.Browser.GetUrl(), "2")),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void ClickNewerPosts_OnPage2_GoesToPage1()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("page/2")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.PreviousPageLink != null),
					new ActionStep(() => platform.Pages.HomePage.PreviousPageLink.Click()),
					new VerificationStep(() => CheckBrowserUrlPageNumber(platform.Browser.GetUrl(), "1")),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
		/// <summary>
		/// subrougine of tests
		/// </summary>
		/// <param name="Url">e.g. http://localhost:5000/page/2</param>
		/// <param name="pageNumber">"2"</param>
		/// <returns>false if badly formed or last segment is other than 2</returns>
		private bool CheckBrowserUrlPageNumber(string Url, string pageNumber)
		{
			var parts = Url.Split("/");
			if (parts.Length == 0)
				return false;
			return parts[parts.Length - 1] == pageNumber;
		}
	}

	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class EmptyPageControlTests : BrowserBasedTestsBase
	{
		public EmptyPageControlTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
			: base(testOutputHelper, browserTestPlatform)
		{
		}

		[Fact(Skip = "")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue)]
		public void Browse_EmptyPage_ShowsNoPageControls()
		{
			try
			{
				DasBlog.Tests.FunctionalTests.Common.Utils.DeleteDirectoryContentsDirect(
					Path.Combine(platform.Sandbox.TestEnvironmentPath, "content"));
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Pages.HomePage.Goto()),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NextPageLink == null),
					new VerificationStep(() => platform.Pages.HomePage.PreviousPageLink == null),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				_ = e;
				throw;
			}
			finally
			{
			}
		}
	}

}
