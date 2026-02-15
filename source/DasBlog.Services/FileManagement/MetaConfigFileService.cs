using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement
{
	public class MetaConfigFileService : IConfigFileService<MetaTags>
	{
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<MetaConfigFileService> logger;

		public MetaConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor, ILogger<MetaConfigFileService> logger)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
		}

		public bool SaveConfig(MetaTags config)
		{
			var ser = new XmlSerializer(typeof(MetaTags));
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var writer = new StreamWriter(options.MetaConfigFilePath))
			{
				try
				{
					ser.Serialize(writer, config, ns);

					return true;
				}
				catch (Exception e)
				{
					logger.LogError(e, "Failed to save meta tags configuration to {Path}", options.MetaConfigFilePath);
					throw;
				}
			}
		}
	}
}
