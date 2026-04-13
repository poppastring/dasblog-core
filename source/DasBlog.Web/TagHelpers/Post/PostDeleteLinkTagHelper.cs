using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostDeleteLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public string BlogPostId { get; set; }

		public string BlogTitle { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Post != null)
			{
				BlogPostId = Post.EntryId;
				BlogTitle = Post.Title;
			}

			var modalId = $"deleteModal_{BlogPostId?.Replace("-", "")}";

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", "#");
			output.Attributes.SetAttribute("data-bs-toggle", "modal");
			output.Attributes.SetAttribute("data-bs-target", $"#{modalId}");
			output.Attributes.SetAttribute("role", "button");
			output.Content.SetHtmlContent("Delete this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
