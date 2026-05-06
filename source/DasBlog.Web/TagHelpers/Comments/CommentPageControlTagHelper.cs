using System.Text;
using System.Threading.Tasks;
using DasBlog.Core.Common;
using DasBlog.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentPageControlTagHelper : TagHelper
	{
		public string NewerPostsText { get; set; } = "Newer";

		public string OlderPostsText { get; set; } = "Older";

		private int OlderPageItemCount { get; set; }
		private int PageNumber { get; set; }

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		private readonly IUrlResolver urlResolver;

		public CommentPageControlTagHelper(IUrlResolver urlResolver)
		{
			this.urlResolver = urlResolver;
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			OlderPageItemCount = (int?)ViewContext.ViewData[Constants.CommentPostCount] ?? 0;
			PageNumber = (int?)ViewContext.ViewData[Constants.CommentPageNumber] ?? 0;

			var hasNewer = PageNumber > 0;
			var hasOlder = OlderPageItemCount > 0;

			var newerUrl = urlResolver.RelativeToRoot(PageNumber <= 1
				? "admin/manage-comments"
				: $"admin/manage-comments/page/{PageNumber - 1}");
			var olderUrl = urlResolver.RelativeToRoot($"admin/manage-comments/page/{PageNumber + 1}");
			var firstUrl = urlResolver.RelativeToRoot("admin/manage-comments");

			var sb = new StringBuilder();
			sb.Append("<ul class=\"pagination pagination-sm mb-0\">");

			// First page (only show when not already on it)
			sb.Append(PageNumber > 0
				? $"<li class=\"page-item\"><a class=\"page-link\" href=\"{firstUrl}\" aria-label=\"First page\" title=\"First page\"><i class=\"fa-solid fa-angles-left\"></i></a></li>"
				: "<li class=\"page-item disabled\" aria-hidden=\"true\"><span class=\"page-link\"><i class=\"fa-solid fa-angles-left\"></i></span></li>");

			// Newer (Previous)
			sb.Append(hasNewer
				? $"<li class=\"page-item\"><a class=\"page-link\" href=\"{newerUrl}\" rel=\"prev\"><i class=\"fa-solid fa-chevron-left me-1\"></i>{NewerPostsText}</a></li>"
				: $"<li class=\"page-item disabled\" aria-disabled=\"true\"><span class=\"page-link\"><i class=\"fa-solid fa-chevron-left me-1\"></i>{NewerPostsText}</span></li>");

			// Current page indicator
			sb.Append($"<li class=\"page-item active\" aria-current=\"page\"><span class=\"page-link\">Page {PageNumber + 1}</span></li>");

			// Older (Next)
			sb.Append(hasOlder
				? $"<li class=\"page-item\"><a class=\"page-link\" href=\"{olderUrl}\" rel=\"next\">{OlderPostsText}<i class=\"fa-solid fa-chevron-right ms-1\"></i></a></li>"
				: $"<li class=\"page-item disabled\" aria-disabled=\"true\"><span class=\"page-link\">{OlderPostsText}<i class=\"fa-solid fa-chevron-right ms-1\"></i></span></li>");

			sb.Append("</ul>");

			output.TagName = "nav";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("aria-label", "Comment pagination");
			output.Attributes.SetAttribute("class", "dbc-comment-page-control");
			output.Content.SetHtmlContent(sb.ToString());

			return Task.CompletedTask;
		}
	}
}
