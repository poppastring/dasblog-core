using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class AcceptRequest
	{
		[JsonPropertyName("@context")]
		public object context { get; set; } = default!;

		public string id { get; set; } = default!;

		public string type { get; } = "Accept";

		public string actor { get; set; } = default!;

		[JsonPropertyName("object")]
		public object Object { get; set; } = default!;
	}
}
