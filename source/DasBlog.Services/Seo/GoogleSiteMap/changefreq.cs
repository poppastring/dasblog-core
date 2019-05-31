using System;
using System.Xml.Serialization;

namespace DasBlog.Services.Seo.GoogleSiteMap
{
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public enum changefreq
	{

		/// <remarks/>
		always,

		/// <remarks/>
		hourly,

		/// <remarks/>
		daily,

		/// <remarks/>
		weekly,

		/// <remarks/>
		monthly,

		/// <remarks/>
		yearly,

		/// <remarks/>
		never,
	}
}
