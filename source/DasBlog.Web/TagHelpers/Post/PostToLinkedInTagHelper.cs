using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers;

public class PostToLinkedInTagHelper : TagHelper
{
	private const string LINKEDIN_SHARE_URL = "https://www.linkedin.com/sharing/share-offsite/?url={0}";
	private readonly IDasBlogSettings dasBlogSettings;
	public PostViewModel Post { get; set; }

	public PostToLinkedInTagHelper(IDasBlogSettings dasBlogSettings)
	{
		this.dasBlogSettings = dasBlogSettings;
	}

	public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
	{
		output.TagName = "a";
		output.TagMode = TagMode.StartTagAndEndTag;
		output.Attributes.SetAttribute("class", "dasblog-a-share-linkedin");
		output.Attributes.SetAttribute("href", string.Format(LINKEDIN_SHARE_URL, 
			UrlEncoder.Default.Encode(dasBlogSettings.RelativeToRoot(Post.PermaLink))));

		var content = await output.GetChildContentAsync();

		output.Content.SetHtmlContent(content.GetContent());
	}
}
