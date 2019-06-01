using System;
using System.Xml.Serialization;

namespace DasBlog.Services.Seo.GoogleSiteMap
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class url
	{
		public url() { }

		public url(string locIn, DateTime lastmodIn, changefreq freqIn, Decimal priorityIn)
		{
			loc = locIn;
			lastmod = lastmodIn;
			changefreq = freqIn;
			changefreqSpecified = true;
			priority = priorityIn;
			prioritySpecified = true;
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
		public string loc;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("lastmod")]
		public string lastmodString
		{
			get
			{
				return lastmod.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			}
			set
			{
				lastmod = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		[XmlIgnore]
		public DateTime lastmod;

		/// <remarks/>
		public changefreq changefreq;

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool changefreqSpecified;

		/// <remarks/>
		public System.Decimal priority;

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool prioritySpecified;
	}
}
