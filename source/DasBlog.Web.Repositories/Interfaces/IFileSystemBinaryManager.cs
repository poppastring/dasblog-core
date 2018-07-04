namespace DasBlog.Managers.Interfaces
{
	public interface IFileSystemBinaryManager
	{
		string SaveFile(System.IO.Stream inputFile, ref string fileName);
	}
}
