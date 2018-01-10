using DasBlog.Web.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Core;

namespace DasBlog.Web.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public CategoryRepository(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public EntryCollection GetEntries()
        {
            return _dataService.GetEntries(false);
        }

        public EntryCollection GetEntries(string category, string acceptLanguages)
        {
            return _dataService.GetEntriesForCategory(category, acceptLanguages);
        }
    }
}
