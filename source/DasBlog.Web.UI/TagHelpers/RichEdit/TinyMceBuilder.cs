using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class TinyMceBuilder : IRichEditBuilder
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private const string TINY_MCE_SERVICE_URL = "https://cdn.tiny.cloud/1/{0}/tinymce/5/tinymce.min.js";
		private const string INIT_SCRIPT_TEMPLATE = @"
		<script language=""javascript"" type=""text/javascript"" src=""{0}/js/tinymce/plugins/code/plugin.min.js""></script>
		<script>
		tinymce.init({{
			selector: '#{1}'
			,plugins: 'code'
		}});
		</script>
		";

		public TinyMceBuilder(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}
		
		public void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "textarea";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("comment", "a rich-edit-scripts element should be included on the page");
			output.Attributes.SetAttribute("id", tagHelper.Id);
			output.Attributes.SetAttribute("name", tagHelper.Name);
			output.Attributes.SetAttribute("style", "height: 100%; width: 99%; min-height: 360px");

		}

		public void ProcessScripts(RichEditScriptsTagHelper tagHeelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("src", string.Format(TINY_MCE_SERVICE_URL, dasBlogSettings.SiteConfiguration.TinyMCEApiKey));
			output.Attributes.SetAttribute("type", "text/javascript");
			output.Attributes.SetAttribute("language", "javascript");
			string htmlContent = string.Format(INIT_SCRIPT_TEMPLATE, dasBlogSettings.SiteConfiguration.Root, tagHeelper.ControlId);
			output.PostElement.SetHtmlContent(htmlContent);
		}
	}
}
