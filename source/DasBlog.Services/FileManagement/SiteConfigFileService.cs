using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public class SiteConfigFileService : IConfigFileService<SiteConfig>
	{
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<SiteConfigFileService> logger;

		public SiteConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor, ILogger<SiteConfigFileService> logger)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
		}

		public bool SaveConfig(SiteConfig config)
		{

			var ser = new XmlSerializer(typeof(SiteConfig));
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var writer = new StreamWriter(options.SiteConfigFilePath))
			{
				try
				{
					ser.Serialize(writer, config, ns);

					return true;
				}
				catch (Exception e)
				{
					logger.LogError(e, "Failed to save site configuration to {Path}", options.SiteConfigFilePath);
					throw;
				}
			}
		}
	}
}
