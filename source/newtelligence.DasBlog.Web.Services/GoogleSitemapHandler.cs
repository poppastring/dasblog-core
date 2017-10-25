using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.Services.Rss20;
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Web.Services
{
    public class GoogleSitemapHandler : IHttpHandler
    {
        public GoogleSitemapHandler()
        {
        }


        public void ProcessRequest(HttpContext context)
        {
            try
            {
                //Cache the sitemap for 12 hours...
                string CacheKey = "GoogleSiteMap";
                DataCache cache = CacheFactory.GetCache();

                urlset root = cache[CacheKey] as urlset;
                if (root == null) //we'll have to build it...
                {
                    ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                    IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);
                    SiteConfig siteConfig = SiteConfig.GetSiteConfig();

                    root = new urlset();

                    root.url = new urlCollection();

                    //Default first...
                    url basePage = new url(SiteUtilities.GetBaseUrl(siteConfig), DateTime.Now, changefreq.daily, 1.0M);
                    root.url.Add(basePage);

                    url defaultPage = new url(SiteUtilities.GetStartPageUrl(siteConfig), DateTime.Now, changefreq.daily, 1.0M);
                    root.url.Add(defaultPage);

                    //Archives next...
                    url archivePage = new url(SiteUtilities.RelativeToRoot(siteConfig, "archives.aspx"), DateTime.Now, changefreq.daily, 1.0M);
                    root.url.Add(archivePage);

                    //All Pages
                    EntryCollection entryCache = dataService.GetEntries(false);
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
                            if (siteConfig.ShowCommentsWhenViewingEntry == false)
                            {
                                url commentPage = new url(SiteUtilities.GetCommentViewUrl(siteConfig, e.EntryId), e.CreatedLocalTime, freq, 0.7M);
                                root.url.Add(commentPage);
                            }

                            //then add permalinks
                            url permaPage = new url(SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)e), e.CreatedLocalTime, freq, 0.9M);
                            root.url.Add(permaPage);
                        }
                    }

                    //All Categories
                    CategoryCacheEntryCollection catCache = dataService.GetCategories();
                    foreach (CategoryCacheEntry cce in catCache)
                    {
                        if (cce.IsPublic)
                        {
                            url catPage = new url(SiteUtilities.GetCategoryViewUrl(siteConfig, cce.Name), DateTime.Now, changefreq.weekly, 0.6M);
                            root.url.Add(catPage);
                        }
                    }
                    cache.Insert(CacheKey, root, DateTime.Now.AddHours(12));
                }

                XmlSerializer x = new XmlSerializer(typeof(urlset));
                x.Serialize(context.Response.OutputStream, root);
                context.Response.ContentType = "text/xml";
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, exc);
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}
