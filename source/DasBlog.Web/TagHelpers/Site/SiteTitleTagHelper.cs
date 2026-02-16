using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class SiteTitleTagHelper : TagHelper
	{
		private readonly ISiteConfig siteConfig;

		public SiteTitleTagHelper(ISiteConfig siteConfig)
		{
			this.siteConfig = siteConfig;
		}

		public string Title { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(siteConfig?.Title);
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await Task.Run(() => Process(context, output));
		}
	}
}
