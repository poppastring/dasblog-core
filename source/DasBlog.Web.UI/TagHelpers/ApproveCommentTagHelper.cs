using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
    public class ApproveCommentTagHelper : TagHelper
    {
		public string BlogPostId { get; set; }

		public string CommentId { get; set; }

		public string CommentorName { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public ApproveCommentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", $"javascript:approveComment(\"{BlogPostId}\",\"{CommentId}\",\"{CommentorName}\")");
			output.Attributes.SetAttribute("class", "dbc-comment-approve-link");
			output.Content.SetHtmlContent("Approve this comment");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
