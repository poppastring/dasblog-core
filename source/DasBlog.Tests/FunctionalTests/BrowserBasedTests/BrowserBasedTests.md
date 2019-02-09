#### Browser Based Tests

##### Usage
change directory to the project directory (containing .git) usually called dasblog-core and do
```
dotnet test source/DasBlog.Tests/FunctionalTests --logger trx;LogfileName=browsser_based_test_results.xml --results-directory ./test_results --filter Category=BrowserBasedTest
``` 
For failing tests the logs will be printed along with the results.  If you need to inspect the logs for passing tests
the the location  is `source/DasBlog.Tests/FunctioalTests/test_results/browser_based_test_results.xml`.  You will
need some sort of tool to format them as the results are fairly unreadable in their raw form.

[DasBlog.Tests/FunctionalTests/BrowserBasedTests/appsettings.json](appsettings.json) can be used to set log levels.  Note that this
overrides the settings in [DasBlog.Tests/FunctionalTests/appsettings.json](../appsettings.json).

The following code fragment shows a typical browser based test in a typical test class.

##### Anatomy of a Browser Based Test
```
using System;
using System.Collections.Generic;
using System.Threading;
using DasBlog.Core.XmlRpc.Blogger;
using DasBlog.Tests.Automation.Selenium;    // (1)
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests           // (2)
{
	[Collection(Constants.TestInfrastructureUsersCollection)]       // (3)
	public class PrototypeBrowserBasedTests : IClassFixture<BrowserTestPlatform>    // (4)
	{

		private BrowserTestPlatform platform;
		private ITestOutputHelper testOutputHelper;
		private ILogger<PrototypeBrowserBasedTests> logger;
		public PrototypeBrowserBasedTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform) // (5)
		{
			browserTestPlatform.CompleteSetup(testOutputHelper);    // (6)
			this.platform = browserTestPlatform;
			LoggerValidator.Validate(platform.ServiceProvider);     // (7)
			this.testOutputHelper = testOutputHelper;
			this.logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PrototypeBrowserBasedTests>();
		}

		[Fact(Skip="")]         // (8)
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]     // (9)
		public void LoggingIn_WithBlankPassword_ShowsErrorMessage()
		{
			try
			{
				logger.LogError("logging starts here");
				List<TestStep> testSteps = new List<TestStep>   // (10)
				{
					new ActionStep(() => platform.Pages.LoginPage.Goto()),      // (11)
					new VerificationStep(() => platform.Pages.LoginPage.IsDisplayed()),     // (12)
					new VerificationStep(() => platform.Pages.LoginPage.LoginButton != null),   // (13)
					new ActionStep(() => platform.Pages.LoginPage.LoginButton.Click()),         // (14)
					new VerificationStep(() =>                                                  // (15)
						platform.Pages.LoginPage.PasswordValidation.Text.ToLower().Contains("the password field is required")),
					new VerificationStep(() => platform.Pages.LoginPage.IsDisplayed())          // (16)
				};
				var results = new TestResults();                    // (17)
				platform.TestExecutor.Execute(testSteps, results);  // (18)
				platform.Publisher.Publish(results.Results);        // (19)
				Assert.True(results.TestPassed);                    // (20)
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
```

1. Browser based tests use [Selenium](../../../SeleniumPlusDasBlogCoreInACoupleOfPages.md) to drive the browser.
2. The namespace name is of no significance but the directory hierarchy should be retained so that documentation links
continue to work.
3. All browser based test classes should be annotated with the `TestInfrastructureUsersCollection` `[Collection]` attribute
This ensures that they will not run in parallel with other test classes such as the ComponentTest classes as all these
tests share the test data sand box.
4. All browser based tests should share the `BrowserTestPlatform` class fixture as they all depend on the same
Selenium mechanism and dasblog-core server instance.
5. The constructor takes an implementation of XUnit's `ITestOutputHelper` in order that a logging provider can be set up.
In addition an instance of the `BrowserTestPlatform` is passed in.  There is one of these for all browser test classes.
It is responsible for setting up Selenium and running the web app.  It is not known whether the same instance of
`ITestOutputHelper` is passed in each time but only the first instance encountered is actually passed to the logger
factory so there could be issues there if there are changes to XUnit's approach.4. `TestSupportPlatform.CompleteSetup()` must be called to ensure that logging is activated correctly.
6. `TestSupportPlatform.CompleteSetup()` must be called to ensure that logging is activated correctly.
7. `LoggerValidator.Validate()` is a utility method that can be used to diagnose logging problems.
8. The tests are standard XUnit tests.
9. Use the Unit-of-work_State-under-test_Expected-behaviour naming convention.
10. A list of test steps is built.  Each step comprises either some action `ActionStep` which changes the state of the browser
or checks the outcome of a previous step `VerificationStep`.
11. `LoginPage.Goto` displays the login page.
12. `LoginPage.IsDisplayed()` checks that the browser is displaying the login page.
13. `LoginPage.LoginButton != null` checks that the page contains the login button.
14. `LoginPage.LoginButton.Click()` causes the login button to be clicked.
15. This step checks that the appropriate messate is displayed.
16. `LoginPage.IsDisplayed()` ensures that the login page is still displayed by the browser.
17. Next an object is created to contain the results.
18. The steps are executed.  If any step fails (an action step throws an exception or a verification step returns false)
then that test is halted.
19. Results are displayed in the log as shown below.
20. `results` exposes a boolean indicating whether the test was successful.

