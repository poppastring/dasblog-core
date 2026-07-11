using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class LoginWith2faViewModel
	{
		[Required]
		[Display(Name = "Authenticator code")]
		[StringLength(7, ErrorMessage = "The authenticator code must be 6 digits.", MinimumLength = 6)]
		[DataType(DataType.Text)]
		public string TwoFactorCode { get; set; }

		[DisplayName("Remember this browser?")]
		public bool RememberMachine { get; set; }

		public bool RememberMe { get; set; }

		public string ReturnUrl { get; set; }
	}
}
