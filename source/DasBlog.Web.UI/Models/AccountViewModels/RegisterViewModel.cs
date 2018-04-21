using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[StringLength(100, ErrorMessage = "Your {0} must be at least {2} and at most {1} characters long.", MinimumLength = 2)]
		[Display(Name = "Display Name")]
		public string Name { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Display(Name = "Notify on new post")]
		public bool NotifyOnNewPost { get; set; }

		[Display(Name = "Notify on all comments")]
		public bool NotifyOnAllComment { get; set; }

		[Display(Name = "Notify on own comments")]
		public bool NotifyOnOwnComment { get; set; }

	}
}
