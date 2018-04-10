using DasBlog.Core.Security;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Identity
{
	public class DasBlogUser : IdentityUser<User>
	{
		public string Role { get; set; }
	}
}
