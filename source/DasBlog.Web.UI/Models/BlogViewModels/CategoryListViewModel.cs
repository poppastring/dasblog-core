using System.Collections.Generic;
using System.Linq;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class CategoryListViewModel
	{
		public Dictionary<string, List<CategoryPostItem>> Categories { get; set; } = new Dictionary<string, List<CategoryPostItem>>();

		public Dictionary<string, List<CategoryListItem>> Items { get; protected set; } = new Dictionary<string, List<CategoryListItem>>();

		public static CategoryListViewModel Create(EntryCollection entries)
		{
			var viewModel = new CategoryListViewModel();
			foreach (var entry in entries)
			{
				var categories = entry.GetSplitCategories();
				foreach (var category in categories)
				{
					var archiveItem = CategoryListItem.CreateFromEntry(entry);
					archiveItem.Category = category;
					if (viewModel.Items.ContainsKey(category))
					{
						viewModel.Items[category].Add(archiveItem);
						continue;
					}

					viewModel.Items[category] = new List<CategoryListItem> { archiveItem };
				}
			}

			return viewModel;
		}

		public class CategoryListItem
		{
			public string Category { get; set; }

			public string BlogTitle { get; set; }

			public string BlogId { get; set; }

			public static CategoryListItem CreateFromEntry(Entry entry)
			{
				return new CategoryListItem
				{
					Category = entry.GetSplitCategories().FirstOrDefault(),
					BlogTitle = entry.Title,
					BlogId = entry.EntryId
				};
			}
		}
	}
}
