using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Services;
using System.Text.Encodings.Web;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostToRedditTagHelper : TagHelper
	{
		private const string REDDIT_SHARE_URL = "https://www.reddit.com/submit?url={0}&title={1}";
		private IDasBlogSettings dasBlogSettings;
		public PostViewModel Post { get; set; }

		public PostToRedditTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-a-share-reddit");
			output.Attributes.SetAttribute("href", string.Format(REDDIT_SHARE_URL,
								UrlEncoder.Default.Encode(new Uri(new Uri(dasBlogSettings.GetBaseUrl()), Post.PermaLink).AbsoluteUri),
								UrlEncoder.Default.Encode(Post.Title)
								));

			var content = await output.GetChildContentAsync();

			output.Content.SetHtmlContent(content.GetContent());
		}
	}
}
