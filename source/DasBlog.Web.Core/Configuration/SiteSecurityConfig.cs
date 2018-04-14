using DasBlog.Core.Security;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DasBlog.Core.Configuration
{
	[Serializable]
	public class SiteSecurityConfig : ISiteSecurityConfig
	{
		public List<User> Users { get; set; } = new List<User>();
	}
}
