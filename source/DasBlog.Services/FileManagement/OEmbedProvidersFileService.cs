using System;
using System.IO;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
			var ser = new JsonSerializer();
			using (var writer = new StreamWriter(options.OEmbedProvidersFilePath))
			{
				try
				{
					ser.Serialize(writer, config);
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
