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
			var title = data["PageTitle"]?.ToString()?.Trim();
			var description = data["Description"]?.ToString()?.Trim();
			var pageImage = data["PageImageUrl"]?.ToString()?.Trim();

			var tags = dasBlogSettings.MetaTags;
			var twitterCard = NormalizeTwitterCard(tags?.TwitterCard);
			var twitterSite = NormalizeTwitterHandle(tags?.TwitterSite);
			var twitterCreator = NormalizeTwitterHandle(tags?.TwitterCreator);
			var twitterImage = string.IsNullOrWhiteSpace(pageImage) ? tags?.TwitterImage?.Trim() : pageImage;

			var sb = new StringBuilder();
			AppendMetaTag(sb, "twitter:card", twitterCard);
			AppendMetaTag(sb, "twitter:site", twitterSite);
			AppendMetaTag(sb, "twitter:creator", twitterCreator);
			AppendMetaTag(sb, "twitter:title", title);
			AppendMetaTag(sb, "twitter:description", description);
			AppendMetaTag(sb, "twitter:image", twitterImage);

			output.Content.SetHtmlContent(sb.ToString());
		}

		private static string NormalizeTwitterCard(string twitterCard)
		{
			if (string.Equals(twitterCard, "summary_large_image", System.StringComparison.OrdinalIgnoreCase))
			{
				return "summary_large_image";
			}

			return "summary";
		}

		private static string NormalizeTwitterHandle(string twitterHandle)
		{
			if (string.IsNullOrWhiteSpace(twitterHandle))
			{
				return null;
			}

			twitterHandle = twitterHandle.Trim();
			return twitterHandle.StartsWith("@", System.StringComparison.Ordinal) ? twitterHandle : $"@{twitterHandle}";
		}

		private static void AppendMetaTag(StringBuilder sb, string tagName, string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return;
			}

			if (sb.Length > 0)
			{
				sb.Append('\n');
			}

			sb.Append("<meta name=\"").Append(tagName).Append("\" content=\"").Append(WebUtility.HtmlEncode(value)).Append("\" />");
		}
	}
}
