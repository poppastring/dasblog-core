using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostReadTimeTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private IDasBlogSettings dasBlogSettings;
		private const string READTIMEMINUTES = "{0} min read";

		public PostReadTimeTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{

			string pattern = @"<img.*?src="".*?"".*?>";
			Regex rx = new Regex(pattern);
			var numberImages = string.Empty;
			foreach(Match m in rx.Matches(Post.Content))
			{
				var numberImages = m.Count;
			}

			var imgMinutes = (numberImages*30)/60; 
			var delimiters = new char[] { ' ', '\r', '\n' };
			var minute = Math.Round((double)dasBlogSettings.FilterHtml(Post.Content).Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length / 200) + imgMinutes;

			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-post-readtime");
			output.Content.SetHtmlContent(string.Format(READTIMEMINUTES, minute));
		}
	}
}
