using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("site-atom-link", TagStructure = TagStructure.WithoutEndTag)]
	public class SiteAtomLinkTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteAtomLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "link";
			output.TagMode = TagMode.SelfClosing;
			output.Attributes.SetAttribute("rel", "alternate");
			output.Attributes.SetAttribute("type", "application/atom+xml");
			output.Attributes.SetAttribute("title", dasBlogSettings.SiteConfiguration.Description);
			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot("feed/atom"));
		}
	}
}
