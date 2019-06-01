using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Services.Users
{
	public interface IUserDataRepo
	{
		IEnumerable<User> LoadUsers();
		void SaveUsers(List<User> users);
	}
}
