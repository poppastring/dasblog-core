using System.IO;
using DasBlog.Services.ConfigFile.Interfaces;

namespace DasBlog.Managers.Interfaces
{
	public interface IFileSystemBinaryManager
	{
		string SaveFile(Stream inputFile, string fileName);

		bool SaveSiteConfig(ISiteConfig config);

		bool SaveMetaConfig(IMetaTags config);
	}
}
