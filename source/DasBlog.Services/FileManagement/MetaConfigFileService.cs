using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement
{
	public class MetaConfigFileService : IConfigFileService<MetaTags>
	{
		private readonly ConfigFilePathsDataOption options;

		public MetaConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}

		public bool SaveConfig(MetaTags config)
		{
			var ser = new XmlSerializer(typeof(MetaTags));
			using (var writer = new StreamWriter(options.MetaConfigFilePath))
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
