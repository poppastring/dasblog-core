using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Layout
{
	[HtmlTargetElement("success-alert", TagStructure = TagStructure.WithoutEndTag)]
	public class SuccessAlertTagHelper : TagHelper
	{
		public string AlertId { get; set; } = "successAlert";
		public string MessageKey { get; set; } = "SuccessMessage";

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var alertId = string.IsNullOrEmpty(AlertId) ? "successAlert" : AlertId;
			var messageKey = string.IsNullOrEmpty(MessageKey) ? "SuccessMessage" : MessageKey;
			var message = ViewContext.TempData[messageKey]?.ToString();

			if (string.IsNullOrEmpty(message))
			{
				output.SuppressOutput();
				return;
			}

			var sb = new StringBuilder();
			sb.Append("<div class=\"alert alert-success alert-dismissible fade show\" role=\"alert\" id=\"").Append(WebUtility.HtmlEncode(alertId)).Append("\">\n");
			sb.Append("    <i class=\"fa-solid fa-circle-check me-2\"></i>\n");
			sb.Append("    <strong>Success!</strong> ").Append(WebUtility.HtmlEncode(message)).Append("\n");
			sb.Append("    <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\" aria-label=\"Close\"></button>\n");
			sb.Append("</div>\n");
			sb.Append("<script>\n");
			sb.Append("    (function() {\n");
			sb.Append("        const alert = document.getElementById('").Append(alertId).Append("');\n");
			sb.Append("        if (alert) {\n");
			sb.Append("            window.scrollTo({ top: 0, behavior: 'smooth' });\n");
			sb.Append("            setTimeout(function() {\n");
			sb.Append("                if (typeof bootstrap !== 'undefined') {\n");
			sb.Append("                    const bsAlert = bootstrap.Alert.getOrCreateInstance(alert);\n");
			sb.Append("                    bsAlert.close();\n");
			sb.Append("                }\n");
			sb.Append("            }, 5000);\n");
			sb.Append("        }\n");
			sb.Append("    })();\n");
			sb.Append("</script>");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
