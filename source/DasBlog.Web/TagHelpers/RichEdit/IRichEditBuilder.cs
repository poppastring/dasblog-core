using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public interface IRichEditBuilder
	{
		void ProcessControl(RichEditTagHelper tagHelper, TagHelperContext context, TagHelperOutput output);
		void ProcessScripts(RichEditScriptsTagHelper tagHelper, TagHelperContext context, TagHelperOutput output);
	}
}
