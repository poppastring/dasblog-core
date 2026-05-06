using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Site
{
	/// <summary>
	/// Renders an archive listing of months or years that have published posts,
	/// linked to the corresponding archive page.
	///
	/// Usage:
	///     &lt;archive-list /&gt;
	///     &lt;archive-list grouping="year" /&gt;
	///     &lt;archive-list show-counts="true" /&gt;
	///
	/// Inner content is treated as a per-item template where:
	///     {0} = label, {1} = URL, {2} = post count
	/// </summary>
	[HtmlTargetElement("archive-list")]
	public class ArchiveListTagHelper : TagHelper
	{
		private const string DefaultCss = "dbc-archive-list";

		private readonly IArchiveManager archiveManager;
		private readonly IUrlResolver urlResolver;

		/// <summary>
		/// "month" (default) groups by year+month; "year" groups by year only.
		/// </summary>
		public string Grouping { get; set; } = "month";

		public bool ShowCounts { get; set; }

		public string Css { get; set; }

		public ArchiveListTagHelper(IArchiveManager archiveManager, IUrlResolver urlResolver)
		{
			this.archiveManager = archiveManager;
			this.urlResolver = urlResolver;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "ul";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", string.IsNullOrEmpty(Css) ? DefaultCss : $"{DefaultCss} {Css}");

			var template = (await output.GetChildContentAsync()).GetContent();

			var days = archiveManager.GetDaysWithEntries() ?? new List<DateTime>();

			var byYear = string.Equals(Grouping, "year", StringComparison.OrdinalIgnoreCase);

			IEnumerable<(string Label, string Url, int Count)> items;

			if (byYear)
			{
				items = days
					.GroupBy(d => d.Year)
					.OrderByDescending(g => g.Key)
					.Select(g => (
						Label: g.Key.ToString(CultureInfo.InvariantCulture),
						Url: urlResolver.RelativeToRoot($"archive/{g.Key}"),
						Count: g.Count()));
			}
			else
			{
				items = days
					.GroupBy(d => new { d.Year, d.Month })
					.OrderByDescending(g => g.Key.Year)
					.ThenByDescending(g => g.Key.Month)
					.Select(g => (
						Label: new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture),
						Url: urlResolver.RelativeToRoot($"archive/{g.Key.Year}/{g.Key.Month:00}"),
						Count: g.Count()));
			}

			var sb = new StringBuilder();

			foreach (var item in items)
			{
				if (!string.IsNullOrWhiteSpace(template))
				{
					sb.Append(string.Format(template, WebUtility.HtmlEncode(item.Label), item.Url, item.Count));
				}
				else
				{
					sb.Append("<li class=\"dbc-archive-list-item\"><a href=\"");
					sb.Append(WebUtility.HtmlEncode(item.Url));
					sb.Append("\">");
					sb.Append(WebUtility.HtmlEncode(item.Label));
					sb.Append("</a>");

					if (ShowCounts)
					{
						sb.Append(" <span class=\"dbc-archive-count text-muted\">(");
						sb.Append(item.Count);
						sb.Append(")</span>");
					}

					sb.Append("</li>");
				}
			}

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
