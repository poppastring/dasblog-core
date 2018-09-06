using DasBlog.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class EditPostTagHelper : TagHelper
	{
		public string BlogPostId { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public EditPostTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", dasBlogSettings.GetPermaLinkUrl(BlogPostId + "/edit"));
			output.Content.SetHtmlContent("Edit this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
