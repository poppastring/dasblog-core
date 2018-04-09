using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DasBlog.Web.Models.Identity;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class LoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DisplayName("Remember me?")]
		public bool RememberMe { get; set; }
	}
}
