using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostImageTagHelper: TagHelper
	{
		public PostViewModel Post { get; set; }

		public string DefaultImage { get; set; }
		public string Css { get; set; }
		public string Style { get; set; }
		public string Alt { get; set; }

		private readonly IUrlResolver urlResolver;

		public PostImageTagHelper(IUrlResolver urlResolver)
		{
			this.urlResolver = urlResolver;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var imgUrl = Post.ImageUrl;

			output.TagMode = TagMode.SelfClosing;
			output.TagName = "img";

			if (!string.IsNullOrEmpty(imgUrl))
			{
				output.Attributes.SetAttribute("src", urlResolver.RelativeToRoot(imgUrl));
			}
			else
			{
				if (!string.IsNullOrEmpty(DefaultImage))
				{
					output.Attributes.SetAttribute("src", DefaultImage);
				}
			}

			if (!string.IsNullOrEmpty(Css))
			{
				output.Attributes.SetAttribute("class", Css);
			}

			if (!string.IsNullOrEmpty(Style))
			{
				output.Attributes.SetAttribute("style", Style);
			}

			// Always emit an alt attribute for valid HTML / WCAG compliance.
			// Resolution order: explicit Alt -> Post.ImageAlt -> Post.Title -> empty (decorative).
			string altText;
			if (!string.IsNullOrEmpty(Alt))
			{
				altText = Alt;
			}
			else if (!string.IsNullOrEmpty(Post?.ImageAlt))
			{
				altText = Post.ImageAlt;
			}
			else
			{
				altText = Post?.Title ?? string.Empty;
			}

			output.Attributes.SetAttribute("alt", altText);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
