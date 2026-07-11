using System.Collections.Generic;

namespace DasBlog.Web.Models.AccountViewModels
{
	public class RecoveryCodesViewModel
	{
		public IList<string> RecoveryCodes { get; set; } = new List<string>();
	}
}
