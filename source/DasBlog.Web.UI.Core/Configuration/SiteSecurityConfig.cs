using DasBlog.Web.UI.Core.Security;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DasBlog.Web.UI.Core.Configuration
{
    [Serializable]
    [XmlRoot("SiteSecurityConfig")]
    public class SiteSecurityConfig : ISiteSecurityConfig
    {
        [XmlArray("Users")]
        public List<User> Users { get; set; }
    }
}
