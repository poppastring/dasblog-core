using DasBlog.Core.Security;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DasBlog.Web.Identity
{
	public class DasBlogUser : IdentityUser<User>
	{
		public string DisplayName { get; set; }

		public string Role { get; set; }

		public bool NotifyOnNewPost { get; set; }

		public bool NotifyOnAllComment { get; set; }

		public bool NotifyOnOwnComment { get; set; }

		public bool TwoFactorEnabled { get; set; }

		public string AuthenticatorSecret { get; set; }

		public IList<RecoveryCode> RecoveryCodes { get; set; } = new List<RecoveryCode>();
	}
}
