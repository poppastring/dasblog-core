using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class WebFingerLinkViewModel
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

	public class WebFingerViewModel
	{
		public string subject { get; set; }
		public string[] aliases { get; set; }
		public List<WebFingerLinkViewModel> links { get; set; }
	}
}

