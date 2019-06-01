using DasBlog.Core.Common;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;


namespace DasBlog.Web.TagHelpers
{
	public class CommentPostTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public CommentPostTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", dasBlogSettings.GetCommentViewUrl(Post.EntryId));
			output.Attributes.SetAttribute("id", Constants.CommentOnThisPostId);
			output.Attributes.SetAttribute("class", "dbc-comment-on-post-link");
			output.Content.SetHtmlContent("Comment on this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
