using System;
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
		private readonly IAtomicFileWriter atomicFileWriter;
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<MetaConfigFileService> logger;

		public MetaConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor,
			ILogger<MetaConfigFileService> logger, IAtomicFileWriter atomicFileWriter)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
			this.atomicFileWriter = atomicFileWriter;
		}

		public bool SaveConfig(MetaTags config)
		{
			try
			{
				var serializer = new XmlSerializer(typeof(MetaTags));
				var namespaces = new XmlSerializerNamespaces();
				namespaces.Add("", "");

				atomicFileWriter.Write(options.MetaConfigFilePath,
					stream => serializer.Serialize(stream, config, namespaces));
				return true;
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Failed to save meta tags configuration to {Path}", options.MetaConfigFilePath);
				throw;
			}
		}
	}
}
