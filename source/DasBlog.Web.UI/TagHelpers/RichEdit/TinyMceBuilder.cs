using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class TinyMceBuilder : IRichEditBuilder
	{
		private const string TINY_MCE_SERVICE_URL = "https://cloud.tinymce.com/stable/tinymce.min.js";
		private const string INIT_SCRIPT_TEMPLATE = @"
		<script>
		tinymce.init({{
			selector: '#{0}'
		}});
		</script>
		";
		
		public void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "textarea";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("comment", "a rich-edit-scripts element should be included on the page");
			output.Attributes.SetAttribute("id", tagHelper.Id);
			output.Attributes.SetAttribute("name", tagHelper.Name);
			output.Attributes.SetAttribute("style", "width: 100%; height: 90%;");
		}

		public void ProcessScripts(RichEditScriptsTagHelper tagHeelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("src", TINY_MCE_SERVICE_URL);
			string htmlContent = string.Format(INIT_SCRIPT_TEMPLATE, tagHeelper.ControlId);
			output.PostElement.SetHtmlContent(htmlContent);
		}
	}
}
