using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentMailtoLinkTagHelper : TagHelper
	{
		public CommentAdminViewModel Comment { get; set; }

		private const string MAILTOLINK = "mailto:{0}?subject={1}";

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var maillink = string.Format(MAILTOLINK, Comment.Email, Comment.Title);

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", maillink);
			output.Attributes.SetAttribute("class", "dbc-comment-mailto-link");

			var content = await output.GetChildContentAsync();

			output.Content.SetHtmlContent(content.GetContent());
		}
	}
}
