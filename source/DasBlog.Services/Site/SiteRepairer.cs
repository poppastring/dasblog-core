using System;
using System.IO;

namespace DasBlog.Services.Site
{
	public class SiteRepairer : ISiteRepairer
	{
		private string _binariesPath = "";
		public SiteRepairer(IDasBlogSettings dasBlogSettings)
		{
			_binariesPath = Path.Combine(dasBlogSettings.WebRootDirectory,
								dasBlogSettings.SiteConfiguration.BinariesDir.TrimStart('~', '/'));
		}
		public (bool result, string errorMessage) RepairSite()
		{
			try
			{
				if (!Directory.Exists(_binariesPath))
				{
					// TODO log activity
					Directory.CreateDirectory(_binariesPath);
				}

				return (true, "");
			}
			catch (Exception e)
			{
				// TODO log error and cause error page to be shown
				return (false, $"Failed to start service.  Failed to create {_binariesPath}.  Detail: {e.Message}");
			}
		}
	}
}
