using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostLinkTagHelper : TagHelper

	{
		public PostViewModel Post { get; set; }

		public string Css { get; set; }

		private readonly IUrlResolver urlResolver;

			public PostLinkTagHelper(IUrlResolver urlResolver)
			{
				this.urlResolver = urlResolver;
			}

			public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
			{
				output.TagMode = TagMode.StartTagAndEndTag;
				output.TagName = "a";

				if (!string.IsNullOrEmpty(Css))
				{
					output.Attributes.SetAttribute("class", Css);
				}

				output.Attributes.SetAttribute("href", urlResolver.RelativeToRoot(Post.PermaLink));
			await Task.CompletedTask;
		}
	}
}
