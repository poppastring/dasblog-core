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

        public SiteManager(IDasBlogSettings settings, IBlogDataService dataService, ILoggingDataService loggingDataService)
        {
            dasBlogSettings = settings;
            this.dataService = dataService;
            this.loggingDataService = loggingDataService;
		}

        public UrlSet GetGoogleSiteMap()
        {
            var root = new UrlSet();
            root.url = new UrlCollection();

            //Default first...
            var basePage = new Url(dasBlogSettings.GetBaseUrl(), DateTime.Now, ChangeFreq.daily, 1.0M);
            root.url.Add(basePage);

            var archivePage = new Url(dasBlogSettings.RelativeToRoot("archive"), DateTime.Now, ChangeFreq.daily, 1.0M);
            root.url.Add(archivePage);

			var categorpage = new Url(dasBlogSettings.RelativeToRoot("category"), DateTime.Now, ChangeFreq.daily, 1.0M);
			root.url.Add(categorpage);

			//All Pages
			var entryCache = dataService.GetEntries(false);
            foreach (var e in entryCache)
            {
                if (e.IsPublic)
                {
                    //Start with a RARE change freq...newer posts are more likely to change more often.
                    // The older a post, the less likely it is to change...
                    var freq = ChangeFreq.daily;

                    //new stuff?
                    if (e.CreatedLocalTime < DateTime.Now.AddMonths(-9))
                    {
                        freq = ChangeFreq.yearly;
                    }
                    else if (e.CreatedLocalTime < DateTime.Now.AddDays(-30))
                    {
                        freq = ChangeFreq.monthly;
                    }
                    else if (e.CreatedLocalTime < DateTime.Now.AddDays(-7))
                    {
                        freq = ChangeFreq.weekly;
                    }
                    if (e.CreatedLocalTime > DateTime.Now.AddDays(-2))
                    {
                        freq = ChangeFreq.hourly;
                    }

                    //Add comments pages, since comments have indexable content...
                    // Only add comments if we aren't showing comments on permalink pages already
                    if (dasBlogSettings.SiteConfiguration.ShowCommentsWhenViewingEntry == false)
                    {
                        var commentPage = new Url(dasBlogSettings.GetCommentViewUrl(e.CompressedTitle), e.CreatedLocalTime, freq, 0.7M);
                        root.url.Add(commentPage);
                    }

                    //then add permalinks
                    var permaPage = new Url(dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(e)), e.CreatedLocalTime, freq, 0.9M);
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
					var caturl = new Url(dasBlogSettings.GetCategoryViewUrl(catname), DateTime.Now, ChangeFreq.weekly, 0.6M);
                    root.url.Add(caturl);
                }
            }

            return root;
        }
    }
}

