using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using DasBlog.Services.Site;
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
		private readonly IConfigFileService<OEmbedProviders> oembedProvidersService;
		private readonly IConfigFileService<SiteConfig> siteConfigFileService;
		private readonly ConfigFilePathsDataOption options;

		public FileSystemBinaryManager(IDasBlogSettings dasBlogSettings, IConfigFileService<MetaTags> metaTagFileService,
										 IConfigFileService<OEmbedProviders> oembedProvidersService,
										IConfigFileService<SiteConfig> siteConfigFileService, IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.metaTagFileService = metaTagFileService;
			this.oembedProvidersService = oembedProvidersService;
			this.siteConfigFileService = siteConfigFileService;;
			options = optionsAccessor.Value;

			Uri physBinaryPathUrl;

			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.Root))
			{
				physBinaryPathUrl = new Uri(dasBlogSettings.RelativeToRoot(options.BinaryUrlRelative));
			}
			else
			{
				physBinaryPathUrl = new Uri(new Uri(SiteHttpContext.AppBaseUrl), options.BinaryUrlRelative);
			}

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			var cdnManager = CdnManagerFactory.GetService(dasBlogSettings.SiteConfiguration.CdnFrom, dasBlogSettings.SiteConfiguration.CdnTo);

			this.binaryDataService = BinaryDataServiceFactory.GetService(options.BinaryFolder, physBinaryPathUrl, loggingDataService, cdnManager);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			return binaryDataService.SaveFile(inputFile, ref fileName);
		}

		public bool SaveMetaConfig(MetaTags config)
		{
			return metaTagFileService.SaveConfig(config);
		}

		public bool SaveOEmbedProviders(OEmbedProviders providers)
		{
			return oembedProvidersService.SaveConfig(providers);
		}

		public bool SaveSiteConfig(SiteConfig config)
		{
			config.ValidCommentTags[0].Name = "0";
			return siteConfigFileService.SaveConfig(config);
		}
	}
}
