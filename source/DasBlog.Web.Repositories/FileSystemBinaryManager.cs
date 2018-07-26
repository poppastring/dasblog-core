using System;
using System.IO;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;

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
			string physBinaryPath = Path.Combine(settings.WebRootDirectory,virtBinaryPathRelativeToContentRoot.TrimStart('/'));  // => "c:\...\DaBlog.Web.UI\content/binary"
					// WebRootDirectory is a misnomer.  It should be called ContentRootDirectory
					// ContentRootDirectory is not "c:\...\DasBlog.Web.UI\contnet".  It is actually "c:\...\DasBlog.Web.UI"
					// WebRootDirectory is "wwwroot".
			Uri physBinaryPathUrl = new Uri(physBinaryPath);
			var loggingDataService = LoggingDataServiceFactory.GetService(settings.WebRootDirectory + settings.SiteConfiguration.LogDir);
			dataService = BinaryDataServiceFactory.GetService(physBinaryPath, physBinaryPathUrl ,loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			dataService.SaveFile(inputFile, ref fileName);
			string newPathAndFileName = fileName;			// "c:\...\DasBlog.Web.UI\content/bianry/my-image.jpg"
			string newFileName = Path.GetFileName(newPathAndFileName);		// might be same as original or maybe different
			return Path.Combine(virtBinaryPathRelativeToContentRoot, newFileName);  // s/be "/Content/binary/mypic-etc.jpg"
		}
	}
}
