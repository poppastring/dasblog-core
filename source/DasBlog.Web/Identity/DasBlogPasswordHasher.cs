using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Identity
{
	/// <summary>
	/// Password hasher that produces ASP.NET Identity PBKDF2 v3 hashes for new passwords
	/// while still verifying legacy MD5 / raw SHA-512 hashes stored in siteSecurity.config.
	/// When a legacy hash matches, returns <see cref="PasswordVerificationResult.SuccessRehashNeeded"/>
	/// so that <c>SignInManager</c> upgrades the stored hash on the next successful login.
	/// </summary>
	public class DasBlogPasswordHasher : PasswordHasher<DasBlogUser>
	{
		private readonly ISiteSecurityManager siteSecurityManager;

		public DasBlogPasswordHasher(ISiteSecurityManager siteSecurityManager)
		{
			this.siteSecurityManager = siteSecurityManager;
		}

		public override PasswordVerificationResult VerifyHashedPassword(DasBlogUser user, string hashedPassword, string providedPassword)
		{
			if (string.IsNullOrEmpty(hashedPassword) || providedPassword == null)
			{
				return PasswordVerificationResult.Failed;
			}

			if (!siteSecurityManager.IsLegacyHash(hashedPassword))
			{
				return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
			}

			return siteSecurityManager.VerifyHashedPassword(hashedPassword, providedPassword)
				? PasswordVerificationResult.SuccessRehashNeeded
				: PasswordVerificationResult.Failed;
		}
	}
}
