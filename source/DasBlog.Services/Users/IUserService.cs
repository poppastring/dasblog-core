using System;
using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Services.Users
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
		(bool userFound, User user) FindMatchingUser(string email);
		bool DeleteUser(string email);
		void AddOrReplaceUser(User user, string originalEmail);
	}
}
