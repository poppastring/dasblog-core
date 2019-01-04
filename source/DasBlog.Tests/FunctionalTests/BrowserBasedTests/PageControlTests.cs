using System;
using System.Collections.Generic;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	public class PageControlTests : BrowserBasedTestsBase
	{
		public PageControlTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
			: base(testOutputHelper, browserTestPlatform)
		{
		}
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void Browse_OnHomeScreen_ShowsPageControlNextOnly()
		{
			try
			{
				var dp = platform.CreateTestDataProcessor();
				dp.SetSiteConfigValue("EnableComments", "true");
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
		
	}
}
