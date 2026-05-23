using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DasBlog.Managers
{
	public class SiteSecurityManager : ISiteSecurityManager
	{
		private const int Md5HashLength = 47;
		private const int Sha512HashLength = 191;

		private static readonly PasswordHasher<object> identityHasher = new PasswordHasher<object>();

		private readonly IDasBlogSettings dasBlogSettings;

		public SiteSecurityManager(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public string HashPassword(string password)
		{
			return identityHasher.HashPassword(null, password);
		}

		public bool IsMd5Hash(string hash)
		{
			return !string.IsNullOrEmpty(hash)
				&& hash.Length == Md5HashLength
				&& hash.Replace("-", string.Empty).Length == 32;
		}

		public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
		{
			if (string.IsNullOrEmpty(hashedPassword) || providedPassword == null)
			{
				return false;
			}

			if (IsLegacyHash(hashedPassword, out var algorithm))
			{
				using (algorithm)
				{
					return VerifyLegacyHash(hashedPassword, providedPassword, algorithm);
				}
			}

			var result = identityHasher.VerifyHashedPassword(null, hashedPassword, providedPassword);
			return result == PasswordVerificationResult.Success
				|| result == PasswordVerificationResult.SuccessRehashNeeded;
		}

		public static bool IsLegacyHash(string hashedPassword, out HashAlgorithm algorithm)
		{
			algorithm = null;
			if (string.IsNullOrEmpty(hashedPassword))
			{
				return false;
			}

			// Helper to check if string is hex or '-' only
			bool IsHexOrDash(string s) =>
				s.All(c => (c >= '0' && c <= '9') ||
				           (c >= 'a' && c <= 'f') ||
				           (c >= 'A' && c <= 'F') ||
				           c == '-');

			if (hashedPassword.Length == Md5HashLength && IsHexOrDash(hashedPassword))
			{
				var hexLength = hashedPassword.Replace("-", "").Length;
				if (hexLength == 32) // MD5 is 128 bits = 32 hex chars
				{
					algorithm = MD5.Create();
					return true;
				}
			}

			if (hashedPassword.Length == Sha512HashLength)
			{
				algorithm = SHA512.Create();
				return true;
			}

			return false;
		}

		private static bool VerifyLegacyHash(string hashedPassword, string providedPassword, HashAlgorithm algorithm)
		{
			byte[] expected;
			try
			{
				expected = Convert.FromHexString(hashedPassword.Replace("-", string.Empty));
			}
			catch (FormatException)
			{
				return false;
			}

			var computed = algorithm.ComputeHash(Encoding.Unicode.GetBytes(providedPassword));
			return CryptographicOperations.FixedTimeEquals(expected, computed);
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
