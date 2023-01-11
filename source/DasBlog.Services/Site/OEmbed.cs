using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace DasBlog.Services.Site
{
	[Serializable]
	[XmlRoot("oembed")]
	public class OEmbed
	{
		[JsonProperty("url")]
		[XmlElement(ElementName = "url")]
		public string Url { get; set; }

		[JsonProperty("author_name")]
		[XmlElement(ElementName = "author_name")]
		public string AuthorName { get; set; }

		[JsonProperty("author_url")]
		[XmlElement(ElementName = "author_url")]
		public string AuthorUrl { get; set; }

		[JsonProperty("html")]
		[XmlElement(ElementName = "html")]
		public string Html { get; set; }

		[JsonProperty("width")]
		[XmlElement(ElementName = "width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		[XmlElement(ElementName = "height")]
		public object Height { get; set; }

		[JsonProperty("type")]
		[XmlElement(ElementName = "type")]
		public string Type { get; set; }

		[JsonProperty("cache_age")]
		[XmlElement(ElementName = "cache_age")]
		public string CacheAge { get; set; }

		[JsonProperty("provider_name")]
		[XmlElement(ElementName = "provider_name")]
		public string ProviderName { get; set; }

		[JsonProperty("provider_url")]
		[XmlElement(ElementName = "provider_url")]
		public string ProviderUrl { get; set; }

		[JsonProperty("version")]
		[XmlElement(ElementName = "version")]
		public string Version { get; set; }

	}
}
