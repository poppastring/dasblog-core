using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class DeletePostTagHelper : TagHelper
	{
		public string BlogPostId { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("id", "deletePost");
			output.Attributes.SetAttribute("href", $"post/{BlogPostId}/delete");
			output.Content.SetHtmlContent("Delete this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
