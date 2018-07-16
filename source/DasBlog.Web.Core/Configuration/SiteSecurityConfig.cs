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
		private readonly IUserService _userService;
		public SiteSecurityConfig(IUserService userService)
		{
			this._userService = userService;
			Refresh();
		}

		public void Refresh()
		{
			Users = _userService.GetAllUsers().ToList();
		}

		public List<User> Users { get; set; } = new List<User>();
	}
}
