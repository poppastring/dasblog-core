using DasBlog.Core.Exceptions;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ActivityLogs;
using EventDataItem = DasBlog.Services.ActivityLogs.EventDataItem;
using EventCodes = DasBlog.Services.ActivityLogs.EventCodes;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using System;
using System.Linq;
using DasBlog.Services;
using System.Net.Mail;

namespace DasBlog.Managers
{
	public class BlogManager : IBlogManager
	{
		private readonly IBlogDataService dataService;
		private readonly ILogger logger;
		private readonly IDasBlogSettings dasBlogSettings;

		public BlogManager( ILogger<BlogManager> logger, IDasBlogSettings dasBlogSettings, IBlogDataService dataService)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.logger = logger;
			this.dataService = dataService;
		}

		/// <param name="dt">if non-null then the post must be dated on that date</param>
		public Entry GetBlogPost(string posttitle, DateTime? dt)
		{
            if (string.IsNullOrEmpty(posttitle))
			{
				return null;
			}

			if (dt == null)
				{
					posttitle = posttitle.Replace("-", string.Empty)
														.Replace(" ", string.Empty)
														.Replace(".aspx", string.Empty);

				return dataService.GetEntry(posttitle);
			}
			else
				{
					var entries = dataService.GetEntriesForDay(dt.Value, null, null, 1, 10, null);
					var normalizedTitle = posttitle.Replace(" ", "-");

					return entries.FirstOrDefault(e => dasBlogSettings.GeneratePostUrl(e)
												.EndsWith(normalizedTitle, StringComparison.OrdinalIgnoreCase));
				}
		}

		public StaticPage GetStaticPage(string posttitle)
		{
			return dataService.GetStaticPage(posttitle);
		}

		public Entry GetBlogPostByGuid(Guid postid)
		{
			return dataService.GetEntry(postid.ToString());
		}

		public Entry GetEntryForEdit(string postid)
		{
			return dataService.GetEntryForEdit(postid);
		}

		public EntryCollection GetFrontPagePosts(string acceptLanguageHeader)
		{			
			return dataService.GetEntriesForDay(dasBlogSettings.GetContentLookAhead(), dasBlogSettings.GetConfiguredTimeZone(), 
											acceptLanguageHeader, dasBlogSettings.SiteConfiguration.FrontPageEntryCount, 
											dasBlogSettings.SiteConfiguration.FrontPageEntryCount, 
											dasBlogSettings.SiteConfiguration.FrontPageCategory);
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
			dataService.DeleteEntry(postid);

			LogEvent(EventCodes.EntryDeleted, entry);
		}

		public EntryCollection GetAllEntries()
		{
			return dataService.GetEntries(false);
		}

		private void LogEvent(EventCodes eventCode, Entry entry)
		{
			logger.LogInformation(new EventDataItem(eventCode, MakePermaLinkFromCompressedTitle(entry), entry.Title));
		}

		private Uri MakePermaLinkFromCompressedTitle(Entry entry)
		{
			if (dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique)
			{
				return new Uri(dasBlogSettings.RelativeToRoot(entry.CompressedTitle));
			}
			else
			{
				return new Uri(dasBlogSettings.RelativeToRoot(entry.CreatedUtc.ToString("yyyyMMdd") + "/" + entry.CompressedTitle));
			}
		}

		private EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList)
		{

			EntrySaveState rtn = EntrySaveState.Failed;

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

				rtn = dataService.SaveEntry(entry, entry.IsPublic
											? trackbackList : null, crosspostList);

				//TODO: SendEmail(entry, siteConfig, logService);
			}
			catch (Exception ex)
			{
				//TODO: Do something with this????
				// StackTrace st = new StackTrace();
				// logService.AddEvent(new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), ""));

				LoggedException le = new LoggedException("file failure", ex);

				var edi = new EventDataItem(EventCodes.Error, null, "Failed to Save a Post on {date}", DateTime.Now.ToShortDateString());
				logger.LogError(edi,le);
			}

			// we want to invalidate all the caches so users get the new post
			// BreakCache(entry.GetSplitCategories());

			return rtn;
		}

		public CategoryCacheEntryCollection GetCategories()
		{
			return dataService.GetCategories();
		}

		private string GetFromEmail()
		{
			var fromEmail = !string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.SmtpFromEmail)
				? dasBlogSettings.SiteConfiguration.SmtpFromEmail.Trim()
				: dasBlogSettings.SiteConfiguration.SmtpUserName?.Trim();

			if (string.IsNullOrWhiteSpace(fromEmail))
			{
				throw new FormatException("No valid 'From' email address configured. Please set SmtpFromEmail or SmtpUserName in settings.");
			}

			return fromEmail;
		}

		public bool SendTestEmail()
		{
			var emailMessage = new MailMessage();
			emailMessage.From = new MailAddress(GetFromEmail());

			// Add NotificationEMailAddress if valid
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.NotificationEMailAddress))
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.NotificationEMailAddress);
			}

			// Add Contact email if valid
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.Contact))
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);
			}

			foreach (var user in dasBlogSettings.SecurityConfiguration.Users)
			{
				if (!string.IsNullOrWhiteSpace(user.EmailAddress))
				{
					emailMessage.To.Add(user.EmailAddress);
				}
			}

			// Ensure we have at least one recipient
			if (emailMessage.To.Count == 0)
			{
				throw new FormatException("No valid recipient email addresses configured. Please set NotificationEMailAddress, Contact, or user email addresses in settings.");
			}

			emailMessage.Subject = string.Format("SMTP email from {0}", dasBlogSettings.SiteConfiguration.Title);
			emailMessage.Body = "Test ";

			var sendMailInfo = dasBlogSettings.GetMailInfo(emailMessage);

			try
			{
				sendMailInfo.SendMyMessage();
			}
			catch(Exception ex)
			{
				logger.LogInformation(new EventDataItem(EventCodes.SmtpError, new Uri(dasBlogSettings.SiteConfiguration.Root), 
									string.Format("SMTP Test Error: {0}", ex.Message)));

				return false;
			}

			return true;
		}

	}
}

















