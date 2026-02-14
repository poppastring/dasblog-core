using System;
using System.Security.Cryptography;
using System.Text;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Identity
{
    public class DasBlogPasswordHasher : PasswordHasher<DasBlogUser>
	{
		private ISiteSecurityManager _siteSecurityManager;

		public DasBlogPasswordHasher(ISiteSecurityManager siteSecurityManager)
		{
			_siteSecurityManager = siteSecurityManager;
		}

		public override string HashPassword(DasBlogUser user, string password)
		{
			return _siteSecurityManager.HashPassword(password);
		}

		public override PasswordVerificationResult VerifyHashedPassword(DasBlogUser user, string hashedPassword, string providedPassword)
		{
			if (_siteSecurityManager.VerifyHashedPassword(hashedPassword, providedPassword))
			{
				return PasswordVerificationResult.Success;
			}

			return PasswordVerificationResult.Failed;
		}
	}
}
