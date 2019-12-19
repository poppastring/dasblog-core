using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class MetaViewModel
	{
		[DisplayName("Site meta data")]
		[Description("The text in these tags is not displayed, but parsable and tells the browsers specific information about the page.")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string MetaDescription { get; set; }

		[DisplayName("Site meta keywords")]
		[Description("HTML code that help tells the search engines the topic of the home page.")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string MetaKeywords { get; set; }

		[DisplayName("Twitter card type")]
		[Description("Must be set to a value of 'summary'")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterCard { get; set; }

		[DisplayName("Twitter Site (@username)")]
		[Description("The Twitter @username the card should be attributed to.")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterSite { get; set; }

		[DisplayName("Twitter Creator (@username)")]
		[Description("Used when creators (contributors) differ from the twitter name associated with the site")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterCreator { get; set; }

		[DisplayName("Twitter Image")]
		[Description("The Twitter share button will use this image if no image exists in the blog post...")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterImage { get; set; }

		[DisplayName("Facebook Admin")]
		[Description("")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string FaceBookAdmins { get; set; }

		[DisplayName("Facebook App Id")]
		[Description("")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string FaceBookAppID { get; set; }

		[DisplayName("Google Analytics ID")]
		[Description("Used in the RSS Feed")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string GoogleAnalyticsID {get; set;}
	}
}
