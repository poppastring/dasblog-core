using DasBlog.Core.Common;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Net;
using System.Text;

namespace DasBlog.Web.TagHelpers.Post
{
	/// <summary>
	/// Renders a post's author byline using the configured display name.
	///
	/// Usage:
	///     &lt;post-author post="Model" /&gt;
	///     &lt;post-author post="Model" link="true" /&gt;
	///     &lt;post-author post="Model" avatar="true" avatar-size="32" /&gt;
	/// </summary>
	[HtmlTargetElement("post-author")]
	public class PostAuthorTagHelper : TagHelper
	{
		private const string DefaultCss = "dbc-post-author";
		private const string GravatarLink = "//www.gravatar.com/avatar/{0}?rating={1}&size={2}&default={3}";

		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ISiteConfig siteConfig;

		public PostViewModel Post { get; set; }

		/// <summary>
		/// When true, wraps the display name in a mailto: link to the author's email.
		/// </summary>
		public bool Link { get; set; }

		/// <summary>
		/// When true, prefixes the byline with the author's gravatar image.
		/// </summary>
		public bool Avatar { get; set; }

		public int AvatarSize { get; set; } = 24;

		public string Css { get; set; }

		public PostAuthorTagHelper(IDasBlogSettings dasBlogSettings, ISiteConfig siteConfig)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.siteConfig = siteConfig;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "span";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", string.IsNullOrEmpty(Css) ? DefaultCss : $"{DefaultCss} {Css}");

			var email = Post?.Author;
			var user = string.IsNullOrEmpty(email) ? null : dasBlogSettings.GetUserByEmail(email);
			var displayName = user?.DisplayName ?? email ?? string.Empty;

			var sb = new StringBuilder();

			if (Avatar && !string.IsNullOrEmpty(email))
			{
				var hash = Utils.GetGravatarHash(email);
				var src = string.Format(GravatarLink, hash, siteConfig.CommentsGravatarRating, AvatarSize, siteConfig.CommentsGravatarNoImgPath);

				sb.Append("<img class=\"dbc-post-author-avatar rounded-circle\" src=\"");
				sb.Append(WebUtility.HtmlEncode(src));
				sb.Append("\" alt=\"");
				sb.Append(WebUtility.HtmlEncode(displayName));
				sb.Append("\" width=\"");
				sb.Append(AvatarSize);
				sb.Append("\" height=\"");
				sb.Append(AvatarSize);
				sb.Append("\" /> ");
			}

			var encodedName = WebUtility.HtmlEncode(displayName);

			if (Link && !string.IsNullOrEmpty(email))
			{
				sb.Append("<a class=\"dbc-post-author-link\" href=\"mailto:");
				sb.Append(WebUtility.HtmlEncode(email));
				sb.Append("\">");
				sb.Append(encodedName);
				sb.Append("</a>");
			}
			else
			{
				sb.Append(encodedName);
			}

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
