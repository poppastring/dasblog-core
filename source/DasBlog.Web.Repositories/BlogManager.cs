using System;
using newtelligence.DasBlog.Runtime;
using DasBlog.Managers.Interfaces;
using DasBlog.Core;
//using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Web.Services.MetaWeblog;
using EventDataItem = DasBlog.Core.EventDataItem;
using EventCodes = DasBlog.Core.EventCodes;
using DasBlog.Core.Extensions;

namespace DasBlog.Managers
{
	public class BlogManager : IBlogManager
	{
		private IBlogDataService _dataService;
		private ILoggingDataService _loggingDataService;
		private ISiteSecurityManager _siteSecurity;
		private readonly IDasBlogSettings _dasBlogSettings;
		private Microsoft.Extensions.Logging.ILogger _logger;

		public BlogManager(IDasBlogSettings settings , Microsoft.Extensions.Logging.ILogger<BlogManager> logger)
		{
			_dasBlogSettings = settings;
			_loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
			_dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
			_logger = logger;
		}

		public Entry GetBlogPost(string postid)
		{
			return _dataService.GetEntry(postid);
		}

		public Entry GetEntryForEdit(string postid)
		{
			return _dataService.GetEntryForEdit(postid);
		}

		public EntryCollection GetFrontPagePosts(string acceptLanguageHeader)
		{
			DateTime fpDayUtc;
			TimeZone tz;

			//Need to insert the Request.Headers["Accept-Language"];
			string languageFilter = acceptLanguageHeader;
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

		public EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader)
		{
			Predicate<Entry> pred = null;

			//Shallow copy as we're going to modify it...and we don't want to modify THE cache.
			EntryCollection cache = _dataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

			// remove the posts from the front page
			EntryCollection fp = GetFrontPagePosts(acceptLanguageHeader);

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

		public EntrySaveState CreateEntry(Entry entry)
		{
			var rtn = InternalSaveEntry(entry, null, null);
			LogEvent(EventCodes.EntryAdded, entry);
			return rtn;
		}

		public EntrySaveState UpdateEntry(Entry entry)
		{
			var rtn = InternalSaveEntry(entry, null, null);
			LogEvent(EventCodes.EntryChanged, entry);
			return rtn;
		}

		public void DeleteEntry(string postid)
		{
			Entry entry = GetEntryForEdit(postid);
			_dataService.DeleteEntry(postid, null);
			LogEvent(EventCodes.EntryDeleted, entry);
		}

		private void LogEvent(EventCodes eventCode, Entry entry)
		{
			_logger.LogInformation(
				new EventDataItem(
					eventCode,
					MakePermaLinkFromCompressedTitle(entry), entry.Title));
		}

		private string MakePermaLink(Entry entry)
		{
			return new Uri(new Uri(_dasBlogSettings.SiteConfiguration.Root)
				,_dasBlogSettings.RelativeToRoot(entry.EntryId)).ToString();
		}

		private string MakePermaLinkFromCompressedTitle(Entry entry)
		{
			return new Uri(new Uri(_dasBlogSettings.SiteConfiguration.Root)
				,_dasBlogSettings.RelativeToRoot(
				_dasBlogSettings.GetPermaTitle(entry.CompressedTitle))).ToString();
		}

		private EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList)
		{

			EntrySaveState rtn = EntrySaveState.Failed;
			// we want to prepopulate the cross post collection with the crosspost footer
			if (_dasBlogSettings.SiteConfiguration.EnableCrossPostFooter && _dasBlogSettings.SiteConfiguration.CrossPostFooter != null 
				&& _dasBlogSettings.SiteConfiguration.CrossPostFooter.Length > 0)
			{
				foreach (CrosspostInfo info in crosspostList)
				{
					info.CrossPostFooter = _dasBlogSettings.SiteConfiguration.CrossPostFooter;
				}
			}

			// now save the entry, passign in all the necessary Trackback and Pingback info.
			try
			{
				// if the post is missing a title don't publish it
				if (entry.Title == null || entry.Title.Length == 0)
				{
					entry.IsPublic = false;
				}

				// if the post is missing categories, then set the categories to empty string.
				if (entry.Categories == null)
					entry.Categories = "";

				rtn = _dataService.SaveEntry(entry, 
					(_dasBlogSettings.SiteConfiguration.PingServices.Count > 0) ?
						new WeblogUpdatePingInfo(_dasBlogSettings.SiteConfiguration.Title, _dasBlogSettings.GetBaseUrl(), _dasBlogSettings.GetBaseUrl(), _dasBlogSettings.RsdUrl, _dasBlogSettings.SiteConfiguration.PingServices) : null,
					(entry.IsPublic) ?
						trackbackList : null,
					_dasBlogSettings.SiteConfiguration.EnableAutoPingback && entry.IsPublic ?
						new PingbackInfo(
							_dasBlogSettings.GetPermaLinkUrl(entry.EntryId),
							entry.Title,
							entry.Description,
							_dasBlogSettings.SiteConfiguration.Title) : null,
					crosspostList);

				//TODO: SendEmail(entry, siteConfig, logService);

			}
			catch (Exception ex)
			{
				//TODO: Do something with this????
				// StackTrace st = new StackTrace();
				// logService.AddEvent(new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), ""));
			}

			// we want to invalidate all the caches so users get the new post
			// TODO: BreakCache(entry.GetSplitCategories());

			return rtn;
		}

		private void BreakCache(string[] categories)
		{
			newtelligence.DasBlog.Web.Core.DataCache cache = newtelligence.DasBlog.Web.Core.CacheFactory.GetCache();

			// break the caching
			cache.Remove("BlogCoreData");
			cache.Remove("Rss::" + _dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + _dasBlogSettings.SiteConfiguration.RssEntryCount.ToString());

			foreach (string category in categories)
			{
				string CacheKey = "Rss:" + category + ":" + _dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + _dasBlogSettings.SiteConfiguration.RssEntryCount.ToString();
				cache.Remove(CacheKey);
			}
		}

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = _dataService.GetEntry(postid);

			if (entry != null)
			{
				// Are comments allowed

				_dataService.AddComment(comment);

				est = CommentSaveState.Added;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentSaveState DeleteComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = _dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				_dataService.DeleteComment(postid, commentid);

				est = CommentSaveState.Deleted;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentSaveState ApproveComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;
			Entry entry = _dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				_dataService.ApproveComment(postid, commentid);

				est = CommentSaveState.Approved;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentCollection GetComments(string postid, bool allComments)
		{
			return _dataService.GetCommentsFor(postid, allComments);
		}


		public CategoryCacheEntryCollection GetCategories()
		{
			return _dataService.GetCategories();
		}
	}
}
