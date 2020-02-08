using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class SiteRootTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteRootTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public string Title { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(dasBlogSettings?.SiteConfiguration?.Root);
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await Task.Run(() => Process(context, output));
		}
	}
}
