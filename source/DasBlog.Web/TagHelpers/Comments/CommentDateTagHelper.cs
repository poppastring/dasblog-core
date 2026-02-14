using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentDateTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public string DateTimeFormat { get; set; }

		public string Css { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if(string.IsNullOrWhiteSpace(DateTimeFormat))
			{
				DateTimeFormat = "MMMM dd, yyyy H:mm";
			}

			if (string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", "dbc-comment-date");
			}
			else
			{
				output.Attributes.SetAttribute("class", Css);
			}

			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Content.SetHtmlContent(Comment.Date.ToString(DateTimeFormat));
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
