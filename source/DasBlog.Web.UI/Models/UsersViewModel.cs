using DasBlog.Core.Security;

namespace DasBlog.Web.Models
{
	public class UsersViewModel
	{
		public UsersViewModel()
			{}
		public string Name { get; set; }

		public Role Role { get; set; }

		public bool Ask { get; set; }

		public string EmailAddress { get; set; }

		public string DisplayName { get; set; }

		public string OpenIDUrl { get; set; }

		public bool NotifyOnNewPost { get; set; }

		public bool NotifyOnAllComment { get; set; }

		public bool NotifyOnOwnComment { get; set; }

		public bool Active { get; set; }

		public string Password { get; set; }
	}
}
