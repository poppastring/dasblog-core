using System;
using System.Collections.Generic;
using System.Threading;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
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
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class PrototypeBrowserBasedTests : BrowserBasedTestsBase
	{

		public PrototypeBrowserBasedTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
		  : base(testOutputHelper, browserTestPlatform)
		{
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
		/**
		 * all the AddComment tests currently pass on my Imac/Parallels/Windows 10/1803 installation
		 * but fail on my Surface Pro/Windows 10/1803.  Both with latest of everything as far as I know.
		 * Selenium appears to take no action on the Click() step.
		 *
		 * Note that some of the AddComment tests appear to pass under these circumstances which is misleading.
		 */
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void AddComment_AfterActivatingComments_AddsComment()
		{
			try
			{
				var dp = platform.CreateTestDataProcessor();
				dp.SetSiteConfigValue("EnableComments", "true");
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("post/5125c596-d6d5-46fe-9f9b-c13f851d8b0d/comments")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NameTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.EmailTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.ContentTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.SaveContentButton != null),
					new ActionStep(() => platform.Pages.HomePage.NameTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.EmailTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.ContentTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.SaveContentButton.Click()),
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
		/**
		 * This test is living on borrowed time.  It relies on the fact that the BlogPostController uses
		 * the dasBlogSettings which has EnableComments=true (as it does not respond to config changes
		 * at runtime whereas the BlogManager does honour the runtime config change.
		 * When the controller's config usage becomes responsive to runtime changes then
		 * only the first three steps will be required and the third step will be changed to
		 * verify that the NameTextBox does NOT exist.
		 * 
		 */
		[Fact(Skip="")]
		[Trait(Constants.CategoryTraitType, Constants.BrowserBasedTestTraitValue )]
		public void AddComment_AfterDeactivatingComments_DoesNotAddComment()
		{
			try
			{
				var dp = platform.CreateTestDataProcessor();
				dp.SetSiteConfigValue("EnableComments", "false");
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("post/5125c596-d6d5-46fe-9f9b-c13f851d8b0d/comments")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NameTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.EmailTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.ContentTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.SaveContentButton != null),
					new ActionStep(() => platform.Pages.HomePage.NameTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.EmailTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.ContentTextBox.SetText( "myemail@myemail.com")),
					new ActionStep(() => platform.Pages.HomePage.SaveContentButton.Click()),
					new VerificationStep(() => !platform.Pages.HomePage.IsDisplayed())
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
		public void AddComment_IfBlankForm_DoesNotAddPermanentComment()
		{
			try
			{
				var dp = platform.CreateTestDataProcessor();
				dp.SetSiteConfigValue("EnableComments", "true");
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("post/5125c596-d6d5-46fe-9f9b-c13f851d8b0d/comments")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.NameTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.EmailTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.ContentTextBox != null),
					new VerificationStep(() => platform.Pages.HomePage.SaveContentButton != null),
					new ActionStep(() => platform.Pages.HomePage.NameTextBox.SetText( string.Empty)),
					new ActionStep(() => platform.Pages.HomePage.EmailTextBox.SetText( string.Empty)),
					new ActionStep(() => platform.Pages.HomePage.ContentTextBox.SetText( string.Empty)),
					new ActionStep(() => platform.Pages.HomePage.SaveContentButton.Click()),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed())
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
				var comments = dp.GetDayExtraFileContents(new DateTime(2018, 8, 3)).data;
				Assert.False(comments.HasElements);
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
		public void GotoComments_InLongPost_MakesCommentSectionVisible()
		{
			try
			{
				var dp = platform.CreateTestDataProcessor();
				dp.SetSiteConfigValue("EnableComments", "true");
				List<TestStep> testSteps = new List<TestStep>
				{
					new ActionStep(() => platform.Browser.Goto("post/cbc12a57-d377-4698-93ab-6d4b0622ba6e")),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Pages.HomePage.CommentOnThisPostLink != null),
					new ActionStep(() => platform.Pages.HomePage.CommentOnThisPostLink.Click()),
					new VerificationStep(() => platform.Pages.HomePage.IsDisplayed()),
					new VerificationStep(() => platform.Browser.IsElementVisible(platform.Pages.HomePage.CommentsStartDiv)),
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
				var comments = dp.GetDayExtraFileContents(new DateTime(2018, 8, 3)).data;
				Assert.False(comments.HasElements);
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
