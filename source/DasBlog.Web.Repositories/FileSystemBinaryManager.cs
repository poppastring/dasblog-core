using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;

namespace DasBlog.Managers
{
	public class FileSystemBinaryManager : IFileSystemBinaryManager
	{
		private readonly IBinaryDataService dataService;
		private readonly string virtBinaryPathRelativeToContentRoot;
		private readonly IDasBlogSettings dasBlogSettings;
		public FileSystemBinaryManager(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			var siteConfig = this.dasBlogSettings.SiteConfiguration;
			virtBinaryPathRelativeToContentRoot = siteConfig.BinariesDir.TrimStart('~');
			var physBinaryPath = Path.Combine(this.dasBlogSettings.WebRootDirectory,virtBinaryPathRelativeToContentRoot.TrimStart('/')); 

			var physBinaryPathUrl = new Uri(physBinaryPath);
			var loggingDataService = LoggingDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.LogDir);
			dataService = BinaryDataServiceFactory.GetService(physBinaryPath, physBinaryPathUrl ,loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			dataService.SaveFile(inputFile, ref fileName);

			var file = string.Format("{0}{1}", virtBinaryPathRelativeToContentRoot, Path.GetFileName(fileName));

			return dasBlogSettings.RelativeToRoot(file);
		}
	}
}
