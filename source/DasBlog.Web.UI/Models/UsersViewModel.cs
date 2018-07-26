using System.ComponentModel.DataAnnotations;
using DasBlog.Core.Security;

namespace DasBlog.Web.Models
{
	public class UsersViewModel
	{
		public UsersViewModel()
			{}
		
		public string Name { get; set; }

		[Required]
		public Role Role { get; set; }

		public string /*bool*/ Ask { get; set; }
		
		[Required]
		[EmailAddress]
		[Display(Name="Email Address")]
		public string EmailAddress { get; set; }
		
		[Display(Name= "Dipslay Name")]
		public string DisplayName { get; set; }

		
		[Display(Name= "Open ID Url")]
		public string OpenIDUrl { get; set; }

		[Display(Name= "Receive Notifications For A New Post")]
		public string /*bool*/ NotifyOnNewPost { get; set; }

		[Display(Name= "Receive Notifications for All Comments")]
		public string /*bool*/ NotifyOnAllComment { get; set; }

		[Display(Name= "Receive Notifications for Own Comments")]
		public string /*bool*/ NotifyOnOwnComment { get; set; }

		public string /*bool*/ Active { get; set; }
		
		//[Required] - nor required for delete - so validate this conditionally
		public string Password { get; set; }

		public string Writability { get; set; }
	}
}
