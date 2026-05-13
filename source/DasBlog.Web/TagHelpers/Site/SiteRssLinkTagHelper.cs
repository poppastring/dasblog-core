using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("site-rss-link", TagStructure = TagStructure.WithoutEndTag)]
	public class SiteRssLinkTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteRssLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			// Emit two <link> tags; suppress the wrapping element.
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var rssLink = $"<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{System.Net.WebUtility.HtmlEncode(dasBlogSettings.SiteConfiguration.Description)}\" href=\"{dasBlogSettings.RssUrl}\" />";
			var rsdLink = $"<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{dasBlogSettings.RsdUrl}\" />";

			output.Content.SetHtmlContent(rssLink + System.Environment.NewLine + rsdLink);
		}
	}
}
