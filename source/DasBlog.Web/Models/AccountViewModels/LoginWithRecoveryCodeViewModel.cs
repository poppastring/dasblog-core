using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class LoginWithRecoveryCodeViewModel
	{
		[Required]
		[DataType(DataType.Text)]
		[Display(Name = "Recovery code")]
		public string RecoveryCode { get; set; }

		public string ReturnUrl { get; set; }
	}
}
