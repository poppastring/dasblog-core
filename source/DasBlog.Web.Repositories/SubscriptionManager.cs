using DasBlog.Core.Security;
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

        public SubscriptionManager(IDasBlogSettings settings)
        {
            dasBlogSettings = settings;

			loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);
		}

        public RssRoot GetRss()
        {
            return GetRssCore(null,  dasBlogSettings.SiteConfiguration.RssDayCount, dasBlogSettings.SiteConfiguration.RssMainEntryCount);
        }

		public RssItem GetRssItem(string entryId)
		{
			Entry entry = dataService.GetEntry(entryId);
			if (entry != null)
			{
				return MapEntryToRssItem(entry);
			}
			return null;			
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
				var item = MapEntryToRssItem(entry, ch, true);
				ch.Items.Add(item);
			}

			return documentRoot;
        }

		private RssItem MapEntryToRssItem(Entry entry, RssChannel ch = null, bool feedView = false)
		{
			var doc2 = new XmlDocument();
			var anyElements = new List<XmlElement>();
			var item = new RssItem();
			item.Id = entry.EntryId;
			item.Title = entry.Title;
			item.Guid = new DasBlog.Services.Rss.Rss20.Guid();
			item.Guid.IsPermaLink = false;
			item.Guid.Text = dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
			item.Link = dasBlogSettings.RelativeToRoot(dasBlogSettings.GeneratePostUrl(entry));
			User user = dasBlogSettings.GetUserByEmail(entry.Author);

			if (dasBlogSettings.SiteConfiguration.EnableTrackbackService)
			{
				XmlElement trackbackPing = doc2.CreateElement("trackback", "ping", "http://madskills.com/public/xml/rss/module/trackback/");
				trackbackPing.InnerText = dasBlogSettings.GetTrackbackUrl(entry.EntryId);
				anyElements.Add(trackbackPing);
			}

			if (dasBlogSettings.SiteConfiguration.EnablePingbackService)
			{
				XmlElement pingbackServer = doc2.CreateElement("pingback", "server", "http://madskills.com/public/xml/rss/module/pingback/");
				pingbackServer.InnerText = dasBlogSettings.PingBackUrl;
				anyElements.Add(pingbackServer);

				XmlElement pingbackTarget = doc2.CreateElement("pingback", "target", "http://madskills.com/public/xml/rss/module/pingback/");
				pingbackTarget.InnerText = dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
				anyElements.Add(pingbackTarget);
			}

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
			if (ch != null && (ch.LastBuildDate == null || ch.LastBuildDate.Length == 0))
			{
				ch.LastBuildDate = item.PubDate;
			}

			if (feedView || !dasBlogSettings.SiteConfiguration.AlwaysIncludeContentInRSS &&
				entry.Description != null &&
				entry.Description.Trim().Length > 0)
			{
				if (feedView)
				{
					item.Description = PreprocessItemContent(entry.EntryId, entry.Description);
				}
				else
				{
					item.Description = entry.Description;
				}
			}
			else
			{
				var content = (feedView ? PreprocessItemContent(entry.EntryId, entry.Content) : entry.Content);
				if (!dasBlogSettings.SiteConfiguration.HtmlTidyContent && feedView)
				{
					item.Description = "<div>" + content + "</div>";
				}
				else
				{
					item.Description = ContentFormatter.FormatContentAsHTML(!string.IsNullOrEmpty(entry.Description)?entry.Description:content);
					try
					{
						string xhtml = ContentFormatter.FormatContentAsXHTML(System.Net.WebUtility.HtmlDecode(content));
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
			return item;
		}

		protected EntryCollection BuildEntries(string category, int maxDayCount, int maxEntryCount)
        {
            var entryList = new EntryCollection();

            if (category != null)
            {
                int entryCount = dasBlogSettings.SiteConfiguration.RssEntryCount;
                category = category.Replace(dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement, " ");
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
