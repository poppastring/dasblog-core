using DasBlog.Managers.Interfaces;
using System;
using System.Collections.Generic;
using newtelligence.DasBlog.Web.Services.Rss20;
using newtelligence.DasBlog.Runtime;
using DasBlog.Core;
using System.Xml;
using System.Globalization;
using newtelligence.DasBlog.Util.Html;
using DasBlog.Core.Security;
using newtelligence.DasBlog.Web.Services.Rsd;


namespace DasBlog.Managers
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public SubscriptionManager(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public RssRoot GetRss()
        {
            return GetRssCore(null,  this._dasBlogSettings.SiteConfiguration.RssDayCount, this._dasBlogSettings.SiteConfiguration.RssMainEntryCount);
        }

        public RssRoot GetRssCategory(string categoryName)
        {
            return GetRssCore(categoryName, 0, 0);
        }

        public RssRoot GetAtom()
        {
            throw new NotImplementedException();
        }

        public RssRoot GetAtomCategory(string categoryName)
        {
            throw new NotImplementedException();
        }

        private RssRoot GetRssCore(string category, int maxDayCount, int maxEntryCount)
        {
            EntryCollection entries = null;
            //We only build the entries if blogcore doesn't exist and we'll need them later...
            if (_dataService.GetLastEntryUpdate() == DateTime.MinValue)
            {
                entries = BuildEntries(category, maxDayCount, maxEntryCount);
            }

            // TODO: Detecting modified data?
            //DateTime lastModified = this.GetLatestModifedEntryDateTime(entries);

            //if (SiteUtilities.GetStatusNotModified(lastModified))
            //    return null;

            RssRoot documentRoot = new RssRoot(); ;

            //However, if we made it this far, the not-modified check didn't work, and we may not have entries...
            if (entries == null)
            {
                entries = BuildEntries(category, maxDayCount, maxEntryCount);
            }

            documentRoot.Namespaces.Add("dc", "http://purl.org/dc/elements/1.1/");
            documentRoot.Namespaces.Add("trackback", "http://madskills.com/public/xml/rss/module/trackback/");
            documentRoot.Namespaces.Add("pingback", "http://madskills.com/public/xml/rss/module/pingback/");
            if (_dasBlogSettings.SiteConfiguration.EnableComments)
            {
                documentRoot.Namespaces.Add("wfw", "http://wellformedweb.org/CommentAPI/");
                documentRoot.Namespaces.Add("slash", "http://purl.org/rss/1.0/modules/slash/");
            }
            if (_dasBlogSettings.SiteConfiguration.EnableGeoRss)
            {
                documentRoot.Namespaces.Add("georss", "http://www.georss.org/georss");
            }

            RssChannel ch = new RssChannel();

            if (category == null)
            {
                ch.Title = _dasBlogSettings.SiteConfiguration.Title;
            }
            else
            {
                ch.Title = _dasBlogSettings.SiteConfiguration.Title + " - " + category;
            }

            if (_dasBlogSettings.SiteConfiguration.Description == null || _dasBlogSettings.SiteConfiguration.Description.Trim().Length == 0)
            {
                ch.Description = _dasBlogSettings.SiteConfiguration.Subtitle;
            }
            else
            {
                ch.Description = _dasBlogSettings.SiteConfiguration.Description;
            }

            ch.Link = _dasBlogSettings.GetBaseUrl();
            ch.Copyright = _dasBlogSettings.SiteConfiguration.Copyright;
            if (_dasBlogSettings.SiteConfiguration.RssLanguage != null && _dasBlogSettings.SiteConfiguration.RssLanguage.Length > 0)
            {
                ch.Language = _dasBlogSettings.SiteConfiguration.RssLanguage;
            }
            ch.ManagingEditor = _dasBlogSettings.SiteConfiguration.Contact;
            ch.WebMaster = _dasBlogSettings.SiteConfiguration.Contact;
            ch.Image = null;
            if (_dasBlogSettings.SiteConfiguration.ChannelImageUrl != null && _dasBlogSettings.SiteConfiguration.ChannelImageUrl.Trim().Length > 0)
            {
                newtelligence.DasBlog.Web.Services.Rss20.ChannelImage channelImage = new newtelligence.DasBlog.Web.Services.Rss20.ChannelImage();
                channelImage.Title = ch.Title;
                channelImage.Link = ch.Link;
                if (_dasBlogSettings.SiteConfiguration.ChannelImageUrl.StartsWith("http"))
                {
                    channelImage.Url = _dasBlogSettings.SiteConfiguration.ChannelImageUrl;
                }
                else
                {
                    channelImage.Url = _dasBlogSettings.RelativeToRoot(_dasBlogSettings.SiteConfiguration.ChannelImageUrl);
                }
                ch.Image = channelImage;
            }

            documentRoot.Channels.Add(ch);

            foreach (Entry entry in entries)
            {
                if (entry.IsPublic == false || entry.Syndicated == false)
                {
                    continue;
                }
                XmlDocument doc2 = new XmlDocument();
                List<XmlElement> anyElements = new List<XmlElement>();
                RssItem item = new RssItem();
                item.Title = entry.Title;
                item.Guid = new newtelligence.DasBlog.Web.Services.Rss20.Guid();
                item.Guid.IsPermaLink = false;
                item.Guid.Text = _dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
                item.Link = _dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
                User user = _dasBlogSettings.GetUser(entry.Author);

                XmlElement trackbackPing = doc2.CreateElement("trackback", "ping", "http://madskills.com/public/xml/rss/module/trackback/");
                trackbackPing.InnerText = _dasBlogSettings.GetTrackbackUrl(entry.EntryId);
                anyElements.Add(trackbackPing);

                XmlElement pingbackServer = doc2.CreateElement("pingback", "server", "http://madskills.com/public/xml/rss/module/pingback/");
                pingbackServer.InnerText = _dasBlogSettings.RelativeToRoot("pingback");
                anyElements.Add(pingbackServer);

                XmlElement pingbackTarget = doc2.CreateElement("pingback", "target", "http://madskills.com/public/xml/rss/module/pingback/");
                pingbackTarget.InnerText = _dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
                anyElements.Add(pingbackTarget);

                XmlElement dcCreator = doc2.CreateElement("dc", "creator", "http://purl.org/dc/elements/1.1/");
                if (user != null)
                {
                    dcCreator.InnerText = user.DisplayName;
                }
                anyElements.Add(dcCreator);

                // Add GeoRSS if it exists.
                if (_dasBlogSettings.SiteConfiguration.EnableGeoRss)
                {
                    Nullable<double> latitude = new Nullable<double>();
                    Nullable<double> longitude = new Nullable<double>();

                    if (entry.Latitude.HasValue)
                    {
                        latitude = entry.Latitude;
                    }
                    else
                    {
                        if (_dasBlogSettings.SiteConfiguration.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            latitude = _dasBlogSettings.SiteConfiguration.DefaultLatitude;
                        }
                    }

                    if (entry.Longitude.HasValue)
                    {
                        longitude = entry.Longitude;
                    }
                    else
                    {
                        if (_dasBlogSettings.SiteConfiguration.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            longitude = _dasBlogSettings.SiteConfiguration.DefaultLongitude;
                        }
                    }

                    if (latitude.HasValue && longitude.HasValue)
                    {
                        XmlElement geoLoc = doc2.CreateElement("georss", "point", "http://www.georss.org/georss");
                        geoLoc.InnerText = String.Format(CultureInfo.InvariantCulture, "{0:R} {1:R}", latitude, longitude);
                        anyElements.Add(geoLoc);
                    }
                }

                if (_dasBlogSettings.SiteConfiguration.EnableComments)
                {
                    if (entry.AllowComments)
                    {
                        XmlElement commentApi = doc2.CreateElement("wfw", "comment", "http://wellformedweb.org/CommentAPI/");
                        commentApi.InnerText = _dasBlogSettings.GetCommentViewUrl(entry.EntryId);
                        anyElements.Add(commentApi);
                    }

                    XmlElement commentRss = doc2.CreateElement("wfw", "commentRss", "http://wellformedweb.org/CommentAPI/");
                    commentRss.InnerText = _dasBlogSettings.GetEntryCommentsRssUrl(entry.EntryId);
                    anyElements.Add(commentRss);

                    //for RSS conformance per FeedValidator.org
                    int commentsCount = _dataService.GetPublicCommentsFor(entry.EntryId).Count;
                    if (commentsCount > 0)
                    {
                        XmlElement slashComments = doc2.CreateElement("slash", "comments", "http://purl.org/rss/1.0/modules/slash/");
                        slashComments.InnerText = commentsCount.ToString();
                        anyElements.Add(slashComments);
                    }
                    item.Comments = _dasBlogSettings.GetCommentViewUrl(entry.EntryId);
                }
                item.Language = entry.Language;

                if (entry.Categories != null && entry.Categories.Length > 0)
                {
                    if (item.Categories == null) item.Categories = new RssCategoryCollection();

                    string[] cats = entry.Categories.Split(';');
                    foreach (string c in cats)
                    {
                        RssCategory cat = new RssCategory();
                        string cleanCat = c.Replace('|', '/');
                        cat.Text = cleanCat;
                        item.Categories.Add(cat);
                    }
                }
                if (entry.Attachments.Count > 0)
                {
                    // RSS currently supports only a single enclsoure so we return the first one	
                    item.Enclosure = new Enclosure();
                    item.Enclosure.Url = entry.Attachments[0].Name;
                    item.Enclosure.Type = entry.Attachments[0].Type;
                    item.Enclosure.Length = entry.Attachments[0].Length.ToString();
                }
                item.PubDate = entry.CreatedUtc.ToString("R");
                if (ch.LastBuildDate == null || ch.LastBuildDate.Length == 0)
                {
                    ch.LastBuildDate = item.PubDate;
                }


                if (!_dasBlogSettings.SiteConfiguration.AlwaysIncludeContentInRSS &&
                    entry.Description != null &&
                    entry.Description.Trim().Length > 0)
                {
                    item.Description = PreprocessItemContent(entry.EntryId, entry.Description);

                }
                else
                {
                    if (_dasBlogSettings.SiteConfiguration.HtmlTidyContent == false)
                    {
                        item.Description = "<div>" + PreprocessItemContent(entry.EntryId, entry.Content) + "</div>";
                    }
                    else
                    {
                        item.Description = ContentFormatter.FormatContentAsHTML(PreprocessItemContent(entry.EntryId, entry.Content));


                        try
                        {
                            string xhtml = ContentFormatter.FormatContentAsXHTML(PreprocessItemContent(entry.EntryId, entry.Content));
                            doc2.LoadXml(xhtml);
                            anyElements.Add((XmlElement)doc2.SelectSingleNode("//*[local-name() = 'body'][namespace-uri()='http://www.w3.org/1999/xhtml']"));
                        }
                        catch //(Exception ex)
                        {
                            //Debug.Write(ex.ToString());
                            // absorb
                        }
                    }
                }

                item.anyElements = anyElements.ToArray();
                ch.Items.Add(item);
            }
            
            return documentRoot;
        }

        protected EntryCollection BuildEntries(string category, int maxDayCount, int maxEntryCount)
        {
            EntryCollection entryList = new EntryCollection();

            if (category != null)
            {
                int entryCount = _dasBlogSettings.SiteConfiguration.RssEntryCount;
                category = category.ToUpper();
                foreach (CategoryCacheEntry catEntry in _dataService.GetCategories())
                {
                    if (catEntry.Name.ToUpper() == category)
                    {
                        foreach (CategoryCacheEntryDetail detail in catEntry.EntryDetails)
                        {
                            Entry entry = _dataService.GetEntry(detail.EntryId);
                            if (entry != null)
                            {
                                entryList.Add(entry);
                                entryCount--;
                            }
                            if (entryCount <= 0) break;
                        } // foreach (CategoryCacheEntryDetail
                    }
                    if (entryCount <= 0) break;
                } // foreach (CategoryCacheEntry
            }
            else
            {
                entryList = _dataService.GetEntriesForDay(DateTime.Now.AddDays(_dasBlogSettings.SiteConfiguration.ContentLookaheadDays).ToUniversalTime(), new newtelligence.DasBlog.Util.UTCTimeZone(), null, maxDayCount, maxEntryCount, null);
            }
            entryList.Sort(new EntrySorter());
            return entryList;
        }

        protected string PreprocessItemContent(string entryId, string content)
        {
            if (_dasBlogSettings.SiteConfiguration.EnableRssItemFooters &&
                _dasBlogSettings.SiteConfiguration.RssItemFooter != null &&
                _dasBlogSettings.SiteConfiguration.RssItemFooter.Length > 0)
            {
                content = content + "<br/><hr/>" + _dasBlogSettings.SiteConfiguration.RssItemFooter;
            }

            return content;
        }

        public RsdRoot GetRsd()
        {
            RsdApiCollection apiCollection = new RsdApiCollection();

            UriBuilder home = new UriBuilder(_dasBlogSettings.GetBaseUrl());
            home.Path = "feed/blogger";
            string blogapiurl = home.ToString();

            RsdRoot rsd = new RsdRoot();
            RsdService dasBlogService = new RsdService();
            dasBlogService.HomePageLink = _dasBlogSettings.GetBaseUrl();

            RsdApi metaWeblog = new RsdApi();
            metaWeblog.Name = "MetaWeblog";
            metaWeblog.Preferred = (_dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == metaWeblog.Name);
            metaWeblog.ApiLink = blogapiurl;
            metaWeblog.BlogID = dasBlogService.HomePageLink;
            apiCollection.Add(metaWeblog);

            RsdApi blogger = new RsdApi();
            blogger.Name = "Blogger";
            blogger.Preferred = (_dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == blogger.Name);
            blogger.ApiLink = blogapiurl;
            blogger.BlogID = dasBlogService.HomePageLink;
            apiCollection.Add(blogger);

            RsdApi moveableType = new RsdApi();
            moveableType.Name = "Moveable Type";
            moveableType.Preferred = (_dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == moveableType.Name);
            moveableType.ApiLink = blogapiurl;
            moveableType.BlogID = dasBlogService.HomePageLink;
            apiCollection.Add(moveableType);

            dasBlogService.RsdApiCollection = apiCollection;
            rsd.Services.Add(dasBlogService);

            return rsd;
        }
    }

    public class EntrySorter : IComparer<Entry>
    {
        public int Compare(Entry left, Entry right)
        {
            return right.CreatedUtc.CompareTo(left.CreatedUtc);
        }
    }
}
