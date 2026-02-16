using System;
using System.IO;
using DasBlog.Services.FileManagement;

namespace DasBlog.Services.Site
{
	public class SiteRepairer : ISiteRepairer
	{
		private readonly string binariesPath;
		private readonly string themeFolder;
		private readonly string radioStoriesFolder;

		public SiteRepairer(IDasBlogPathResolver pathResolver)
		{
			binariesPath = pathResolver.BinariesPath;
			themeFolder = pathResolver.ThemeFolderPath;
			radioStoriesFolder = pathResolver.RadioStoriesFolderPath;
		}

		public (bool result, string errorMessage) RepairSite()
		{
			try
			{
				if (!Directory.Exists(binariesPath))
				{
					Directory.CreateDirectory(binariesPath);
				}

				if (!Directory.Exists(themeFolder))
				{
					Directory.CreateDirectory(themeFolder);
				}

				if (!Directory.Exists(radioStoriesFolder))
				{
					Directory.CreateDirectory(radioStoriesFolder);
				}

				return (true, string.Empty);
			}
			catch (Exception e)
			{
				return (false, $"Failed to start service.  Failed to create {binariesPath}.  Detail: {e.Message}");
			}
		}
	}
}
