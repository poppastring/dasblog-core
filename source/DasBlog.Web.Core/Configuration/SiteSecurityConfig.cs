using DasBlog.Core.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Core.Configuration
{
	[Serializable]
	public class SiteSecurityConfig : ISiteSecurityConfig
	{
		private readonly ILocalUserDataService _localUserDataService;
		public SiteSecurityConfig(ILocalUserDataService localUserDataService)
		{
			_localUserDataService = localUserDataService;
			Refresh();
		}

		public void Refresh()
		{
			Users = _localUserDataService.LoadUsers().ToList();
		}

		public List<User> Users { get; set; } = new List<User>();
	}
}
