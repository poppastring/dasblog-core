using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class EnableAuthenticatorViewModel
	{
		public string SharedKey { get; set; }

		public string AuthenticatorUri { get; set; }

		public string QrCodeSvg { get; set; }

		public bool IsTwoFactorEnabled { get; set; }

		[Required]
		[Display(Name = "Verification code")]
		[StringLength(7, ErrorMessage = "The verification code must be 6 digits.", MinimumLength = 6)]
		[DataType(DataType.Text)]
		public string Code { get; set; }

		public IList<string> RecoveryCodes { get; set; } = new List<string>();
	}
}
