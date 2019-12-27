using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public class SiteConfigFileService : IConfigFileService<SiteConfig>
	{
		private readonly ConfigFilePathsDataOption options;

		public SiteConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}

		public bool SaveConfig(SiteConfig config)
		{

			var ser = new XmlSerializer(typeof(SiteConfig));
			var ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			using (var writer = new StreamWriter(options.SiteConfigFilePath))
			{
				try
				{
					ser.Serialize(writer, config, ns);

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
