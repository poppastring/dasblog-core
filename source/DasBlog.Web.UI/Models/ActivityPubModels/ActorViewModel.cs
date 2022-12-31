using System;
using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class ActorViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string following { get; set; }
		public string followers { get; set; }
		public string inbox { get; set; }
		public string outbox { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string featured { get; set; }

		public string preferredUsername { get; set; }
		public string name { get; set; }
		
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string summary { get; set; }
		public string url { get; set; }
		public bool manuallyApprovesFollowers { get; set; } = false;
		public bool discoverable { get; set; } = true;
		public DateTime published { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Publickey publicKey { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Endpoints endpoints { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Icon icon { get; set; } = null;

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public Image image { get; set; } = null;
	}

	public class Publickey
	{
		public string id { get; set; }
		public string owner { get; set; }
		public string publicKeyPem { get; set; }
	}

	public class Endpoints
	{
		public string sharedInbox { get; set; }
	}

	public class Icon
	{
		public string type { get; set; } = "Image";
		public string mediaType { get; set; } = "image/jpeg";
		public string url { get; set; }
	}

	public class Image
	{
		public string type { get; set; } = "Image";
		public string mediaType { get; set; } = "image/jpeg";
		public string url { get; set; }
	}

}
