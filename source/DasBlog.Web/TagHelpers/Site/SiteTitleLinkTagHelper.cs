using System.Threading.Tasks;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class SiteTitleLinkTagHelper : TagHelper
	{
		public string Css { get; set; }

		private readonly ISiteConfig siteConfig;

		public SiteTitleLinkTagHelper(ISiteConfig siteConfig)
		{
			this.siteConfig = siteConfig;
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

			output.Attributes.SetAttribute("href", siteConfig.Root);
			output.Content.SetHtmlContent(siteConfig.Title);
		}

	}
}
