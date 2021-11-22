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
			string css;
			switch (Comment.SpamState)
			{
				case SpamStateViewModel.Spam:
					css = "table-danger";
					break;
				case SpamStateViewModel.NotSpam:
					css = "table-success";
					break;
				default:
					css = "table-warning";
					break;
			}

			output.TagName = "tr";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", css);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}

	}
}
