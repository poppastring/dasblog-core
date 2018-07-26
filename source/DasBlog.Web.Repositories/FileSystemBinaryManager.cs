using System;
using System.IO;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers
{
	public class FileSystemBinaryManager : IFileSystemBinaryManager
	{
		private IBinaryDataService _dataService;
		private ILoggingDataService _loggingDataService;
		private ISiteSecurityManager _siteSecurity;
		private string _virtBinaryPathRelativeToContentRoot;
		public FileSystemBinaryManager(IDasBlogSettings settings)
		{
			var siteConfig = settings.SiteConfiguration;
			_virtBinaryPathRelativeToContentRoot = siteConfig.BinariesDir.TrimStart('~'); // => "/content/binary"
			string physBinaryPath = Path.Combine(settings.WebRootDirectory,_virtBinaryPathRelativeToContentRoot.TrimStart('/'));  // => "c:\...\DaBlog.Web.UI\content/binary"
					// WebRootDirectory is a misnomer.  It should be called ContentRootDirectory
					// ContentRootDirectory is not "c:\...\DasBlog.Web.UI\contnet".  It is actually "c:\...\DasBlog.Web.UI"
					// WebRootDirectory is "wwwroot".
			Uri physBinaryPathUrl = new Uri(physBinaryPath);
			_loggingDataService = LoggingDataServiceFactory.GetService(settings.WebRootDirectory + settings.SiteConfiguration.LogDir);
			_dataService = BinaryDataServiceFactory.GetService(physBinaryPath, physBinaryPathUrl ,_loggingDataService);
		}

		public string SaveFile(Stream inputFile, string fileName)
		{
			_dataService.SaveFile(inputFile, ref fileName);
			string newPathAndFileName = fileName;			// "c:\...\DasBlog.Web.UI\content/bianry/my-image.jpg"
			string newFileName = Path.GetFileName(newPathAndFileName);		// might be same as original or maybe different
			return Path.Combine(_virtBinaryPathRelativeToContentRoot, newFileName);  // s/be "/Content/binary/mypic-etc.jpg"
		}
	}
}
