using DasBlog.Core.Exceptions;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ActivityLogs;
using EventDataItem = DasBlog.Services.ActivityLogs.EventDataItem;
using EventCodes = DasBlog.Services.ActivityLogs.EventCodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using DasBlog.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using DasBlog.Core.Security;
using System.Net.Mail;
using System.Net;
using System.IO;
using CloudNative.CloudEvents;
using System.Xml.Linq;
using CloudNative.CloudEvents.Http;
using CloudNative.CloudEvents.SystemTextJson;
using System.Net.Http;

namespace DasBlog.Managers
{
	public class BlogManager : IBlogManager
	{
		private readonly IBlogDataService dataService;
		private readonly ILogger logger;
		private static readonly Regex stripTags = new Regex("<[^>]*>", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		private readonly IDasBlogSettings dasBlogSettings;
		private const int COMMENT_PAGE_SIZE = 5;

		public BlogManager( ILogger<BlogManager> logger, IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.logger = logger;

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));;
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);
		}

		/// <param name="dt">if non-null then the post must be dated on that date</param>
		public Entry GetBlogPost(string posttitle, DateTime? dt)
		{
			if (dt == null)
			{
				posttitle = posttitle.Replace(dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement, string.Empty)
									.Replace(".aspx", string.Empty);

				return dataService.GetEntry(posttitle);
			}
			else
			{
				var entries = dataService.GetEntriesForDay(dt.Value, null, null, 1, 10, null);

				return entries.FirstOrDefault(e => dasBlogSettings.GeneratePostUrl(e)
											.EndsWith(posttitle, StringComparison.OrdinalIgnoreCase));
			}
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

		
		public EntryCollection SearchEntries(string searchString, string acceptLanguageHeader)
		{
			var searchWords = GetSearchWords(searchString);

			var entries = dataService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), 
											dasBlogSettings.GetConfiguredTimeZone(), 
											acceptLanguageHeader, int.MaxValue, int.MaxValue, null);

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
			RaisePostCreatedCloudEvent(entry);
			return rtn;
		}


		private void RaisePostCreatedCloudEvent(Entry entry)
		{
			var ext = CloudEventAttribute.CreateExtension("tags", CloudEventAttributeType.String);
			var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, new[] { ext })
			{
				Type = "dasblog.post.created",
				Source = new Uri(dasBlogSettings.GetBaseUrl()),
				Subject = entry.Link,
				Data = MapEntryToCloudEventData(entry),
				Id = Guid.NewGuid().ToString(),
				Time = DateTime.UtcNow,
			};
			cloudEvent.SetAttributeFromString("tags", entry.Categories);
			RaiseCloudEvent(cloudEvent);

		}

		private void RaiseCloudEvent(CloudEvent cloudEvent)
		{
			if ( dasBlogSettings.SiteConfiguration.EnableCloudEvents &&
				 dasBlogSettings.SiteConfiguration.CloudEventsTargets != null)
			{
				foreach (var target in dasBlogSettings.SiteConfiguration.CloudEventsTargets)
				{
					if (!string.IsNullOrEmpty(target.Uri))
					{
						try
						{
							var content = cloudEvent.ToHttpContent(ContentMode.Structured, new JsonEventFormatter());
							var uriBuilder = new UriBuilder(target.Uri);
							if (target.Headers != null)
							{
								foreach (var header in target.Headers)
								{
									if (!string.IsNullOrEmpty(header.Name))
									{
										content.Headers.Add(header.Name, header.Value);
									}
								}
							}
							if (target.QueryArgs!= null)
							{
								foreach (var queryArgs in target.QueryArgs)
								{
									uriBuilder.Query = (string.IsNullOrEmpty(uriBuilder.Query) ? string.Empty : uriBuilder.Query + "&") + queryArgs.Name + "=" + queryArgs.Value;
								}
							}
							var httpClient = new HttpClient();
							var result = httpClient.PostAsync(uriBuilder.Uri, content).GetAwaiter().GetResult();
						}
						catch(Exception ex)
						{
							logger.LogError(ex, "Failed to post CloudEvent");
						}
					}
				}
			}
		}

		private EntryCloudEventData MapEntryToCloudEventData(Entry entry)
		{
			var data = new EntryCloudEventData();
			data.Id = entry.EntryId;
			data.Title = entry.Title;
			data.CreatedUtc = entry.CreatedUtc;
			data.ModifiedUtc = entry.ModifiedUtc;
			data.Tags = entry.Categories;
			data.Description = entry.Description;
			data.PermaLink = entry.Link;
			data.DetailsLink = dasBlogSettings.GetRssEntryUrl(entry.EntryId);
			data.IsPublic = entry.IsPublic;
			data.Author = entry.Author;
			data.Longitude = entry.Longitude;
			data.Latitude = entry.Latitude;
			return data;
		}

		public EntrySaveState UpdateEntry(Entry entry)
		{
			var rtn = InternalSaveEntry(entry, null, null);
			LogEvent(EventCodes.EntryChanged, entry);
			RaisePostUpdatedCloudEvent(entry);
			return rtn;
		}

		private void RaisePostUpdatedCloudEvent(Entry entry)
		{
			var ext = CloudEventAttribute.CreateExtension("tags", CloudEventAttributeType.String);
			var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, new[] { ext })
			{
				Type = "dasblog.post.updated",
				Source = new Uri(dasBlogSettings.GetBaseUrl()),
				Subject = entry.Link,
				Data = MapEntryToCloudEventData(entry),
				Id = Guid.NewGuid().ToString(),
				Time = DateTime.UtcNow,
			};
			cloudEvent.SetAttributeFromString("tags", entry.Categories);
			RaiseCloudEvent(cloudEvent);
		}

		public void DeleteEntry(string postid)
		{
			var entry = GetEntryForEdit(postid);
			dataService.DeleteEntry(postid, null);
			LogEvent(EventCodes.EntryDeleted, entry);
			RaisePostDeletedCloudEvent(entry);
		}

		private void RaisePostDeletedCloudEvent(Entry entry)
		{
			var ext = CloudEventAttribute.CreateExtension("tags", CloudEventAttributeType.String);
			var cloudEvent = new CloudEvent(CloudEventsSpecVersion.V1_0, new[] { ext })
			{
				Type = "dasblog.post.deleted",
				Source = new Uri(dasBlogSettings.GetBaseUrl()),
				Subject = entry.Link,
				Id = Guid.NewGuid().ToString(),
				Time = DateTime.UtcNow,
				Data = MapEntryToCloudEventData(entry)
			};
			cloudEvent.SetAttributeFromString("tags", entry.Categories);
			RaiseCloudEvent(cloudEvent);
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

				rtn = dataService.SaveEntry(entry, MaybeBuildWeblogPingInfo(), entry.IsPublic
											? trackbackList : null, MaybeBuildPingbackInfo(entry), crosspostList);

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
					Endpoint = "http://ping.feedburner.com",
					Name = "FeedBurner",
					Url = "http://www.feedburner.com",
					PingApi = PingService.PingApiType.Basic
				}
			};
			return
				fakePingServices.Count > 0
				? new WeblogUpdatePingInfo(dasBlogSettings.SiteConfiguration.Title,
												dasBlogSettings.SiteConfiguration.Root,
												dasBlogSettings.PingBackUrl,
												dasBlogSettings.RssUrl,
												fakePingServices)
				: null;
		}

		/// <summary>
		/// not sure what this is about but it is legacy
		/// TODO: reconsider when strategy for handling pingback in legacy site.config is decided.
		/// </summary>
		private PingbackInfo MaybeBuildPingbackInfo(Entry entry)
		{
			return dasBlogSettings.SiteConfiguration.EnableAutoPingback && entry.IsPublic
				? new PingbackInfo(dasBlogSettings.GetPermaLinkUrl(entry.EntryId), entry.Title,
									entry.Description, dasBlogSettings.SiteConfiguration.Title) 
				: null;
		}

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			var saveState = CommentSaveState.Failed;
			var entry = dataService.GetEntry(postid);

			if (!dasBlogSettings.SiteConfiguration.EnableComments || !entry.AllowComments)
			{
				return CommentSaveState.SiteCommentsDisabled;
			}

			if (entry != null)
			{
				var targetComment = DateTime.UtcNow.AddDays(-1 * dasBlogSettings.SiteConfiguration.DaysCommentsAllowed);

				if ((targetComment > entry.CreatedUtc))
				{
					return CommentSaveState.PostCommentsDisabled;
				}

				// FilterHtml html encodes anything we don't like
				string filteredText = dasBlogSettings.FilterHtml(comment.Content);
				comment.Content = filteredText;

				if (dasBlogSettings.SiteConfiguration.SendCommentsByEmail)
				{
					var actions = ComposeMailForUsers(entry, comment);
					dataService.AddComment(comment, actions);
				}
				else
				{
					dataService.AddComment(comment);
				}

				
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

		public CommentCollection GetAllComments()
		{
			return dataService.GetAllComments();
		}

		public List<Comment> GetCommentsFrontPage()
		{
			var comments = dataService.GetAllComments().OrderByDescending(d => d.CreatedUtc).ToList();

			return comments.Take(COMMENT_PAGE_SIZE).ToList();
		}

		public List<Comment> GetCommentsForPage(int pageIndex)
		{
			var comments = dataService.GetAllComments().OrderByDescending(d => d.CreatedUtc).ToList();

			return comments.Skip((pageIndex) * COMMENT_PAGE_SIZE).Take(COMMENT_PAGE_SIZE).ToList();
		}

		public CategoryCacheEntryCollection GetCategories()
		{
			return dataService.GetCategories();
		}

		private string GetFromEmail()
		{
			if (string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.SmtpFromEmail))
			{
				return dasBlogSettings.SiteConfiguration.SmtpUserName?.Trim();
			}

			return dasBlogSettings.SiteConfiguration.SmtpFromEmail?.Trim();
		}
		public bool SendTestEmail()
		{
			var emailMessage = new MailMessage();
			emailMessage.From = new MailAddress(GetFromEmail());
			emailMessage.To.Add(dasBlogSettings.SiteConfiguration.NotificationEMailAddress);
			emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);

			foreach (var user in dasBlogSettings.SecurityConfiguration.Users)
			{
				if (!string.IsNullOrWhiteSpace(user.EmailAddress))
				{
					emailMessage.To.Add(user.EmailAddress);
				}
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

		private object[] ComposeMailForUsers(Entry entry, Comment c)
		{
			var actions = new List<object>();
			
			foreach (var user in dasBlogSettings.SecurityConfiguration.Users)
			{
				if (string.IsNullOrWhiteSpace(user.EmailAddress))
					continue;

				if (user.NotifyOnAllComment || (user.NotifyOnOwnComment && entry.Author.ToUpper() == user.Name.ToUpper()))
				{
					var sendMailInfo = ComposeMail(c);
					sendMailInfo.Message.To.Add(user.EmailAddress);
					actions.Add(sendMailInfo);
				}
			}

			return actions.ToArray();
		}

		private SendMailInfo ComposeMail(Comment c)
		{
			var emailMessage = new MailMessage();

			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.NotificationEMailAddress))
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.NotificationEMailAddress);
			}
			else
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);
			}

			emailMessage.Subject = string.Format("Weblog comment by '{0}' from '{1}' on '{2}'", c.Author, c.AuthorHomepage, c.TargetTitle);

			if (dasBlogSettings.SiteConfiguration.CommentsRequireApproval)
			{
				emailMessage.Body = string.Format("{0}\r\nComments page: {1}\r\n\r\nRequires approval.\r\n\r\nCommentor Email: {2}\r\n\r\nIP Address: {3}\r\n\r\nLogin Here: {4}",
				   WebUtility.HtmlDecode(c.Content),
				   dasBlogSettings.GetCommentViewUrl(c.TargetEntryId),
				   c.AuthorEmail,
				   c.AuthorIPAddress,
				   dasBlogSettings.RelativeToRoot("account/login"));
			}
			else
			{
				emailMessage.Body = string.Format("{0}\r\nComments page: {1}\r\n\r\nCommentor Email: {2}\r\n\r\nIP Address: {3}\r\n\r\nLogin Here: {4}",
				   WebUtility.HtmlDecode(c.Content),
				   dasBlogSettings.GetCommentViewUrl(c.TargetEntryId),
				   c.AuthorEmail,
				   c.AuthorIPAddress,
				   dasBlogSettings.RelativeToRoot("account/login"));
			}

			emailMessage.IsBodyHtml = false;
			emailMessage.BodyEncoding = System.Text.Encoding.UTF8;

			emailMessage.From = new MailAddress(GetFromEmail());

			return dasBlogSettings.GetMailInfo(emailMessage);
		}
	}
}

















