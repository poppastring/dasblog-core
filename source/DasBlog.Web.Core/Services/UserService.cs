using System;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Core.Security;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Core.Services
{
	public class UserService : IUserService
	{
		private readonly IUserDataRepo _userRepo;
		public UserService(IUserDataRepo userRepo)
		{
			_userRepo = userRepo;
		}
		public IEnumerable<User> GetAllUsers()
		{
			return _userRepo.LoadUsers();
		}

		public void SaveUsers(List<User> users)
		{
			_userRepo.SaveUsers(users);
		}

		public User GetFirstUser() => _userRepo.LoadUsers().FirstOrDefault() ?? new User();
		public bool HasUsers()
		{
			return _userRepo.LoadUsers().Count() != 0;
		}

		public (bool userFound, User user) FindFirstMatchingUser(Func<User, bool> pred)
		{
			User user;
			if ((user = _userRepo.LoadUsers().FirstOrDefault(pred)) != null)
			{
				return (true, user);
			}
			else
			{
				return (false, new User());
			}
		}

		public bool DeleteUser(Func<User, bool> pred)
		{
			var users = _userRepo.LoadUsers().ToList();
			var user = users.FirstOrDefault(pred);
			if (users.Remove(user))
			{
				_userRepo.SaveUsers(users);
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
			var users = _userRepo.LoadUsers().ToList();
			var index = users.FindIndex(u => u.EmailAddress == originalEmail);
			if (index == -1)
			{
				users.Add(user);
			}
			else
			{
				users[index] = user;
			}
			_userRepo.SaveUsers(users);
		}
	}
}
