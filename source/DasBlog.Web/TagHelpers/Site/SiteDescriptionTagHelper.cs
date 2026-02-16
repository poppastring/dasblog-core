using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class SiteDescriptionTagHelper : TagHelper
	{
		private readonly ISiteConfig siteConfig;

		public SiteDescriptionTagHelper(ISiteConfig siteConfig)
		{
			this.siteConfig = siteConfig;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(siteConfig.Description);
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await Task.Run(() => Process(context, output));
		}
	}
}
