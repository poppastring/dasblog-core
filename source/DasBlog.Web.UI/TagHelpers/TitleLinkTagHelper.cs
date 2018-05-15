using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
    public class TitleLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private readonly IDasBlogSettings _dasBlogSettings;

		public TitleLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			_dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "a";

			var content = await output.GetChildContentAsync();
			var css = content.GetContent();
			output.Attributes.SetAttribute("class", css);
			output.Attributes.SetAttribute("href", _dasBlogSettings.RelativeToRoot(Post.PermaLink));
			output.Content.SetHtmlContent(Post.Title);
		}
	}
}
