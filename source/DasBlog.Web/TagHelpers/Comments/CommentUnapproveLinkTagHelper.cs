using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentUnapproveLinkTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public string Css { get; set; }

		private const string COMMENTTEXT_MSG = "Move the comment from '{0}' back to pending review?";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Move to Pending";

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{Comment.BlogPostId}\",\"{Comment.CommentId}\",\"{commenttxt}\",\"PATCH\",\"UNAPPROVE\")");
			var cssClass = string.IsNullOrWhiteSpace(Css) ? "dbc-comment-unapprove-link" : $"dbc-comment-unapprove-link {Css}";
			output.Attributes.SetAttribute("class", cssClass);

			var content = await output.GetChildContentAsync();

			if (!string.IsNullOrWhiteSpace(content.GetContent()))
			{
				message = content.GetContent().Trim();
			}

			output.Content.SetHtmlContent(message);
		}
	}
}
