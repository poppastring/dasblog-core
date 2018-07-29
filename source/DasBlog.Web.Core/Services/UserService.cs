using System;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Core.Security;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserDataRepo userRepo;
		public UserService(IUserDataRepo userRepo)
		{
			this.userRepo = userRepo;
		}
		public IEnumerable<User> GetAllUsers()
		{
			return userRepo.LoadUsers();
		}

		public void SaveUsers(List<User> users)
		{
			userRepo.SaveUsers(users);
		}

		public User GetFirstUser() => userRepo.LoadUsers().FirstOrDefault() ?? new User();
		public bool HasUsers()
		{
			return userRepo.LoadUsers().Count() != 0;
		}

		public (bool userFound, User user) FindMatchingUser(string email)
		{
			User matchingUser;
			if ((matchingUser = userRepo.LoadUsers().FirstOrDefault(user => user.EmailAddress == email)) != null)
			{
				return (true, matchingUser);
			}
			else
			{
				return (false, new User());
			}
		}

		public bool DeleteUser(string email)
		{
			var users = userRepo.LoadUsers().ToList();
			var userToDelete = users.FirstOrDefault(user => user.EmailAddress == email);
			if (users.Remove(userToDelete))
			{
				userRepo.SaveUsers(users);
				return true;
			}
			else
			{
				return false;	// no user with that email address
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user">recently created or modified user</param>
		/// <param name="originalEmail">if the user is to be added then this will be an empty string
		/// otherwise the original email address can be used to locate the user that has been modified.
		/// This covers the case where the user's email address itself has been mdofied</param>
		public void AddOrReplaceUser(User user, string originalEmail)
		{
			var users = userRepo.LoadUsers().ToList();
			var index = users.FindIndex(u => u.EmailAddress == originalEmail);
			if (index == -1)
			{
				users.Add(user);
			}
			else
			{
				users[index] = user;
			}
			userRepo.SaveUsers(users);
		}
	}
}
