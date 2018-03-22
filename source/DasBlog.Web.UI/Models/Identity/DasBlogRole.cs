using System;
using DasBlog.Web.Core.Security;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogRole
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public Role Role { get; set; }
	}
}
