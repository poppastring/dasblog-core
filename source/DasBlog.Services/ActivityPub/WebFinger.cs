using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class WebFinger
	{
		public string Subject { get; set; }
		public List<string> Aliases { get; set; } = new List<string>();
		public List<WebFingerLink> Links { get; set; } = new List<WebFingerLink>();
	}

	public class WebFingerLink
	{
		public string Relationship { get; set;}
		public string Type { get; set;}	
		public string HRef { get; set;}
		public string Template { get; set;}
	}

}
