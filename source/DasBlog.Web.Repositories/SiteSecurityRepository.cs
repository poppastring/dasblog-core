using DasBlog.Web;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace DasBlog.Managers
{
	public class SiteSecurityRepository : ISiteSecurityRepository
	{
		private IPrincipal _principal;
		private IDasBlogSettings _dasBlogSettings;

		public SiteSecurityRepository(IPrincipal principal, IDasBlogSettings dasBlogSettings)
		{
			_dasBlogSettings = dasBlogSettings;
			_principal = principal;
		}

		public bool IsValidContributor { get { return _principal.IsInRole(null); } }

		public bool IsInRole(Role? role)
		{
			return _principal.IsInRole(role?.ToString());
		}

		public void AddUser(string userName, string password, Role? role, bool ask, string emailAddress)
		{
			if (string.IsNullOrEmpty(userName)) { throw new ArgumentNullException(nameof(userName)); }
			if (string.IsNullOrEmpty(password)) { throw new ArgumentNullException(nameof(password)); }

			User user = new User
			{
				Name = userName,
				Password = this.Encrypt(password),
				Role = role ?? Role.Contributor,
				Ask = ask,
				EmailAddress = emailAddress
			};

			AddUser(user);
		}

		public void AddUser(User user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			_dasBlogSettings.SecurityConfiguration.Users.Add(user);
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

		public void UpdateUser(User user)
		{
			throw new NotImplementedException();
		}

		public bool DoSuperChallenge(string challenge, string passwordIN, string userName, string clientHash)
		{
			throw new NotImplementedException();
		}

		public string Encrypt(string cleanString)
		{
			if (!IsCleanStringEncrypted(cleanString))
			{
				Byte[] clearBytes = Encoding.Unicode.GetBytes(cleanString);

				MD5 md5 = MD5CryptoServiceProvider.Create();
				Byte[] hashedBytes = md5.ComputeHash(clearBytes);

				return BitConverter.ToString(hashedBytes);
			}
			else
				return cleanString;
		}

		public UserToken GetToken(string userName)
		{
			User user = GetUser(userName);
			return user?.ToToken();
		}

		public bool IsCleanStringEncrypted(string cleanString)
		{
			return (cleanString.Length == 47 && cleanString.Replace("-", String.Empty).Length == 32);
		}

		public void Login(UserToken token, string userName)
		{
			if (token == null)
			{
				// SiteSecurity.LogFailure(userName);
			}
			else
			{
				// SiteSecurity.LogSuccess(token.Name);
				GenericIdentity identity = new GenericIdentity(token.Name, "Custom");
				GenericPrincipal principal = new GenericPrincipal(identity, new string[] { token.Role });
				// HttpContext.Current.User = principal;
				System.Threading.Thread.CurrentPrincipal = principal;
			}
		}

		public UserToken Login(string userName, string password)
		{
			UserToken token = null;

			User user = GetUser(userName);

			if (user != null && user.Active)
			{
				//Make sure password is encrypted
				if (!IsCleanStringEncrypted(password))
				{
					password = Encrypt(password);
				}

				if ((IsCleanStringEncrypted(user.Password) && user.Password == password) || (Encrypt(user.Password) == password))
				{
					token = user.ToToken();
				}
			}

			Login(token, userName);

			return token;
		}

		public UserToken Login(string userName, string clientHash, string challenge)
		{
			throw new NotImplementedException();
		}

		public void SetPassword(string userName, string password)
		{
			throw new NotImplementedException();
		}
	}
}
