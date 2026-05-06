using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Site
{
	/// <summary>
	/// Renders the list of public categories defined for the blog with links
	/// pointing to each category's view page.
	///
	/// Usage:
	///     &lt;category-list /&gt;
	///     &lt;category-list show-counts="true" /&gt;
	///
	/// Inner content is treated as a per-item template where:
	///     {0} = category display name, {1} = category URL, {2} = post count
	///     &lt;category-list&gt;&lt;li&gt;&lt;a href="{1}"&gt;{0} ({2})&lt;/a&gt;&lt;/li&gt;&lt;/category-list&gt;
	/// </summary>
	[HtmlTargetElement("category-list")]
	public class CategoryListTagHelper : TagHelper
	{
		private const string DefaultCss = "dbc-category-list";

		private readonly IBlogManager blogManager;
		private readonly IUrlResolver urlResolver;
		private readonly IContentProcessor contentProcessor;

		public bool ShowCounts { get; set; }

		public string Css { get; set; }

		public CategoryListTagHelper(IBlogManager blogManager, IUrlResolver urlResolver, IContentProcessor contentProcessor)
		{
			this.blogManager = blogManager;
			this.urlResolver = urlResolver;
			this.contentProcessor = contentProcessor;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "ul";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", string.IsNullOrEmpty(Css) ? DefaultCss : $"{DefaultCss} {Css}");

			var template = (await output.GetChildContentAsync()).GetContent();

			var categories = blogManager.GetCategories()
				?.Where(c => c.IsPublic)
				.OrderBy(c => c.DisplayName)
				.ToList();

			var sb = new StringBuilder();

			if (categories != null)
			{
				foreach (var cat in categories)
				{
					var name = WebUtility.HtmlEncode(cat.DisplayName ?? cat.Name ?? string.Empty);
					var url = urlResolver.GetCategoryViewUrl(contentProcessor.CompressTitle(cat.Name));
					var count = cat.EntryDetails?.Count ?? 0;

					if (!string.IsNullOrWhiteSpace(template))
					{
						sb.Append(string.Format(template, name, url, count));
					}
					else
					{
						sb.Append("<li class=\"dbc-category-list-item\"><a href=\"");
						sb.Append(WebUtility.HtmlEncode(url));
						sb.Append("\">");
						sb.Append(name);
						sb.Append("</a>");

						if (ShowCounts)
						{
							sb.Append(" <span class=\"dbc-category-count text-muted\">(");
							sb.Append(count);
							sb.Append(")</span>");
						}

						sb.Append("</li>");
					}
				}
			}

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
