using System;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public class SiteSecurityConfigFileService : IConfigFileService<SiteSecurityConfigData>
	{
		private readonly IAtomicFileWriter atomicFileWriter;
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<SiteSecurityConfigFileService> logger;

		public SiteSecurityConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor,
			ILogger<SiteSecurityConfigFileService> logger, IAtomicFileWriter atomicFileWriter)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
			this.atomicFileWriter = atomicFileWriter;
		}

		public bool SaveConfig(SiteSecurityConfigData config)
		{
			try
			{
				var serializer = new XmlSerializer(typeof(SiteSecurityConfigData));
				atomicFileWriter.Write(options.SecurityConfigFilePath,
					stream => serializer.Serialize(stream, config));
				return true;
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Failed to save site security configuration to {Path}", options.SecurityConfigFilePath);
				throw;
			}
		}
	}
}
