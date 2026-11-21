using DasBlog.Core.Security;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
	public interface ISiteSecurityManager
	{
		string HashPassword(string password);

		bool IsLegacyHash(string hashedPassword);

		bool VerifyHashedPassword(string hashedPassword, string providedPassword);

		bool VerifyAppPassword(User user, string providedPassword, string lastIp);

		User GetUser(string userName);

		User GetUserByDisplayName(string displayName);

		User GetUserByEmail(string email);
	}
}
