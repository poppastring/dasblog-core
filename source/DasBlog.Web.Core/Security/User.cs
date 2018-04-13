using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DasBlog.Core.Security
{
	[Serializable]
	[XmlType(nameof(User))]
	public class User : IEquatable<User>
	{
		public string Name { get; set; }

		public Role Role { get; set; }

		public bool Ask { get; set; }

		public string EmailAddress { get; set; }

		public string DisplayName { get; set; }

		public string OpenIDUrl { get; set; }

		public bool NotifyOnNewPost { get; set; }

		public bool NotifyOnAllComment { get; set; }

		public bool NotifyOnOwnComment { get; set; }

		public bool Active { get; set; }

		[XmlElement("Password")]
		public string Password { get; set; }

		public string XmlPassword { get; set; }

		public UserToken ToToken() => new UserToken(Name, Role.ToString());

		public bool Equals(User other)
		{
			return EmailAddress == other.EmailAddress;
		}
	}
}
