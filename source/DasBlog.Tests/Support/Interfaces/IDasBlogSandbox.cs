namespace DasBlog.Tests.Support.Interfaces
{
	public interface IDasBlogSandbox
	{
		/// <summary>
		/// Ensures that the file system is clean and ready for tests
		/// </summary>
		/// <throws exception="Exception">if the environment is not in good order</throws>
		void Init();
		void Terminate();
		string GetConfigPathAndFile();
		string GetContentDirectoryPath();
		string GetLogDirectoryPath();
		string GetWwwRootDirectoryPath();
		string TestEnvironmentPath { get; }
	}
}
