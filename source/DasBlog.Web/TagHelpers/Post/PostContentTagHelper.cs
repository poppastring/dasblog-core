using DasBlog.Core.Extensions;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using System.Web;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostContentTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public bool StripHtml { get; set; } = false;

		public int ContentLength { get; set; } = 100000;

		private readonly IDasBlogSettings dasBlogSettings;

		public PostContentTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var content = HttpUtility.HtmlDecode(Post.Content);

			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dbc-post-content");

			if(StripHtml)
			{
				content = content.StripHTMLFromText().CutLongString(ContentLength);
			}

			output.Content.SetHtmlContent(content);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
