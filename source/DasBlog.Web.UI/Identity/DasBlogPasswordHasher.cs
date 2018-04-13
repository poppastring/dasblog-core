using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.Identity
{
    public class DasBlogPasswordHasher : PasswordHasher<DasBlogUser>
	{
		public override string HashPassword(DasBlogUser user, string password)
		{
			var sha512 = SHA512Managed.Create();
			return HashPassword(sha512, password);
		}

		public override PasswordVerificationResult VerifyHashedPassword(DasBlogUser user, string hashedPassword, string providedPassword)
		{
			HashAlgorithm ha = null;
			string hashprovidedpassword = string.Empty;

			if (IsThisMd5Hash(hashedPassword))
			{
				ha = MD5CryptoServiceProvider.Create();
			}
			else
			{
				ha = SHA512Managed.Create();
			}

			hashprovidedpassword = HashPassword(ha, providedPassword);

			if (hashedPassword.Equals(hashprovidedpassword, StringComparison.InvariantCultureIgnoreCase))
			{
				return PasswordVerificationResult.Success;
			}

			return PasswordVerificationResult.Failed;
		}

		private string HashPassword(HashAlgorithm hashalgorithm, string password)
		{
			Byte[] clearBytes = Encoding.Unicode.GetBytes(password);

			Byte[] hashedBytes = hashalgorithm.ComputeHash(clearBytes);

			return BitConverter.ToString(hashedBytes);
		}

		private bool IsThisMd5Hash(string hash)
		{
			return (hash.Length == 47 && hash.Replace("-", string.Empty).Length == 32);
		}

	}
}
