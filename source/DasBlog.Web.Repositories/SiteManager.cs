using DasBlog.Web;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers
{
    public class SiteManager : ISiteManager
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public SiteManager(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public urlset GetGoogleSiteMap()
        {
            urlset root = new urlset();
            root.url = new urlCollection();

            //Default first...
            url basePage = new url(_dasBlogSettings.GetBaseUrl(), DateTime.Now, changefreq.daily, 1.0M);
            root.url.Add(basePage);

            //Archives next...
            url archivePage = new url(_dasBlogSettings.RelativeToRoot("archives"), DateTime.Now, changefreq.daily, 1.0M);
            root.url.Add(archivePage);

            //All Pages
            EntryCollection entryCache = _dataService.GetEntries(false);
            foreach (Entry e in entryCache)
            {
                if (e.IsPublic)
                {
                    //Start with a RARE change freq...newer posts are more likely to change more often.
                    // The older a post, the less likely it is to change...
                    changefreq freq = changefreq.daily;

                    //new stuff?
                    if (e.CreatedLocalTime < DateTime.Now.AddMonths(-9))
                    {
                        freq = changefreq.yearly;
                    }
                    else if (e.CreatedLocalTime < DateTime.Now.AddDays(-30))
                    {
                        freq = changefreq.monthly;
                    }
                    else if (e.CreatedLocalTime < DateTime.Now.AddDays(-7))
                    {
                        freq = changefreq.weekly;
                    }
                    if (e.CreatedLocalTime > DateTime.Now.AddDays(-2))
                    {
                        freq = changefreq.hourly;
                    }

                    //Add comments pages, since comments have indexable content...
                    // Only add comments if we aren't showing comments on permalink pages already
                    if (_dasBlogSettings.SiteConfiguration.ShowCommentsWhenViewingEntry == false)
                    {
                        url commentPage = new url(_dasBlogSettings.GetCommentViewUrl(e.EntryId), e.CreatedLocalTime, freq, 0.7M);
                        root.url.Add(commentPage);
                    }

                    //then add permalinks
                    url permaPage = new url(_dasBlogSettings.GetPermaLinkUrl(e.EntryId), e.CreatedLocalTime, freq, 0.9M);
                    root.url.Add(permaPage);
                }
            }

            //All Categories
            CategoryCacheEntryCollection catCache = _dataService.GetCategories();
            foreach (CategoryCacheEntry cce in catCache)
            {
                if (cce.IsPublic)
                {
                    url catPage = new url(_dasBlogSettings.GetCategoryViewUrl(cce.Name), DateTime.Now, changefreq.weekly, 0.6M);
                    root.url.Add(catPage);
                }
            }

            return root;
        }
    }
}

