using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using newtelligence.DasBlog.Runtime;
using DasBlog.Managers.Interfaces;
using DasBlog.Core;
using EventDataItem = DasBlog.Core.EventDataItem;
using EventCodes = DasBlog.Core.EventCodes;
using DasBlog.Core.Extensions;
using DasBlog.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.CompilerServices;
using NodaTime;

namespace DasBlog.Managers
{
	public class BlogManager : IBlogManager
	{
		private class Options
		{
			private BlogManagerOptions staticOptions;
			private IOptionsMonitor<BlogManagerModifiableOptions> monitoredOptionsAccessor;
			private BlogManagerExtraOptions extraOptions;

			internal string ContentDir => staticOptions.ContentDir;
			internal bool EnableAutoPingBack => staticOptions.EnableAutoPingback;
			internal bool EnableTitlePermaLinkUnique => staticOptions.EnableTitlePermaLinkUnique;
			internal string LogDir => staticOptions.LogDir;
			internal string Root => staticOptions.Root;
			internal string Title => staticOptions.Title;
			internal string TitlePermalinkSpaceReplacement => staticOptions.TitlePermalinkSpaceReplacement;

			internal bool AdjustDisplayTimeZone => monitoredOptionsAccessor.CurrentValue.AdjustDisplayTimeZone;
			internal int ContentLookaheadDays => monitoredOptionsAccessor.CurrentValue.ContentLookaheadDays;
			internal string CrossPostFooter => monitoredOptionsAccessor.CurrentValue.CrossPostFooter;
			internal int DaysCommentsAllowed => monitoredOptionsAccessor.CurrentValue.DaysCommentsAllowed;
			internal decimal DisplayTimeZoneIndex => monitoredOptionsAccessor.CurrentValue.DisplayTimeZoneIndex;
			internal bool EnableCommentDays => monitoredOptionsAccessor.CurrentValue.EnableCommentDays;
			internal bool EnableComments => monitoredOptionsAccessor.CurrentValue.EnableComments;
			internal bool EnableCrossPostFooter => monitoredOptionsAccessor.CurrentValue.EnableCrossPostFooter;
			internal int EntriesPerPage => monitoredOptionsAccessor.CurrentValue.EntriesPerPage;
			internal int FrontPageEntryCount => monitoredOptionsAccessor.CurrentValue.FrontPageEntryCount;
			internal int RssDayCount => monitoredOptionsAccessor.CurrentValue.RssDayCount;
			internal int RssEntryCount => monitoredOptionsAccessor.CurrentValue.RssEntryCount;

			internal string WebRootDirectory => extraOptions.ContentRootPath;		

			public Options(
				IOptions<BlogManagerOptions> settingsOptionsAccessor
				,IOptionsMonitor<BlogManagerModifiableOptions> monitoredOptionsAccessor
				,IOptions<BlogManagerExtraOptions> extraOptionsAccessor
			)
			{
				staticOptions = settingsOptionsAccessor.Value;
				extraOptions = extraOptionsAccessor.Value;
				this.monitoredOptionsAccessor = monitoredOptionsAccessor;
			}
		}
		private readonly IBlogDataService dataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ILogger logger;
		private static Regex stripTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private Options opts;

		public BlogManager(IDasBlogSettings settings , ILogger<BlogManager> logger
		  ,IOptions<BlogManagerOptions> settingsOptionsAccessor
		  ,IOptionsMonitor<BlogManagerModifiableOptions> monitoredOptionsAccessor
		  ,IOptions<BlogManagerExtraOptions> extraOptionsAccessor
			)
		{
			opts = new Options(settingsOptionsAccessor, monitoredOptionsAccessor, extraOptionsAccessor);
			this.logger = logger;
			dasBlogSettings = settings;
			var loggingDataService = LoggingDataServiceFactory.GetService(Pass(() => dasBlogSettings.WebRootDirectory, () => opts.WebRootDirectory) 
			  + Pass(() => dasBlogSettings.SiteConfiguration.LogDir, () => opts.LogDir));
			dataService = BlogDataServiceFactory.GetService(Pass(() => dasBlogSettings.WebRootDirectory, () => opts.WebRootDirectory) 
			  + Pass(() => dasBlogSettings.SiteConfiguration.ContentDir,() => opts.ContentDir), loggingDataService);
		}
		/// <param name="dt">if non-null then the post must be dated on that date</param>
		public Entry GetBlogPost(string postid, DateTime? dt)
		{
			if (dt == null)
			{
				return dataService.GetEntry(postid);
			}
			else
			{
				EntryCollection entries = dataService.GetEntriesForDay(dt.Value, null, null, 1, 10, null);
				return entries.FirstOrDefault(e => 
				  Pass(() => dasBlogSettings.GetPermaTitle(e.CompressedTitle), () => SettingsUtils.GetPermaTitle(e.CompressedTitle, opts.TitlePermalinkSpaceReplacement))
				  .Replace(Pass(() => dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement, () => opts.TitlePermalinkSpaceReplacement), string.Empty)
				  == postid);
			}
		}

