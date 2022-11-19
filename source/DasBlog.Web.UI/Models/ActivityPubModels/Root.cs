using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class Link
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string rel { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string type { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string href { get; set; }

		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public string template { get; set; }
	}

	public class Root
	{
		public string subject { get; set; }
		public List<string> aliases { get; set; }
		public List<Link> links { get; set; }
	}
}

