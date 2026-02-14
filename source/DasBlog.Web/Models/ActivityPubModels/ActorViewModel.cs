using System;
using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class ActorViewModel
	{
		[JsonPropertyName("@context")]
		public object[] context { get; set; }
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
		public Image icon { get; set; } = null;

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

	public class Image
	{
		public string type { get; set; } = "Image";
		public string mediaType { get; set; } = "image/jpeg";
		public string url { get; set; }
	}

	public class ActorContext
	{
		public string manuallyApprovesFollowers { get; set; }
		public string toot { get; set; }
		public Featured featured { get; set; }
		public Featuredtags featuredTags { get; set; }
		public Alsoknownas alsoKnownAs { get; set; }
		public Movedto movedTo { get; set; }
		public string schema { get; set; }
		public string PropertyValue { get; set; }
		public string value { get; set; }
		public string discoverable { get; set; }
		public string Device { get; set; }
		public string Ed25519Signature { get; set; }
		public string Ed25519Key { get; set; }
		public string Curve25519Key { get; set; }
		public string EncryptedMessage { get; set; }
		public string publicKeyBase64 { get; set; }
		public string deviceId { get; set; }
		public Claim claim { get; set; }
		public Fingerprintkey fingerprintKey { get; set; }
		public Identitykey identityKey { get; set; }
		public Devices devices { get; set; }
		public string messageFranking { get; set; }
		public string messageType { get; set; }
		public string cipherText { get; set; }
		public string suspended { get; set; }
		public string Emoji { get; set; }
		public Focalpoint focalPoint { get; set; }
	}

	public class Featured
	{
		public string id { get; set; }
		public string type { get; set; }
	}

	public class Featuredtags
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Alsoknownas
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Movedto
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Claim
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Fingerprintkey
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Identitykey
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Devices
	{
		[JsonPropertyName("@id")]
		public string id { get; set; }

		[JsonPropertyName("@type")]
		public string type { get; set; }
	}

	public class Focalpoint
	{
		[JsonPropertyName("@container")]
		public string container { get; set; }

		[JsonPropertyName("@id")]
		public string id { get; set; }
	}

}
