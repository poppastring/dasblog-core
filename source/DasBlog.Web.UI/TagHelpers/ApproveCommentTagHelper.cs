using System.Threading.Tasks;
using DasBlog.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
    public class ApproveCommentTagHelper : TagHelper
    {
		public string BlogPostId { get; set; }

		public string CommentId { get; set; }

		public string CommentorName { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;
		private const string COMMENTAPPROVE_URL = "{0}/comments/{1}";
		private const string COMMENTTEXT_MSG = "Are you sure you want to approve the comment from '{0}'?";

		public ApproveCommentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var approvalurl = string.Format(COMMENTAPPROVE_URL, dasBlogSettings.GetPermaLinkUrl(BlogPostId), CommentId);
			var commenttxt = string.Format(COMMENTTEXT_MSG, CommentorName);

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{approvalurl}\",\"{commenttxt}\",\"PATCH\")");
			output.Attributes.SetAttribute("class", "dbc-comment-approve-link");
			output.Content.SetHtmlContent("Approve this comment");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
