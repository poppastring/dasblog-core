using System;
using System.Xml.Serialization;

namespace DasBlog.Web.UI.Core.Security
{
    [Serializable]
    [XmlType("User")]
    public class User
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool Ask { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string OpenIDUrl { get; set; }
        public bool NotifyOnNewPost { get; set; }
        public bool NotifyOnAllComment { get; set; }
        public bool NotifyOnOwnComment { get; set; }
        public bool Active { get; set; }

        [XmlElement("Password")]
        public string XmlPassword { get; set; }

        public UserToken ToToken() => new UserToken(Name, Role);
    }
}
