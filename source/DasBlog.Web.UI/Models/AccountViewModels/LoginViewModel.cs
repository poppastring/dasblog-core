using DasBlog.Web.UI.Models.Identity;

namespace DasBlog.Web.UI.Models.AccountViewModels
{
	public class LoginViewModel
	{
		public string Email { get; set; }

		public string Password { get; set; }

		public bool RememberMe { get; set; }
	}
}
