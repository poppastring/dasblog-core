using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class AddCommentViewModel
    {
		[Required]
		[Display(Name = "Name")]
		[StringLength(60, MinimumLength = 1)]
		[RegularExpression("^[\\w'\\-,.][^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\\]]{2,}$", ErrorMessage = "Invalid name format")]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Email (will not be displayed or shared)")]
		[StringLength(60, MinimumLength = 1)]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; }

		[Display(Name = "Home page (optional)")]
		[StringLength(60, MinimumLength = 1)]
		[Url(ErrorMessage ="Invalid home page")]
		public string HomePage { get; set; }

		[Required]
		[Display(Name = "Content")]
		[StringLength(600, MinimumLength = 1)]
		public string Content { get; set; }

		public string CheesyQuestionAnswered { get; set; }

		[Required]
		public string TargetEntryId { get; set; }

		
	}
}
