using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class TextAreaBuilder : IRichEditBuilder
	{
		public void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "textarea";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("id", tagHelper.Id);
			output.Attributes.SetAttribute("name", tagHelper.Name);
			output.Attributes.SetAttribute("style", "width: 100%; height: 100%;");
		}

		public void ProcessScripts(RichEditScriptsTagHelper tagHeelper, TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
		}
	}
}