		public Entry GetEntryForEdit(string postid)
		{
			return dataService.GetEntryForEdit(postid);
		}

		public EntryCollection GetFrontPagePosts(string acceptLanguageHeader)
		{			
			return dataService.GetEntriesForDay(Pass(() => dasBlogSettings.GetContentLookAhead(), () => SettingsUtils.GetContentLookAhead(opts.ContentLookaheadDays), AreStringsEqual)
				, Pass(() =>dasBlogSettings.GetConfiguredTimeZone(), () => SettingsUtils.GetConfiguredTimeZone(opts.AdjustDisplayTimeZone, opts.DisplayTimeZoneIndex)),
								acceptLanguageHeader, Pass(() =>dasBlogSettings.SiteConfiguration.FrontPageDayCount, () => opts.FrontPageEntryCount), 
				Pass(() => dasBlogSettings.SiteConfiguration.FrontPageEntryCount, () => opts.FrontPageEntryCount), string.Empty);
		}

		public EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader)
		{
			Predicate<Entry> pred = null;

			//Shallow copy as we're going to modify it...and we don't want to modify THE cache.
			EntryCollection cache = dataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

			// remove the posts from the front page
			EntryCollection fp = GetFrontPagePosts(acceptLanguageHeader);

			cache.RemoveRange(0, fp.Count);

			int entriesPerPage = Pass(() => dasBlogSettings.SiteConfiguration.EntriesPerPage, () => opts.EntriesPerPage);

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

				return dataService.GetEntries(null, EntryCollectionFilter.DefaultFilters.IsInEntryIdCacheEntryCollection(cache),
					Int32.MaxValue,
					Int32.MaxValue);
			}

