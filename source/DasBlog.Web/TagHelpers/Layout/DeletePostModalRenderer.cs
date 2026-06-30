﻿using System.Net;
using System.Text;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DasBlog.Web.TagHelpers.Layout
{
	internal static class DeletePostModalRenderer
	{
		private const string RenderedItemKeyPrefix = "DasBlog.DeleteModalRendered.";

		public static bool HasBeenRendered(HttpContext httpContext, string entryId)
		{
			if (httpContext == null || string.IsNullOrEmpty(entryId))
			{
				return false;
			}

			return httpContext.Items.ContainsKey(RenderedItemKeyPrefix + entryId);
		}

		public static void MarkRendered(HttpContext httpContext, string entryId)
		{
			if (httpContext == null || string.IsNullOrEmpty(entryId))
			{
				return;
			}

			httpContext.Items[RenderedItemKeyPrefix + entryId] = true;
		}

		public static string BuildModalHtml(
			PostViewModel post,
			HttpContext httpContext,
			IAntiforgery antiforgery,
			LinkGenerator linkGenerator)
		{
			if (post == null || string.IsNullOrEmpty(post.EntryId))
			{
				return string.Empty;
			}

			var entryIdClean = post.EntryId.Replace("-", string.Empty);
			var modalId = $"deleteModal_{entryIdClean}";
			var formId = $"deleteForm_{entryIdClean}";

			var tokens = antiforgery.GetAndStoreTokens(httpContext);
			var action = linkGenerator.GetPathByAction(
				httpContext,
				action: "DeletePost",
				controller: "BlogPost",
				values: new { postid = post.EntryId }) ?? string.Empty;

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
			sb.Append("                <p><strong>").Append(WebUtility.HtmlEncode(post.Title ?? string.Empty)).Append("</strong></p>\n");
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

			return sb.ToString();
		}
	}
}
