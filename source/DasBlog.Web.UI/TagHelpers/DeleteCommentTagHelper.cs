using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
    public class DeleteCommentTagHelper : TagHelper
	{
		public string BlogPostId { get; set; }

		public string CommentId { get; set; }

		public string CommentorName { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;
		private const string COMMENTDELETE_URL = "{0}/comments/{1}";
		private const string COMMENTTEXT_MSG = "Are you sure you want to delete the comment from '{0}'?";

		public DeleteCommentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var deleteurl = string.Format(COMMENTDELETE_URL, dasBlogSettings.GetPermaLinkUrl(BlogPostId), CommentId);
			var commenttxt = string.Format(COMMENTTEXT_MSG, CommentorName);

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{deleteurl}\",\"{commenttxt}\",\"DELETE\")");
			output.Attributes.SetAttribute("class", "dbc-comment-delete-link");
			output.Content.SetHtmlContent("Delete this comment");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