			return new EntryCollection();
		}

		
		public EntryCollection SearchEntries(string searchString, string acceptLanguageHeader)
		{
			var searchWords = GetSearchWords(searchString);

			var entries = dataService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), 
				Pass(() => dasBlogSettings.GetConfiguredTimeZone(),() => SettingsUtils.GetConfiguredTimeZone(opts.AdjustDisplayTimeZone, opts.DisplayTimeZoneIndex)), 
				acceptLanguageHeader,
				int.MaxValue, 
				int.MaxValue, 
				null);

			// no search term provided, return all the results
			if (searchWords.Count == 0) return entries;

			EntryCollection matchEntries = new EntryCollection();

			foreach (Entry entry in entries)
			{
				string entryTitle = entry.Title;
				string entryDescription = entry.Description;
				string entryContent = entry.Content;

				foreach (string searchWord in searchWords)
				{
					if (entryTitle != null)
					{
						if (searchEntryForWord(entryTitle, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
					if (entryDescription != null)
					{
						if (searchEntryForWord(entryDescription, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
					if (entryContent != null)
					{
						if (searchEntryForWord(entryContent, searchWord))
						{
							if (!matchEntries.Contains(entry))
							{
								matchEntries.Add(entry);
							}
							continue;
						}
					}
				}
			}

			// log the search to the event log
			/*
						ILoggingDataService logService = requestPage.LoggingService;
						string referrer = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri : Request.ServerVariables["REMOTE_ADDR"];	
						logger.LogInformation(
							new EventDataItem(EventCodes.Search, String.Format("{0}", searchString), referrer));
			*/

			return matchEntries;
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
			var entry = GetEntryForEdit(postid);
			dataService.DeleteEntry(postid, null);
			// BreakCache(entry.GetSplitCategories());

			LogEvent(EventCodes.EntryDeleted, entry);
		}

		private static StringCollection GetSearchWords(string searchString)
		{
			var searchWords = new StringCollection();

			if (string.IsNullOrWhiteSpace(searchString))
				return searchWords;

			string[] splitString = Regex.Split(searchString, @"(""[^""]*"")", RegexOptions.IgnoreCase |
				RegexOptions.Compiled);

			for (int index = 0; index < splitString.Length; index++)
			{
				if (splitString[index] != "")
				{
					if (index == splitString.Length - 1)
					{
						foreach (string s in splitString[index].Split(' '))
						{
							if (s != "") searchWords.Add(s);
						}
					}
					else
					{
						searchWords.Add(splitString[index].Substring(1, splitString[index].Length - 2));
					}
				}
			}

			return searchWords;
		}

		private bool searchEntryForWord(string sourceText, string searchWord)
		{
			// Remove any tags from sourceText.
			sourceText = stripTags.Replace(sourceText, String.Empty);

			CompareInfo myComp = CultureInfo.InvariantCulture.CompareInfo;
			return (myComp.IndexOf(sourceText, searchWord, CompareOptions.IgnoreCase) >= 0);
		}

		private void LogEvent(EventCodes eventCode, Entry entry)
		{
			logger.LogInformation(
				new EventDataItem(
					eventCode,
					MakePermaLinkFromCompressedTitle(entry), entry.Title));
		}

		private Uri MakePermaLinkFromCompressedTitle(Entry entry)
		{
			if (Pass(() => dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique, () => opts.EnableTitlePermaLinkUnique))
			{
				return Pass(() => 
					new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root)
					, dasBlogSettings.RelativeToRoot(
						entry.CreatedUtc.ToString("yyyyMMdd") + "/" +
						dasBlogSettings.GetPermaTitle(entry.CompressedTitle)))
				  ,() =>
						new Uri(new Uri(opts.Root)
							, SettingsUtils.RelativeToRoot(
								entry.CreatedUtc.ToString("yyyyMMdd") + "/" +
								SettingsUtils.GetPermaTitle(entry.CompressedTitle, opts.TitlePermalinkSpaceReplacement)
								, opts.Root))
				);
			}
			else
			{
				return Pass( 
				  () =>
					new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root)
					,dasBlogSettings.RelativeToRoot(
						dasBlogSettings.GetPermaTitle(entry.CompressedTitle)))
				  ,() =>
					  new Uri(new Uri(opts.Root)
						  ,SettingsUtils.RelativeToRoot(
							SettingsUtils.GetPermaTitle(entry.CompressedTitle,opts.TitlePermalinkSpaceReplacement), opts.Root))
					);
			}
		}

		private EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList)
		{

			EntrySaveState rtn = EntrySaveState.Failed;
			// we want to prepopulate the cross post collection with the crosspost footer
			if (Pass(() => dasBlogSettings.SiteConfiguration.EnableCrossPostFooter,() => opts.EnableCrossPostFooter) && Pass(() => dasBlogSettings.SiteConfiguration.CrossPostFooter, () => opts.CrossPostFooter) != null 
				&& Pass(() =>dasBlogSettings.SiteConfiguration.CrossPostFooter.Length, () => opts.CrossPostFooter.Length) > 0)
			{
				foreach (CrosspostInfo info in crosspostList)
				{
					info.CrossPostFooter = Pass(() => dasBlogSettings.SiteConfiguration.CrossPostFooter, () => opts.CrossPostFooter);
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

				rtn = dataService.SaveEntry(
					entry, 
					MaybeBuildWeblogPingInfo(),
					entry.IsPublic
						? trackbackList 
						: null,
					MaybeBuildPingbackInfo(entry),
					crosspostList);

				//TODO: SendEmail(entry, siteConfig, logService);

			}
			catch (Exception ex)
			{
				//TODO: Do something with this????
				// StackTrace st = new StackTrace();
				// logService.AddEvent(new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), ""));

				LoggedException le = new LoggedException("file failure", ex);
				var edi = new EventDataItem(EventCodes.Error, null
				  , "Failed to Save a Post on {date}", System.DateTime.Now.ToShortDateString());
				logger.LogError(edi,le);
			}

			// we want to invalidate all the caches so users get the new post
			// BreakCache(entry.GetSplitCategories());

			return rtn;
		}

		/// <summary>
		/// not sure what this is about but it is legacy
		/// TODO: reconsider when strategy for handling pingback in legacy site.config is decided.
		/// </summary>
		private WeblogUpdatePingInfo MaybeBuildWeblogPingInfo()
		{
			var fakePingServices = new PingServiceCollection
			{
				new PingService
				{
					Endpoint = "http://ping.feedburner.com"
					,Name = "FeedBurner"
					,Url = "http://www.feedburner.com"
					,PingApi = PingService.PingApiType.Basic
				}
			};
			return Pass<PingServiceCollection>(
				() => dasBlogSettings.SiteConfiguration.PingServices
				,() => fakePingServices
				, (a, b) => ArePingServiceCollectionsEqual((PingServiceCollection)a,(PingServiceCollection)b)
				).Count > 0
				? new WeblogUpdatePingInfo(
					Pass(() => dasBlogSettings.SiteConfiguration.Title, () => opts.Title), 
					Pass(() => dasBlogSettings.GetBaseUrl(), () => SettingsUtils.GetBaseUrl(opts.Root)), 
					Pass(() => dasBlogSettings.GetBaseUrl(), () => SettingsUtils.GetBaseUrl(opts.Root)),
					Pass(() => dasBlogSettings.RsdUrl, () => SettingsUtils.RelativeToRoot("feed/rsd", opts.Root)), 
					Pass<PingServiceCollection>(
						() => dasBlogSettings.SiteConfiguration.PingServices
						,() => fakePingServices
						,(a, b) => ArePingServiceCollectionsEqual((PingServiceCollection)a,(PingServiceCollection)b)
					)
				) 
				: null;
		}

		/// <summary>
		/// not sure what this is about but it is legacy
		/// TODO: reconsider when strategy for handling pingback in legacy site.config is decided.
		/// </summary>
		private PingbackInfo MaybeBuildPingbackInfo(Entry entry)
		{
			return Pass(() =>dasBlogSettings.SiteConfiguration.EnableAutoPingback, () => opts.EnableAutoPingBack) && entry.IsPublic
				? new PingbackInfo(
					Pass(() => dasBlogSettings.GetPermaLinkUrl(entry.EntryId),() => SettingsUtils.GetPermaLinkUrl(entry.EntryId, opts.Root)),
					entry.Title,
					entry.Description,
					Pass(() => dasBlogSettings.SiteConfiguration.Title, () => opts.Title)) 
				: null;
		}

#if !POSIX
		private void BreakCache(string[] categories)
		{
			var cache = newtelligence.DasBlog.Web.Core.CacheFactory.GetCache();

			// break the caching
			cache.Remove("BlogCoreData");
			cache.Remove("Rss::" + Pass(() =>dasBlogSettings.SiteConfiguration.RssDayCount.ToString(), () => opts.RssDayCount.ToString()) + ":" + Pass(() => dasBlogSettings.SiteConfiguration.RssEntryCount.ToString(), () => opts.RssEntryCount.ToString()));

			foreach (string category in categories)
			{
				string CacheKey = "Rss:" + category + ":" + Pass(() =>dasBlogSettings.SiteConfiguration.RssDayCount.ToString(), () => opts.RssDayCount.ToString()) + ":" + Pass(() => dasBlogSettings.SiteConfiguration.RssEntryCount.ToString(), () => opts.RssEntryCount.ToString());
				cache.Remove(CacheKey);
			}
		}
#endif

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			var saveState = CommentSaveState.Failed;

			if (!Pass(() => dasBlogSettings.SiteConfiguration.EnableComments, () => opts.EnableComments))
			{
				return CommentSaveState.SiteCommentsDisabled;
			}

			var entry = dataService.GetEntry(postid);
			if (entry != null)
			{
				if (Pass(() => dasBlogSettings.SiteConfiguration.EnableCommentDays, () => opts.EnableComments))
				{
					var targetComment = DateTime.UtcNow.AddDays(-1 * Pass(() =>dasBlogSettings.SiteConfiguration.DaysCommentsAllowed, () => opts.DaysCommentsAllowed));

					if (targetComment > entry.CreatedUtc)
					{
						return CommentSaveState.PostCommentsDisabled;
					}
				}

				dataService.AddComment(comment);
				saveState = CommentSaveState.Added;
			}
			else
			{
				saveState = CommentSaveState.NotFound;
			}

			return saveState;
		}

		public CommentSaveState DeleteComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				dataService.DeleteComment(postid, commentid);

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
			Entry entry = dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				dataService.ApproveComment(postid, commentid);

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
			return dataService.GetCommentsFor(postid, allComments);
		}

		public CategoryCacheEntryCollection GetCategories()
		{
			return dataService.GetCategories();
		}

		private static bool AreStringsEqual<T>(T _this, T other)
		{
			return _this.ToString() == other.ToString();
		}
