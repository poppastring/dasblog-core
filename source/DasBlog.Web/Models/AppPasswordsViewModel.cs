using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models
{
	public class AppPasswordsViewModel
	{
		[Required]
		[StringLength(80, MinimumLength = 1)]
		[Display(Name = "App password name")]
		public string NewPasswordName { get; set; }

		public string GeneratedPassword { get; set; }

		public string AuthorName { get; set; }

		public string LoginName { get; set; }

		public List<AppPasswordListItem> AppPasswords { get; set; } = new List<AppPasswordListItem>();
	}

	public class AppPasswordListItem
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Created { get; set; }

		public string LastUsed { get; set; }

		public string LastIp { get; set; }
	}
}
