using DasBlog.Core.Security;

namespace DasBlog.Web.Models
{
	public class UsersViewModel
	{
		private readonly User _user;
		public UsersViewModel(User user)
		{
			_user = user;
		}

		public User User => _user;
	}
}
