using System.Net;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Layout
{
	[HtmlTargetElement("confirmation-modal", TagStructure = TagStructure.WithoutEndTag)]
	public class ConfirmationModalTagHelper : TagHelper
	{
		public string ModalId { get; set; } = "confirmModal";
		public string Title { get; set; } = "Confirm Action";
		public string Message { get; set; } = "Are you sure you want to proceed?";
		public string ConfirmText { get; set; } = "Confirm";
		public string ConfirmClass { get; set; } = "btn-primary";
		public string HiddenButtonId { get; set; } = "hiddenSubmitButton";

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var modalId = string.IsNullOrEmpty(ModalId) ? "confirmModal" : ModalId;
			var title = string.IsNullOrEmpty(Title) ? "Confirm Action" : Title;
			var message = string.IsNullOrEmpty(Message) ? "Are you sure you want to proceed?" : Message;
			var confirmText = string.IsNullOrEmpty(ConfirmText) ? "Confirm" : ConfirmText;
			var confirmClass = string.IsNullOrEmpty(ConfirmClass) ? "btn-primary" : ConfirmClass;
			var hiddenButtonId = string.IsNullOrEmpty(HiddenButtonId) ? "hiddenSubmitButton" : HiddenButtonId;

			var sb = new StringBuilder();
			sb.Append("<div class=\"modal fade\" id=\"").Append(WebUtility.HtmlEncode(modalId)).Append("\" tabindex=\"-1\" aria-labelledby=\"").Append(WebUtility.HtmlEncode(modalId)).Append("Label\" aria-hidden=\"true\">\n");
			sb.Append("    <div class=\"modal-dialog\">\n");
			sb.Append("        <div class=\"modal-content\">\n");
			sb.Append("            <div class=\"modal-header\">\n");
			sb.Append("                <h5 class=\"modal-title\" id=\"").Append(WebUtility.HtmlEncode(modalId)).Append("Label\">").Append(WebUtility.HtmlEncode(title)).Append("</h5>\n");
			sb.Append("                <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"modal\" aria-label=\"Close\"></button>\n");
			sb.Append("            </div>\n");
			sb.Append("            <div class=\"modal-body\">\n");
			sb.Append("                ").Append(WebUtility.HtmlEncode(message)).Append("\n");
			sb.Append("            </div>\n");
			sb.Append("            <div class=\"modal-footer\">\n");
			sb.Append("                <button type=\"button\" class=\"btn btn-secondary\" data-bs-dismiss=\"modal\">Cancel</button>\n");
			sb.Append("                <button type=\"button\" class=\"btn ").Append(WebUtility.HtmlEncode(confirmClass)).Append("\" id=\"").Append(WebUtility.HtmlEncode(modalId)).Append("Button\">").Append(WebUtility.HtmlEncode(confirmText)).Append("</button>\n");
			sb.Append("            </div>\n");
			sb.Append("        </div>\n");
			sb.Append("    </div>\n");
			sb.Append("</div>\n");
			sb.Append("<script>\n");
			sb.Append("    (function() {\n");
			sb.Append("        const confirmButton = document.getElementById('").Append(modalId).Append("Button');\n");
			sb.Append("        const hiddenButton = document.getElementById('").Append(hiddenButtonId).Append("');\n");
			sb.Append("        const modal = document.getElementById('").Append(modalId).Append("');\n");
			sb.Append("        if (confirmButton && hiddenButton && modal) {\n");
			sb.Append("            confirmButton.addEventListener('click', function() {\n");
			sb.Append("                const modalInstance = bootstrap.Modal.getInstance(modal);\n");
			sb.Append("                if (modalInstance) { modalInstance.hide(); }\n");
			sb.Append("                setTimeout(function() { hiddenButton.click(); }, 300);\n");
			sb.Append("            });\n");
			sb.Append("        }\n");
			sb.Append("    })();\n");
			sb.Append("</script>");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
