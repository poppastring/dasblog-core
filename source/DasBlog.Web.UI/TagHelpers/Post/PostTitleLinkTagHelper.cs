using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostTitleLinkTagHelper : TagHelper

	{
		public PostViewModel Post { get; set; }

		public string Css { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public PostTitleLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "a";

			var content = await output.GetChildContentAsync();
			var title = content.GetContent();

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			if (string.IsNullOrWhiteSpace(title))
			{
				title = Post.Title;
			}

			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot(Post.PermaLink));
			output.Content.SetHtmlContent(title);
		}
	}
}
