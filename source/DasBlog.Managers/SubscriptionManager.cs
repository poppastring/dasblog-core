using DasBlog.Core.Security;
using DasBlog.Services.Rss.Atom;
using DasBlog.Services.Rss.Rsd;
using DasBlog.Services.Rss.Rss20;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Runtime.Util.Html;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;
using DasBlog.Services;
using System.IO;

namespace DasBlog.Managers
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private readonly IBlogDataService dataService;
        private readonly ILoggingDataService loggingDataService;
        private readonly IDasBlogSettings dasBlogSettings;

        public SubscriptionManager(IDasBlogSettings settings, IBlogDataService dataService, ILoggingDataService loggingDataService)
        {
            dasBlogSettings = settings;
            this.dataService = dataService;
            this.loggingDataService = loggingDataService;
		}

        public RssRoot GetRss()
        {
            return GetRssCore(null,  dasBlogSettings.SiteConfiguration.RssDayCount, dasBlogSettings.SiteConfiguration.RssMainEntryCount);
        }

        public RssRoot GetRssCategory(string categoryName)
        {
            return GetRssCore(categoryName, 0, 0);
        }

        public AtomRoot GetAtom()
        {
            return GetAtomCore(null, dasBlogSettings.SiteConfiguration.RssDayCount, dasBlogSettings.SiteConfiguration.RssMainEntryCount);
        }

        public AtomRoot GetAtomCategory(string categoryName)
        {
            return GetAtomCore(categoryName, 0, 0);
        }

        private AtomRoot GetAtomCore(string category, int maxDayCount, int maxEntryCount)
        {
            var entries = BuildEntries(category, maxDayCount, maxEntryCount);
            var feed = new AtomRoot();

            // Set feed title
            if (category == null)
            {
                feed.Title = new AtomText(dasBlogSettings.SiteConfiguration.Title);
            }
            else
            {
                feed.Title = new AtomText(dasBlogSettings.SiteConfiguration.Title + " - " + category);
            }

            // Set feed subtitle/description
            if (!string.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.Description))
            {
                feed.Subtitle = new AtomText(dasBlogSettings.SiteConfiguration.Description);
            }
            else if (!string.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.Subtitle))
            {
                feed.Subtitle = new AtomText(dasBlogSettings.SiteConfiguration.Subtitle);
            }

            // Set feed ID and links
            feed.Id = dasBlogSettings.GetBaseUrl();
            feed.Links.Add(new AtomLink(dasBlogSettings.GetBaseUrl(), "alternate", "text/html"));
            feed.Links.Add(new AtomLink(dasBlogSettings.RelativeToRoot("feed/atom"), "self", "application/atom+xml"));

            // Set rights/copyright
            if (!string.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.Copyright))
            {
                feed.Rights = dasBlogSettings.SiteConfiguration.Copyright;
            }

            // Set logo/icon
            if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.ChannelImageUrl))
            {
                if (dasBlogSettings.SiteConfiguration.ChannelImageUrl.StartsWith("http"))
                {
                    feed.Logo = dasBlogSettings.SiteConfiguration.ChannelImageUrl;
                }
                else
                {
                    feed.Logo = dasBlogSettings.RelativeToRoot(dasBlogSettings.SiteConfiguration.ChannelImageUrl);
                }
            }

            // Set author
            feed.Author = new AtomPerson(dasBlogSettings.SiteConfiguration.Title, dasBlogSettings.SiteConfiguration.Contact);

            // Set updated time
            string lastUpdated = DateTime.UtcNow.ToString("o");

            foreach (var entry in entries)
            {
                if (entry.IsPublic == false || entry.Syndicated == false)
                {
                    continue;
                }

                var atomEntry = new AtomEntry();
                atomEntry.Title = new AtomText(entry.Title);
                atomEntry.Id = dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(entry));
                atomEntry.Published = entry.CreatedUtc.ToString("o");
                atomEntry.Updated = (entry.ModifiedUtc != DateTime.MinValue ? entry.ModifiedUtc : entry.CreatedUtc).ToString("o");

                // Update feed's last updated time based on first entry
                if (feed.Updated == null)
                {
                    feed.Updated = atomEntry.Updated;
                }

                // Set entry link
                atomEntry.Links.Add(new AtomLink(dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(entry)), "alternate", "text/html"));

                // Set author
                User user = dasBlogSettings.GetUserByEmail(entry.Author);
                if (user != null)
                {
                    atomEntry.Author = new AtomPerson(user.DisplayName, entry.Author);
                }
                else
                {
                    atomEntry.Author = new AtomPerson(entry.Author);
                }

                // Set categories
                if (entry.Categories != null && entry.Categories.Length > 0)
                {
                    string[] cats = entry.Categories.Split(';');
                    foreach (string c in cats)
                    {
                        string cleanCat = c.Replace('|', '/');
                        atomEntry.Categories.Add(new AtomCategory(cleanCat, cleanCat));
                    }
                }

                // Set content
                if (!dasBlogSettings.SiteConfiguration.AlwaysIncludeContentInRSS &&
                    entry.Description != null &&
                    entry.Description.Trim().Length > 0)
                {
                    atomEntry.Summary = new AtomText(PreprocessItemContent(entry.EntryId, entry.Description), "html");
                }
                else
                {
                    string content = PreprocessItemContent(entry.EntryId, entry.Content);
                    if (dasBlogSettings.SiteConfiguration.HtmlTidyContent)
                    {
                        content = ContentFormatter.FormatContentAsHTML(content);
                    }
                    atomEntry.Content = new AtomContent(content, "html");
                }

                // Handle enclosures/attachments
                if (entry.Attachments.Count > 0)
                {
                    var attachment = entry.Attachments[0];
                    atomEntry.Links.Add(new AtomLink(attachment.Name, "enclosure", attachment.Type) 
                    { 
                        Length = attachment.Length.ToString() 
                    });
                }

                feed.Entries.Add(atomEntry);
            }

            // If no entries, set updated to now
            if (feed.Updated == null)
            {
                feed.Updated = lastUpdated;
            }

            return feed;
        }

        private RssRoot GetRssCore(string category, int maxDayCount, int maxEntryCount)
        {
            EntryCollection entries = null;
            //We only build the entries if blogcore doesn't exist and we'll need them later...
            if (dataService.GetLastEntryUpdate() == DateTime.MinValue)
            {
                entries = BuildEntries(category, maxDayCount, maxEntryCount);
            }

            var documentRoot = new RssRoot(); ;

            //However, if we made it this far, the not-modified check didn't work, and we may not have entries...
            if (entries == null)
            {
                entries = BuildEntries(category, maxDayCount, maxEntryCount);
            }

            documentRoot.Namespaces.Add("dc", "http://purl.org/dc/elements/1.1/");
            documentRoot.Namespaces.Add("trackback", "http://madskills.com/public/xml/rss/module/trackback/");
            documentRoot.Namespaces.Add("pingback", "http://madskills.com/public/xml/rss/module/pingback/");
			documentRoot.Namespaces.Add("webfeeds", "http://webfeeds.org/rss/1.0");
			if (dasBlogSettings.SiteConfiguration.EnableComments)
            {
                documentRoot.Namespaces.Add("wfw", "http://wellformedweb.org/CommentAPI/");
                documentRoot.Namespaces.Add("slash", "http://purl.org/rss/1.0/modules/slash/");
            }
            if (dasBlogSettings.SiteConfiguration.EnableGeoRss)
            {
                documentRoot.Namespaces.Add("georss", "http://www.georss.org/georss");
            }

            var ch = new RssChannel();

            if (category == null)
            {
                ch.Title = dasBlogSettings.SiteConfiguration.Title;
            }
            else
            {
                ch.Title = dasBlogSettings.SiteConfiguration.Title + " - " + category;
            }

            if (string.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.Description))
            {
                ch.Description = dasBlogSettings.SiteConfiguration.Subtitle;
            }
            else
            {
                ch.Description = dasBlogSettings.SiteConfiguration.Description;
            }

            ch.Link = dasBlogSettings.GetBaseUrl();
            ch.Copyright = dasBlogSettings.SiteConfiguration.Copyright;
            if (!string.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.RssLanguage))
            {
                ch.Language = dasBlogSettings.SiteConfiguration.RssLanguage;
            }

            ch.ManagingEditor = dasBlogSettings.SiteConfiguration.Contact;
            ch.WebMaster = dasBlogSettings.SiteConfiguration.Contact;
            ch.Image = null;

            if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.ChannelImageUrl))
            {
				var channelImage = new DasBlog.Services.Rss.Rss20.ChannelImage();
                channelImage.Title = ch.Title;
                channelImage.Link = ch.Link;
                if (dasBlogSettings.SiteConfiguration.ChannelImageUrl.StartsWith("http"))
                {
                    channelImage.Url = dasBlogSettings.SiteConfiguration.ChannelImageUrl;
                }
                else
                {
                    channelImage.Url = dasBlogSettings.RelativeToRoot(dasBlogSettings.SiteConfiguration.ChannelImageUrl);
                }
                ch.Image = channelImage;
            }

			var xdoc = new XmlDocument();
			var rootElements = new List<XmlElement>();

			var wflogo = xdoc.CreateElement("webfeeds", "logo", "http://webfeeds.org/rss/1.0");
			wflogo.InnerText = dasBlogSettings.RelativeToRoot(dasBlogSettings.SiteConfiguration.ChannelImageUrl);
			rootElements.Add(wflogo);

			var wfanalytics = xdoc.CreateElement("webfeeds", "analytics", "http://webfeeds.org/rss/1.0");
			var attribId = xdoc.CreateAttribute("id");
			attribId.Value = dasBlogSettings.MetaTags.GoogleAnalyticsID;
			wfanalytics.Attributes.Append(attribId);

			var attribEngine = xdoc.CreateAttribute("engine");
			attribEngine.Value = "GoogleAnalytics";
			wfanalytics.Attributes.Append(attribEngine);

			rootElements.Add(wfanalytics);

			ch.anyElements = rootElements.ToArray();

			ch.Items = new RssItemCollection();
			documentRoot.Channels.Add(ch);

			foreach (var entry in entries)
            {
                if (entry.IsPublic == false || entry.Syndicated == false)
                {
                    continue;
                }
                var doc2 = new XmlDocument();
                var anyElements = new List<XmlElement>();
                var item = new RssItem();
                item.Title = entry.Title;
                item.Guid = new DasBlog.Services.Rss.Rss20.Guid();
                item.Guid.IsPermaLink = false;
                item.Guid.Text = dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
                item.Link = dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(entry));
                User user = dasBlogSettings.GetUserByEmail(entry.Author);

                XmlElement trackbackPing = doc2.CreateElement("trackback", "ping", "http://madskills.com/public/xml/rss/module/trackback/");
                trackbackPing.InnerText = dasBlogSettings.GetTrackbackUrl(entry.EntryId);
                anyElements.Add(trackbackPing);

                XmlElement pingbackServer = doc2.CreateElement("pingback", "server", "http://madskills.com/public/xml/rss/module/pingback/");
                pingbackServer.InnerText = dasBlogSettings.PingBackUrl;
                anyElements.Add(pingbackServer);

                XmlElement pingbackTarget = doc2.CreateElement("pingback", "target", "http://madskills.com/public/xml/rss/module/pingback/");
                pingbackTarget.InnerText = dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
                anyElements.Add(pingbackTarget);

				XmlElement dcCreator = doc2.CreateElement("dc", "creator", "http://purl.org/dc/elements/1.1/");
                if (user != null)
                {
                    dcCreator.InnerText = user.DisplayName;
                }
                anyElements.Add(dcCreator);

                // Add GeoRSS if it exists.
                if (dasBlogSettings.SiteConfiguration.EnableGeoRss)
                {
                    var latitude = new Nullable<double>();
                    var longitude = new Nullable<double>();

                    if (entry.Latitude.HasValue)
                    {
                        latitude = entry.Latitude;
                    }
                    else
                    {
                        if (dasBlogSettings.SiteConfiguration.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            latitude = dasBlogSettings.SiteConfiguration.DefaultLatitude;
                        }
                    }

                    if (entry.Longitude.HasValue)
                    {
                        longitude = entry.Longitude;
                    }
                    else
                    {
                        if (dasBlogSettings.SiteConfiguration.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            longitude = dasBlogSettings.SiteConfiguration.DefaultLongitude;
                        }
                    }

                    if (latitude.HasValue && longitude.HasValue)
                    {
                        XmlElement geoLoc = doc2.CreateElement("georss", "point", "http://www.georss.org/georss");
                        geoLoc.InnerText = String.Format(CultureInfo.InvariantCulture, "{0:R} {1:R}", latitude, longitude);
                        anyElements.Add(geoLoc);
                    }
                }

                if (dasBlogSettings.SiteConfiguration.EnableComments)
                {
                    if (entry.AllowComments)
                    {
                        XmlElement commentApi = doc2.CreateElement("wfw", "comment", "http://wellformedweb.org/CommentAPI/");
                        commentApi.InnerText = dasBlogSettings.GetCommentViewUrl(dasBlogSettings.GeneratePostUrl(entry));
                        anyElements.Add(commentApi);
                    }

                    XmlElement commentRss = doc2.CreateElement("wfw", "commentRss", "http://wellformedweb.org/CommentAPI/");
					commentRss.InnerText = dasBlogSettings.GetEntryCommentsRssUrl(entry.EntryId);
					anyElements.Add(commentRss);

                    //for RSS conformance per FeedValidator.org
                    int commentsCount = dataService.GetPublicCommentsFor(entry.EntryId).Count;
                    if (commentsCount > 0)
                    {
                        XmlElement slashComments = doc2.CreateElement("slash", "comments", "http://purl.org/rss/1.0/modules/slash/");
                        slashComments.InnerText = commentsCount.ToString();
                        anyElements.Add(slashComments);
                    }
                    item.Comments = dasBlogSettings.GetCommentViewUrl(dasBlogSettings.GeneratePostUrl(entry));
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

                if (!dasBlogSettings.SiteConfiguration.AlwaysIncludeContentInRSS &&
                    entry.Description != null &&
                    entry.Description.Trim().Length > 0)
                {
                    item.Description = PreprocessItemContent(entry.EntryId, entry.Description);

                }
                else
                {
                    if (dasBlogSettings.SiteConfiguration.HtmlTidyContent == false)
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
            var entryList = new EntryCollection();

            if (category != null)
            {
                int entryCount = dasBlogSettings.SiteConfiguration.RssEntryCount;
                category = category.Replace("-", " ");
                foreach (var catEntry in dataService.GetCategories())
                {
					if (string.Compare(catEntry.Name, category, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase | CompareOptions.IgnoreSymbols) == 0)
                    {
                        foreach (CategoryCacheEntryDetail detail in catEntry.EntryDetails)
                        {
                            Entry entry = dataService.GetEntry(detail.EntryId);
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
                entryList = dataService.GetEntriesForDay(dasBlogSettings.GetContentLookAhead(), dasBlogSettings.GetConfiguredTimeZone(), null, maxDayCount, maxEntryCount, null);
            }
            entryList.Sort(new EntrySorter());
            return entryList;
        }

        protected string PreprocessItemContent(string entryId, string content)
        {
            if (dasBlogSettings.SiteConfiguration.EnableRssItemFooters &&
                dasBlogSettings.SiteConfiguration.RssItemFooter != null &&
                dasBlogSettings.SiteConfiguration.RssItemFooter.Length > 0)
            {
                content = content + "<br/><hr/>" + dasBlogSettings.SiteConfiguration.RssItemFooter;
            }

            return content;
        }

        public RsdRoot GetRsd()
        {
            var apiCollection = new RsdApiCollection();

            // UriBuilder home = new UriBuilder(dasBlogSettings.RelativeToRoot("feed/blogger"));
            var blogapiurl = dasBlogSettings.RelativeToRoot("feed/blogger");

            var rsd = new RsdRoot();
            var dasBlogService = new RsdService();
            dasBlogService.HomePageLink = dasBlogSettings.GetBaseUrl();

            var metaWeblog = new RsdApi();
            metaWeblog.Name = "MetaWeblog";
            metaWeblog.Preferred = (dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == metaWeblog.Name);
            metaWeblog.ApiLink = blogapiurl;
            metaWeblog.BlogID = dasBlogService.HomePageLink;
            apiCollection.Add(metaWeblog);

            var blogger = new RsdApi();
            blogger.Name = "Blogger";
            blogger.Preferred = (dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == blogger.Name);
            blogger.ApiLink = blogapiurl;
            blogger.BlogID = dasBlogService.HomePageLink;
            apiCollection.Add(blogger);

            var moveableType = new RsdApi();
            moveableType.Name = "Moveable Type";
            moveableType.Preferred = (dasBlogSettings.SiteConfiguration.PreferredBloggingAPI == moveableType.Name);
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
