using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Text.RegularExpressions;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostReadTimeTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private readonly IContentProcessor contentProcessor;
		private const string READTIMEMINUTES = "{0} min read";

		public PostReadTimeTagHelper(IContentProcessor contentProcessor)
		{
			this.contentProcessor = contentProcessor;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var rx = new Regex(@"<img.*?src="".*?"".*?>");
			var numberImages = rx.Matches(Post.Content).Count;

			var imgMinutes = (double)(numberImages*30)/60;
			var delimiters = new char[] { ' ', '\r', '\n' };
			var minute = Math.Round((double)contentProcessor.FilterHtml(Post.Content).Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length / 200) + Math.Round(imgMinutes, MidpointRounding.AwayFromZero);
			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-post-readtime");
			output.Content.SetHtmlContent(string.Format(READTIMEMINUTES, minute));
		}
	}
}
