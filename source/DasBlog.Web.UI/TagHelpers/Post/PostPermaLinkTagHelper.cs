using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostPermaLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public string Css { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public PostPermaLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var linkvalue = Post.Title;
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "a";

			var content = await output.GetChildContentAsync();

			if(string.IsNullOrWhiteSpace(content.GetContent()))
			{
				linkvalue = content.GetContent();
			}

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot(Post.PermaLink));
			output.Content.SetHtmlContent(linkvalue);
		}

	}
}
