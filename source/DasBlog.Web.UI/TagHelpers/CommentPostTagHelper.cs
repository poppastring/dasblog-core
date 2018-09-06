using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class CommentPostTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public CommentPostTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot(Post.PermaLink + "/comment"));
			output.Content.SetHtmlContent("Comment on this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
