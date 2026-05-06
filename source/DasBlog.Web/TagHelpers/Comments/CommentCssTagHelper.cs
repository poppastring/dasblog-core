using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentCssTagHelper : TagHelper
	{
		public CommentAdminViewModel Comment { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			string statusClass;
			switch (Comment.SpamState)
			{
				case SpamStateViewModel.Spam:
					statusClass = "dbc-comment-card--spam";
					break;
				case SpamStateViewModel.NotSpam:
					statusClass = "dbc-comment-card--approved";
					break;
				default:
					statusClass = "dbc-comment-card--pending";
					break;
			}

			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", $"card border-0 shadow-sm dbc-comment-card {statusClass}");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}

	}
}