All tests run against the same server process.

##### Results
The logs show either that all test steps were executed successfuly (Passed == True)

```
DasBlog.Tests.Support.Publisher [0] Test Results
DasBlog.Tests.Support.Publisher [0] Test Passed Failed Step
DasBlog.Tests.Support.Publisher [0]            ============================== ===== =====================================================
DasBlog.Tests.Support.Publisher [0]              Goto_HomePage_DisplaysNavBar True 
DasBlog.Tests.Support.Publisher [0]
```

or that the test step which failed.  Note that the XUnit assertion does not include
the reason for failure - you must consult the logs for that.

```
Assert.True() Failure
Expected: True
Actual:   False
....
DasBlog.Tests.Support.Publisher [0] Test Results
DasBlog.Tests.Support.Publisher [0]                                                         Test Passed Failed Step
DasBlog.Tests.Support.Publisher [0]                               ============================== ====== ====================================================
DasBlog.Tests.Support.Publisher [0]                    LogIn_WithBlankPassword_ShowsErrorMessage False  () => value(DasBlog.Tests.FunctionalTests.BrowserBasedTests.PrototypeBrowserBasedTests).platform.Pages.LoginPage.PasswordValidation.Text.ToLower().Contains("the typo password field is required")
DasBlog.Tests.Support.Publisher [0]
```

##### Logs
"DasBlog.Web says" indicates log output from the web server

There is no output from the geckodriver.exe (not sure why not)

Sample Log
```
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Request starting HTTP/1.1 GEThttp://localhost:5000/page 
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Route matched with {action = "Page",controller = "Home"}. Executing action DasBlog.Web.Controllers.HomeController.Page (DasBlog.Web)
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Executing action methodDasBlog.Web.Controllers.HomeController.Page (DasBlog.Web) - Validation state: Valid
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: dbug: DasBlog.Managers.BlogManager[0]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: pass £££oldExpression = '() =&gt;value(DasBlog.Managers.BlogManager).dasBlogSettings.GetContentLookAhead()' oldResult = '29/10/201811:53:14', newExpression = '() =&gt;GetContentLookAhead(value(DasBlog.Managers.BlogManager).opts.ContentLookaheadDays)' newResult ='29/10/2018 11:53:14'[[[
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: dbug: DasBlog.Managers.BlogManager[0]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: pass £££oldExpression = '() =&gt;value(DasBlog.Managers.BlogManager).dasBlogSettings.GetConfiguredTimeZone()' oldResult = 'UTC',newExpression = '() =&gt;GetConfiguredTimeZone(value(DasBlog.Managers.BlogManager).opts.AdjustDisplayTimeZone,value(DasBlog.Managers.BlogManager).opts.DisplayTimeZoneIndex)' newResult = 'UTC'[[[
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: dbug: DasBlog.Managers.BlogManager[0]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: pass £££oldExpression = '() =&gt;value(DasBlog.Managers.BlogManager).dasBlogSettings.SiteConfiguration.FrontPageDayCount' oldResult ='5', newExpression = '() =&gt; value(DasBlog.Managers.BlogManager).opts.FrontPageEntryCount'newResult = '5'[[[
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: dbug: DasBlog.Managers.BlogManager[0]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: pass £££oldExpression = '() =&gt;value(DasBlog.Managers.BlogManager).dasBlogSettings.SiteConfiguration.FrontPageEntryCount' oldResult= '5', newExpression = '() =&gt; value(DasBlog.Managers.BlogManager).opts.FrontPageEntryCount'newResult = '5'[[[
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: dbug:DasBlog.Web.Controllers.HomeController[0]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: In Index - 1 post found
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Executed action methodDasBlog.Web.Controllers.HomeController.Page (DasBlog.Web), returned resultMicrosoft.AspNetCore.Mvc.ViewResult in 7.6641ms.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.ViewFeatures.ViewResultExecutor[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Executing ViewResult, running view Page.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Authorization.DefaultAuthorizationService[1]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Authorization was successful.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.ViewFeatures.ViewResultExecutor[4]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Executed ViewResult - view Page executedin 2.7012ms.
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker[2]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Executed actionDasBlog.Web.Controllers.HomeController.Page (DasBlog.Web) in 10.6077ms
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: info:Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
DasBlog.Tests.Support.WebServerRunner [0] DasBlog.Web says: Request finished in 14.8671ms 200text/html; charset=utf-8
DasBlog.Tests.Support.Publisher [0] Test Results
DasBlog.Tests.Support.Publisher [0] Test Passed Failed Step
DasBlog.Tests.Support.Publisher [0] ============================== ==========================================================
DasBlog.Tests.Support.Publisher [0] Goto_HomePage_DisplaysNavBar True 
DasBlog.Tests.Support.Publisher [0]
```

##### Detail
Automated browser based tests exercise the web app end-to-end just as interactive
manual use of the web app. does.

The test mechanism employs [Selenium](../../../SeleniumPlusDasBlogCoreInACoupleOfPages.md).

All browser based tests must use the same test data environment as they rely on the web app running continuously
in a separate process.

##### Configuration
One approach to inspecting and manipulating the Vanilla environment (used by the browser based tests)
is to point the web app at the Vanilla environment and run it in normal interactive mode.

To change the data environment set up the following environment variable:
```
DAS_BLOG_DATA_ROOT=<projects>/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla
```

Remember to commit all changes in the Environments directory tree and remove any un-tracked files created 
such as the logs to avoid subsequent tests failing
