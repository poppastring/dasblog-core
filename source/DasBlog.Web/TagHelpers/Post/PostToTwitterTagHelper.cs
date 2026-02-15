using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class PostToTwitterTagHelper : TagHelper
	{
		private const string TWITTER_SHARE_URL = "https://twitter.com/intent/tweet?url={0}&amp;text={1}&amp;via={2}{3}";
		private IDasBlogSettings dasBlogSettings;
		public PostViewModel Post { get; set; }

		public  PostToTwitterTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			string author = dasBlogSettings.MetaTags.TwitterSite == string.Empty ? Post.Author : dasBlogSettings.MetaTags.TwitterSite;
			string categorylist = string.Empty;
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-a-share-twitter");

			output.Attributes.SetAttribute("href", string.Format(TWITTER_SHARE_URL, 
								UrlEncoder.Default.Encode(dasBlogSettings.RelativeToRoot(Post.PermaLink)),
								UrlEncoder.Default.Encode(Post.Title),
								UrlEncoder.Default.Encode(author.TrimStart('@')), 
								RetrieveFormattedCategories(Post.Categories)));

			var content = await output.GetChildContentAsync();

			output.Content.SetHtmlContent(content.GetContent());
		}

		private string RetrieveFormattedCategories(IList<CategoryViewModel> categories)
		{
			StringBuilder sb = new StringBuilder();

			if(categories != null && categories.Count > 0)
			{
				foreach (var category in categories)
				{
					sb.Append("+%23" + UrlEncoder.Default.Encode(category.Category));
				}
				return sb.ToString();
			}

			return string.Empty;
		}
	}
}
