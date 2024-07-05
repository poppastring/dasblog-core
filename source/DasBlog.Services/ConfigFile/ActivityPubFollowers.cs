using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DasBlog.Services.ConfigFile
{
	public class ActivityPubFollowers
	{
		[JsonPropertyName("Followers")]
		public List<string> Followers { get; set; } = new List<string>();
	}
}
