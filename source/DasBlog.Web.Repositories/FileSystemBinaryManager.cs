using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;

namespace DasBlog.Managers
{
	public class FileSystemBinaryManager : IFileSystemBinaryManager
	{
		private readonly IBinaryDataService binaryDataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IConfigFileService<MetaTags> metaTagFileService;
		private readonly IConfigFileService<SiteConfig> siteConfigFileService;
		private readonly ConfigFilePathsDataOption options;
		private readonly string contentBinaryUrl;

		public FileSystemBinaryManager(IDasBlogSettings dasBlogSettings, IConfigFileService<MetaTags> metaTagFileService, 
										IConfigFileService<SiteConfig> siteConfigFileService, IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.metaTagFileService = metaTagFileService;
			this.siteConfigFileService = siteConfigFileService;
			options = optionsAccessor.Value;
			contentBinaryUrl = dasBlogSettings.RelativeToRoot(options.BinaryUrlRelative);

			var physBinaryPathUrl = new Uri(contentBinaryUrl);

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));

			this.binaryDataService = BinaryDataServiceFactory.GetService(options.BinaryFolder, physBinaryPathUrl, loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			return binaryDataService.SaveFile(inputFile, ref fileName);
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
