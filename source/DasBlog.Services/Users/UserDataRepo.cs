using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using User = DasBlog.Core.Security.User;
using DasBlog.Services.FileManagement;

namespace DasBlog.Services.Users
{
	public class UserDataRepo : IUserDataRepo
	{
		public class SiteSecurityConfig
		{
			public List<User> Users { get; set; } = new List<User>();
		}

		private readonly ConfigFilePathsDataOption options;
		
		public UserDataRepo(IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}

		public IEnumerable<User> LoadUsers()
		{
			SiteSecurityConfig ssc;

			var ser = new XmlSerializer(typeof(SiteSecurityConfig));

			using (var reader = new StreamReader(options.SecurityConfigFilePath))
			{
				try
				{
					ssc = (SiteSecurityConfig)ser.Deserialize(reader);
				}
				catch (Exception e)
				{
					// ToDO log
					Console.WriteLine(e);
					throw;
				}
			}
			return ssc.Users;
		}
	}
}
