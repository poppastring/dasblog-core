using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using System.IO;
using System.Xml.Serialization;
using User = DasBlog.Core.Security.User;

namespace DasBlog.Core.Services
{
	public class LocalUserDataService : Interfaces.ILocalUserDataService
	{
		public class SiteSecurityData
		{
			public List<User> Users { get; set; } = new List<User>();
		}
		private LocalUserDataOptions options;
		public LocalUserDataService(IOptions<LocalUserDataOptions> optionsAccessor)
		{
			options = optionsAccessor.Value;
		}
		public IEnumerable<User> LoadUsers()
		{
			SiteSecurityData ssd;
			var ser = new XmlSerializer(typeof(SiteSecurityData));
//			var fileInfo = fileProvider.GetFileInfo(Startup.SITESECURITYCONFIG);
			using (var reader = new StreamReader(options.Path))
			{
				try
				{
					ssd = (SiteSecurityData)ser.Deserialize(reader);
				}
				catch (Exception e)
				{
					// ToDO log
					Console.WriteLine(e);
					throw;
				}
			}

			return ssd.Users;
		}

		public void SaveUsers(List<User> users)
		{
			SiteSecurityData ssd = new SiteSecurityData();
			ssd.Users = users;
			var ser = new XmlSerializer(typeof(SiteSecurityData));
			using (var writer = new StreamWriter(options.Path))
			{
				try
				{
					ser.Serialize(writer, ssd);
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
