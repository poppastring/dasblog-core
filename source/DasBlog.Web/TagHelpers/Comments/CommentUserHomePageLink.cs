using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentUserHomePageLink : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;

			if (Comment.HomePageUrl?.Length > 0)
			{
				output.TagName = "a";
				output.Attributes.SetAttribute("href", Comment.HomePageUrl);
				output.Attributes.SetAttribute("rel", "nofollow");
			}
			else 
			{
				output.TagName = "span";
			}
			
			output.Attributes.SetAttribute("class", "dbc-comment-user-homepage-name");
			output.Content.SetHtmlContent(Comment.Name);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
