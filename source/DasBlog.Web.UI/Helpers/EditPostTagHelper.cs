using System.Threading.Tasks;
using DasBlog.Web.UI.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.UI.Helpers
{
	public class EditPostTagHelper : TagHelper
	{
		public string Blog { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"Blogger/{Blog}/Edit"); //TODO: rest API subject to change
			output.Content.SetHtmlContent("Edit this post");
		}
	}
}
