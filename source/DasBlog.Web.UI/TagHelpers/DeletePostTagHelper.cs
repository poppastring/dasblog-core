using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class DeletePostTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public string BlogPostId { get; set; }

		public string BlogTitle { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public DeletePostTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (Post != null)
			{
				BlogPostId = Post.EntryId;
				BlogTitle = Post.Title;
			}

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:deleteEntry(\"{dasBlogSettings.GetPermaLinkUrl(BlogPostId + "/delete")}\",\"{BlogTitle}\")");
			output.Content.SetHtmlContent("Delete this post");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
