using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;
using DasBlog.Core;

namespace DasBlog.Managers
{
	public class CategoryManager : ICategoryManager
    {
        private IBlogDataService dataService;
        private ILoggingDataService loggingDataService;
        private readonly IDasBlogSettings dasBlogSettings;

        public CategoryManager(IDasBlogSettings settings)
        {
            dasBlogSettings = settings;
            loggingDataService = LoggingDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.LogDir);
            dataService = BlogDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.ContentDir, loggingDataService);
        }

        public EntryCollection GetEntries()
        {
            return dataService.GetEntries(false);
        }

        public EntryCollection GetEntries(string category, string acceptLanguages)
        {
            return dataService.GetEntriesForCategory(category, acceptLanguages);
        }
    }
}
