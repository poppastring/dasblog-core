using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace DasBlog.Managers
{
	public class ArchiveManager : IArchiveManager
    {
        private readonly IBlogDataService dataService;
        private readonly ILoggingDataService loggingDataService;
        private readonly IDasBlogSettings dasBlogSettings;

        public ArchiveManager(IDasBlogSettings settings)
        {
            dasBlogSettings = settings;

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);
		}

        public EntryCollection GetEntriesForMonth(DateTime date, string acceptLanguages)
        {
            return dataService.GetEntriesForMonth(date, dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages);
        }

        public EntryCollection GetEntriesForYear(DateTime date, string acceptLanguages)
        {
            var yearCollection = new EntryCollection();
            for (int i = 1; i < 13; i++)
            {
                DateTime dt = new DateTime(date.Year, i, 1);
                yearCollection.AddRange(dataService.GetEntriesForMonth(dt, dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages));
            }

            return yearCollection;
        }

        public List<DateTime> GetDaysWithEntries()
        {
            var dateTime = new List<DateTime>();
            dateTime = dataService.GetDaysWithEntries(dasBlogSettings.GetConfiguredTimeZone()).ToList<DateTime>();

            return dateTime;
        }

        public EntryCollection GetEntriesForDay(DateTime date, string acceptLanguages)
        {
            return dataService.GetEntriesForDay(date, dasBlogSettings.GetConfiguredTimeZone(), acceptLanguages,
                                        dasBlogSettings.SiteConfiguration.FrontPageDayCount, dasBlogSettings.SiteConfiguration.FrontPageDayCount, "");
        }
    }
}
