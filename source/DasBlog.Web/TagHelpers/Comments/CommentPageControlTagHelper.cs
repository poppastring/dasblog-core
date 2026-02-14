using System;
using System.Threading.Tasks;
using DasBlog.Core.Common;
using DasBlog.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentPageControlTagHelper : TagHelper
	{
		public string NewerPostsText { get; set; } = "Newer Comments &gt;&gt;";

		public string OlderPostsText { get; set; } = "&lt;&lt; Older Comments";

		public bool SeperatorRequired { get; set; } = true;

		private int PostCount { get; set; }
		private int PageNumber { get; set; }
		private const string PAGEANCHOR = "<span class='dbc-span-page-control-{2}'><a href='{0}'>{1}</a></span>";

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		private IDasBlogSettings dasBlogSettings;

		public CommentPageControlTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			PostCount = (int?)ViewContext.ViewData[Constants.CommentPostCount] ?? 0;
			PageNumber = (int?)ViewContext.ViewData[Constants.CommentPageNumber] ?? 0;

			var pagecontrol = string.Empty;

			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-span-comment-page-control");

			var content = await output.GetChildContentAsync();
			var seperator = " | ";
			
			if(!string.IsNullOrEmpty(content.GetContent()))
			{
				seperator = content.GetContent();
			}

			var separatorRequired = PostCount > 0 && PageNumber > 0;

			if (PostCount > 0)
			{
				pagecontrol = string.Format(PAGEANCHOR, dasBlogSettings.RelativeToRoot(string.Format("admin/manage-comments/page/{0}", PageNumber + 1)), OlderPostsText, "older");
			}
			
			if (separatorRequired)
			{
				pagecontrol += seperator;
			}

			if (PageNumber > 0)
			{
				pagecontrol += string.Format(PAGEANCHOR, dasBlogSettings.RelativeToRoot(string.Format("admin/manage-comments/page/{0}", PageNumber - 1)), NewerPostsText, "newer");
			}

			output.Content.SetHtmlContent(pagecontrol);
		}
	}
}
