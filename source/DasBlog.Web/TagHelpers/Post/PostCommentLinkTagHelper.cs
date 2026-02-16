using DasBlog.Core.Common;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostCommentLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }
		public string LinkText { get; set; }

		private readonly ISiteConfig siteConfig;
		private readonly IUrlResolver urlResolver;

		public PostCommentLinkTagHelper(ISiteConfig siteConfig, IUrlResolver urlResolver)
		{
			this.siteConfig = siteConfig;
			this.urlResolver = urlResolver;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", urlResolver.GetCommentViewUrl(Post.PermaLink));
			output.Attributes.SetAttribute("class", "dbc-comment-on-post-link");

			var content = "Comment on this post";
			var commentCount = Post.Comments?.Comments.Count ?? 0;
			if (string.IsNullOrWhiteSpace(LinkText))
			{
				if (siteConfig.ShowCommentCount)
				{
					content = string.Format("{0} [{1}]", content, commentCount);
				}
			}
			else
			{
				content = string.Format(LinkText, commentCount);
			}

			output.Content.SetHtmlContent(content);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
