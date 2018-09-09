using System.Threading.Tasks;
using DasBlog.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
    public class DeleteCommentTagHelper : TagHelper
	{
		public string BlogPostId { get; set; }

		public string CommentId { get; set; }

		public string CommentorName { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public DeleteCommentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:deleteComment(\"{BlogPostId}\",\"{CommentId}\",\"{CommentorName}\")");
			output.Attributes.SetAttribute("class", "dbc-comment-delete-link");
			output.Content.SetHtmlContent("Delete this comment");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
