using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostImageTagHelper: TagHelper

	{
		public PostViewModel Post { get; set; }

		public string Css { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public PostImageTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "img";

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			output.Attributes.SetAttribute("src", dasBlogSettings.RelativeToRoot(Post.ImageUrl));
			output.Attributes.SetAttribute("alt", Post.Title);
			await Task.CompletedTask;
		}
	}
}
