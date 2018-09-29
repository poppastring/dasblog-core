namespace DasBlog.SmokeTest
{
	public class GitVersionedFileServiceOptions
	{
		// this is a directory where .git is a sub-directory e.g. "c:/projects/dasblog-core"
		public string GitRepoDirectory { get; set; }
		// this is the root for test data relative to the git repo directory of which
		// it must be a descendant.
		// e.g. "source/DasBlog.Tests/Resources/Environments"
		public string TestDataDirectroy { get; set; }
	}
}
