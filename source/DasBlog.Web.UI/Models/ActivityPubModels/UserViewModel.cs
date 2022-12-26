using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class UserViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string first { get; set; }

		[JsonPropertyName("@context")]
		public string context { get; set; }

	}
}
