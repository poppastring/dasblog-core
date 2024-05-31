using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class Outbox
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string summary { get; set; }
		public int totalItems { get; set; }
		public Ordereditem[] orderedItems { get; set; }
	}

	public class Ordereditem
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string actor { get; set; }
		public string[] to { get; set; }
		public object[] cc { get; set; }
		public string published { get; set; }

		[JsonPropertyName("object")]
		public Note _object { get; set; }
	}

	public class Note
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string hash { get; set; }
		public string content { get; set; }
		public string url { get; set; }
		public string attributedTo { get; set; }
		public string[] to { get; set; }
		public object[] cc { get; set; }
		public string published { get; set; }
		public Tag[] tag { get; set; }
		public Replies replies { get; set; }
	}

	public class Replies
	{
		public string id { get; set; }
		public string type { get; set; }
		public First first { get; set; }
	}

	public class First
	{
		public string type { get; set; }
		public string next { get; set; }
		public string partOf { get; set; }
		public object[] items { get; set; }
	}

	public class Tag
	{
		[JsonPropertyName("Type")]
		public string Type { get; set; }
		[JsonPropertyName("Href")]
		public string Href { get; set; }
		[JsonPropertyName("Name")]
		public string Name { get; set; }
	}

}
