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
	public class SiteSecurityConfigFileService : IConfigFileService<SiteSecurityConfigData>
	{
		private readonly ConfigFilePathsDataOption options;

		public SiteSecurityConfigFileService(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}

		public bool SaveConfig(SiteSecurityConfigData config)
		{
			var ser = new XmlSerializer(typeof(SiteSecurityConfigData));
			using (var writer = new StreamWriter(options.SecurityConfigFilePath))
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
