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
			var data = ViewContext.ViewData;
			var schemaType = string.Equals(data["SchemaType"]?.ToString(), "WebPage", System.StringComparison.Ordinal)
				? "WebPage"
				: "BlogPosting";
			var datePublished = data["DatePublished"]?.ToString();
			var canonical = data["Canonical"]?.ToString();
			if (string.IsNullOrEmpty(canonical) || (schemaType == "BlogPosting" && string.IsNullOrEmpty(datePublished)))
			{
				output.SuppressOutput();
				return;
			}

			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("type", "application/ld+json");

			var schemaContext = "https://schema.org";
			var title = data["PageTitle"]?.ToString();
			var description = data["Description"]?.ToString();
			var url = data["Canonical"]?.ToString();
			var image = data["PageImageUrl"]?.ToString();
			var dateModified = data["DateModified"]?.ToString();
			if (string.IsNullOrEmpty(dateModified))
			{
				dateModified = datePublished;
			}
			var authorName = data["Author"]?.ToString();
			var authorUrl = data["AuthorUrl"]?.ToString();
			var publisherName = data["PublisherName"]?.ToString();
			var publisherUrl = data["PublisherUrl"]?.ToString();

			// Build the JSON manually to preserve the @context / @type keys.
			var sb = new StringBuilder();
			sb.Append('{');
			sb.Append("\"@context\":").Append(JsonSerializer.Serialize(schemaContext, JsonOptions));
			sb.Append(",\"@type\":").Append(JsonSerializer.Serialize(schemaType, JsonOptions));
			AppendStringIfPresent(sb, schemaType == "WebPage" ? "name" : "headline", title);
			AppendStringIfPresent(sb, "description", description);
			AppendStringIfPresent(sb, "url", url);
			AppendStringIfPresent(sb, "image", image);

			if (schemaType == "BlogPosting")
			{
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
			}

			if (!string.IsNullOrEmpty(publisherName) || !string.IsNullOrEmpty(publisherUrl))
			{
				sb.Append(",\"publisher\":{");
				sb.Append("\"@type\":\"Organization\"");
				if (!string.IsNullOrEmpty(publisherName))
				{
					sb.Append(",\"name\":").Append(JsonSerializer.Serialize(publisherName, JsonOptions));
				}
				if (!string.IsNullOrEmpty(publisherUrl))
				{
					sb.Append(",\"url\":").Append(JsonSerializer.Serialize(publisherUrl, JsonOptions));
				}
				sb.Append('}');
			}

			if (schemaType == "BlogPosting")
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
