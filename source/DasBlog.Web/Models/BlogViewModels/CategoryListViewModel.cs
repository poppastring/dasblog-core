using System.Collections.Generic;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class CategoryListViewModel
	{
		public SortedDictionary<string, List<CategoryPostItem>> Categories { get; protected set; } = new SortedDictionary<string, List<CategoryPostItem>>();
		
		public static CategoryListViewModel Create(EntryCollection entries, IDasBlogSettings dasBlogSettings, string categoryName = "")
		{
			var viewModel = new CategoryListViewModel();
			foreach (var entry in entries)
			{
				string[] categories = null;
				if (categoryName == string.Empty)
				{
					categories = entry.GetSplitCategories();
				}
				else
				{
					categories = new string[] { categoryName };
				}

				foreach (var category in categories)
				{
					var archiveItem = CategoryPostItem.CreateFromEntry(entry, dasBlogSettings);
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
