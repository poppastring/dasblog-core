namespace DasBlog.Tests.Support.Common
{
	public static class Constants
	{
		public const int ScriptProcessFailedToRunCode = int.MaxValue;
		public const int ScriptTimedOutCode = int.MaxValue - 1;
		// this is the amount of time the host (functional test) will wait around
		// before pulling the plug on cmd.exe
		public const string DasBlogTestScriptTimeout = "DAS_BLOG_TEST_SCRIPT_TIMEOUT";
		public const int DefaultScriptTimeout = 20_000;
		// this is the amount of time the script will linger so that
		// the host process can gather any output
		public const string DasBlogTestScriptExitTimeout = "DAS_BLOG_TEST_SCRIPT_EXIT_TIMEOUT";
		public const int DefaultScriptExitTimeout = 100;
		public const string ScriptsRelativePath = "source/DasBlog.Tests/Support/Scripts/";
		public const string VanillaTestData = "source/DasBlog.Tests/Resources/Environments/Vanilla";
		public const string TestDataDirectory = "source/DasBlog.Tests/Resources/Environments";
		public const string FunctionalTestsRelativeToProject = "source/DasBlog.Tests/FunctionalTests";
		public const string ComponentTestsRelativeToProject = "source/DasBlog.Tests/FunctionalTests/ComponentTests";
		public const string TestInfrastructureTestsRelativeToProject = "source/DasBlog.Tests/FunctionalTests/TestInfrastructureTests";
		public const string BrowserBasedTestsRelativeToProject = "source/DasBlog.Tests/FunctionalTests/BrowserBasedTests";
		public const string DasBlogGitRepo = "DAS_BLOG_GIT_REPO";
				// e.g. c:/projects/dasblog-core
				// note this is the local repo not the remote one.
		public const string DasBlogProjectRootRelativeToAssemblies = "../../../../../../";
			// e.g. difference between c:/projects/dasblog-core and
			//						   c:/projects/dasblog-core/source/DasBlog.Tests/some-project/bin/debug/netcoreapp2.1
		public const int GitRequiredMajorVersion = 2;
		public const int GitRequiredMinorVersion = 15;
		// ****** script names ******
		public const string DetectChangesScriptName = "DetectChanges.cmd";
		public const string GetGitVersionScriptName = "GetVersion.cmd";
		public const string StashCurrentStateScriptName = "StashCurrentState.cmd";
		public const string ConfirmStashScriptName = "ConfirmStash.cmd";
		// ****** Environments ******
		public const string VanillaEnvironment = "Vanilla";
		public const string LanguageEnvironment = "Language";
		public const string UrlContainsDateEnvironment = "UrlContainsDate";
		// ****** XUnit Traits ******
		public const string CategoryTraitType = "Category";
		public const string TestInfrastructureTestTraitValue = "TestInfrastructureTest";
		public const string UnitTestTraitValue = "UnitTest";
		public const string ComponentTestTraitValue = "ComponentTest";
		public const string IntegrationTestTraitValue = "IntegrationTest";
		public const string BrowserBasedTestTraitValue = "BrowserBasedTest";
		public const string TestInfrastructureUsersCollection = "TestInfrastructureUsers";
		public const string DescriptionTraitType = "Description";
		public const string FailureTraitTraitType = "Failure";
		public const string ApiFailureTraitValue = "API Failure";
				// this implies that although the test fails - the API may not be used currently in the app.
		// ****** DasBlog Content Directories and other path fragments ******
		public const string ConfigDirectory = "Config";
		public const string ContentDirectory = "content";
		public const string LogDirectory = "logs";
		public const string SiteConfigPathFragment = "Config/site.config";
		public const string SiteSecurityConfigPathFragment = "Config/siteSecurity.config";
		// ****** XML Paths ******
		public const string SiteConfigRoot = "/SiteConfig";
		public const string SiteSecurityConfigRoot = "/SiteSecurityConfig";
	}
}
