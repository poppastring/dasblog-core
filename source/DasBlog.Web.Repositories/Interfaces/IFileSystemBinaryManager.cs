using System.IO;

namespace DasBlog.Managers.Interfaces
{
	public interface IFileSystemBinaryManager
	{
		string SaveFile(Stream inputFile, string fileName);
	}
}
