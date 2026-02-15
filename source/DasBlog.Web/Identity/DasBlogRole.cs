using System;
using DasBlog.Core.Security;

namespace DasBlog.Web.Identity
{
	public class DasBlogRole
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public Role Role { get; set; }
	}
}
