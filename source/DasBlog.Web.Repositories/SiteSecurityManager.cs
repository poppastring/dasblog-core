
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DasBlog.Managers
{
	public class SiteSecurityManager : ISiteSecurityManager
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteSecurityManager( IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public string HashPassword(string password)
		{
			var hashAlgorithm = SHA512Managed.Create();
			return HashPassword(password, hashAlgorithm);
		}

		private string HashPassword(string password, HashAlgorithm hashAlgorithm)
		{
			var clearBytes = Encoding.Unicode.GetBytes(password);

			var hashedBytes = hashAlgorithm.ComputeHash(clearBytes);

			return BitConverter.ToString(hashedBytes);
		}

		public bool IsMd5Hash(string hash)
		{
			return (hash.Length == 47 && hash.Replace("-", string.Empty).Length == 32);
		}

		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			string hashprovidedpassword = string.Empty;

			HashAlgorithm hashAlgorithm = SHA512Managed.Create();
			if (this.IsMd5Hash(hashedPassword))
			{
				hashAlgorithm = MD5CryptoServiceProvider.Create();
			}

			hashprovidedpassword = HashPassword(providedPassword, hashAlgorithm);

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
