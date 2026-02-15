using System;
using System.IO;
using System.Text.Json;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement
{
	public class OEmbedProvidersFileService : IConfigFileService<OEmbedProviders>
	{
		private readonly ConfigFilePathsDataOption options;

		public OEmbedProvidersFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			options = optionsAccessor.Value;
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
					// TODO log
					Console.WriteLine(e);
					throw;
				}
			}
		}
	}
}
