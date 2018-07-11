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
	}
}
