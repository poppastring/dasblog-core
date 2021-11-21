using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentApprovalLinkTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public bool Admin { get; set; } = false;

		private readonly IDasBlogSettings dasBlogSettings;

		private const string COMMENTAPPROVE_URL = "{0}/comments/{1}";
		private const string COMMENTTEXT_MSG = "Are you sure you want to approve the comment from '{0}'?";

		public CommentApprovalLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var approvalurl = string.Format(COMMENTAPPROVE_URL, dasBlogSettings.GetPermaLinkUrl(Comment.BlogPostId), Comment.CommentId);
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Comment Approved";
			var admin = string.Empty;

			if (Admin)
			{
				admin = "ADMIN";
			}

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{approvalurl}\",\"{commenttxt}\",\"PATCH\",\"{admin}\")");
			output.Attributes.SetAttribute("class", "dbc-comment-approve-link");

			var content = await output.GetChildContentAsync();

			if(string.IsNullOrWhiteSpace(content.GetContent()))
			{
				if (Comment.SpamState != SpamStateViewModel.NotSpam)
				{
					message = "Approve this comment";
				}
				else
				{
					message = "Comment Approved";
				}
			}
			else
			{
				message = content.GetContent().Trim();
			}

			output.Content.SetHtmlContent(message);
		}
	}
}
