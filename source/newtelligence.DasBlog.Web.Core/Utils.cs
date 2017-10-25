using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web.Core
{
    [Obsolete("Utils has been renamed to SiteUtilities")]
    public static class Utils 
    {
        /// <summary>
        /// Maps a virtual path to a physical path, respects the mapping rules from Server.MapPath.
        /// </summary>
        /// <param name="virtualPath">The virtual path to map.</param>
        /// <returns>The physical Path.</returns>
        public static string MapPath(string virtualPath) {
            return SiteUtilities.MapPath(virtualPath);
        }


		public static string GetUserAgent()
		{
			return SiteUtilities.GetUserAgent();
		}

        public static string RelativeToRoot(string relative)
        {
            return SiteUtilities.RelativeToRoot(relative);
        }

        public static string RelativeToRoot(SiteConfig siteConfig, string relative)
        {
			return SiteUtilities.RelativeToRoot(siteConfig, relative);
        }

		public static bool ReferralFromSelf(SiteConfig siteConfig, string referral)
		{
			return SiteUtilities.ReferralFromSelf(siteConfig,referral);
		}

		public static bool ReferralFromSelf(string referral, string baseUri)
		{
			return SiteUtilities.ReferralFromSelf(referral,baseUri);
		}

		public static string SpamBlocker(string email)
		{
			return SiteUtilities.SpamBlocker(email);
		}
		
		public static string GetRsdUrl()
		{
			return SiteUtilities.GetRsdUrl();
		}
    	
		public static string GetRsdUrl(SiteConfig siteConfig)
		{
			return SiteUtilities.GetRsdUrl();
		}

		public static string GetMicrosummaryUrl()
		{
			return SiteUtilities.GetMicrosummaryUrl();
		}

		public static string GetMicrosummaryUrl(SiteConfig siteConfig)
		{
			return SiteUtilities.GetMicrosummaryUrl(siteConfig);
		}



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRssUrl()
        {
            return SiteUtilities.GetRsdUrl();
        }

        public static string GetRssUrl(SiteConfig siteConfig)
        {
			return SiteUtilities.GetRsdUrl(siteConfig);
        }

		/// <summary>
		/// Returns the Rss Url, where the default protocol "http" is replaced by the
		/// standard "feed" protocol
		/// </summary>
		/// <returns>a string like this: "feed://www.mysite.com/SyndicationService.asmx/GetRss"</returns>
		public static string GetFeedUrl() {
			return SiteUtilities.GetFeedUrl();
		}

		/// <summary>
		/// Returns the Rss Url, where the default protocol "http" is replaced by the
		/// standard "feed" protocol
		/// </summary>
		/// <param name="siteConfig">SiteConfig</param>
		/// <returns>a string like this: "feed://www.mysite.com/SyndicationService.asmx/GetRss"</returns>
		public static string GetFeedUrl(SiteConfig siteConfig) {
			return SiteUtilities.GetFeedUrl(siteConfig);
		}

		public static string GetCdfUrl() {
			return SiteUtilities.GetCdfUrl();
		}
		
		public static string GetCdfUrl(SiteConfig siteConfig)
        {
            return SiteUtilities.GetCdfUrl(siteConfig);
        }

        public static string GetCommentsRssUrl()
        {
            return SiteUtilities.GetCommentsRssUrl();
        }

        public static string GetEntryCommentsRssUrl(string id)
        {
            return SiteUtilities.GetEntryCommentsRssUrl(id);
        }

        public static string GetCommentsRssUrl(SiteConfig siteConfig)
        {
            return SiteUtilities.GetCommentsRssUrl(siteConfig);
        }

        public static string GetEntryCommentsRssUrl(SiteConfig siteConfig, string id)
        {
            return SiteUtilities.GetEntryCommentsRssUrl(siteConfig, id);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetBloggerUrl()
		{
            return SiteUtilities.GetBloggerUrl();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetBloggerUrl(SiteConfig siteConfig)
		{
			return SiteUtilities.GetBloggerUrl(siteConfig);
		}
    	
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAtomUrl()
        {
            return SiteUtilities.GetAtomUrl();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAtomUrl(SiteConfig siteConfig)
        {
			return SiteUtilities.GetAtomUrl(siteConfig);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetAtomCategoryUrl(string categoryName)
        {
            return SiteUtilities.GetAtomCategoryUrl(categoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetAtomCategoryUrl(SiteConfig siteConfig, string categoryName)
        {
            return SiteUtilities.GetAtomCategoryUrl(siteConfig,categoryName);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static string GetFeedCategoryUrl(string categoryName)
		{
			return SiteUtilities.GetFeedCategoryUrl(categoryName);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static string GetFeedCategoryUrl(SiteConfig siteConfig, string categoryName)
		{
			return SiteUtilities.GetFeedCategoryUrl(siteConfig,categoryName);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetRssCategoryUrl(string categoryName)
        {
            return SiteUtilities.GetRssCategoryUrl(categoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetRssCategoryUrl(SiteConfig siteConfig, string categoryName)
        {
            return SiteUtilities.GetRssCategoryUrl(siteConfig,categoryName);
        }

        /// <summary>
        /// Gets the raw style permalink of a post. Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
        /// </summary>
        public static string GetPermaLinkUrl( string id )
        {
            return SiteUtilities.GetPermaLinkUrl(id);
        }

		/// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
		public static string GetPermaLinkUrl(Entry entry)
		{	
			return SiteUtilities.GetPermaLinkUrl(entry);
		}

		/// <summary>
		/// Gets the compressed title URL.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <returns></returns>
		public static string GetCompressedTitleUrl( string title )
		{
			return SiteUtilities.GetCompressedTitleUrl(title);
		}


		public static string GetEnclosureLinkUrl( string id, Attachment attachment )
		{
			return SiteUtilities.GetEnclosureLinkUrl(id,attachment);
		}

        public static string GetEditViewUrl( string id )
        {
            return SiteUtilities.GetEditViewUrl(id);
        }

		public static string GetCommentViewUrl( string targetEntryId)
		{
			return SiteUtilities.GetCommentViewUrl(targetEntryId);
		}
		
		public static string GetCommentViewUrl( string targetEntryId, string commentId )
        {
            return SiteUtilities.GetCommentViewUrl(targetEntryId, commentId);
        }
		
		public static string GetCommentDeleteUrl( string targetEntryId, string commentId )
		{
			return SiteUtilities.GetCommentDeleteUrl(targetEntryId,commentId);
		}

		public static string GetCommentApproveUrl( string targetEntryId, string commentId ){
			return SiteUtilities.GetCommentApproveUrl(targetEntryId,commentId);
		}

        public static string GetTrackbackUrl( string id )
        {
            return SiteUtilities.GetTrackbackUrl(id);
        }
    	
		public static string GetTrackbackDeleteUrl( string entryId, string referralPermalink, TrackingType type)
		{
			return SiteUtilities.GetTrackbackDeleteUrl(entryId,referralPermalink,type);
		}

        public static string GetBaseUrl()
        {
            return SiteUtilities.GetBaseUrl();
        }

        public static string GetStartPageUrl()
        {
            return SiteUtilities.GetStartPageUrl();
        }

		public static string GetAdminPageUrl()
		{
			return SiteUtilities.GetAdminPageUrl();
		}

        public static string GetDayViewUrl( DateTime day )
        {
            return SiteUtilities.GetDayViewUrl(day);
        }

		public static string GetMonthViewUrl( DateTime month )
		{
			return SiteUtilities.GetMonthViewUrl(month);
		}

		public static string GetPagedViewUrl(int page)
		{
			return SiteUtilities.GetPagedViewUrl(page);
		}

		public static string GetCompressedTitleUrl( SiteConfig siteConfig, string title )
		{
			return SiteUtilities.GetCompressedTitleUrl(siteConfig,title);
		}

		/// <summary>
		/// Gets the raw style permalink of a post. Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
		public static string GetPermaLinkUrl( SiteConfig siteConfig, string id )
		{
			return SiteUtilities.GetPermaLinkUrl(siteConfig,id);
		}

		/// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
        public static string GetPermaLinkUrl( SiteConfig siteConfig, Entry entry )
        {
			return SiteUtilities.GetPermaLinkUrl(siteConfig,entry);
        }

		/// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="EntryIdCacheEntry"/>.
		/// </summary>
		public static string GetPermaLinkUrl( SiteConfig siteConfig, EntryIdCacheEntry e )
		{
			return SiteUtilities.GetPermaLinkUrl(siteConfig,e);
		}

		public static string GetEnclosureLinkUrl( SiteConfig siteConfig, string id, Attachment attachment )
		{
			return SiteUtilities.GetEnclosureLinkUrl(siteConfig,id,attachment);
		}

        public static string GetEditViewUrl( SiteConfig siteConfig, string id )
        {
            return SiteUtilities.GetEditViewUrl(siteConfig,id);
        }

		public static string GetCommentViewUrl( SiteConfig siteConfig, string entryId )
		{
		    return SiteUtilities.GetCommentViewUrl(siteConfig,entryId);
		}
		
		public static string GetCommentViewUrl( SiteConfig siteConfig, string entryId, string commentId )
        {
            return SiteUtilities.GetCommentViewUrl(siteConfig,entryId,commentId);
        }
    	
		public static string GetTrackbackDeleteUrl( SiteConfig siteConfig, string entryId, string referralPermalink, TrackingType type)
		{
			return SiteUtilities.GetTrackbackDeleteUrl(siteConfig,entryId,referralPermalink,type);
		}
    	
		public static string GetCommentDeleteUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return SiteUtilities.GetCommentDeleteUrl(siteConfig,entryId,commentId);
		}

		public static string GetCommentApproveUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return SiteUtilities.GetCommentApproveUrl(siteConfig,entryId,commentId);
		}

		public static string GetCommentReportUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return SiteUtilities.GetCommentReportUrl(siteConfig,entryId,commentId);
		}

        public static string GetClickThroughUrl( SiteConfig siteConfig, string id )
        {
            return SiteUtilities.GetClickThroughUrl(siteConfig,id);
        }

        public static string GetAggregatorBugUrl( SiteConfig siteConfig, string id )
        {
            return SiteUtilities.GetAggregatorBugUrl(siteConfig,id);
        }

        public static string GetCrosspostTrackingUrlBase( SiteConfig siteConfig )
        {
            return SiteUtilities.GetCrosspostTrackingUrlBase(siteConfig);
        }

        public static string GetTrackbackUrl( SiteConfig siteConfig, string id )
        {
			return SiteUtilities.GetTrackbackUrl(siteConfig,id);
        }

        public static string GetBaseUrl(SiteConfig siteConfig)
        {
            return SiteUtilities.GetBaseUrl(siteConfig);
        }

        public static string GetStartPageUrl(SiteConfig siteConfig)
        {
			return SiteUtilities.GetStartPageUrl(siteConfig);
        }

		public static string GetAdminPageUrl(SiteConfig siteConfig)
		{
			return SiteUtilities.GetAdminPageUrl(siteConfig);
		}

        public static string GetDayViewUrl( SiteConfig siteConfig, DateTime day )
        {
            return SiteUtilities.GetDayViewUrl(siteConfig,day);
        }

		public static string GetMonthViewUrl( SiteConfig siteConfig, DateTime month )
		{
			return SiteUtilities.GetMonthViewUrl(siteConfig,month);
		}

		public static string GetPagedViewUrl(SiteConfig siteConfig, int page)
		{
			return SiteUtilities.GetPagedViewUrl(siteConfig,page);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(string categoryName) {
            return SiteUtilities.GetCategoryViewUrl(categoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="page">The page this link points to, or 0 (zero) when there is no paging.</param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(string categoryName, int page) {
            return SiteUtilities.GetCategoryViewUrl(categoryName,page);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(SiteConfig siteConfig, string categoryName) {
            return SiteUtilities.GetCategoryViewUrl(siteConfig,categoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <param name="categoryName"></param>
        /// <param name="page">The page this link points to, or 0 (zero) when there is no paging.</param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(SiteConfig siteConfig, string categoryName, int page) {
            return SiteUtilities.GetCategoryViewUrl(siteConfig,categoryName,page);
        }


		public static string GetUserViewUrl(string userName)
		{
			return SiteUtilities.GetUserViewUrl(userName);
		}

		public static string GetUserViewUrl(SiteConfig siteConfig, string userName)
		{
			return SiteUtilities.GetUserViewUrl(siteConfig,userName);
		}

        public static string GetProfileViewUrl( SiteConfig siteConfig, string userName, bool profileMapping )
        {
            return SiteUtilities.GetProfileViewUrl(siteConfig,userName,profileMapping);

        }        

        public static string FilterContent( SiteConfig siteConfig, string entryId, string content )
        {
            return SiteUtilities.FilterContent(siteConfig,entryId,content);
        }

		public static HyperLink ParseSearchString(string referral)
		{
			return SiteUtilities.ParseSearchString(referral);
		}

		public static MatchCollection FindHyperLinks(string content)
		{
			return SiteUtilities.FindHyperLinks(content);
		}

		public static string ApplyClickThrough( SiteConfig siteConfig, string entryId, string content )
        {
            return SiteUtilities.ApplyClickThrough(siteConfig,entryId,content);
        }

    	public static string ApplyContentFilters(SiteConfig siteConfig, string content)
    	{
    		return SiteUtilities.ApplyContentFilters(siteConfig,content);
    	}

        public static string FilterContent( string entryId, string content )
        {
            return SiteUtilities.FilterContent(entryId, content);
        }

		/// <summary>
		/// Maps a re-written Url written using <see cref="LinkRewriter"/> to a normal PathAndQuery url.
		/// </summary>
		/// <param name="requestUrl">The re-written url</param>
		/// <returns>A normal url</returns>
		public static string MapUrl (string requestUrl)
		{
			return SiteUtilities.MapUrl(requestUrl);
		}

		public static string ClipString( string text, int length )
		{
			return SiteUtilities.ClipString(text,length);
		}
		
		public static string ClipString(string text, int length, string trailer)
        {
            return SiteUtilities.ClipString(text, length,trailer);
        }

		public static string GetObfuscatedEmailUrl( string emailAddress ){

			  return SiteUtilities.GetObfuscatedEmailUrl(emailAddress);
		}

		public static string GetObfuscatedEmailUrl( string emailAddress, string subject ){
            return SiteUtilities.GetObfuscatedEmailUrl(emailAddress,subject);
		}

		/// <summary>
		/// Obfuscates a string using HTML character references (hex) and character entities. This is usefull for hiding e-mail addresses from spammers.
		/// </summary>
		/// <param name="input">The string to obfuscate.</param>
		/// <returns>A string.</returns>
		/// <remarks>The resulant string has a mix of HTML character references, salted characters and character entities</remarks>
		public static string GetObfuscatedString(string input)
		{
			return SiteUtilities.GetObfuscatedString(input);
		}

		public static bool AreCommentsAllowed(Entry entry, SiteConfig siteConfig)
		{
			return SiteUtilities.AreCommentsAllowed(entry,siteConfig);
		}



		public static bool GetStatusNotModified(DateTime latest)
		{
			return SiteUtilities.GetStatusNotModified(latest);
		}

		public static DateTime GetLatestModifedEntryDateTime(IBlogDataService dataService, EntryCollection entries)
		{
            return SiteUtilities.GetLatestModifedEntryDateTime(dataService, entries);
		}

		public static DateTime GetLatestModifedCommentDateTime(IBlogDataService dataService, CommentCollection comments)
		{
			return SiteUtilities.GetLatestModifedCommentDateTime(dataService,comments);
		}

		public static string FilterHtml( string input, ValidTagCollection allowedTags ){
            return SiteUtilities.FilterHtml(input,allowedTags);
		}

        public static void DeleteEntry(string entryId, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            SiteUtilities.DeleteEntry(entryId,siteConfig,logService,dataService);
        }

        public static void SaveEntry(Entry entry, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
             SiteUtilities.SaveEntry(entry,siteConfig,logService,dataService);
        }

        public static void SaveEntry(Entry entry, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
             SiteUtilities.SaveEntry(entry,crosspostList,siteConfig,logService,dataService);
        }

        public static void SaveEntry(Entry entry, TrackbackInfoCollection trackbackList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
             SiteUtilities.SaveEntry(entry,trackbackList,siteConfig,logService,dataService);
        }

        public static void SaveEntry(Entry entry, string trackbackUrl, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
             SiteUtilities.SaveEntry(entry,trackbackUrl,crosspostList,siteConfig,logService,dataService);
        }

        public static void SaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
             SiteUtilities.SaveEntry(entry,trackbackList,crosspostList,siteConfig,logService,dataService);
        }

        public static EntrySaveState UpdateEntry(Entry entry, string trackbackUrl, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            return SiteUtilities.UpdateEntry(entry,trackbackUrl,crosspostList,siteConfig,logService,dataService);
        }



        /// <summary>
        /// Send an email notification that an entry has been made.
        /// </summary>
        /// <param name="siteConfig">The page making the request.</param>
        /// <param name="entry">The entry being added.</param>
        internal static void SendEmail(Entry entry, SiteConfig siteConfig, ILoggingDataService logService)
        {
             SiteUtilities.SendEmail(entry,siteConfig,logService);
        }

	
    }
}
