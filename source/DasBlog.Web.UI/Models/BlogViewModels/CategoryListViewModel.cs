using System.Collections.Generic;
using System.Linq;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class CategoryListViewModel
	{
		public Dictionary<string, List<CategoryPostItem>> Categories { get; protected set; } = new Dictionary<string, List<CategoryPostItem>>();

		public static CategoryListViewModel Create(EntryCollection entries)
		{
			var viewModel = new CategoryListViewModel();
			foreach (var entry in entries)
			{
				var categories = entry.GetSplitCategories();
				foreach (var category in categories)
				{
					var archiveItem = CategoryPostItem.CreateFromEntry(entry);
					archiveItem.Category = category;
					if (viewModel.Categories.ContainsKey(category))
					{
						viewModel.Categories[category].Add(archiveItem);
						continue;
					}

					viewModel.Categories[category] = new List<CategoryPostItem> { archiveItem };
				}
			}

			return viewModel;
		}
	}
}
