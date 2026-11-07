using System;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public class SiteConfigFileService : IConfigFileService<SiteConfig>
	{
		private readonly IAtomicFileWriter atomicFileWriter;
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<SiteConfigFileService> logger;

		public SiteConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor,
			ILogger<SiteConfigFileService> logger, IAtomicFileWriter atomicFileWriter)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
			this.atomicFileWriter = atomicFileWriter;
		}

		public bool SaveConfig(SiteConfig config)
		{
			try
			{
				var serializer = new XmlSerializer(typeof(SiteConfig));
				var namespaces = new XmlSerializerNamespaces();
				namespaces.Add("", "");

				atomicFileWriter.Write(options.SiteConfigFilePath,
					stream => serializer.Serialize(stream, config, namespaces));
				return true;
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Failed to save site configuration to {Path}", options.SiteConfigFilePath);
				throw;
			}
		}
	}
}
