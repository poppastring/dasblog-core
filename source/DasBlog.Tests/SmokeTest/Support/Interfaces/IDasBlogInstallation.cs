namespace DasBlog.Tests.SmokeTest.Support.Interfaces
{
	public interface IDasBlogInstallation
	{
		void Init();
		void Terminate();
		string GetConfigPathAndFile();
		string GetContentDirectoryPath();
		string GetLogDirectoryPath();
		string GetWwwRootDirectoryPath();
	}
}
