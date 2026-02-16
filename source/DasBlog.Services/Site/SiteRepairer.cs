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
			string currentFolder = null;
			try
			{
				foreach (var folder in new[] { binariesPath, themeFolder, radioStoriesFolder })
				{
					currentFolder = folder;
					if (!Directory.Exists(folder))
					{
						Directory.CreateDirectory(folder);
					}
				}

				return (true, string.Empty);
			}
			catch (Exception e)
			{
				return (false, $"Failed to start service. Failed to create {currentFolder}. Detail: {e.Message}");
			}
		}
	}
}
