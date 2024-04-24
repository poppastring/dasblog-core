using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class WebFinger
	{
		public string subject { get; set; }
		public string[] aliases { get; set; }
		public Link[] links { get; set; }
	}

	public class Link
	{
		public string rel { get; set; }
		public string type { get; set; }
		public string href { get; set; }
	}

}




