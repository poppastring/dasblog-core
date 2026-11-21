using System;
using System.Xml.Serialization;

namespace DasBlog.Core.Security
{
	[Serializable]
	public class AppPassword
	{
		[XmlAttribute("id")]
		public string Id { get; set; }

		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("hash")]
		public string Hash { get; set; }

		[XmlAttribute("created")]
		public string Created { get; set; }

		[XmlAttribute("lastUsed")]
		public string LastUsed { get; set; }

		[XmlAttribute("lastIp")]
		public string LastIp { get; set; }
	}
}
