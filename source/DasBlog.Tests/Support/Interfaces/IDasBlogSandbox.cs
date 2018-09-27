namespace DasBlog.Tests.Support.Interfaces
{
	public interface IDasBlogSandbox
	{
		void Init();
		void Terminate();
		string GetConfigPathAndFile();
		string GetContentDirectoryPath();
		string GetLogDirectoryPath();
		string GetWwwRootDirectoryPath();
	}
}
