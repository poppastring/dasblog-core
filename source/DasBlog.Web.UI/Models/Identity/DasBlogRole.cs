using System;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogRole
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Role { get; set; }
	}
}
