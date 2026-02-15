using System.IO;
using DasBlog.Services.ConfigFile;

namespace DasBlog.Managers.Interfaces
{
	public interface IFileSystemBinaryManager
	{
		string SaveFile(Stream inputFile, string fileName);

		bool SaveSiteConfig(SiteConfig config);

		bool SaveMetaConfig(MetaTags config);
	}
}
