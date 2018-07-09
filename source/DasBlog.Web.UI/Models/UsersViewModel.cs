using System.ComponentModel.DataAnnotations;
using DasBlog.Core.Security;

namespace DasBlog.Web.Models
{
	public class UsersViewModel
	{
		public UsersViewModel()
			{}
		
		[Required]
		public string Name { get; set; }

		[Required]
		public Role Role { get; set; }

		public bool Ask { get; set; }
		
		[Required]
		[EmailAddress]
		public string EmailAddress { get; set; }
		
		[Display(Name= "Dipslay Name")]
		public string DisplayName { get; set; }

		
		[Display(Name= "Open ID Url")]
		public string OpenIDUrl { get; set; }

		[Display(Name= "Receive Notifications For A New Post")]
		public bool NotifyOnNewPost { get; set; }

		[Display(Name= "Receive Notifications for Own Comments")]
		public bool NotifyOnAllComment { get; set; }

		[Display(Name= "Receive Notifications for All Comments")]
		public bool NotifyOnOwnComment { get; set; }

		public bool Active { get; set; }
		
		[Required]
		public string Password { get; set; }
	}
}
