using System.Threading.Tasks;
using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class SiteTitleLinkTagHelper : TagHelper
	{
		public string Css { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public SiteTitleLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "a";

			var content = await output.GetChildContentAsync();

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			output.Attributes.SetAttribute("href", dasBlogSettings.SiteConfiguration.Root);
			output.Content.SetHtmlContent(dasBlogSettings.SiteConfiguration.Title);
		}

	}
}
