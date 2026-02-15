using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
	public class PostToBlueSkyTagHelper : TagHelper
	{
		private const string BLUESKY_SHARE_URL = "https://bsky.app/intent/compose?text={0}";
		private readonly IDasBlogSettings dasBlogSettings;
		public PostViewModel Post { get; set; }

		public PostToBlueSkyTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-a-share-bluesky");
			string text = $"{Post.Title} {dasBlogSettings.RelativeToRoot(Post.PermaLink)}";
			output.Attributes.SetAttribute("href", string.Format(BLUESKY_SHARE_URL, UrlEncoder.Default.Encode(text)));

			var content = await output.GetChildContentAsync();
			output.Content.SetHtmlContent(content.GetContent());
		}
	}
}
