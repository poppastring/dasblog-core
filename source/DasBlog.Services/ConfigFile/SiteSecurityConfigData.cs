using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DasBlog.Services.ConfigFile
{
	[Serializable]
	[XmlType(TypeName = "SiteSecurityConfig")]
	public class SiteSecurityConfigData : ISiteSecurityConfig
	{
		public SiteSecurityConfigData()
		{
		}

		public List<User> Users { get; set; } = new List<User>();
	}
}
