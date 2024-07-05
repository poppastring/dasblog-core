using System;
using System.IO;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DasBlog.Services.FileManagement
{
	public class ActivityPubFollowingFilePath : IConfigFileService<ActivityPubFollowers>
	{
		private readonly ConfigFilePathsDataOption filePathsDataOption;

		public ActivityPubFollowingFilePath(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			filePathsDataOption = optionsAccessor.Value;
		}

		public bool SaveConfig(ActivityPubFollowers config)
		{
			var ser = new JsonSerializer();
			using (var writer = new StreamWriter(filePathsDataOption.ActivityPubFollowingFilePath))
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
