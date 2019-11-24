using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class FroalaBuilder : IRichEditBuilder
	{
		private const string FROALA_JS_CDN = @"https://cdn.jsdelivr.net/npm/froala-editor@3.0.6/js/froala_editor.pkgd.min.js";
		private const string FROALA_CSS_CDN = @"<link href=""https://cdn.jsdelivr.net/npm/froala-editor@3.0.6/css/froala_editor.pkgd.min.css"" rel=""stylesheet"" type=""text/css"" />";
		private const string FROALA_SCRIPT_TEMPLATE =
				@"<script>var editor = new FroalaEditor('textarea#{0}'  )</script>";

		public void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "textarea";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("id", tagHelper.Id);
			output.Attributes.SetAttribute("name", tagHelper.Name);
			output.Attributes.SetAttribute("style", "width: 100%; height: 90%;");
		}

		public void ProcessScripts(RichEditScriptsTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("src", FROALA_JS_CDN);
			output.Attributes.SetAttribute("type", "text/javascript");
			output.Attributes.SetAttribute("language", "javascript");
			var htmlContent = string.Format(FROALA_SCRIPT_TEMPLATE, tagHelper.ControlId);
			output.PostElement.SetHtmlContent(htmlContent + FROALA_CSS_CDN);

		}
	}
}
