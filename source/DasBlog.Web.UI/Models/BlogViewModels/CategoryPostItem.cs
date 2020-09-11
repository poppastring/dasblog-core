using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class CategoryPostItem
    {
		public string Category { get; set; }

		public string BlogTitle { get; set; }

		public string BlogId { get; set; }

		public DateTime Date { get; set; }

		public static CategoryPostItem CreateFromEntry(Entry entry, IDasBlogSettings dasBlogSettings)
		{
			return new CategoryPostItem
			{
				Category = entry.GetSplitCategories().FirstOrDefault(),
				BlogTitle = entry.Title,
				BlogId = dasBlogSettings.GeneratePostUrl(entry),
				Date = entry.CreatedLocalTime
				
			};
		}
	}
}
