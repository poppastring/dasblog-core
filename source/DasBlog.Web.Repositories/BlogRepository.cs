using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Repositories.Interfaces;
using DasBlog.Web.Core;
using newtelligence.DasBlog.Util;

namespace DasBlog.Web.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public BlogRepository(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public Entry GetBlogPost(string postid)
        {
            return _dataService.GetEntry(postid);
        }

        public EntryCollection GetFrontPagePosts()
        {
            DateTime fpDayUtc;
            TimeZone tz;

            //Need to insert the Request.Headers["Accept-Language"];
            string languageFilter = "en-US"; // Request.Headers["Accept-Language"];
            fpDayUtc = DateTime.UtcNow.AddDays(_dasBlogSettings.SiteConfiguration.ContentLookaheadDays);

            if (_dasBlogSettings.SiteConfiguration.AdjustDisplayTimeZone)
            {
                tz = WindowsTimeZone.TimeZones.GetByZoneIndex(_dasBlogSettings.SiteConfiguration.DisplayTimeZoneIndex);
            }
            else
            {
                tz = new UTCTimeZone();
            }

            return _dataService.GetEntriesForDay(fpDayUtc, TimeZone.CurrentTimeZone,
                                languageFilter,
                                _dasBlogSettings.SiteConfiguration.FrontPageDayCount, _dasBlogSettings.SiteConfiguration.FrontPageEntryCount, string.Empty);
        }

        public EntryCollection GetEntriesForPage(int pageIndex)
        {
            Predicate<Entry> pred = null;

            //Shallow copy as we're going to modify it...and we don't want to modify THE cache.
            EntryCollection cache = _dataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

            // remove the posts from the front page
            EntryCollection fp = GetFrontPagePosts();

            cache.RemoveRange(0, fp.Count);

            int entriesPerPage = _dasBlogSettings.SiteConfiguration.EntriesPerPage;

            // compensate for frontpage
            if ((pageIndex - 1) * entriesPerPage < cache.Count)
            {
                // Remove all entries before the current page's first entry.
                int end = (pageIndex - 1) * entriesPerPage;
                cache.RemoveRange(0, end);

                // Remove all entries after the page's last entry.
                if (cache.Count - entriesPerPage > 0)
                {
                    cache.RemoveRange(entriesPerPage, cache.Count - entriesPerPage);
                    // should match
                    bool postCount = cache.Count <= entriesPerPage;
                }

                return _dataService.GetEntries(null, EntryCollectionFilter.DefaultFilters.IsInEntryIdCacheEntryCollection(cache),
                    Int32.MaxValue,
                    Int32.MaxValue);
            }

            return new EntryCollection();
        }

    }
}
