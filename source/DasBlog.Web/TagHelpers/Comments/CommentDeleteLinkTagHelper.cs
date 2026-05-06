using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentDeleteLinkTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public CommentDeleteLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public CommentViewModel Comment { get; set; }

		public bool Admin { get; set; } = false;

		public string Css { get; set; }

		private const string COMMENTTEXT_MSG = "Are you sure you want to delete the comment from '{0}'?";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Delete Comment";

			var actionUrl = dasBlogSettings.RelativeToRoot($"admin/post/{Comment.BlogPostId}/comments/{Comment.CommentId}");

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{actionUrl}\",\"{commenttxt}\",\"DELETE\")");
			var cssClass = string.IsNullOrWhiteSpace(Css) ? "dbc-comment-delete-link" : $"dbc-comment-delete-link {Css}";
			output.Attributes.SetAttribute("class", cssClass);
			output.Attributes.SetAttribute("aria-label", "Delete Comment");
			
			var content = await output.GetChildContentAsync();

			if (!string.IsNullOrWhiteSpace(content.GetContent()))
			{
				message = content.GetContent().Trim();
			}

			output.Content.SetHtmlContent(message);
		}

	}
}
