using DasBlog.Core.Services.GoogleSiteMap;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;
using System.Linq;

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
			var now = DateTime.UtcNow;
			var publicEntries = dataService.GetEntries(false)
				.Cast<Entry>()
				.Where(entry => entry.IsPublic)
				.ToList();
			var latestModified = publicEntries
				.Select(GetLastModified)
				.DefaultIfEmpty(now)
				.Max();

            //Default first...
            var basePage = new Url(dasBlogSettings.GetBaseUrl(), latestModified, ChangeFreq.daily, 1.0M);
            root.url.Add(basePage);

            var archivePage = new Url(dasBlogSettings.RelativeToRoot("archive"), latestModified, ChangeFreq.daily, 1.0M);
            root.url.Add(archivePage);

			var categorpage = new Url(dasBlogSettings.RelativeToRoot("category"), latestModified, ChangeFreq.daily, 1.0M);
			root.url.Add(categorpage);

			//All Pages
            foreach (var e in publicEntries)
            {
				var lastModified = GetLastModified(e);

				//Start with a RARE change freq...newer posts are more likely to change more often.
				// The older a post, the less likely it is to change...
				var freq = ChangeFreq.daily;

				//new stuff?
				if (e.CreatedUtc < now.AddMonths(-9))
				{
					freq = ChangeFreq.yearly;
				}
				else if (e.CreatedUtc < now.AddDays(-30))
				{
					freq = ChangeFreq.monthly;
				}
				else if (e.CreatedUtc < now.AddDays(-7))
				{
					freq = ChangeFreq.weekly;
				}
				if (e.CreatedUtc > now.AddDays(-2))
				{
					freq = ChangeFreq.hourly;
				}

				//Add comments pages, since comments have indexable content...
				// Only add comments if we aren't showing comments on permalink pages already
				if (dasBlogSettings.SiteConfiguration.ShowCommentsWhenViewingEntry == false)
				{
					var commentPage = new Url(dasBlogSettings.GetCommentViewUrl(e.CompressedTitle), lastModified, freq, 0.7M);
					root.url.Add(commentPage);
				}

				//then add permalinks
				var permaPage = new Url(dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(e)), lastModified, freq, 0.9M);
				root.url.Add(permaPage);
            }

            //All Categories
            var catCache = dataService.GetCategories();
            foreach (var cce in catCache)
            {
                if (cce.IsPublic)
                {
					var catname = Entry.InternalCompressTitle(cce.Name, "-").ToLower();
					var categoryModified = publicEntries
						.Where(entry => entry.GetSplitCategories().Any(category =>
							string.Equals(category, cce.Name, StringComparison.OrdinalIgnoreCase)))
						.Select(GetLastModified)
						.DefaultIfEmpty(latestModified)
						.Max();
					var caturl = new Url(dasBlogSettings.GetCategoryViewUrl(catname), categoryModified, ChangeFreq.weekly, 0.6M);
                    root.url.Add(caturl);
                }
            }

            return root;
        }

		private static DateTime GetLastModified(Entry entry)
		{
			return entry.ModifiedUtc == DateTime.MinValue
				? entry.CreatedUtc
				: entry.ModifiedUtc;
		}
    }
}
