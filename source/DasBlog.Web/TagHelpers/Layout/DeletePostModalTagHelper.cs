using System.Net;
using System.Text;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace DasBlog.Web.TagHelpers.Layout
{
	[HtmlTargetElement("delete-post-modal", TagStructure = TagStructure.WithoutEndTag)]
	public class DeletePostModalTagHelper : TagHelper
	{
		private readonly IAntiforgery antiforgery;
		private readonly LinkGenerator linkGenerator;

		public DeletePostModalTagHelper(IAntiforgery antiforgery, LinkGenerator linkGenerator)
		{
			this.antiforgery = antiforgery;
			this.linkGenerator = linkGenerator;
		}

		public PostViewModel Post { get; set; }

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			if (Post == null || string.IsNullOrEmpty(Post.EntryId))
			{
				output.SuppressOutput();
				return;
			}

			var entryIdClean = Post.EntryId.Replace("-", string.Empty);
			var modalId = $"deleteModal_{entryIdClean}";
			var formId = $"deleteForm_{entryIdClean}";

			var tokens = antiforgery.GetAndStoreTokens(ViewContext.HttpContext);
			var action = linkGenerator.GetPathByAction(
				ViewContext.HttpContext,
				action: "DeletePost",
				controller: "BlogPost",
				values: new { postid = Post.EntryId }) ?? string.Empty;

			var sb = new StringBuilder();
			sb.Append("<div class=\"modal fade\" id=\"").Append(modalId).Append("\" tabindex=\"-1\" aria-labelledby=\"").Append(modalId).Append("Label\">\n");
			sb.Append("    <div class=\"modal-dialog\">\n");
			sb.Append("        <div class=\"modal-content\">\n");
			sb.Append("            <div class=\"modal-header\">\n");
			sb.Append("                <h5 class=\"modal-title\" id=\"").Append(modalId).Append("Label\">Confirm Delete</h5>\n");
			sb.Append("                <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button>\n");
			sb.Append("            </div>\n");
			sb.Append("            <div class=\"modal-body\">\n");
			sb.Append("                <p>Are you sure you want to delete this post?</p>\n");
			sb.Append("                <p><strong>").Append(WebUtility.HtmlEncode(Post.Title ?? string.Empty)).Append("</strong></p>\n");
			sb.Append("                <p class=\"text-danger\"><small>This action cannot be undone.</small></p>\n");
			sb.Append("            </div>\n");
			sb.Append("            <div class=\"modal-footer\">\n");
			sb.Append("                <button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">Cancel</button>\n");
			sb.Append("                <form id=\"").Append(formId).Append("\" action=\"").Append(WebUtility.HtmlEncode(action)).Append("\" method=\"post\" class=\"dbc-delete-post-form\">\n");
			sb.Append("                    <input name=\"").Append(tokens.FormFieldName).Append("\" type=\"hidden\" value=\"").Append(tokens.RequestToken).Append("\" />\n");
			sb.Append("                    <button type=\"submit\" class=\"btn btn-danger\">Delete</button>\n");
			sb.Append("                </form>\n");
			sb.Append("            </div>\n");
			sb.Append("        </div>\n");
			sb.Append("    </div>\n");
			sb.Append("</div>");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
