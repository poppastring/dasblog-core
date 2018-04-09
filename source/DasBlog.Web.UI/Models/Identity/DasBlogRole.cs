using System;
using DasBlog.Core.Security;

namespace DasBlog.Web.Models.Identity
{
	public class DasBlogRole
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public Role Role { get; set; }
	}
}
