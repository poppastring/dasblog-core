using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostEditLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }
		public string BlogPostId { get; set; }
		public string EditLinkText { get; set; } = "Edit this post";

		private readonly IDasBlogSettings dasBlogSettings;

		public PostEditLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Post != null)
			{
				BlogPostId = Post.EntryId;
			}

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", dasBlogSettings.GetPermaLinkUrl(BlogPostId + "/edit"));
			if (!string.IsNullOrEmpty(EditLinkText))
			{
				output.Content.SetHtmlContent("Edit this post");
			}

		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
