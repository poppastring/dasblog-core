using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentApprovalLinkTagHelper : TagHelper
	{
		private readonly IUrlHelperFactory urlHelperFactory;

		public CommentApprovalLinkTagHelper(IUrlHelperFactory urlHelperFactory)
		{
			this.urlHelperFactory = urlHelperFactory;
		}

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public CommentViewModel Comment { get; set; }

		public bool Admin { get; set; } = false;

		public string Css { get; set; }

		private const string COMMENTTEXT_MSG = "Are you sure you want to approve the comment from '{0}'?";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Comment Approved";

			var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
			var actionUrl = urlHelper.Action(
				"ApproveComment",
				"BlogPost",
				new { postid = Comment.BlogPostId, commentid = Comment.CommentId });

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{actionUrl}\",\"{commenttxt}\",\"PATCH\")");
			var cssClass = string.IsNullOrWhiteSpace(Css) ? "dbc-comment-approve-link" : $"dbc-comment-approve-link {Css}";
			output.Attributes.SetAttribute("class", cssClass);

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
