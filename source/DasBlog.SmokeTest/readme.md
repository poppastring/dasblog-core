#### Smoke Test

##### Usage
Change directory to <project>/source/DasBlog.SmokeTest and do `dotnet run`  In VS make sure you 
set the working directory before running.  The smoke test process starts
and the dasblog-core web app.  The browser driver (e.g. geckodriver.exe is kicked off
which at some stage will invoke the browser process itself)  4 processes in all. The web app is kicked off (on a port of 5000)
and something like the following will show in the console or debug window:



    info: DasBlog.SmokeTest.App
    Current directory at start up is C:\projects\dasblog-core\source\DasBlog.SmokeTest
    info: DasBlog.SmokeTest.App[0]
      Started App
    1532783198757   geckodriver     INFO    geckodriver 0.21.0
    1532783198766   geckodriver     INFO    Listening on 127.0.0.1:65346
    DasBlog.Web says: Hosting environment: Development
    DasBlog.Web says: Content root path: c:\projects\dasblog-core\source\DasBlog.Web.UI
    DasBlog.Web says: Now listening on: http://localhost:5000
    DasBlog.Web says: Application started. Press Ctrl+C to shut down.
    1532783203310   mozrunner::runner       INFO    Running command: "C:\\Program Files\\Mozilla Firefox\\firefox.exe" "-marionette" "-foreground" "-no-remote" "-profile" "C:\\Users\\MIKEDA~1\\AppData\\Local\\Temp\\rust_mozprofile.
    zFA4OckiEGBj"`

The lines staring 'Info' originate rom DasBlog.SmokeTest.  The lines starting with a number
come from the in-process or perhaps the out-of-process Firefox driver and those beginning 
"DasBlog.Web says:" come from the the web app that we know and love.

##### Test Results
After som time you can expect test results to appear in the console or debug window and look something like:

    Test Results
                          Test Passed Failed Step
    ============================== ====== ====================================================
     Login_WithBlankPassword_Fails True
      Click_OnNavBarItem_ShowsPage True

##### Test Restrictions
Tests are intended to operate on the live environment and therefore should make as few
changes as possible to that environment.

* Log files will be updaed with log in attempts.

* Razor files will be compiled

* There should be no other changes.  Tests should not log in and therefore will not be able
to change any live data.

##### Purpose
The smoke test is intended as a sanity check when the app has just been instaalled or is about
to be published.

The smoke test should not be part of CI.

##### Testing Mechanism
For a discussin of the dasblog automation framework see [Selenium Plus DasBlog](../SeleniumPlusDasBlogCoreInACoupleOfPages.md).

There is no 3rd party framework involved in the smoke tests.

Tests are located in the Tester class.  Each test is placed in a separate method of that class.
The naming standard described in DasBlog.Tests/UI should be applied to the functional and smoke
tests:

		public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		{
		}

Optionally the TestExecutor object can be used:

A list of test steps is built where each step is an action or a check (function returning a bool).
For example the first step might be to go to a page and the second step could be to
check that the page is now displayed.

The list of steps is passed to the Execute method of the TestExecutor.

The test is terrminated either when all steps have been executed or when a step
has returned false.

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

The art is to choose steps which will provide good failure messages.  And the secret, as always, is to
find a test that looks like it does the sort of thing you want to do and to copy that.





