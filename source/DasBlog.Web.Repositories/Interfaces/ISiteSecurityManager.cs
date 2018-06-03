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

		bool IsMd5Hash(string hash);

		bool VerifyHashedPassword(string hashedPassword, string providedPassword);

		User GetUser(string userName);

		User GetUserByDisplayName(string displayName);

		User GetUserByEmail(string email);
	}
}
