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

	}
}
