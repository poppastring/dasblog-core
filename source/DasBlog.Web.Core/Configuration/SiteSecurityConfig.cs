using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DasBlog.Web.Core.Configuration
{
    [Serializable]
    [XmlRoot("SiteSecurityConfig")]
    public class SiteSecurityConfig : ISiteSecurityConfig
    {
        [XmlArray("Users")]
        public List<User> Users { get; set; }
    }
}
