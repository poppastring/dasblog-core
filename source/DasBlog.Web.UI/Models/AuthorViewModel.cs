using System.ComponentModel.DataAnnotations;
using DasBlog.Core.Security;

namespace DasBlog.Web.Models
{
	public class AuthorViewModel
	{
		public string Name { get; set; }

		[Required]
		public Role Role { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email Address")]
		public string EmailAddress { get; set; }

		[Required]
		[Display(Name = "Display Name")]
		[StringLength(50, ErrorMessage = "Invalid Display Name length", MinimumLength = 1)]
		public string DisplayName { get; set; }

		[Display(Name = "Open ID Url")]
		public string OpenIDUrl { get; set; }

		[Display(Name = "Receive Notifications For A New Post")]
		public bool NotifyOnNewPost { get; set; }

		[Display(Name = "Receive Notifications for All Comments")]
		public bool NotifyOnAllComment { get; set; }

		[Display(Name = "Receive Notifications for Own Comments")]
		public bool NotifyOnOwnComment { get; set; }

		[Required]
		[Display(Name = "Password")]
		[StringLength(100, ErrorMessage = "Invalid Password length", MinimumLength = 1)]
		public string Password { get; set; }

		[Required]
		public bool Active { get; set; }

		[Required]
		public bool Ask { get; set; }

		public string Writability { get; set; }

		public string OriginalEmail { get; set; }
	}
}
