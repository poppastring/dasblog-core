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
		private readonly object saveLock = new();
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<SiteConfigFileService> logger;

		public SiteConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor, ILogger<SiteConfigFileService> logger)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
		}

		public bool SaveConfig(SiteConfig config)
		{
			lock (saveLock)
			{
				var tempPath = $"{options.SiteConfigFilePath}.{Guid.NewGuid():N}.tmp";

				try
				{
					var serializer = new XmlSerializer(typeof(SiteConfig));
					var namespaces = new XmlSerializerNamespaces();
					namespaces.Add("", "");

					using (var writer = new StreamWriter(tempPath))
					{
						serializer.Serialize(writer, config, namespaces);
					}

					if (File.Exists(options.SiteConfigFilePath))
					{
						File.Replace(tempPath, options.SiteConfigFilePath, destinationBackupFileName: null);
					}
					else
					{
						File.Move(tempPath, options.SiteConfigFilePath);
					}
					return true;
				}
				catch (Exception exception)
				{
					logger.LogError(exception, "Failed to save site configuration to {Path}", options.SiteConfigFilePath);
					throw;
				}
				finally
				{
					File.Delete(tempPath);
				}
			}
		}
	}
}
