using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Services.FileManagement
{
	public class ConfigFilePathsDataOption
	{
		public string MetaConfigFilePath { get; set; }

		public string SiteConfigFilePath { get; set; }

		public string SecurityConfigFilePath { get; set; }

		public string IISUrlRewriteFilePath { get; set; }
	}
}
