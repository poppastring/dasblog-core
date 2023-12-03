using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
	public class PostToFacebookTagHelper : TagHelper
	{
		private const string FACEBOOK_SHARE_URL = "https://facebook.com/sharer.php?u={0}";
		private readonly IDasBlogSettings dasBlogSettings;
		public PostViewModel Post { get; set; }

		public PostToFacebookTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-a-share-facebook");
			output.Attributes.SetAttribute("href", string.Format(FACEBOOK_SHARE_URL, 
				UrlEncoder.Default.Encode(dasBlogSettings.RelativeToRoot(Post.PermaLink))));

			var content = await output.GetChildContentAsync();

			output.Content.SetHtmlContent(content.GetContent());
		}
	}
}
