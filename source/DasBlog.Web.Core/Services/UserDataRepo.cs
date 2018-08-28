using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using User = DasBlog.Core.Security.User;

namespace DasBlog.Core.Services
{
	public class UserDataRepo : Interfaces.IUserDataRepo
	{
		public class SiteSecurityConfig
		{
			public List<User> Users { get; set; } = new List<User>();
		}

		private LocalUserDataOptions options;
		public UserDataRepo(IOptions<LocalUserDataOptions> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}
		public IEnumerable<User> LoadUsers()
		{
			SiteSecurityConfig ssc;
				// don't confuse this with DasBlog.Core.SiteSecurityConfig
				// this vesion is striictly for deserializing data from the file
			var ser = new XmlSerializer(typeof(SiteSecurityConfig));
//			var fileInfo = fileProvider.GetFileInfo(Startup.SITESECURITYCONFIG);
			using (var reader = new StreamReader(options.Path))
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

		public void SaveUsers(List<User> users)
		{
			var ssc = new SiteSecurityConfig
			{
				Users = users
			};
			var ser = new XmlSerializer(typeof(SiteSecurityConfig));
			using (var writer = new StreamWriter(options.Path))
			{
				try
				{
					ser.Serialize(writer, ssc);
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

	public class LocalUserDataOptions
	{
		public string Path{ get; set; }
	}
}
