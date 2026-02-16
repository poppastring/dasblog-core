using System.Threading.Tasks;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Comments
{
	public class CommentGravatarImageTagHelper : TagHelper
	{
		public CommentViewModel Comment { get; set; }

		public string Css { get; set; }

		private readonly ISiteConfig siteConfig;
		private const string gravatarLink = "//www.gravatar.com/avatar/{0}?rating={1}&size={2}&default={3}";

		public CommentGravatarImageTagHelper(ISiteConfig siteConfig)
		{
			this.siteConfig = siteConfig;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "img";
			output.TagMode = TagMode.SelfClosing;
			output.Attributes.SetAttribute("src", string.Format(gravatarLink, Comment.GravatarHashId, 
					siteConfig.CommentsGravatarRating, siteConfig.CommentsGravatarSize, 
					siteConfig.CommentsGravatarNoImgPath));

			output.Attributes.SetAttribute("alt", "gravatar");

			var style = "dbc-comment-gravatar";

			if (!string.IsNullOrEmpty(Css))
			{
				style = string.Format("{0} {1}", style, Css);
			}

			output.Attributes.SetAttribute("class", style);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}

}
