using System;
using System.IO;
using System.Text.Json;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement
{
	public class OEmbedProvidersFileService : IConfigFileService<OEmbedProviders>
	{
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<OEmbedProvidersFileService> logger;

		public OEmbedProvidersFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor, ILogger<OEmbedProvidersFileService> logger)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
		}

		public bool SaveConfig(OEmbedProviders config)
		{
			using (var writer = new FileStream(options.OEmbedProvidersFilePath, FileMode.Create))
			{
				try
				{
					JsonSerializer.Serialize(writer, config);
					return true;
				}
				catch (Exception e)
				{
					logger.LogError(e, "Failed to save OEmbed providers configuration to {Path}", options.OEmbedProvidersFilePath);
					throw;
				}
			}
		}
	}
}
