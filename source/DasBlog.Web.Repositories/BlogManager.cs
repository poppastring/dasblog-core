﻿using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using newtelligence.DasBlog.Runtime;
using DasBlog.Managers.Interfaces;
using DasBlog.Core;
using EventDataItem = DasBlog.Core.EventDataItem;
using EventCodes = DasBlog.Core.EventCodes;
using DasBlog.Core.Extensions;

namespace DasBlog.Managers
{
	public class BlogManager : IBlogManager
	{
		private readonly IBlogDataService dataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly Microsoft.Extensions.Logging.ILogger logger;
		private static Regex stripTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		public BlogManager(IDasBlogSettings settings , Microsoft.Extensions.Logging.ILogger<BlogManager> logger)
		{
			dasBlogSettings = settings;
			var loggingDataService = LoggingDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.LogDir);
			dataService = BlogDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.ContentDir, loggingDataService);
			this.logger = logger;
		}

		public Entry GetBlogPost(string postid)
		{
			return dataService.GetEntry(postid);
		}

		public Entry GetEntryForEdit(string postid)
		{
			return dataService.GetEntryForEdit(postid);
		}

		public EntryCollection GetFrontPagePosts(string acceptLanguageHeader)
		{
			return dataService.GetEntriesForDay(dasBlogSettings.GetContentLookAhead(), dasBlogSettings.GetConfiguredTimeZone(),
								acceptLanguageHeader, dasBlogSettings.SiteConfiguration.FrontPageDayCount, 
								dasBlogSettings.SiteConfiguration.FrontPageEntryCount, string.Empty);
		}

		public EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader)
		{
			Predicate<Entry> pred = null;

			//Shallow copy as we're going to modify it...and we don't want to modify THE cache.
			EntryCollection cache = dataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

			// remove the posts from the front page
			EntryCollection fp = GetFrontPagePosts(acceptLanguageHeader);

			cache.RemoveRange(0, fp.Count);

			int entriesPerPage = dasBlogSettings.SiteConfiguration.EntriesPerPage;

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
			StringCollection searchWords = new StringCollection();

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

			EntryCollection matchEntries = new EntryCollection();

			foreach (Entry entry in dataService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), dasBlogSettings.GetConfiguredTimeZone(), 
																	acceptLanguageHeader, int.MaxValue, int.MaxValue, null))
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
			Entry entry = GetEntryForEdit(postid);
			dataService.DeleteEntry(postid, null);
			LogEvent(EventCodes.EntryDeleted, entry);
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

		private string MakePermaLink(Entry entry)
		{
			return new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root)
				,dasBlogSettings.RelativeToRoot(entry.EntryId)).ToString();
		}

		private Uri MakePermaLinkFromCompressedTitle(Entry entry)
		{
			return new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root)
				,dasBlogSettings.RelativeToRoot(
				dasBlogSettings.GetPermaTitle(entry.CompressedTitle)));
		}

		private EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList)
		{

			EntrySaveState rtn = EntrySaveState.Failed;
			// we want to prepopulate the cross post collection with the crosspost footer
			if (dasBlogSettings.SiteConfiguration.EnableCrossPostFooter && dasBlogSettings.SiteConfiguration.CrossPostFooter != null 
				&& dasBlogSettings.SiteConfiguration.CrossPostFooter.Length > 0)
			{
				foreach (CrosspostInfo info in crosspostList)
				{
					info.CrossPostFooter = dasBlogSettings.SiteConfiguration.CrossPostFooter;
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

				rtn = dataService.SaveEntry(entry, 
					(dasBlogSettings.SiteConfiguration.PingServices.Count > 0) ?
						new WeblogUpdatePingInfo(dasBlogSettings.SiteConfiguration.Title, dasBlogSettings.GetBaseUrl(), dasBlogSettings.GetBaseUrl(), dasBlogSettings.RsdUrl, dasBlogSettings.SiteConfiguration.PingServices) : null,
					(entry.IsPublic) ?
						trackbackList : null,
					dasBlogSettings.SiteConfiguration.EnableAutoPingback && entry.IsPublic ?
						new PingbackInfo(
							dasBlogSettings.GetPermaLinkUrl(entry.EntryId),
							entry.Title,
							entry.Description,
							dasBlogSettings.SiteConfiguration.Title) : null,
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
			cache.Remove("Rss::" + dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + dasBlogSettings.SiteConfiguration.RssEntryCount.ToString());

			foreach (string category in categories)
			{
				string CacheKey = "Rss:" + category + ":" + dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + dasBlogSettings.SiteConfiguration.RssEntryCount.ToString();
				cache.Remove(CacheKey);
			}
		}

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = dataService.GetEntry(postid);

			if (entry != null)
			{
				// Are comments allowed

				dataService.AddComment(comment);

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
	}
}
