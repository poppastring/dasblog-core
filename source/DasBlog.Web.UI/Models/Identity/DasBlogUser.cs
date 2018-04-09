using DasBlog.Core.Security;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Models.Identity
{
	public class DasBlogUser : IdentityUser<User>
	{
		public string Role { get; set; }
	}
}
