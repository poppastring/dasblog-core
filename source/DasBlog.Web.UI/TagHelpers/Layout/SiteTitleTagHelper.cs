using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class SiteTitleTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteTitleTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public string Title { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var title = dasBlogSettings?.SiteConfiguration?.Title ?? Title;
			output.TagName = "";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(title);
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await Task.Run(() => Process(context, output));
		}
	}
}
