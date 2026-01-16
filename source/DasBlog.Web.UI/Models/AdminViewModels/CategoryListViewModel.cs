using System.Linq;
using System.Collections.Generic;
using DasBlog.Web.Models.BlogViewModels;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class CategoryListViewModel
	{
		public string Name { get; set; }

		public string Category { get; set; }

		public List<CategoryListViewModel> Init(List<string> categories)
		{
			var allposts = categories.Select(p => new CategoryListViewModel { Name = p, Category = p}).ToList();

			allposts.Insert(0, new CategoryListViewModel { Name = "--Disable Home Category--", Category = "" });

			return allposts;
		}
	}
}
