using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class TitleLinkTagHelper : TagHelper
	{
		public PostViewModel Post { get; set; }

		public string Css { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;

		public TitleLinkTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagMode = TagMode.StartTagAndEndTag;
			output.TagName = "a";

			var content = await output.GetChildContentAsync();

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			output.Attributes.SetAttribute("href", dasBlogSettings.RelativeToRoot(Post.PermaLink));
			output.Content.SetHtmlContent(Post.Title);
		}
	}
}
