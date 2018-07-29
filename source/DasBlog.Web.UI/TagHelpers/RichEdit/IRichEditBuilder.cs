using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public interface IRichEditBuilder
	{
		void ProcessControl(RichEditTagHelper tagHeelper, TagHelperContext context, TagHelperOutput output);
		void ProcessScripts(RichEditScriptsTagHelper tagHeelper, TagHelperContext context, TagHelperOutput output);
	}
}
