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
			var payload = new
			{
				context = "http://schema.org",
				type = "BlogPosting",
				headline = data["PageTitle"]?.ToString() ?? string.Empty,
				description = data["Description"]?.ToString() ?? string.Empty,
				url = data["Canonical"]?.ToString() ?? string.Empty,
				image = data["PageImageUrl"]?.ToString() ?? string.Empty
			};

			// Build the JSON manually to preserve the @context / @type keys.
			var sb = new StringBuilder();
			sb.Append('{');
			sb.Append("\"@context\":").Append(JsonSerializer.Serialize(payload.context, JsonOptions)).Append(',');
			sb.Append("\"@type\":").Append(JsonSerializer.Serialize(payload.type, JsonOptions)).Append(',');
			sb.Append("\"headline\":").Append(JsonSerializer.Serialize(payload.headline, JsonOptions)).Append(',');
			sb.Append("\"description\":").Append(JsonSerializer.Serialize(payload.description, JsonOptions)).Append(',');
			sb.Append("\"url\":").Append(JsonSerializer.Serialize(payload.url, JsonOptions)).Append(',');
			sb.Append("\"image\":").Append(JsonSerializer.Serialize(payload.image, JsonOptions));
			sb.Append('}');

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
