using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.UI.Helpers
{
	public class CommentPostTagHelper : TagHelper
	{
		public string Blog { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"Blogger/{Blog}/Comment"); //TODO: rest API subject to change
			output.Content.SetHtmlContent("Comment on this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
