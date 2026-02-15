using System;
using System.Text.Json.Serialization;

namespace DasBlog.Web.Models.ActivityPubModels
{
	public class UserPageViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string next { get; set; }
		public string prev { get; set; }
		public string partOf { get; set; }
		public OrderedItemViewModel[] orderedItems { get; set; }
	}

	public class OrderedItemViewModel
	{
		public string id { get; set; }
		public string type { get; set; }
		public string actor { get; set; }
		public DateTime published { get; set; }
		public string[] to { get; set; }
		public string[] cc { get; set; }
		public bool sensitive { get; set; }
		public string content { get; set; }
	}

}
