using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class AddCommentViewModel
    {
		[Required]
		[StringLength(60, MinimumLength = 1)]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Email (will not be displayed or shared)")]
		[StringLength(60, MinimumLength = 1)]
		public string Email { get; set; }

		[Display(Name = "Home page (optional)")]
		[StringLength(60, MinimumLength = 1)]
		public string HomePage { get; set; }

		[Required]
		[Display(Name = "Comment")]
		[StringLength(600, MinimumLength = 1)]
		public string Comment { get; set; }

		public string TargetEntryId { get; set; }
	}
}
