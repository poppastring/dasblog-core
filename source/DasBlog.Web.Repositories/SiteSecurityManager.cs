
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace DasBlog.Managers
{
	public class SiteSecurityManager : ISiteSecurityManager
	{
		private HashAlgorithm hashAlgorithm;
		private IDasBlogSettings dasBlogSettings;

		public SiteSecurityManager( IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			hashAlgorithm = SHA512Managed.Create();
		}

		public string HashPassword(string password)
		{
			hashAlgorithm = MD5CryptoServiceProvider.Create();
			byte[] clearBytes = Encoding.Unicode.GetBytes(password);

			byte[] hashedBytes = hashAlgorithm.ComputeHash(clearBytes);

			return BitConverter.ToString(hashedBytes);
		}

		public bool IsMd5Hash(string hash)
		{
			return (hash.Length == 47 && hash.Replace("-", string.Empty).Length == 32);
		}

		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			string hashprovidedpassword = string.Empty;

			if (this.IsMd5Hash(hashedPassword))
			{
				hashAlgorithm = MD5CryptoServiceProvider.Create();
			}

			hashprovidedpassword = this.HashPassword(providedPassword);

			if (hashedPassword.Equals(hashprovidedpassword, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}

			return false;
		}

		public User GetUser(string userName)
		{
			return dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.Name, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public User GetUserByDisplayName(string displayName)
		{
			return dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public User GetUserByEmail(string email)
		{
			return dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase) == 0);
		}
	}
}
