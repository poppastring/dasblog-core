using System;
using System.Xml.Serialization;

namespace DasBlog.Core.Security
{
	[Serializable]
	[XmlType("User")]
	public class User : IEquatable<User>
	{
		[XmlElement("Name")]
		public string Name
		{
			get { return EmailAddress;} set {/* ignore name */} }

		[XmlElement("Role")]
		public Role Role { get; set; }

		[XmlElement("Ask")]
		public bool Ask { get; set; }

		[XmlElement("EmailAddress")]
		public string EmailAddress { get; set; }

		[XmlElement("DisplayName")]
		public string DisplayName { get; set; }

		[XmlElement("OpenIDUrl")]
		public string OpenIDUrl { get; set; }

		[XmlElement("NotifyOnNewPost")]
		public bool NotifyOnNewPost { get; set; }

		[XmlElement("NotifyOnAllComment")]
		public bool NotifyOnAllComment { get; set; }

		[XmlElement("NotifyOnOwnComment")]
		public bool NotifyOnOwnComment { get; set; }

		[XmlElement("Active")]
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
