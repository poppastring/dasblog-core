using DasBlog.Web.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Core;
using System.Linq;

namespace DasBlog.Web.Repositories
{
    public class ArchiveRepository : IArchiveRepository
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public ArchiveRepository(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public EntryCollection GetEntriesForMonth(DateTime date, string acceptLanguages)
        {
            return _dataService.GetEntriesForMonth(date, _dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages);
        }

        public EntryCollection GetEntriesForYear(DateTime date, string acceptLanguages)
        {
            EntryCollection yearCollection = new EntryCollection();
            for (int i = 1; i < 13; i++)
            {
                DateTime dt = new DateTime(date.Year, i, 1);
                yearCollection.AddRange(_dataService.GetEntriesForMonth(dt, _dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages));
            }

            return yearCollection;
        }

        public List<DateTime> GetDaysWithEntries()
        {
            List<DateTime> dateTime = new List<DateTime>();
            dateTime = _dataService.GetDaysWithEntries(_dasBlogSettings.GetConfiguredTimeZone()).ToList<DateTime>();

            return dateTime;
        }

        public EntryCollection GetEntriesForDay(DateTime date, string acceptLanguages)
        {
            return _dataService.GetEntriesForDay(date, _dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages,
                                        _dasBlogSettings.SiteConfiguration.FrontPageDayCount, _dasBlogSettings.SiteConfiguration.FrontPageDayCount, "");
        }
    }
}
