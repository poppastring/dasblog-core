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

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if(string.IsNullOrWhiteSpace(DateTimeFormat))
			{
				DateTimeFormat = "MMMM dd, yyyy H:mm";
			}

			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-comment-date");
			output.Content.SetHtmlContent(Comment.Date.ToString(DateTimeFormat));
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
