using System;
using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Core.Services.Interfaces
{
	public interface IUserService
	{
		IEnumerable<User> GetAllUsers();
		void SaveUsers(List<User> users);
		/// <summary>
		/// provides the first user in the repo  - typically to provide somee sort of default
		/// for the UI.
		/// </summary>
		/// <returns>The first user or if none then a new user.</returns>
		User GetFirstUser();

		bool HasUsers();
		(bool userFound, User user) FindFirstMatchingUser(Func<User, bool> pred);
		bool DeleteUser(Func<User, bool> pred);
		void AddOrReplaceUser(User user, string originalEmail);
	}
}
