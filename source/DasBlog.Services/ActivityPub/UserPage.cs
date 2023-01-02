using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Services.ActivityPub
{
	public class UserPage
	{
		public string Id { get; set; }
		public string Type { get; set; }
		public string Next { get; set; }
		public string Previous { get; set; }
		public string PartOf { get; set; }
		public List<OrderedItem> OrderItems { get; set; }
	}

	public class OrderedItem
	{
		public string Id { get; set; }
		public string Type { get; set; }
		public string Actor { get; set; }
		public DateTime Published { get; set; }
		public List<string> To { get; set; }
		public List<string> Cc { get; set; }
		public bool Sensitive { get; set; }
		public string Content { get; set; }
	}

	public class User
	{
		public string Id { get; set; }
		public string Type { get; set; }
		public string First { get; set; }
		public string Context { get; set; }
	}
}
