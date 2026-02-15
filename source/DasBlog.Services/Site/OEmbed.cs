using System;
using System.Xml.Serialization;
using System.Text.Json.Serialization;

namespace DasBlog.Services.Site
{
	[Serializable]
	[XmlRoot("oembed")]
	public class OEmbed
	{
		[JsonPropertyName("url")]
		[XmlElement(ElementName = "url")]
		public string Url { get; set; }

		[JsonPropertyName("author_name")]
		[XmlElement(ElementName = "author_name")]
		public string AuthorName { get; set; }

		[JsonPropertyName("author_url")]
		[XmlElement(ElementName = "author_url")]
		public string AuthorUrl { get; set; }

		[JsonPropertyName("html")]
		[XmlElement(ElementName = "html")]
		public string Html { get; set; }

		[JsonPropertyName("width")]
		[XmlElement(ElementName = "width")]
		public int Width { get; set; }

		[JsonPropertyName("height")]
		[XmlElement(ElementName = "height")]
		public object Height { get; set; }

		[JsonPropertyName("type")]
		[XmlElement(ElementName = "type")]
		public string Type { get; set; }

		[JsonPropertyName("cache_age")]
		[XmlElement(ElementName = "cache_age")]
		public string CacheAge { get; set; }

		[JsonPropertyName("provider_name")]
		[XmlElement(ElementName = "provider_name")]
		public string ProviderName { get; set; }

		[JsonPropertyName("provider_url")]
		[XmlElement(ElementName = "provider_url")]
		public string ProviderUrl { get; set; }

		[JsonPropertyName("version")]
		[XmlElement(ElementName = "version")]
		public string Version { get; set; }

	}
}
