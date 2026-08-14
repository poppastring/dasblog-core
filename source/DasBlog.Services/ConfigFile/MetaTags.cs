using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile.Interfaces;

namespace DasBlog.Services.ConfigFile
{
	[Serializable]
	[XmlType("MetaTags")]
	public class MetaTags : IMetaTags
    {
		[XmlElement("MetaDescription")]
		public string MetaDescription { get; set; }

		[XmlElement("MetaKeywords")]
		public string MetaKeywords  { get; set; }

		[XmlElement("TwitterCard")]
		public string TwitterCard  { get; set; }

		[XmlElement("TwitterSite")]
		public string TwitterSite  { get; set; }

		[XmlElement("TwitterCreator")]
		public string TwitterCreator { get; set; }

		[XmlElement("TwitterImage")]
		public string TwitterImage  { get; set; }

		[XmlElement("MastodonServerUrl")]
		public string MastodonServerUrl { get; set; }

		[XmlElement("MastodonAccount")]
		public string MastodonAccount { get; set; }

		[XmlElement("AtprotoEnabled")]
		public bool AtprotoEnabled { get; set; }

		[XmlElement("AtprotoHandle")]
		public string AtprotoHandle { get; set; }

		[XmlElement("AtprotoPdsUrl")]
		public string AtprotoPdsUrl { get; set; }

		[XmlElement("AtprotoPublicationRkey")]
		public string AtprotoPublicationRkey { get; set; }
	}
}
