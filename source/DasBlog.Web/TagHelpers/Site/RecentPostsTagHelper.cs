using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Site
{
	/// <summary>
	/// Renders a list of the most recent published posts. Theme authors can place
	/// this in a sidebar without needing to inject IBlogManager.
	///
	/// Usage:
	///     &lt;recent-posts /&gt;
	///     &lt;recent-posts count="10" /&gt;
	///     &lt;recent-posts count="5" show-date="true" /&gt;
	///
	/// Inner content is treated as a per-item template where:
	///     {0} = post title, {1} = post URL, {2} = created date (short)
	///     &lt;recent-posts&gt;&lt;li&gt;&lt;a href="{1}"&gt;{0}&lt;/a&gt; - {2}&lt;/li&gt;&lt;/recent-posts&gt;
	/// </summary>
	[HtmlTargetElement("recent-posts")]
	public class RecentPostsTagHelper : TagHelper
	{
		private const string DefaultCss = "dbc-recent-posts";
		private const int DefaultCount = 5;

		private readonly IBlogManager blogManager;
		private readonly IUrlResolver urlResolver;
		private readonly IHttpContextAccessor httpContextAccessor;

		public int Count { get; set; } = DefaultCount;

		public bool ShowDate { get; set; }

		public string DateFormat { get; set; } = "MMM d, yyyy";

		public string Css { get; set; }

		public RecentPostsTagHelper(IBlogManager blogManager, IUrlResolver urlResolver, IHttpContextAccessor httpContextAccessor)
		{
			this.blogManager = blogManager;
			this.urlResolver = urlResolver;
			this.httpContextAccessor = httpContextAccessor;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "ul";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", string.IsNullOrEmpty(Css) ? DefaultCss : $"{DefaultCss} {Css}");

			var template = (await output.GetChildContentAsync()).GetContent();

			var acceptLanguage = httpContextAccessor.HttpContext?.Request?.Headers["Accept-Language"].ToString();

			var posts = blogManager
				.GetFrontPagePosts(acceptLanguage)
				.OrderByDescending(e => e.CreatedUtc)
				.Take(Math.Max(1, Count))
				.ToList();

			var sb = new StringBuilder();

			foreach (var entry in posts)
			{
				var title = WebUtility.HtmlEncode(entry.Title ?? string.Empty);
				var url = urlResolver.RelativeToRoot(urlResolver.GeneratePostUrl(entry));
				var date = entry.CreatedUtc.ToLocalTime().ToString(DateFormat);

				if (!string.IsNullOrWhiteSpace(template))
				{
					sb.Append(string.Format(template, title, url, date));
				}
				else
				{
					sb.Append("<li class=\"dbc-recent-post-item\"><a href=\"");
					sb.Append(WebUtility.HtmlEncode(url));
					sb.Append("\">");
					sb.Append(title);
					sb.Append("</a>");

					if (ShowDate)
					{
						sb.Append(" <span class=\"dbc-recent-post-date text-muted\">");
						sb.Append(WebUtility.HtmlEncode(date));
						sb.Append("</span>");
					}

					sb.Append("</li>");
				}
			}

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
