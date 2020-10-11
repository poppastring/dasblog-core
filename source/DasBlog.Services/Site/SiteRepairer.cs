using System;
using System.IO;

namespace DasBlog.Services.Site
{
	public class SiteRepairer : ISiteRepairer
	{
		private readonly string binariesPath = string.Empty;
		private readonly string themeFolder = string.Empty;
		private readonly string radioStoriesFolder = string.Empty;

		public SiteRepairer(IDasBlogSettings dasBlogSettings)
		{
			binariesPath = new DirectoryInfo(Path.Combine(dasBlogSettings.WebRootDirectory,
								dasBlogSettings.SiteConfiguration.BinariesDir.TrimStart('~', '/'))).FullName;

			themeFolder = new DirectoryInfo(Path.Combine(dasBlogSettings.WebRootDirectory, "Themes",
								dasBlogSettings.SiteConfiguration.Theme)).FullName;

			radioStoriesFolder = new DirectoryInfo(Path.Combine(dasBlogSettings.WebRootDirectory, "content/radioStories")).FullName;
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
