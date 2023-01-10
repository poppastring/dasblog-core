using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class UserFollowersViewModel
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public int totalItems { get; set; }
		public string first { get; set; }
		public string type { get; set; }
	}

	public class UserFollowersPageViewModel
	{
		[JsonPropertyName("@context")]
		public string context { get; set; }
		public string id { get; set; }
		public int totalItems { get; set; }
		public string next { get; set; }
		public string type { get; set; }
		public string partOf { get; set; }
		public string[] orderedItems { get; set; }
	}
}
