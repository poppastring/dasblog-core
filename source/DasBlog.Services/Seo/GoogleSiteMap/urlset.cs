using System;
using System.Xml.Serialization;

namespace DasBlog.Services.Seo.GoogleSiteMap
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class urlset
	{

		public urlset() { }

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("url")]
		public urlCollection url;
	}
}
