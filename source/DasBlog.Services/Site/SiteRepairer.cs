using System;
using System.IO;

namespace DasBlog.Services.Site
{
	public class SiteRepairer : ISiteRepairer
	{
		private string binariesPath = string.Empty;
		private string themeFolder = string.Empty;

		public SiteRepairer(IDasBlogSettings dasBlogSettings)
		{
			binariesPath = new DirectoryInfo(Path.Combine(dasBlogSettings.WebRootDirectory,
								dasBlogSettings.SiteConfiguration.BinariesDir.TrimStart('~', '/'))).FullName;

			themeFolder = new DirectoryInfo(Path.Combine(dasBlogSettings.WebRootDirectory, "Themes",
								dasBlogSettings.SiteConfiguration.Theme)).FullName;
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

				return (true, "");
			}
			catch (Exception e)
			{
				return (false, $"Failed to start service.  Failed to create {binariesPath}.  Detail: {e.Message}");
			}
		}
	}
}
