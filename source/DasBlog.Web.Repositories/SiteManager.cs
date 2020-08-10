using DasBlog.Core.Services.GoogleSiteMap;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;

namespace DasBlog.Managers
{
    public class SiteManager : ISiteManager
    {
        private readonly IBlogDataService dataService;
        private readonly ILoggingDataService loggingDataService;
        private readonly IDasBlogSettings dasBlogSettings;

        public SiteManager(IDasBlogSettings settings)
        {
            dasBlogSettings = settings;

			loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);
		}

        public urlset GetGoogleSiteMap()
        {
            var root = new urlset();
            root.url = new urlCollection();

            //Default first...
            var basePage = new url(dasBlogSettings.GetBaseUrl(), DateTime.Now, changefreq.daily, 1.0M);
            root.url.Add(basePage);

            var archivePage = new url(dasBlogSettings.RelativeToRoot("archive"), DateTime.Now, changefreq.daily, 1.0M);
            root.url.Add(archivePage);

			var categorpage = new url(dasBlogSettings.RelativeToRoot("category"), DateTime.Now, changefreq.daily, 1.0M);
			root.url.Add(categorpage);

			//All Pages
			var entryCache = dataService.GetEntries(false);
            foreach (var e in entryCache)
            {
                if (e.IsPublic)
                {
                    //Start with a RARE change freq...newer posts are more likely to change more often.
                    // The older a post, the less likely it is to change...
                    var freq = changefreq.daily;

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
                    if (dasBlogSettings.SiteConfiguration.ShowCommentsWhenViewingEntry == false)
                    {
                        var commentPage = new url(dasBlogSettings.GetCommentViewUrl(e.CompressedTitle), e.CreatedLocalTime, freq, 0.7M);
                        root.url.Add(commentPage);
                    }

                    //then add permalinks
                    var permaPage = new url(dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(e)), e.CreatedLocalTime, freq, 0.9M);
                    root.url.Add(permaPage);
                }
            }

            //All Categories
            var catCache = dataService.GetCategories();
            foreach (var cce in catCache)
            {
                if (cce.IsPublic)
                {
					var catname = Entry.InternalCompressTitle(cce.Name, dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement).ToLower();
					var caturl = new url(dasBlogSettings.GetCategoryViewUrl(catname), DateTime.Now, changefreq.weekly, 0.6M);
                    root.url.Add(caturl);
                }
            }

            return root;
        }
    }
}

