using System.IO;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers
{
	public class CategoryManager : ICategoryManager
	{
		private readonly IBlogDataService dataService;
		private readonly ISiteConfig siteConfig;

		public CategoryManager(ISiteConfig siteConfig, IBlogDataService dataService)
		{
			this.siteConfig = siteConfig;
			this.dataService = dataService;
		}

		public EntryCollection GetEntries()
		{
			return dataService.GetEntries(false);
		}

		public EntryCollection GetEntries(string category, string acceptLanguages)
		{
			category = category.Replace("-", "+");
			return dataService.GetEntriesForCategory(category, acceptLanguages);
		}

		public string GetCategoryTitle(string categoryurl)
		{
			categoryurl = categoryurl.Replace("-", "+");
			return dataService.GetCategoryTitle(categoryurl);
		}
	}
}
