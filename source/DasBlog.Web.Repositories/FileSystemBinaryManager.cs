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
		public FileSystemBinaryManager(IDasBlogSettings settings)
		{
			var siteConfig = settings.SiteConfiguration;
			virtBinaryPathRelativeToContentRoot = siteConfig.BinariesDir.TrimStart('~'); // => "/content/binary"
			var physBinaryPath = Path.Combine(settings.WebRootDirectory,virtBinaryPathRelativeToContentRoot.TrimStart('/')); 

			Uri physBinaryPathUrl = new Uri(physBinaryPath);
			var loggingDataService = LoggingDataServiceFactory.GetService(settings.WebRootDirectory + settings.SiteConfiguration.LogDir);
			dataService = BinaryDataServiceFactory.GetService(physBinaryPath, physBinaryPathUrl ,loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			dataService.SaveFile(inputFile, ref fileName);
			var newPathAndFileName = fileName;
			var newFileName = Path.GetFileName(newPathAndFileName);

			return Path.Combine(virtBinaryPathRelativeToContentRoot, newFileName);
		}
	}
}
