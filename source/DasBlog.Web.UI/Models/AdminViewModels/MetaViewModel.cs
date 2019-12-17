using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class MetaViewModel
	{
		[DisplayName("Site meta data")]
		[Description("The text in these tags is not displayed, but parsable and tells the browsers specific information about the page.")]
		public string MetaDescription { get; set; }

		[DisplayName("Site meta keywords")]
		[Description("HTML code that help tells the search engines the topic of the home page.")]
		public string MetaKeywords { get; set; }

		[DisplayName("Twitter card type")]
		[Description("Must be set to a value of 'summary'")]
		public string TwitterCard { get; set; }

		[DisplayName("Twitter Site (@username)")]
		[Description("The Twitter @username the card should be attributed to.")]
		public string TwitterSite { get; set; }

		[DisplayName("Twitter Creator (@username)")]
		[Description("Used when creators (contributors) differ from the twitter name associated with the site")]
		public string TwitterCreator { get; set; }

		[DisplayName("Twitter Image")]
		[Description("Use this image if no image exists in the post")]
		public string TwitterImage { get; set; }

		[DisplayName("Facebook Admin")]
		[Description("")]
		public string FaceBookAdmins { get; set; }

		[DisplayName("Facebook App Id")]
		[Description("")]
		public string FaceBookAppID { get; set; }

		[DisplayName("Google Analytics ID")]
		[Description("Used in the RSS Feed")]
		public string GoogleAnalyticsID {get; set;}
	}
}
