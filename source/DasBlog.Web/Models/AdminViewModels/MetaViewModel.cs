using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class MetaViewModel : IValidatableObject
	{
		public const string DefaultPublisherType = "Person";

		public static readonly IReadOnlyList<string> PublisherTypes = new[]
		{
			"Person",
			"Organization"
		};

		public static string NormalizePublisherType(string publisherType)
		{
			if (string.IsNullOrWhiteSpace(publisherType))
			{
				return DefaultPublisherType;
			}

			return PublisherTypes.FirstOrDefault(type => string.Equals(type, publisherType.Trim(), System.StringComparison.OrdinalIgnoreCase)) ?? DefaultPublisherType;
		}

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

		[DisplayName("Default site image")]
		[Description("The fallback image used for social previews, including Open Graph and X/Twitter cards, and structured site identity when a page or post does not provide its own image.")]
		[StringLength(300, MinimumLength = 0, ErrorMessage = "{0} should be between 1 to 300 characters")]
		public string TwitterImage { get; set; }

		[DisplayName("Structured data publisher type")]
		[Description("Schema.org type used for the site's publisher. Name, URL, image, and profile links come from existing site metadata.")]
		public string PublisherType { get; set; }

		[DisplayName("Mastodon Server")]
		[Description("")]
		[DataType(DataType.Url, ErrorMessage = "Invalid URL format")]
		public string MastodonServerUrl { get; set; }

		[DisplayName("Mastodon Account (@username)")]
		[Description("")]
		[RegularExpression("(@)((?:[A-Za-z0-9-_]*))")]
		public string MastodonAccount { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!string.IsNullOrWhiteSpace(PublisherType)
				&& !PublisherTypes.Any(type => string.Equals(type, PublisherType, System.StringComparison.OrdinalIgnoreCase)))
			{
				yield return new ValidationResult("Publisher type must be a supported schema.org publisher type.", new[] { nameof(PublisherType) });
			}
		}
	}
}
