using System;
using System.Collections.Generic;
using System.Threading;
using DasBlog.Core.XmlRpc.Blogger;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

/*
 * THERE IS NO logging to the console - in the debugger the log appears in the detail of the test results
 * when run from the console the log appears in a log file assuming you provide the correct command line
 * dotnet xunit cli is required to get console output
 * If I do "dotnet xunit -diagnostics" it barfs with a reference to a missing Microsoft.Extensions.Options
 */
namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	public class BrowserOptionsAccessor : IOptions<BrowserOptions>
	{
		public BrowserOptionsAccessor(BrowserOptions opts)
		{
			Value = opts;
		}
		public BrowserOptions Value { get; }
	}
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class PrototypeBrowserBasedTests : IClassFixture<BrowserTestPlatform>
	{

		private BrowserTestPlatform platform;
		private ITestOutputHelper testOutputHelper;
		private ILogger<PrototypeBrowserBasedTests> logger;
		public PrototypeBrowserBasedTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
		{
//			testOutputHelper.WriteLine("hello from browser constructor");
					// the above message and others like it appear in the detail pane of Rider's test runner for
					// Running a successful test
					// Debugging a successful test
					// Running a failed test
					// Debugging a failed test
					// logger under for Run and Debug
			// I want to get hold of the xunit test results when running from the command line.
			// "dotnet test" isn't providing them as far as I can see from my limited investigations
			// Even after specifying RuntimeFramework as 2.1.4 in the csproj I got
			// "dotnet xunit" should be more fruitful but fails with the following issue:
			//   Could not load file or assembly 'Microsoft.Extensions.Options, Version=2.1.0.0,
			// There is a nuget package for M.E.O 2.1.1 but the install fails and is rolled back
			// There are no other v2 nuget packages except a preview 2.2.
			// "dotnet xunit" seems to be a no-go - did not investigate whether it solved
			// the original problem of locating and using test results.
			// 
			// It turns out that the following is not a bad start:
			// "dotnet test --logger trx;LogfileName=test_results.xml --results-directory ./test_results"
			//			test_results.xml will appear in <proj>/source/DasBlog.Tests/FunctionalTests/test_results
			browserTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = browserTestPlatform;
			LoggerValidator.Validate(platform.ServiceProvider);
			this.testOutputHelper = testOutputHelper;
			this.logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PrototypeBrowserBasedTests>();
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void Test1()
		{
			Assert.True(true);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue)]
		public void Goto_HomePage_DisplaysNavBar()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Pages.HomePage.Goto()),
					new VerificationStep(() => platform.Pages.NavBar.IsDisplayed()),
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
		public void LogIn_WithBlankPassword_ShowsErrorMessage()
		{
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Pages.LoginPage.Goto()),
					new VerificationStep(() => platform.Pages.LoginPage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.LoginPage.PasswordTextBox != null),
					new ActionStep(() => platform.Pages.LoginPage.PasswordTextBox.SetText(string.Empty)),
					new VerificationStep(() => platform.Pages.LoginPage.LoginButton != null),
					new ActionStep(() => platform.Pages.LoginPage.LoginButton.Click()),
					new VerificationStep(() =>
						platform.Pages.LoginPage.PasswordValidation.Text.ToLower().Contains("the password field is required")),
					new VerificationStep(() => platform.Pages.LoginPage.IsDisplayed())
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
		public void Login_WithValidCredentials_ShowsHomePage()
		{
			Thread.Sleep(5000);
			try
			{
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Pages.LoginPage.Goto()),
					new VerificationStep(() => platform.Pages.LoginPage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.LoginPage.LoginButton != null),
					new VerificationStep(() => platform.Pages.LoginPage.EmailTextBox != null),
					new ActionStep(() => platform.Pages.LoginPage.EmailTextBox.SetText( "myemail@myemail.com")),
					new VerificationStep(() => platform.Pages.LoginPage.PasswordTextBox != null),
					new ActionStep(() => platform.Pages.LoginPage.PasswordTextBox.SetText("admin")),
					new ActionStep(() => platform.Pages.LoginPage.LoginButton.Click()),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed())
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
