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

		[DisplayName("X (Twitter) card type")]
		[Description("Allowed values: summary, summary_large_image.")]
		[RegularExpression("^(summary|summary_large_image)$", ErrorMessage = "X (Twitter) card type must be 'summary' or 'summary_large_image'.")]
		public string TwitterCard { get; set; }

		[DisplayName("X (Twitter) site (@username)")]
		[Description("The X (Twitter) @username this card should be attributed to.")]
		[RegularExpression("^$|^@?[A-Za-z0-9_]{1,15}$", ErrorMessage = "X (Twitter) handle must be 1-15 letters, numbers, or underscores, optionally prefixed with @.")]
		[StringLength(16, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 16 characters")]
		public string TwitterSite { get; set; }

		[DisplayName("X (Twitter) creator (@username)")]
		[Description("Used when creators (contributors) differ from the X (Twitter) account associated with the site.")]
		[RegularExpression("^$|^@?[A-Za-z0-9_]{1,15}$", ErrorMessage = "X (Twitter) handle must be 1-15 letters, numbers, or underscores, optionally prefixed with @.")]
		[StringLength(16, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 16 characters")]
		public string TwitterCreator { get; set; }

		[DisplayName("X (Twitter) image")]
		[Description("Used for X (Twitter) card previews when the blog post does not provide a page image.")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterImage { get; set; }

		[DisplayName("Mastodon Server")]
		[Description("")]
		[DataType(DataType.Url, ErrorMessage = "Invalid URL format")]
		public string MastodonServerUrl { get; set; }

		[DisplayName("Mastodon Account (@username)")]
		[Description("")]
		[RegularExpression("(@)((?:[A-Za-z0-9-_]*))")]
		public string MastodonAccount { get; set; }
	}
}
