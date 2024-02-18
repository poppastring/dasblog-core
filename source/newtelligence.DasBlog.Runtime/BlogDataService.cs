#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using NodaTime;

namespace newtelligence.DasBlog.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    public static class BlogDataServiceFactory
    {

        private static Hashtable services = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentLocation"></param>
        /// <returns></returns>
        public static IBlogDataService GetService(string contentLocation, ILoggingDataService loggingService)
        {
            IBlogDataService service;

            lock (services.SyncRoot)
            {
                service = services[contentLocation.ToUpper()] as IBlogDataService;
                if (service == null)
                {
                    service = new BlogDataServiceXml(contentLocation, loggingService);
                    services.Add(contentLocation.ToUpper(), service);
                }
            }
            return service;
        }

        public static bool RemoveService(string contentLocation)
        {
            lock (services.SyncRoot)
            {
                if (services.ContainsKey(contentLocation.ToUpper()))
                {
                    services.Remove(contentLocation.ToUpper());
                    return true;
                }
            }

            return false;
        }
    }

    internal class BlogDataServiceXml : IBlogDataService
    {
        private string contentBaseDirectory;
        private DataManager data;
        private AutoResetEvent trackingQueueEvent;
        private Queue trackingQueue;
        private Thread trackingHandlerThread;
        private AutoResetEvent sendMailInfoQueueEvent;
        private Queue sendMailInfoQueue;
        private Thread sendMailInfoHandlerThread;
        private ILoggingDataService loggingService;

        private CommentFile allComments;
        protected string ContentBaseDirectory
        {
            get
            {
                return contentBaseDirectory;
            }
        }

        protected string UserAgent
        {
            get
            {
                string version = GetType().Assembly.GetName().Version.ToString();
                return "dasBlog Core/" + version;
            }
        }

        /// <summary>
        /// The BlogDataServiceXml constructor is entrypoint for the dasBlog Runtime.
        /// </summary>
        /// <param name="contentLocation">The path of the content directory</param>
        /// <param name="loggingService">The <see cref="ILoggingDataService"/></param>
        internal BlogDataServiceXml(string contentLocation, ILoggingDataService loggingService)
        {
            contentBaseDirectory = contentLocation;
            this.loggingService = loggingService;
            if (!Directory.Exists(contentBaseDirectory))
            {
                throw new ArgumentException(
                    String.Format("Invalid directory {0}", contentBaseDirectory),
                    "contentLocation");
            }
            
            data = new DataManager();
            data.Resolver = new ResolveFileCallback(this.GetAbsolutePath);
            
            trackingQueue = new Queue();
            trackingQueueEvent = new AutoResetEvent(false);
            trackingHandlerThread = new Thread(new ThreadStart(this.TrackingHandler));
            trackingHandlerThread.IsBackground = true;
            trackingHandlerThread.Start();
            
            sendMailInfoQueue = new Queue();
            sendMailInfoQueueEvent = new AutoResetEvent(false);
            sendMailInfoHandlerThread = new Thread(new ThreadStart(this.SendMailHandler));
            sendMailInfoHandlerThread.IsBackground = true;
            sendMailInfoHandlerThread.Start();
            
            allComments = new CommentFile(contentBaseDirectory);

            //OmarS: now we want to initialize the EntryIdCache so this doesn't happen elsewhere later on
//            InitCache();
        }


        protected string GetAbsolutePath(string file)
        {
            return Path.Combine(contentBaseDirectory, file);
        }

//        private object cacheLock = new object();
//        private EntryIdCache theCache;


        EntryCollection IBlogDataService.GetEntries(bool fullContent) {
    
            if (fullContent) {
                throw new NotImplementedException("EntryCollection with entries with full content not yet implemted.");
            }

            //if (theCache == null || theCache.ChangeNumber != data.CurrentEntryChangeCount) {
            //    InitCache();
            //}

            return EntryIdCache.GetInstance(data).GetEntries();

            //return theCache.GetEntries();
        }

        protected DateTime GetDateForEntry(string entryId)
        {
            if (String.IsNullOrEmpty(entryId))
            {
                // TODO: Web Core compatability issues ???
                // HttpContext.Current.Trace.Write("entryId is empty!");
            }


            DateTime foundDate = EntryIdCache.GetInstance(data).GetDateFromEntryId(entryId);
            if (foundDate == DateTime.MinValue)
            {
                foundDate = EntryIdCache.GetInstance(data).GetDateFromCompressedTitle(entryId);
            }

            //SDH: Folks find this warning more irritating and confusing than helpful.
            /* 
             if ((foundDate == DateTime.MinValue))
            {
                StackTrace st = new StackTrace();
                try
                {
                    // if we are running from the runtime, but not asp.net this will fail
                    loggingService.AddEvent(new EventDataItem(EventCodes.Error,"GetDateForEntry: Can't find \"" + entryId + "\" " + st.ToString() + " " + HttpContext.Current.Request.RawUrl,String.Empty));
                }
                catch
                {
                    loggingService.AddEvent(new EventDataItem(EventCodes.Error,"GetDateForEntry: Can't find \"" + entryId + "\" " + st.ToString(),String.Empty));					
                }
            }
            */
            return foundDate;
        }

        DayEntry InternalGetDayEntry(DateTime date)
        {
            DayEntry result = null;

            // TODO: Web Core compatability issues ???
            //System.Diagnostics.Debug.Write("Loading DayEntry for " + date.ToShortDateString());

            if (data.Days.ContainsKey(date))
            {
                DayEntry day = data.Days[date];
                day.Load(data);
                result = day;
            }

            /*
            foreach (DayEntry day in data.Days)
            {
                if (day.DateUtc == date)
                {
                    day.Load(data);
                    result = day;
                    break;
                }
            }
            */

            if (result == null)
            {
                result = new DayEntry();
                result.Initialize();
                result.DateUtc = date;
                result.Save(data);
                data.IncrementEntryChange();
            }

            return result;
        }

        DayEntry IBlogDataService.GetDayEntry(DateTime date)
        {
            return InternalGetDayEntry(date);
        }

        /// <summary>
        /// Returns loaded DayEntries that correspond to the criteria
        /// </summary>
        /// <param name="maxDays"></param>
        /// <param name="dayEntryCriteria"></param>
        /// <returns></returns>
        protected DayEntryCollection InternalGetDayEntries( Predicate<DayEntry> dayEntryCriteria, int maxDays)
        {
            return DayEntryCollectionFilter.FindAll(data.Days, dayEntryCriteria, maxDays);
        }


        /// <summary>
        /// Load the DayEntries that match the criteria of the includeDayEntry delegate
        /// </summary>
        /// <param name="dayEntryCriteria">A delegate that returns true for each DayEntry that should be included in the DayEntryCollection returned</param>
        /// <returns></returns>
        protected DayEntryCollection InternalGetDayEntries(Predicate<DayEntry> dayEntryCriteria)
        {

            return DayEntryCollectionFilter.FindAll(data.Days, dayEntryCriteria);
        }

        /// <summary>
        /// Gets a collection of <see cref="newtelligence.DasBlog.Runtime.DayEntry"/> structures for dates starting at 
        /// the <paramref name="startDate"/> backwards for at most 
        /// <paramref name="maxDays"/>.
        /// </summary>
        /// <param name="startDate">Date at which to start collecting DayEntry structures</param>
        /// <param name="maxDays">Maximum number of days to return. This number relates to
        /// days actually found and not to calendar days.</param>
        /// <returns>A DayEntryCollection containing the collected results.</returns>
        protected DayEntryCollection InternalGetDayEntries(DateTime startDate, int maxDays)
        {
            // we look one day ahead into "UTC" future in order to grab 
            // the timezones ahead of UTC.
            return DayEntryCollectionFilter.FindAll(data.Days, DayEntryCollectionFilter.DefaultFilters.OccursBefore(startDate.Date.AddDays(1)), maxDays);
        }

        /// <summary>
        /// Gets the DayExtra structure for a given date.
        /// </summary>
        /// <param name="date">Date for which the structure shall be returned.</param>
        /// <returns>A day extra structure for the given day.</returns>
        protected DayExtra InternalGetDayExtra(DateTime date)
        {
            DayExtra extra = data.GetDayExtra(date);
            // extra.Load( data ); // we don't need to call this twice
            return extra;
        }

        DayExtra IBlogDataService.GetDayExtra(DateTime date)
        {
            return InternalGetDayExtra(date);
        }



        protected class PingbackJob
        {
            internal PingbackInfo info;
            internal Entry entry;

            internal PingbackJob(PingbackInfo info, Entry entry)
            {
                this.info = info;
                this.entry = entry;
            }
        }



        private static readonly Regex anchors = new Regex("href\\s*=\\s*(?:(?:\\\"(?<url>[^\\\"]*)\\\")|(?<url>[^\\s]* ))", RegexOptions.Compiled);
        private static readonly Regex pingbackRegex = new Regex("<link rel=\"pingback\" href=\"([^\"]+)\" ?/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);



        private class TrackbackJob
        {
            internal TrackbackInfo info;
            internal Entry entry;

            internal TrackbackJob(TrackbackInfo info, Entry entry)
            {
                this.info = info;
                this.entry = entry;
            }
        }

        private static readonly Regex rdfAnchors = new Regex(@"<rdf:\w+\s[^>]*?>(</rdf:rdf>)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex trackbackPingAnchorRegex = new Regex("trackback:ping=\"(?<url>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex identRegex = new Regex("dc:identifier=\"(?<identifier>[^\"]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        protected string GetTrackbackLink(string pageBody, string externalUri)
        {
            foreach (Match match in rdfAnchors.Matches(pageBody))
            {
                Match m = trackbackPingAnchorRegex.Match(match.Value);
                Match m2 = identRegex.Match(match.Value);

                if (m.Groups["url"].Value != "" && m2.Groups["identifier"].Value == externalUri)
                {
                    Uri trackBacklink = new Uri(m.Groups["url"].Value);
                    if (trackBacklink.Scheme == Uri.UriSchemeHttp)
                        return trackBacklink.ToString();
                }
            }

            return null;
        }

        protected void TrackbackWorker(object argument)
        {
            TrackbackJob job = argument as TrackbackJob;

            try
            {
                string trackbackUrl = job.info.TargetUrl;

                if (trackbackUrl != null &&
                    trackbackUrl.Length > 0)
                {
                    string trackbackMsg = "url=" + WebUtility.UrlEncode(job.info.SourceUrl);
                    if (job.info.SourceTitle != null && job.info.SourceTitle.Length > 0)
                    {
                        trackbackMsg +=
                            "&title=" + WebUtility.UrlEncode(job.info.SourceTitle.Length > 80 ? job.info.SourceTitle.Substring(0, 80) : job.info.SourceTitle);
                    }

                    if (job.info.SourceExcerpt != null && job.info.SourceExcerpt.Length > 0)
                    {
                        trackbackMsg += "&excerpt=" + WebUtility.UrlEncode(job.info.SourceExcerpt.Length > 80 ? job.info.SourceExcerpt.Substring(0, 80) : job.info.SourceExcerpt);
                    }

                    trackbackMsg += "&blog_name=" + WebUtility.UrlEncode(job.info.SourceBlogName);

                    WebRequest request = WebRequest.Create(new Uri(trackbackUrl));

                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    using (StreamWriter requestWriter = new StreamWriter(request.GetRequestStream()))
                    {
                        requestWriter.Write(trackbackMsg);
                    }

                    using (request.GetResponse())
                    {
                    }

                    this.loggingService.AddEvent(
                        new EventDataItem(
                        EventCodes.TrackbackSent,
                        job.entry.Title,
                        job.info.SourceUrl,
                        job.info.TargetUrl));
                }
            }
            catch (Exception e)
            {
                ErrorTrace.Trace(TraceLevel.Error, e);
                if (loggingService != null)
                {
                    loggingService.AddEvent(
                        new EventDataItem(
                        EventCodes.TrackbackServerError,
                        e.ToString().Replace("\n", "<br />"),
                        job.info.SourceUrl,
                        job.info.TargetUrl,
                        job.entry.Title));
                }
            }
        }

        private class CrosspostJob
        {
            internal object info;
            internal Entry entry;
            internal IBlogDataService dataService;

            internal CrosspostJob(object info, Entry entry, IBlogDataService dataService)
            {
                this.info = info;
                this.entry = entry;
                this.dataService = dataService;
            }
        }


        protected Entry InternalGetEntry(string entryId)
        {
            Entry entryResult = null;
            DayEntry day;

            // The entry lookup hashtables use the UrlEncoded version of the title
            entryId = WebUtility.UrlEncode(entryId);

            DateTime foundDate = GetDateForEntry(entryId);
            if (foundDate == DateTime.MinValue)
            {
                entryResult = null;
            }
            else
            {
                day = InternalGetDayEntry(foundDate);

                if (day.Entries.ContainsKey(entryId))
                {
                    entryResult = day.Entries[entryId];
                }

                // entryId not found, so find by title
                if (entryResult == null)
                {
                    entryResult = day.GetEntryByTitle(entryId);
                }
            }

            // Don't return entries where IsPublic is false
            // unless the user is in the "admin" role.
            if ((entryResult != null)
                && (!entryResult.IsPublic)
                && !Thread.CurrentPrincipal.IsInRole("admin"))
            {
                entryResult = null;
            }

            return entryResult;
        }

        /// <summary>
        /// Returns the Entry for a given entryId. 
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        Entry IBlogDataService.GetEntry(string entryId)
        {
            return InternalGetEntry(entryId);
        }


        /// <summary>
        /// Returns a copy of the Entry for a given entryId. You must Save the Entry for changes to be
        /// reflected in the Runtime.
        /// </summary>
        /// <param name="entryId"></param>
        /// <returns></returns>
        Entry IBlogDataService.GetEntryForEdit(string entryId)
        {
            return InternalGetEntry(entryId).Clone();
        }

        /// <summary>
        /// Returns an EntryCollection whose entries all fit the criteria
        /// specified by include.
        /// </summary>
        /// <param name="dayEntryCriteria">A delegate that specifies which days should be included.</param>
        /// <param name="entryCriteria">A delegate that specifies which entries should be included.</param>
        /// <param name="maxDays">The maximum number of days to include.</param>
        /// <param name="maxEntries">The maximum number of entries to return.</param>
        /// <returns></returns>
        public EntryCollection /*IBlogDataService*/ GetEntries(
            Predicate<DayEntry> dayEntryCriteria,
            Predicate<Entry> entryCriteria,
            int maxDays, int maxEntries)
        {
            EntryCollection entries = new EntryCollection();
            DayEntryCollection days = this.InternalGetDayEntries(dayEntryCriteria, maxDays);
            int entryCount = 0;

            foreach (DayEntry day in days)
            {
                day.Load(data);

                foreach (Entry entry in day.GetEntries(entryCriteria))
                {
                    if (entryCount < maxEntries)
                    {
                        entries.Add(entry);
                        entryCount++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (entryCount >= maxEntries)
                {
                    break;
                }
            }
            return entries;
        }

        /// <summary>
        /// Gets a collection of at most <paramref name="maxEntries"/> <see cref="newtelligence.DasBlog.Runtime.Entry"/> 
        /// structures for dates starting at the <paramref name="startDateUtc"/> 
        /// backwards for at most <paramref name="maxDays"/>. 
        /// The collection can optionally be  
        /// filtered by a categoryName.
        /// </summary>
        /// <param name="startDateUtc">UTC normalized date at which to start collecting DayEntry structures. See remarks.</param>
        /// <param name="maxDays">Maximum number of days to return. This number relates to
        /// days actually found and not to calendar days.</param>
        /// <param name="maxEntries"></param>
        /// <param name="categoryName">Optional category filter. May be empty or null.</param>
        /// <returns></returns>
        /// <remarks>
        ///     The start date is expressed as a date relative to the UTC timezone and is normalized to
        ///     UTC 0000 hrs. The TimeZone passed to this method serves to offset UTC into display time.
        ///     
        /// </remarks>
        // TODO:  Consider refactoring to use InternalGetDayEntries that takes delegates.
        EntryCollection IBlogDataService.GetEntriesForDay(
            DateTime startDateUtc, DateTimeZone tz, string acceptLanguages, int maxDays, int maxEntries, string categoryName)
        {
            EntryCollection entries;
            Predicate<Entry> entryCriteria = null;

            // the entry is only eligible if its timezone time is within start date or earlier 
            //entryCriteria += EntryCollection.CriteriaHandlerFactory.OccursBetween(
            //	tz, startDateUtc.Date, startDateUtc.AddDays(1).AddSeconds(-1) );
            if (categoryName != null &&
                categoryName.Length > 0)
            {
                entryCriteria += EntryCollectionFilter.DefaultFilters.IsInCategory(categoryName);
            }

            if (acceptLanguages != null && acceptLanguages.Length > 0)
            {
                entryCriteria += EntryCollectionFilter.DefaultFilters.IsInAcceptedLanguagesOrMultiLingual(acceptLanguages);
            }

            // HACK AG AddDays(1) removed to prevent showing entries after the selected date.
            entries = GetEntries(
                DayEntryCollectionFilter.DefaultFilters.OccursBefore(startDateUtc.Date),
                entryCriteria,
                maxDays, maxEntries);

            return entries;
        }


        EntryCollection IBlogDataService.GetEntriesForMonth(
            DateTime month, DateTimeZone timeZone, string acceptLanguages)
        {
            EntryCollection entries;
            Predicate<Entry> entryCriteria = null;
            int daysInMonth;

            // The number of days in the month is equivalent to the last day of the month.
            daysInMonth = (new DateTime(month.Year, month.Month, 1, 0, 0, 0).AddMonths(1).AddSeconds(-1)).Day;

            // the entry is only eligible if its timezone time is within start date or earlier 
            entryCriteria += EntryCollectionFilter.DefaultFilters.OccursInMonth(
                timeZone, month);

            if (acceptLanguages != null && acceptLanguages.Length > 0)
            {
                entryCriteria += EntryCollectionFilter.DefaultFilters.IsInAcceptedLanguagesOrMultiLingual(acceptLanguages);
            }

            // TODO:  In theory it should be unnecessary to specify the maxDays because there cannot be more than one
            // DayEntry per day but it existed in previous code so .  Verify and then remove.
            entries = GetEntries(DayEntryCollectionFilter.DefaultFilters.OccursInMonth(timeZone, month),
                entryCriteria, daysInMonth, int.MaxValue);

            return entries;
        }

        // TODO:  Consider refactoring to use InternalGetDayEntries that takes delegates.  It is slightly more
        // complicated because this method uses CategoryCache().
        EntryCollection IBlogDataService.GetEntriesForCategory(string categoryName, string acceptLanguages)
        {
            CategoryCache cache = new CategoryCache();
            cache.Ensure(data);

            EntryCollection entryList = new EntryCollection();
            Entry entry;

            if (cache.UrlSafeCategories.ContainsKey(categoryName))
            {
                categoryName = cache.UrlSafeCategories[categoryName];
            }

            CategoryCacheEntry catEntry = cache.Entries[categoryName];
            if (catEntry != null)
            {
                foreach (CategoryCacheEntryDetail detail in catEntry.EntryDetails)
                {
                    DayEntry day = data.Days[detail.DayDateUtc];
                    if (day != null)
                    {
                        Predicate<Entry> entryCriteria = null;

                        if (acceptLanguages != null && acceptLanguages.Length > 0)
                        {
                            entryCriteria += EntryCollectionFilter.DefaultFilters.IsInAcceptedLanguagesOrMultiLingual(acceptLanguages);
                        }


                        day.Load(data);
                        entry = day.GetEntries(entryCriteria)[detail.EntryId];
                        if (entry != null)
                        {
                            entryList.Add(entry);
                        }
                    }
                }
            }
            entryList.Sort((left, right) => right.CreatedUtc.CompareTo(left.CreatedUtc));
            return entryList;
        }

		string IBlogDataService.GetCategoryTitle(string categoryurl)
		{
			var cache = new CategoryCache();
			var categoryName = categoryurl;
			cache.Ensure(data);

			if (cache.UrlSafeCategories.ContainsKey(categoryurl))
			{
				categoryName = cache.UrlSafeCategories[categoryurl];
			}

			return categoryName;
		}

		EntryCollection IBlogDataService.GetEntriesForUser(string user)
        {
            Predicate<Entry> entryCriteria = null;
            EntryCollection entries = new EntryCollection();
            entryCriteria += EntryCollectionFilter.DefaultFilters.IsFromUser(user);

            DayEntryCollection dayEntries = InternalGetDayEntries(null, int.MaxValue);

            foreach (DayEntry dayEntry in dayEntries)
            {
                foreach (Entry entry in dayEntry.GetEntries(entryCriteria))
                    entries.Add(entry);
            }

            return entries;
        }

        DateTime[] IBlogDataService.GetDaysWithEntries(DateTimeZone tz)
        {
       
            EntryCollection idCache = ((IBlogDataService)this).GetEntries(false);
            List<DateTime> dayList = new List<DateTime>();

            foreach (Entry entry in idCache)
            {
				DateTime tzTime = tz.AtStrictly(LocalDateTime.FromDateTime(entry.CreatedUtc)).LocalDateTime.ToDateTimeUnspecified().Date;
                if (!dayList.Contains(tzTime))
                {
                    dayList.Add(tzTime);
                }
            }
            return dayList.ToArray();
        }

        void IBlogDataService.DeleteEntry(string entryId, CrosspostSiteCollection crosspostSites)
        {
            DateTime foundDate = GetDateForEntry(entryId);
            if (foundDate == DateTime.MinValue)
                return;

            DayEntry day = InternalGetDayEntry(foundDate);
            Entry currentEntry = day.Entries[entryId];

            if (currentEntry != null)
            {
                if (crosspostSites != null)
                {
                    //foreach (Crosspost cp in currentEntry.Crossposts)
                    //{
                    //    foreach (CrosspostSite site in crosspostSites)
                    //    {
                    //        if (site.ProfileName == cp.ProfileName)
                    //        {
                    //            try
                    //            {
                    //                BloggerAPIClientProxy proxy = new BloggerAPIClientProxy();
                    //                UriBuilder uriBuilder = new UriBuilder("http", site.HostName, site.Port, site.Endpoint);
                    //                proxy.Url = uriBuilder.ToString();
                    //                proxy.UserAgent = this.UserAgent;

                    //                proxy.blogger_deletePost("", cp.TargetEntryId, site.Username, site.Password, true);

                    //                if (loggingService != null)
                    //                {
                    //                    loggingService.AddEvent(
                    //                        new EventDataItem(EventCodes.CrosspostDeleted, currentEntry.Title, site.ProfileName));
                    //                }
                    //            }
                    //            catch (XmlRpcFaultException xrfe)
                    //            {
                    //                ErrorTrace.Trace(TraceLevel.Error, xrfe);
                    //                if (loggingService != null)
                    //                {
                    //                    loggingService.AddEvent(
                    //                        new EventDataItem(EventCodes.Error,
                    //                        xrfe.Message,
                    //                        String.Format("Deleting cross-post entry {0} on {1}; Failed with server-fault code, {2} \"{3}\"", cp.TargetEntryId, cp.ProfileName, xrfe.FaultCode, xrfe.FaultString)));
                    //                }
                    //            }
                    //            catch (Exception e)
                    //            {
                    //                ErrorTrace.Trace(TraceLevel.Error, e);
                    //                if (loggingService != null)
                    //                {
                    //                    loggingService.AddEvent(
                    //                        new EventDataItem(EventCodes.Error,
                    //                        e.ToString().Replace("\n", "<br />"),
                    //                        String.Format("Deleting cross-post entry {0} from {1}", cp.TargetEntryId, cp.ProfileName)));
                    //                }
                    //            }
                    //            break;
                    //        }

                    //    }
                    //}
                }

                day.Entries.Remove(currentEntry);
            }

            day.Save(data);
	        data.IncrementEntryChange();
        }

        EntrySaveState IBlogDataService.SaveEntry(Entry entry, params object[] trackingInfos)
        {
            bool found = false;

            if (entry.EntryId == null || entry.EntryId.Length == 0)
                return EntrySaveState.Failed;

            DayEntry day = InternalGetDayEntry(entry.CreatedUtc.Date);
            Entry currentEntry = day.Entries[entry.EntryId];

            // OmarS: now that all the entries are returned from a cache, and not desrialized
            // for each request, users who call GetEntry() will get the current entry in the runtime.
            // That entry can be modified freely, and changes will not be commited till day.Save()
            // is called. However, since they have made changes, and passed in that entry, currentEntry
            // and entry are the same objects (reference the same object) and now the changes are in the day
            // but they haven't been saved. This can cause weird problems, and the runtime may have data
            // that is not commited, and it will be lost if day.Save() is never called.
            if (entry == null)
            {
                throw new ArgumentNullException("Entry is null");
            }

            if (entry.Equals(currentEntry))
            {
                throw new ArgumentException("You have modified an existing entry and are passing that in. You need to call GetEntryForEdit to get a copy of the entry before modifying it");
            }

            //There's a possibility that they've changed the CreatedLocalTime of the entry
            // which means it CURRENTLY lives in one DayEntry file but will soon live in another.
            // We need to find the old version, if it exists, and if the CreatedLocalTime is different
            // than the one being saved now, blow it away.
            Entry originalEntry = this.InternalGetEntry(entry.EntryId);
            DayExtra originalExtra = null;

            //Get the comments and trackings for the original entry
            CommentCollection originalComments = null;
            TrackingCollection originalTrackings = null;

            //If we found the original and they DID change the CreatedLocalTime
            if (currentEntry == null && originalEntry != null && originalEntry.CreatedLocalTime != entry.CreatedLocalTime)
            {
                //get the comments and trackings
                originalExtra = data.GetDayExtra(originalEntry.CreatedUtc.Date);
                if (originalExtra != null)
                {
                    originalComments = originalExtra.GetCommentsFor(originalEntry.EntryId, data);
                    originalTrackings = originalExtra.GetTrackingsFor(originalEntry.EntryId, data);

                    //O^n slow...
                    foreach (Comment c in originalComments)
                    {
                        originalExtra.Comments.Remove(c);
                    }

                    //O^n slow...be nice if they were hashed
                    foreach (Tracking t in originalTrackings)
                    {
                        originalExtra.Trackings.Remove(t);
                    }
                    originalExtra.Save(data);
                }


                //Get that original's day and delete the entry
                DayEntry originalDay = InternalGetDayEntry(originalEntry.CreatedUtc.Date);
                originalDay.Entries.Remove(originalEntry);
                originalDay.Save(data);
            }


            // we need to check to see if the two objects are equal so that we avoid trasing
            // data like Crossposts.Clear() which will remove the crosspostInfo from both entries
            if (currentEntry != null && !currentEntry.Equals(entry))
            {
                // we will only change the mod date if there has been a change to a few things
                if (currentEntry.CompareTo(entry) == 1)
                {
                    currentEntry.ModifiedUtc = DateTime.Now.ToUniversalTime();
                }

                currentEntry.Categories = entry.Categories;
                currentEntry.Syndicated = entry.Syndicated;
                currentEntry.Content = entry.Content;
                currentEntry.CreatedUtc = entry.CreatedUtc;
                currentEntry.Description = entry.Description;
                currentEntry.anyAttributes = entry.anyAttributes;
                currentEntry.anyElements = entry.anyElements;

                currentEntry.Author = entry.Author;
                currentEntry.IsPublic = entry.IsPublic;
                currentEntry.Language = entry.Language;
                currentEntry.AllowComments = entry.AllowComments;
                currentEntry.Link = entry.Link;
                currentEntry.ShowOnFrontPage = entry.ShowOnFrontPage;
                currentEntry.Title = entry.Title;
				currentEntry.Latitude = entry.Latitude;
				currentEntry.Longitude = entry.Longitude;

                currentEntry.Crossposts.Clear();
                currentEntry.Crossposts.AddRange(entry.Crossposts);
                currentEntry.Attachments.Clear();
                currentEntry.Attachments.AddRange(entry.Attachments);

                day.Save(data);
                data.lastEntryUpdate = currentEntry.ModifiedUtc;
                data.IncrementEntryChange();
                found = true;
            }
            else
            {
                day.Entries.Add(entry);
                day.Save(data);
                data.lastEntryUpdate = entry.CreatedUtc;
                data.IncrementEntryChange();
                found = false;
            }

            //Now, move the comments and trackings to the new Date
            if (originalEntry != null && originalComments != null && originalExtra != null)
            {
                DayExtra newExtra = data.GetDayExtra(entry.CreatedUtc.Date);
                newExtra.Comments.AddRange(originalComments);
                newExtra.Trackings.AddRange(originalTrackings);
                newExtra.Save(data);
            }


            if (trackingInfos != null && entry.IsPublic)
            {
                foreach (object trackingInfo in trackingInfos)
                {
                    //if (trackingInfo != null)
                    //{
                    //    if (trackingInfo is WeblogUpdatePingInfo)
                    //    {
                    //        ThreadPool.QueueUserWorkItem(
                    //            new WaitCallback(this.PingWeblogsWorker),
                    //            (WeblogUpdatePingInfo)trackingInfo);
                    //    }
                    //    else if (trackingInfo is PingbackInfo)
                    //    {
                    //        PingbackJob pingbackJob = new PingbackJob((PingbackInfo)trackingInfo, entry);

                    //        ThreadPool.QueueUserWorkItem(
                    //            new WaitCallback(this.PingbackWorker),
                    //            pingbackJob);
                    //    }
                    //    else if (trackingInfo is PingbackInfoCollection)
                    //    {
                    //        PingbackInfoCollection pic = trackingInfo as PingbackInfoCollection;
                    //        foreach (PingbackInfo pi in pic)
                    //        {
                    //            PingbackJob pingbackJob = new PingbackJob(pi, entry);

                    //            ThreadPool.QueueUserWorkItem(
                    //                new WaitCallback(this.PingbackWorker),
                    //                pingbackJob);
                    //        }
                    //    }
                    //    else if (trackingInfo is TrackbackInfo)
                    //    {
                    //        ThreadPool.QueueUserWorkItem(
                    //            new WaitCallback(this.TrackbackWorker),
                    //            new TrackbackJob((TrackbackInfo)trackingInfo, entry));
                    //    }
                    //    else if (trackingInfo is TrackbackInfoCollection)
                    //    {
                    //        TrackbackInfoCollection tic = trackingInfo as TrackbackInfoCollection;
                    //        foreach (TrackbackInfo ti in tic)
                    //        {
                    //            ThreadPool.QueueUserWorkItem(
                    //                new WaitCallback(this.TrackbackWorker),
                    //                new TrackbackJob(ti, entry));
                    //        }
                    //    }
                    //    else if (trackingInfo is CrosspostInfo ||
                    //        trackingInfo is CrosspostInfoCollection)
                    //    {
                    //        ThreadPool.QueueUserWorkItem(
                    //            new WaitCallback(this.CrosspostWorker),
                    //            new CrosspostJob(trackingInfo, entry, this));
                    //    }
                    //}
                }
            }
            return found ? EntrySaveState.Updated : EntrySaveState.Added;
        }

        CategoryCacheEntryCollection IBlogDataService.GetCategories()
        {
            CategoryCacheEntryCollection result;
            CategoryCache cache = new CategoryCache();
            cache.Ensure(data);
            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.IsInRole("admin"))
            {
                result = cache.Entries;
            }
            else
            {
                result = new CategoryCacheEntryCollection();
                foreach (CategoryCacheEntry category in cache.Entries)
                {
                    if (category.IsPublic)
                    {
                        result.Add(category);
                    }
                }
            }
            return result;
        }

        private void InternalAddTracking(Tracking tracking)
        {
            bool trackFound = false;

            Entry entry = InternalGetEntry(tracking.TargetEntryId);

            if (entry == null)
            {
                StackTrace st = new StackTrace();
                string logtext = String.Format("InternalAddTracking: Entry not found: {0}, {1}, {2} {3}",
                    tracking.TrackingType, tracking.TargetTitle, tracking.TargetEntryId, st.ToString());
                this.loggingService.AddEvent(
                    new EventDataItem(EventCodes.Error, logtext, ""));
                return;
            }

            DayExtra extra = InternalGetDayExtra(entry.CreatedUtc);

            if (extra == null)
            {
                StackTrace st = new StackTrace();
                string logtext = String.Format("InternalAddTracking: DayExtra not found: {0}, {1}, {2}, {3} {4}",
                    tracking.TrackingType, tracking.TargetTitle, tracking.TargetEntryId, entry.CreatedUtc, st.ToString());
                this.loggingService.AddEvent(
                    new EventDataItem(EventCodes.Error, logtext, ""));
                return;
            }

            foreach (Tracking trk in extra.Trackings)
            {
                if (trk.PermaLink == tracking.PermaLink &&
                    String.Compare(trk.TargetEntryId, tracking.TargetEntryId, true) == 0 &&
                    trk.TrackingType == tracking.TrackingType)
                {
                    trackFound = true;
                    break;
                }
            }

            if (!trackFound)
            {
                tracking.TargetTitle = entry.Title;
                extra.Trackings.Add(tracking);
                extra.Save(data);
                data.IncrementExtraChange();
            }
        }

        private void TrackingHandler()
        {
            while (true)
            {
                Tracking tracking;

                // block the thread from entering the next loop till trackingQueueEvent.Set() is called
                trackingQueueEvent.WaitOne();
                while (trackingQueue.Count != 0)
                {
                    try
                    {
                        lock (trackingQueue.SyncRoot)
                        {
                            tracking = trackingQueue.Dequeue() as Tracking;
                        }
                        if (tracking != null)
                        {
                            InternalAddTracking(tracking);
                        }

                        if (trackingQueue.Count == 0)
                        {
                            break;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        if (loggingService != null)
                        {
                            loggingService.AddEvent(
                                new EventDataItem(EventCodes.Error,
                                ex.ToString().Replace("\n", "<br />"),
                                "Dequeue from TrackingHandler"));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (loggingService != null)
                        {
                            loggingService.AddEvent(
                                new EventDataItem(EventCodes.Error,
                                ex.ToString().Replace("\n", "<br />"),
                                "Unhandled Exception from TrackingHandler"));
                        }
                    }
                }
            }
        }

        private void InternalSendMail(SendMailInfo info)
        {
            try
            {
                info.SendMyMessage();
            }
            catch (Exception e)
            {
                ErrorTrace.Trace(TraceLevel.Error, e);
                if (loggingService != null)
                {
                    //CDO is very touchy and it's useful to get ALL the inner exceptions to diagnose the problem.
                    string exMessage = e.ToString();

                    Exception inner = e.InnerException;
                    while (inner != null)
                    {
                        exMessage += " INNER: " + inner.ToString();
                        inner = inner.InnerException;
                    }

                    loggingService.AddEvent(new EventDataItem(EventCodes.SmtpError, exMessage.Replace("\n", "<br />"), "InternalSendMail"));
                }
            }
        }

        private void SendMailHandler()
        {
            while (true)
            {
                SendMailInfo sendMailInfo;

                // block the thread from entering the next loop till trackingQueueEvent.Set() is called
                sendMailInfoQueueEvent.WaitOne();
                while (sendMailInfoQueue.Count != 0)
                {
                    try
                    {
                        lock (sendMailInfoQueue.SyncRoot)
                        {
                            sendMailInfo = sendMailInfoQueue.Dequeue() as SendMailInfo;
                        }
                        if (sendMailInfo != null)
                        {
                            InternalSendMail(sendMailInfo);
                        }

                        if (sendMailInfoQueue.Count == 0)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        ErrorTrace.Trace(TraceLevel.Error, e);
                        if (loggingService != null)
                        {
                            loggingService.AddEvent(
                                new EventDataItem(EventCodes.Error,
                                e.ToString().Replace("\n", "<br />"),
                                "InternalSendMail from SendMailHandler"));
                        }
                    }
                }
            }
        }

        void IBlogDataService.RunActions(object[] actions)
        {
            if (actions != null)
            {
                foreach (object action in actions)
                {
                    if (action is SendMailInfo)
                    {
                        lock (sendMailInfoQueue.SyncRoot)
                        {
                            sendMailInfoQueue.Enqueue(action);
                        }

                        sendMailInfoQueueEvent.Set();
                    }
                }
            }
        }

        void IBlogDataService.AddTracking(Tracking tracking, params object[] actions)
        {
            ((IBlogDataService)this).RunActions(actions);

            lock (trackingQueue.SyncRoot)
            {
                trackingQueue.Enqueue(tracking);
            }
            trackingQueueEvent.Set();
        }

        void IBlogDataService.DeleteTracking(string entryId, string trackingPermalink, TrackingType trackingType)
        {
            DateTime date = GetDateForEntry(entryId);
            if (date != DateTime.MinValue)
            {
                DayExtra extra = data.GetDayExtra(date);

                for (int i = 0; i < extra.Trackings.Count; i++)
                {
                    Tracking tracking = extra.Trackings[i];
                    if (tracking.PermaLink != null && tracking.PermaLink.Length > 0)
                    {
                        // Trimming PermaLink to fix bug 1278194: PermaLink may be stored with '\r' appended.
                        if (String.Compare(tracking.PermaLink.Trim(), trackingPermalink, true) == 0 && trackingType == tracking.TrackingType)
                        {
                            extra.Trackings.Remove(tracking);
                            extra.Save(data);
                            break;
                        }
                    }
                }
            }
        }

        TrackingCollection IBlogDataService.GetTrackingsFor(string entryId)
        {
            TrackingCollection trackingsForEntry = new TrackingCollection();
            DateTime date = GetDateForEntry(entryId);
            if (date == DateTime.MinValue)
                return trackingsForEntry;

            DayExtra extra = data.GetDayExtra(date);
            foreach (Tracking trk in extra.Trackings)
            {
                if (trk.TargetEntryId.ToUpper() == entryId.ToUpper())
                {
                    trackingsForEntry.Add(trk);
                }
            }
            return trackingsForEntry;
        }

        void IBlogDataService.AddComment(Comment comment, params object[] actions)
        {

            DateTime date = GetDateForEntry(comment.TargetEntryId);
            if (date == DateTime.MinValue)
                return;

            //Don't allow anyone to add a comment to a closed entry...
            Entry e = this.InternalGetEntry(comment.TargetEntryId);
            if (e == null || e.AllowComments == false)
            {
                return;
            }

            ((IBlogDataService)this).RunActions(actions);
            data.lastCommentUpdate = comment.CreatedUtc;
            DayExtra extra = data.GetDayExtra(date);
            extra.Comments.Add(comment);
            extra.Save(data);
            data.IncrementExtraChange();
            // update the all comments file
            allComments.AddComment(comment);
        }

        Comment IBlogDataService.GetCommentById(string entryId, string commentId)
        {
            DateTime date = GetDateForEntry(entryId);
            if (date == DateTime.MinValue)
                return null;

            DayExtra extra = data.GetDayExtra(date);

            return extra.Comments[commentId];
        }

        void IBlogDataService.DeleteComment(string entryid, string commentid)
        {
            DateTime date = GetDateForEntry(entryid);
            if (date == DateTime.MinValue)
                return;

            DayExtra extra = data.GetDayExtra(date);

            Comment c = extra.Comments[commentid];
            if (c != null)
            {
                extra.Comments.Remove(c);
                extra.Save(data);
                data.IncrementExtraChange();

                // update the all comments file
                allComments.DeleteComment(commentid);
            }
        }

        void IBlogDataService.ApproveComment(string entryId, string commentId)
        {
            DateTime date = GetDateForEntry(entryId);
            if (date == DateTime.MinValue)
                return;

            DayExtra extra = data.GetDayExtra(date);

            Comment c = extra.Comments[commentId];
            if (c != null)
            {
                c.IsPublic = true;
                c.SpamState = SpamState.NotSpam;
                extra.Save(data);
                data.IncrementExtraChange();

                // update the all comments file
                allComments.UpdateComment(c);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entryId"></param>
        /// <remarks>Only gets the public comments.</remarks>
        [Obsolete("Use the overload to indicate you want allComments, or just the public.")]
        CommentCollection IBlogDataService.GetCommentsFor(string entryId)
        {
            return InternalGetCommentsFor(entryId, false);
        }

        /// <summary>
        /// Gets the public comments for the entry.
        /// </summary>
        /// <param name="entryId">The entry id.</param>
        /// <returns>
        /// A collection of public comments for the entry.
        /// </returns>
        CommentCollection IBlogDataService.GetPublicCommentsFor(string entryId)
        {
            return InternalGetCommentsFor(entryId, false);
        }

        /// <summary>
        /// </summary>
        /// <param name="entryId"></param>
        /// <param name="allComments">Indicates wheter to get all comments, or only the public comments.</param>
        /// <returns></returns>
        CommentCollection IBlogDataService.GetCommentsFor(string entryId, bool allComments)
        {
            return InternalGetCommentsFor(entryId, allComments);
        }

        CommentCollection InternalGetCommentsFor(string entryId, bool allComments)
        {

            CommentCollection commentsForEntry = new CommentCollection();
            DateTime date = GetDateForEntry(entryId);
            if (date == DateTime.MinValue)
                return commentsForEntry;

            DayExtra extra = data.GetDayExtra(date);
            foreach (Comment cm in extra.Comments)
            {

                // check if the comment is for this entry, and is public or allComments are requested
                if (String.Compare(cm.TargetEntryId, entryId, true, CultureInfo.InvariantCulture) == 0 && (cm.IsPublic || allComments))
                {
                    commentsForEntry.Add(cm);
                }
            }
            return commentsForEntry;
        }

        /// <summary>
        /// Gets all comments for this blog.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method will be extremely slow on a blog with a lot of content.</remarks>
        CommentCollection IBlogDataService.GetAllComments()
        {

            CommentCollection _com = null;

            // get the comments
            _com = allComments.LoadComments();

            //			if (_com == null) {
            //				// recreate the comments file.
            //				_com = new CommentCollection();
            //				_com.Rebuild();
            //				// save the comments
            //				allComments.SaveComments( _com );
            //
            //			}
            return _com;
        }

        /// <summary>
        /// The DateTime of the last modified or created post.
        /// </summary>
        /// <returns>DateTime of the last entry modification in UTC </returns>
        DateTime IBlogDataService.GetLastEntryUpdate()
        {
            return data.lastEntryUpdate;
        }

        /// <summary>
        /// This DateTime of the most recent comment entry
        /// </summary>
        /// <returns>DateTime of the last comment entry in UTC</returns>
        DateTime IBlogDataService.GetLastCommentUpdate()
        {

            if (data.lastCommentUpdate == DateTime.MinValue)
            {

                data.lastCommentUpdate = allComments.GetLastCommentUpdate();
            }

            return data.lastCommentUpdate;
        }

        public StaticPage GetStaticPage( string pagename )
        {
            StaticPage page = new StaticPage();
            page.Name = pagename;
            string staticPathLocation = this.contentBaseDirectory + "\\static\\" + pagename + ".html";
            if (File.Exists(staticPathLocation))
            {
                page.Content = File.ReadAllText(staticPathLocation);
            }
            else
            {
                return null;
            }
            return page;
        }
    }
}
