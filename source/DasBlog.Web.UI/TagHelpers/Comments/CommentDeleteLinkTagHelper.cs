using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentDeleteLinkTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public bool Admin { get; set; } = false;

		private readonly IDasBlogSettings dasBlogSettings;
		private const string COMMENTTEXT_MSG = "Are you sure you want to delete the comment from '{0}'?";

		public CommentDeleteLinkTagHelper(IDasBlogSettings dasBlogSettings) 
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var commenttxt = string.Format(COMMENTTEXT_MSG, Comment.Name);
			var message = "Delete Comment";

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:commentManagement(\"{Comment.BlogPostId}\",\"{Comment.CommentId}\",\"{commenttxt}\",\"DELETE\")");
			output.Attributes.SetAttribute("class", "dbc-comment-delete-link");
			
			var content = await output.GetChildContentAsync();

			if (!string.IsNullOrWhiteSpace(content.GetContent()))
			{
				message = content.GetContent().Trim();
			}

			output.Content.SetHtmlContent(message);
		}

	}
}
