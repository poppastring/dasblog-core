using System.Threading.Tasks;
using DasBlog.Web.UI.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.UI.Helpers
{
	public class EditPostTagHelper : TagHelper
	{
		public string BlogPostId { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"post/edit/{BlogPostId}");
			output.Content.SetHtmlContent("Edit this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
