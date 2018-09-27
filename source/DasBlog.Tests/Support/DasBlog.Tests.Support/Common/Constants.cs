namespace DasBlog.Tests.Support.Common
{
	public static class Constants
	{
		public const int ScriptTimeout = 5_000;
		public const int ScriptProcessFailedToRunCode = int.MaxValue;
		public const int ScriptTimedOutCode = int.MaxValue - 1;
		public const string DasBlogTestScriptTimeout = "DAS_BLOG_TEST_SCRIPT_TIMEOUT";
		public const int DefaultScriptTimeout = 5_000;
		public const string ScriptsRelativePath = "source/DasBlog.Tests/Support/DasBlog.Tests.Support/Scripts/";
		public const string VanillaTestData = "source/DasBlog.Tests/Resources/Environments/Vanilla";
		public const string DasBlogGitRepo = "DAS_BLOG_GIT_REPO";
				// e.g. c:/projects/dasblog-core
				// note this is the local repo not the remote one.
		public const string DasBlogProjectRootRelativeToAssemblies = "../../../../../../";
			// e.g. difference between c:/projects/dasblog-core and
			//						   c:/projects/dasblog-core/source/DasBlog.Tests/some-project/bin/debug/netcoreapp2.1

	}
}
