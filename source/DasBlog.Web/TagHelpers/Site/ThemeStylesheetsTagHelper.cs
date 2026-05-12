using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("theme-stylesheets", TagStructure = TagStructure.WithoutEndTag)]
	public class ThemeStylesheetsTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public ThemeStylesheetsTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var siteCss = "<link rel=\"stylesheet\" href=\"/css/site.css\" />";
			var themeCss = $"<link rel=\"stylesheet\" href=\"{dasBlogSettings.ThemeCssUrl}\" />";

			output.Content.SetHtmlContent(siteCss + System.Environment.NewLine + themeCss);
		}
	}
}
