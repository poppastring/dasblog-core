using System;
using System.Text.Json;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement
{
	public class OEmbedProvidersFileService : IConfigFileService<OEmbedProviders>
	{
		private readonly IAtomicFileWriter atomicFileWriter;
		private readonly ConfigFilePathsDataOption options;
		private readonly ILogger<OEmbedProvidersFileService> logger;

		public OEmbedProvidersFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor,
			ILogger<OEmbedProvidersFileService> logger, IAtomicFileWriter atomicFileWriter)
		{
			options = optionsAccessor.Value;
			this.logger = logger;
			this.atomicFileWriter = atomicFileWriter;
		}

		public bool SaveConfig(OEmbedProviders config)
		{
			try
			{
				atomicFileWriter.Write(options.OEmbedProvidersFilePath,
					stream => JsonSerializer.Serialize(stream, config));
				return true;
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Failed to save OEmbed providers configuration to {Path}", options.OEmbedProvidersFilePath);
				throw;
			}
		}
	}
}
