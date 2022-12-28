using System;
using System.Text.Json.Serialization;

namespace DasBlog.Managers
{
	internal class EntryCloudEventData
	{
		[JsonPropertyName("id")]
		public string Id { get; internal set; }
		[JsonPropertyName("title")]
		public string Title { get; internal set; }
		[JsonPropertyName("createdUtc")]
		public DateTime CreatedUtc { get; internal set; }
		[JsonPropertyName("modifiedUtc")]
		public DateTime ModifiedUtc { get; internal set; }
		[JsonPropertyName("tags")]
		public string Tags { get; internal set; }
		[JsonPropertyName("description")]
		public string Description { get; internal set; }
		[JsonPropertyName("permaLink")]
		public string PermaLink { get; internal set; }
		[JsonPropertyName("contentLink")]
		public string DetailsLink { get; internal set; }
		[JsonPropertyName("isPublic")]
		public bool IsPublic { get; internal set; }
		[JsonPropertyName("author")]
		public string Author { get; internal set; }
		[JsonPropertyName("longitude")]
		public double? Longitude { get; internal set; }
		[JsonPropertyName("latitude")]
		public double? Latitude { get; internal set; }
		
	}
}
