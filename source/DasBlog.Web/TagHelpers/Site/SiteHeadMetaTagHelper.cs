using System.Net;
using System.Text;
using DasBlog.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("site-head-meta", TagStructure = TagStructure.WithoutEndTag)]
	public class SiteHeadMetaTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteHeadMetaTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var data = ViewContext.ViewData;
			var pageTitle = data["PageTitle"]?.ToString() ?? string.Empty;
			var description = data["Description"]?.ToString() ?? string.Empty;
			var keywords = data["Keywords"]?.ToString() ?? string.Empty;
			var author = data["Author"]?.ToString() ?? string.Empty;
			var canonical = data["Canonical"]?.ToString() ?? string.Empty;
			var copyright = dasBlogSettings.SiteConfiguration.Copyright ?? string.Empty;

			var sb = new StringBuilder();
			sb.Append("<title>").Append(WebUtility.HtmlEncode(pageTitle)).Append("</title>\n");
			sb.Append("<meta name=\"description\" content=\"").Append(WebUtility.HtmlEncode(description)).Append("\" />\n");
			sb.Append("<meta name=\"keywords\" content=\"").Append(WebUtility.HtmlEncode(keywords)).Append("\" />\n");
			sb.Append("<meta name=\"author\" content=\"").Append(WebUtility.HtmlEncode(author)).Append("\" />\n");
			sb.Append("<meta name=\"copyright\" content=\"").Append(WebUtility.HtmlEncode(copyright)).Append("\" />\n");
			sb.Append("<link rel=\"shortcut icon\" href=\"").Append(dasBlogSettings.ShortCutIconUrl).Append("\" />\n");
			sb.Append("<base href=\"").Append(dasBlogSettings.SiteConfiguration.Root).Append("\">\n");
			sb.Append("<link href=\"").Append(WebUtility.HtmlEncode(canonical)).Append("\" rel=\"canonical\">\n");
			sb.Append("<link rel=\"microsummary\" type=\"application/x.microsummary+xml\" href=\"").Append(dasBlogSettings.MicroSummaryUrl).Append("\" />");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
