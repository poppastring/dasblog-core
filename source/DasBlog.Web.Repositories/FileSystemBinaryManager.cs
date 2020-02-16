using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement.Interfaces;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;

namespace DasBlog.Managers
{
	public class FileSystemBinaryManager : IFileSystemBinaryManager
	{
		private readonly IBinaryDataService binaryDataService;
		private readonly string virtBinaryPathRelativeToContentRoot;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IConfigFileService<MetaTags> metaTagFileService;
		private readonly IConfigFileService<SiteConfig> siteConfigFileService;

		public FileSystemBinaryManager(IDasBlogSettings dasBlogSettings, IConfigFileService<MetaTags> metaTagFileService, IConfigFileService<SiteConfig> siteConfigFileService)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.metaTagFileService = metaTagFileService;
			this.siteConfigFileService = siteConfigFileService;

			virtBinaryPathRelativeToContentRoot = dasBlogSettings.SiteConfiguration.BinariesDir;

			var physBinaryPath = Path.Combine(this.dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.BinariesDir); 
			var physBinaryPathUrl = new Uri(physBinaryPath);

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));

			this.binaryDataService = BinaryDataServiceFactory.GetService(physBinaryPath, physBinaryPathUrl, loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			binaryDataService.SaveFile(inputFile, ref fileName);

			var file = string.Format("{0}{1}", virtBinaryPathRelativeToContentRoot, Path.GetFileName(fileName));

			return dasBlogSettings.RelativeToRoot(file);
		}

		public bool SaveMetaConfig(MetaTags config)
		{
			return metaTagFileService.SaveConfig(config);
		}

		public bool SaveSiteConfig(SiteConfig config)
		{
			config.ValidCommentTags[0].Name = "0";
			return siteConfigFileService.SaveConfig(config);
		}
	}
}
