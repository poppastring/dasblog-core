using DasBlog.Core.Security;
using System;
using System.Collections.Generic;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Core.Configuration
{
	[Serializable]
	public class SiteSecurityConfig : ISiteSecurityConfig
	{
		public SiteSecurityConfig(ILocalUserDataService userDataService)
		{
			Users.AddRange(userDataService.LoadUsers());
		}
		public List<User> Users { get; set; } = new List<User>();
	}
}
