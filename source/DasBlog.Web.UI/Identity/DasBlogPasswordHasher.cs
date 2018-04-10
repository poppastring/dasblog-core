using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Identity
{
    public class DasBlogPasswordHasher : PasswordHasher<DasBlogUser>
	{
		public override string HashPassword(DasBlogUser user, string password)
		{
			return OldHash(password);
		}

		public override PasswordVerificationResult VerifyHashedPassword(DasBlogUser user, string hashedPassword, string providedPassword)
		{
			string hashprovidedpassword = OldHash(providedPassword);

			if (hashedPassword.Equals(hashprovidedpassword, StringComparison.InvariantCultureIgnoreCase))
			{
				return PasswordVerificationResult.Success;
			}

			return PasswordVerificationResult.Failed;
		}

		private string OldHash(string password)
		{
			Byte[] clearBytes = Encoding.Unicode.GetBytes(password);

			MD5 md5 = MD5CryptoServiceProvider.Create();
			Byte[] hashedBytes = md5.ComputeHash(clearBytes);

			return BitConverter.ToString(hashedBytes);
		}

	}
}
