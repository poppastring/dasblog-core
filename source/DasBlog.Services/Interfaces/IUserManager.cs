using DasBlog.Core.Security;

namespace DasBlog.Services
{
	/// <summary>
	/// User lookup and management operations.
	/// </summary>
	public interface IUserManager
	{
		User GetUser(string userName);
		User GetUserByEmail(string email);
		void AddUser(User user);
		bool IsAdmin(string gravatarhash);
	}
}
