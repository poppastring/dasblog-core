using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentContentTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public string Css { get; set; } = "dbc-comment-content";

		private readonly IDasBlogSettings dasBlogSettings;

		public CommentContentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;

			output.Attributes.SetAttribute("class", Css);
			Comment ??= new CommentViewModel();
			Comment.Text = dasBlogSettings.FilterHtml(Comment.Text ?? string.Empty);
			Comment.Text = Regex.Replace(Comment.Text, "\n", "<br />");
			output.Content.SetHtmlContent(HttpUtility.HtmlDecode(Comment.Text));
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
