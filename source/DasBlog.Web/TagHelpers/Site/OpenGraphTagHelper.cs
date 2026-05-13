using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("open-graph", TagStructure = TagStructure.WithoutEndTag)]
	public class OpenGraphTagHelper : TagHelper
	{
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var data = ViewContext.ViewData;
			var canonical = data["Canonical"]?.ToString() ?? string.Empty;
			var title = data["PageTitle"]?.ToString() ?? string.Empty;
			var image = data["PageImageUrl"]?.ToString() ?? string.Empty;
			var description = data["Description"]?.ToString() ?? string.Empty;
			var video = data["PageVideoUrl"]?.ToString() ?? string.Empty;

			var sb = new StringBuilder();
			sb.Append("<meta property=\"og:url\" content=\"").Append(WebUtility.HtmlEncode(canonical)).Append("\" />\n");
			sb.Append("<meta property=\"og:title\" content=\"").Append(WebUtility.HtmlEncode(title)).Append("\" />\n");
			sb.Append("<meta property=\"og:image\" content=\"").Append(WebUtility.HtmlEncode(image)).Append("\" />\n");
			sb.Append("<meta property=\"og:description\" content=\"").Append(WebUtility.HtmlEncode(description)).Append("\" />\n");
			sb.Append("<meta property=\"og:video\" content=\"").Append(WebUtility.HtmlEncode(video)).Append("\" />\n");
			sb.Append("<meta property=\"og:type\" content=\"article\" />");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
