using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class Actor
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public string type { get; set; }
		public string following { get; set; }
		public string followers { get; set; }
		public string inbox { get; set; }
		public string outbox { get; set; }
		public string preferredUsername { get; set; }
		public string name { get; set; }
		public string summary { get; set; }
		public string url { get; set; }
		public bool discoverable { get; set; }
		public bool memorial { get; set; }
		public Icon icon { get; set; }
		public Image image { get; set; }
		public Publickey publicKey { get; set; }
		public Attachment[] attachment { get; set; }
	}

	public class Icon
	{
		public string type { get; set; }
		public string mediaType { get; set; }
		public string url { get; set; }
	}

	public class Image
	{
		public string type { get; set; }
		public string mediaType { get; set; }
		public string url { get; set; }
	}

	public class Publickey
	{
		public string context { get; set; }
		public string type { get; set; }
		public string id { get; set; }
		public string owner { get; set; }
		public string publicKeyPem { get; set; }
	}

	public class Attachment
	{
		public string type { get; set; }
		public string name { get; set; }
		public string value { get; set; }
	}

}

