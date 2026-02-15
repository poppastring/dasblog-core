using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentManagementLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		private const string COMMENT_MANAGEMENT_URL = "admin/manage-comments/{0}";

		public CommentManagementLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var url = string.Format(COMMENT_MANAGEMENT_URL, Post.EntryId);
			var message = "Manage Post Comments";

			output.TagName = "a";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot(url));
			output.Attributes.SetAttribute("class", "dbc-comment-management-link");

			output.Content.SetHtmlContent(message);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
