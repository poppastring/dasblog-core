using System.Collections.Generic;
using DasBlog.Services.ConfigFile.Interfaces;
using System.Text.Json.Serialization;
using System.Reflection;
using System;

namespace DasBlog.Services.ConfigFile
{

	public class OEmbedProviders : IOEmbedProviders
	{
		public OEmbedProviders() { }

		[JsonPropertyName("providers")]
		public List<OEmbedProvider> Providers { get; set; }

	}

	public class OEmbedEndpoint
	{
		[JsonPropertyName("schemes")]
		public List<string> Schemes { get; set; }

		[JsonPropertyName("url")]
		public string Url { get; set; }

		[JsonPropertyName("discovery")]
		public bool? Discovery { get; set; }

		[JsonPropertyName("formats")]
		public List<string> Formats { get; set; }
	}

	public class OEmbedProvider : IComparer<OEmbedProvider>, IComparable<OEmbedProvider>
	{
		[JsonPropertyName("provider_name")]
		public string Provider_Name { get; set; }

		[JsonPropertyName("provider_url")]
		public string Provider_Url { get; set; }

		[JsonPropertyName("endpoints")]
		public List<OEmbedEndpoint> Endpoints { get; set; }

		[JsonPropertyName("usage_count")]
		public long UsageCount { get; set; }

		public int Compare(OEmbedProvider x, OEmbedProvider y)
		{
			return -1 * x.UsageCount.CompareTo(y.UsageCount);
		}

		public int CompareTo(OEmbedProvider other)
		{
			return -1 * UsageCount.CompareTo(other.UsageCount);
		}
	}
}
