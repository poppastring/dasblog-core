using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class SiteCopyrightTagHelper : TagHelper
	{
		private readonly ISiteConfig siteConfig;

		public SiteCopyrightTagHelper(ISiteConfig siteConfig)
		{
			this.siteConfig = siteConfig;
		}

		public string Title { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(string.Format("\u00a9 {0} - {1}", DateTime.Now.Year, siteConfig.Copyright));
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await Task.Run(() => Process(context, output));
		}
	}
}
