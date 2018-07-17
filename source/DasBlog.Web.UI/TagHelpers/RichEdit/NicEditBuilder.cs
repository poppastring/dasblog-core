using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class NicEditBuilder : IRichEditBuilder
	{
		private const string NIC_EDIT_SERVICE_URL = "http://js.nicedit.com/nicEdit-latest.js";

		private const string INIT_SCRIPT_TEMPLATE = @"
		<script type=""text/javascript"">area1 = new nicEditor({{fullPanel : true}}).panelInstance('{0}',{{hasPanel : true}});</script>
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
			output.Attributes.SetAttribute("src", NIC_EDIT_SERVICE_URL);
			string htmlContent = string.Format(INIT_SCRIPT_TEMPLATE, tagHeelper.ControlId);
			output.PostElement.SetHtmlContent(htmlContent);
		}
	}
}
