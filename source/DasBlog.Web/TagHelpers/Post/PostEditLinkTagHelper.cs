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

		private readonly IUrlResolver urlResolver;

		public PostEditLinkTagHelper(IUrlResolver urlResolver)
		{
			this.urlResolver = urlResolver;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Post != null)
			{
				BlogPostId = Post.EntryId;
			}

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", urlResolver.RelativeToRoot("admin/post/" + BlogPostId + "/edit"));
			output.Attributes.SetAttribute("class", "btn btn-sm btn-outline-primary");
			if (!string.IsNullOrEmpty(EditLinkText))
			{
				output.Content.SetHtmlContent("<i class=\"fa-solid fa-pen-to-square me-1\" aria-hidden=\"true\"></i>");
				output.Content.Append(EditLinkText);
			}

		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
