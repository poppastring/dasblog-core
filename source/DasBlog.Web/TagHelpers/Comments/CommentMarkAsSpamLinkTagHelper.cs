using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentMarkAsSpamLinkTagHelper : TagHelper
	{
		private readonly IUrlHelperFactory urlHelperFactory;

		public CommentMarkAsSpamLinkTagHelper(IUrlHelperFactory urlHelperFactory)
		{
			this.urlHelperFactory = urlHelperFactory;
		}

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public CommentViewModel Comment { get; set; }

		public string Css { get; set; }

		private const string COMMENTTEXT_MSG = "Mark the comment from '{0}' as spam? It will be hidden from public view.";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Mark as Spam";

			var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
			var actionUrl = urlHelper.Action(
				"MarkCommentAsSpam",
				"BlogPost",
				new { postid = Comment.BlogPostId, commentid = Comment.CommentId });

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{actionUrl}\",\"{commenttxt}\",\"PATCH\",\"SPAM\")");
			var cssClass = string.IsNullOrWhiteSpace(Css) ? "dbc-comment-spam-link" : $"dbc-comment-spam-link {Css}";
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
