using System;
using System.IO;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.Interfaces;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace DasBlog.Managers
{
	public class FileSystemBinaryManager : IFileSystemBinaryManager
	{
		private IBinaryDataService _dataService;
		private ILoggingDataService _loggingDataService;
		private ISiteSecurityManager _siteSecurity;
		private readonly IDasBlogSettings _dasBlogSettings;

		public FileSystemBinaryManager(IDasBlogSettings settings)
		{
			_dasBlogSettings = settings;
			var siteConfig = _dasBlogSettings.SiteConfiguration;
			string binaryPath = MapPath(siteConfig.BinariesDir, _dasBlogSettings.WebRootDirectory);
			Uri binaryRootUrl = new Uri(binaryPath);
			_loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
			_dataService = BinaryDataServiceFactory.GetService(binaryPath, binaryRootUrl ,_loggingDataService);
		}

		private string MapPath(string binariesPath, string webRootDirectory)
		{
			string trimmedRelative = binariesPath.TrimStart('~', '/');
			return Path.Combine(webRootDirectory, trimmedRelative);
		}

		public string SaveFile(Stream inputFile, ref string fileName)
		{
/*
			string type = fileInput.PostedFile.ContentType;
			numBytes = fileInput.PostedFile.InputStream.Length;

			SharedBasePage requestPage = this.Page as SharedBasePage;

			string postedFileName = Path.GetFileName(fileInput.PostedFile.FileName);

			string filename = Path.Combine(entryId ?? "", postedFileName);

*/
			string savedFileName;
			string absUrl = _dataService.SaveFile(inputFile, ref fileName);
			savedFileName = Path.GetFileName(fileName);

			return absUrl;
		}
	}
}
