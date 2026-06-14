using DasBlog.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("theme-stylesheets", TagStructure = TagStructure.WithoutEndTag)]
	public class ThemeStylesheetsTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IUrlHelperFactory urlHelperFactory;

		public ThemeStylesheetsTagHelper(IDasBlogSettings dasBlogSettings, IUrlHelperFactory urlHelperFactory)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.urlHelperFactory = urlHelperFactory;
		}

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
			var siteCssHref = urlHelper.Content("~/css/site.css");

			var siteCss = $"<link rel=\"stylesheet\" href=\"{siteCssHref}\" />";
			var themeCss = $"<link rel=\"stylesheet\" href=\"{dasBlogSettings.ThemeCssUrl}\" />";

			output.Content.SetHtmlContent(siteCss + System.Environment.NewLine + themeCss);
		}
	}
}
