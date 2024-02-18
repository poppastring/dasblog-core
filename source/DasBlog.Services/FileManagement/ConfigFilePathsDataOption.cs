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

		public string ThemesFolder { get; set; }

		public string BinaryFolder { get; set; }

		public string BinaryUrlRelative { get; set; }
		public string OEmbedProvidersFilePath { get; set; }
	}
}
