using System.Net;
using System.Text;
using DasBlog.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("twitter-card", TagStructure = TagStructure.WithoutEndTag)]
	public class TwitterCardTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public TwitterCardTagHelper(IDasBlogSettings dasBlogSettings)
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
			var title = data["PageTitle"]?.ToString() ?? string.Empty;
			var description = data["Description"]?.ToString() ?? string.Empty;
			var image = data["PageImageUrl"]?.ToString() ?? string.Empty;

			var tags = dasBlogSettings.MetaTags;
			var sb = new StringBuilder();
			sb.Append("<meta name=\"twitter:card\" content=\"").Append(WebUtility.HtmlEncode(tags?.TwitterCard ?? string.Empty)).Append("\" />\n");
			sb.Append("<meta name=\"twitter:site\" content=\"").Append(WebUtility.HtmlEncode(tags?.TwitterSite ?? string.Empty)).Append("\" />\n");
			sb.Append("<meta name=\"twitter:creator\" content=\"").Append(WebUtility.HtmlEncode(tags?.TwitterCreator ?? string.Empty)).Append("\" />\n");
			sb.Append("<meta name=\"twitter:title\" content=\"").Append(WebUtility.HtmlEncode(title)).Append("\" />\n");
			sb.Append("<meta name=\"twitter:description\" content=\"").Append(WebUtility.HtmlEncode(description)).Append("\" />\n");
			sb.Append("<meta name=\"twitter:image\" content=\"").Append(WebUtility.HtmlEncode(image)).Append("\" />");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
