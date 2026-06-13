using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("blog-posting-schema", TagStructure = TagStructure.WithoutEndTag)]
	public class BlogPostingSchemaTagHelper : TagHelper
	{
		private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
		{
			Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			WriteIndented = false
		};

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("type", "application/ld+json");

			var data = ViewContext.ViewData;
			var schemaContext = "http://schema.org";
			var schemaType = "BlogPosting";
			var headline = data["PageTitle"]?.ToString();
			var description = data["Description"]?.ToString();
			var url = data["Canonical"]?.ToString();
			var image = data["PageImageUrl"]?.ToString();
			var datePublished = data["DatePublished"]?.ToString();
			var dateModified = data["DateModified"]?.ToString();
			if (string.IsNullOrEmpty(dateModified))
			{
				dateModified = datePublished;
			}
			var authorName = data["Author"]?.ToString();
			var authorUrl = data["AuthorUrl"]?.ToString();
			var canonical = data["Canonical"]?.ToString();

			// Build the JSON manually to preserve the @context / @type keys.
			var sb = new StringBuilder();
			sb.Append('{');
			sb.Append("\"@context\":").Append(JsonSerializer.Serialize(schemaContext, JsonOptions));
			sb.Append(",\"@type\":").Append(JsonSerializer.Serialize(schemaType, JsonOptions));
			AppendStringIfPresent(sb, "headline", headline);
			AppendStringIfPresent(sb, "description", description);
			AppendStringIfPresent(sb, "url", url);
			AppendStringIfPresent(sb, "image", image);
			AppendStringIfPresent(sb, "datePublished", datePublished);
			AppendStringIfPresent(sb, "dateModified", dateModified);

			if (!string.IsNullOrEmpty(authorName) || !string.IsNullOrEmpty(authorUrl))
			{
				sb.Append(",\"author\":{");
				sb.Append("\"@type\":\"Person\"");
				if (!string.IsNullOrEmpty(authorName))
				{
					sb.Append(",\"name\":").Append(JsonSerializer.Serialize(authorName, JsonOptions));
				}
				if (!string.IsNullOrEmpty(authorUrl))
				{
					sb.Append(",\"url\":").Append(JsonSerializer.Serialize(authorUrl, JsonOptions));
				}
				sb.Append('}');
			}

			if (!string.IsNullOrEmpty(canonical))
			{
				sb.Append(",\"mainEntityOfPage\":{");
				sb.Append("\"@type\":\"WebPage\"");
				sb.Append(",\"@id\":").Append(JsonSerializer.Serialize(canonical, JsonOptions));
				sb.Append('}');
			}

			sb.Append('}');

			output.Content.SetHtmlContent(sb.ToString());
		}

		private static void AppendStringIfPresent(StringBuilder sb, string name, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			sb.Append(',').Append('"').Append(name).Append("\":").Append(JsonSerializer.Serialize(value, JsonOptions));
		}
	}
}
