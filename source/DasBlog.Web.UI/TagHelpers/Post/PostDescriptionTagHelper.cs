using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System.Web;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostDescriptionTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-post-description");
			output.Content.SetHtmlContent(HttpUtility.HtmlDecode(Post.Description));
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
