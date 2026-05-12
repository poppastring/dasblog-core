using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostToMastodonTagHelper : TagHelper
	{
		private const string MASTODON_SHARE_URL = "https://mastodonshare.com/?text={0}&url={1}";
		private readonly IUrlResolver urlResolver;
		public PostViewModel Post { get; set; }

		public PostToMastodonTagHelper(IUrlResolver urlResolver)
		{
			this.urlResolver = urlResolver;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-a-share-mastodon");
			output.Attributes.SetAttribute("href", string.Format(MASTODON_SHARE_URL,
								UrlEncoder.Default.Encode(Post.Title),
								UrlEncoder.Default.Encode(urlResolver.RelativeToRoot(Post.PermaLink))
								));

			var content = await output.GetChildContentAsync();
			output.Content.SetHtmlContent(content.GetContent());
		}
	}
}
