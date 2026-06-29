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

		[XmlElement("FaceBookAdmins")]
		public string FaceBookAdmins { get; set; }

		[XmlElement("FaceBookAppID")]
		public string FaceBookAppID  { get; set; }

		[XmlElement("AnalyticsTrackingId")]
		public string AnalyticsTrackingId { get; set; }

		[XmlElement("AnalyticsProvider")]
		public string AnalyticsProvider { get; set; }

		[XmlIgnore]
		public string GoogleAnalyticsID
		{
			get => AnalyticsTrackingId;
			set => AnalyticsTrackingId = value;
		}
	}
}