/*
		private T Pass<T>(Expression<Func<T>> expr)
		{
			var fun = expr.Compile();
			var result = fun();
			logger.LogDebug($"pass £££expression = '{expr}' result = '{result}'[[[");
			return result;
		}
*/
		private T Pass<T>(Expression<Func<T>> oldExpr, Expression<Func<T>> newExpr, Func<T, T, bool> eqTest = null)
		{
			var oldFun = oldExpr.Compile();
			var oldResult = oldFun();
			var newFun = newExpr.Compile();
			var newResult = newFun();
			logger.LogDebug($"pass £££oldExpression = '{oldExpr}' oldResult = '{oldResult}', newExpression = '{newExpr}' newResult = '{newResult}'[[[");
			if (eqTest != null ? !eqTest(oldResult, newResult) : !oldResult.Equals(newResult))
			{
				throw new DifferenceFoundException($"£££oldExpression = '{oldExpr}' oldResult = '{oldResult}', newExpression = '{newExpr}' newResult = '{newResult}'[[[");
			}
			return newResult;
		}
		private bool ArePingServiceCollectionsEqual(PingServiceCollection aze, PingServiceCollection beeze)
		{
			if (aze == null || beeze == null || aze.Count != beeze.Count)
			{
				return false;
			}

			for (int ii = 0; ii < aze.Count; ii++)
			{
				(var a, var b) = (aze[ii], beeze[ii]);
				if (
					a.Endpoint != b.Endpoint
					|| a.Name != b.Name
					|| a.Url != b.Url
					|| a.PingApi != b.PingApi
				)
				{
					return false;
				}
			}
			return true;
		}
	}

	internal class DifferenceFoundException : Exception
	{
		public DifferenceFoundException(string s) : base(s)
		{
		}
	}

	internal static class SettingsUtils
	{
		public static string GetBaseUrl(string root)
		{
			return new Uri(root).AbsoluteUri;
		}
		public static string RelativeToRoot(string relative, string root)
		{
			return new Uri(new Uri(root), relative).AbsoluteUri;
		}
		public static string GetPermaLinkUrl(string entryId, string root)
		{
			return RelativeToRoot("post/" + entryId, root);
		}
		public static string GetPermaTitle(string title, string permaLinkSpaceReplacement)
		{
			string titlePermalink = title.Trim().ToLower();

			titlePermalink = titlePermalink.Replace("+", permaLinkSpaceReplacement);
			
			return titlePermalink;
		}
		public static DateTimeZone GetConfiguredTimeZone(bool adjustDisplayTimeZone, decimal displayTimeZoneIndex)
		{
			if (adjustDisplayTimeZone)
			{
				return DateTimeZone.ForOffset(Offset.FromSeconds((int)displayTimeZoneIndex * 3600));
			}
			else
			{
				return DateTimeZone.Utc;
			}
		}
		public static DateTime GetContentLookAhead(int contentLookAheadDays)
		{
			return DateTime.UtcNow.AddDays(contentLookAheadDays);
		}
		/// <summary>
		/// sticks root on the front of the feeds url
		/// </summary>
		/// <param name="root">e.g. http://localhost:50432/</param>
		/// <returns>e.g. http://localhost:50432;feed/rsd</returns>
		public static string GetRsdUrl(string root)
		{
			return RelativeToRoot("feed/rsd", root);
		}

/*
		/// <summary>
		/// parent directory for Config, content and logs
		/// </summary>
		/// <param name="env">this is a nuissance</param>
		/// <returns>e.g. C:\alt\projects\dasblog-core\source/DasBlog.Web.UI</returns>
		public static string GetWebHostingDirectory(IHostingEnvironment env)
		{
			return Startup.GetDataRoot(env);
		}
*/
	}
}

















