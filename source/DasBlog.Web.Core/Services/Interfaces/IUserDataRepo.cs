using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Core.Services.Interfaces
{
	public interface IUserDataRepo
	{
		IEnumerable<User> LoadUsers();
		void SaveUsers(List<User> users);
	}
}
