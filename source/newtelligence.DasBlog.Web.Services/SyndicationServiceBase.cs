using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.Services.Rss20;
using System.Collections.Generic;
using NodaTime;

namespace newtelligence.DasBlog.Web
{
    [DesignTimeVisible(false)]
	public class SyndicationServiceBase : System.Web.Services.WebService
	{
        protected IBlogDataService dataService;
        protected ILoggingDataService loggingService;
        protected DataCache cache;
        protected SiteConfig siteConfig;

        #region Component Designer generated code
		
        //Required by the Web Services Designer 
        private IContainer components = null;
				
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if(disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);		
        }
		
        #endregion

		public SyndicationServiceBase()
		{

            InitializeComponent();
            if ( Context != null )
            {
                siteConfig = SiteConfig.GetSiteConfig();
                loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), loggingService );
                cache = CacheFactory.GetCache();
                
            }
        }

        public SyndicationServiceBase(SiteConfig siteConfig, IBlogDataService dataService, ILoggingDataService loggingService)
        {
            InitializeComponent();
            this.dataService = dataService;
            this.loggingService = loggingService; 
            this.siteConfig = siteConfig;
        }


        protected string PreprocessItemContent( string entryId, string content )
        {
			if (siteConfig.ApplyContentFiltersToRSS)
			{
				content = SiteUtilities.FilterContent(entryId, content);
			}
			
			if ( siteConfig.EnableAggregatorBugging )
            {
                content = content + "<img width=\"0\" height=\"0\" src=\""+SiteUtilities.GetAggregatorBugUrl(siteConfig,entryId)+"\"/>";
            }

            if ( siteConfig.EnableRssItemFooters && 
                siteConfig.RssItemFooter != null && 
                siteConfig.RssItemFooter.Length > 0 )
            {
                content = content + "<br/><hr/>" + siteConfig.RssItemFooter;
            }
            
            

            return content;
            
        }
        

        protected class EntrySorter : IComparer<Entry>
        {
            public int Compare(Entry left, Entry right)
            {
                return right.CreatedUtc.CompareTo(left.CreatedUtc);
            }
        }

        protected EntryCollection BuildEntries(string category, int maxDayCount, int maxEntryCount)
        {
            EntryCollection entryList = new EntryCollection();

            if (category != null)
            {
                int entryCount = siteConfig.RssEntryCount;
                category = category.ToUpper();
                foreach (CategoryCacheEntry catEntry in dataService.GetCategories())
                {
                    if (catEntry.Name.ToUpper() == category)
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
                entryList = dataService.GetEntriesForDay(DateTime.Now.AddDays(siteConfig.ContentLookaheadDays).ToUniversalTime(), DateTimeZone.Utc, null, maxDayCount, maxEntryCount, null);
            }
            entryList.Sort( new EntrySorter() );
            return entryList;
        }

	}
}
