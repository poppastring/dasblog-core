using DasBlog.Core;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace DasBlog.Managers
{
	public class SiteSecurityManager : ISiteSecurityManager
	{
		private HashAlgorithm _hashAlgorithm;
		private IPrincipal _principal;
		private IDasBlogSettings _dasBlogSettings;

		public SiteSecurityManager(IPrincipal principal, IDasBlogSettings dasBlogSettings)
		{
			_dasBlogSettings = dasBlogSettings;
			_principal = principal;
			_hashAlgorithm = SHA512Managed.Create();
		}

		public string HashPassword(string password)
		{
			byte[] clearBytes = Encoding.Unicode.GetBytes(password);

			byte[] hashedBytes = _hashAlgorithm.ComputeHash(clearBytes);

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
				_hashAlgorithm = MD5CryptoServiceProvider.Create();
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
			return _dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.Name, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public User GetUserByDisplayName(string displayName)
		{
			return _dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.DisplayName, displayName, StringComparison.InvariantCultureIgnoreCase) == 0);
		}

		public User GetUserByEmail(string email)
		{
			return _dasBlogSettings.SecurityConfiguration.Users.FirstOrDefault(s => string.Compare(s.EmailAddress, email, StringComparison.InvariantCultureIgnoreCase) == 0);
		}
	}
}
