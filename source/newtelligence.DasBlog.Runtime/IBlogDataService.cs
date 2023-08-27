using System;
using NodaTime;

namespace newtelligence.DasBlog.Runtime
{
	/// <summary>
	/// Enumeration used for the return code of <see cref="IBlogDataService.SaveEntry"/>
	/// </summary>
	public enum EntrySaveState
	{
		/// <summary>
		/// Indicates that a new entry has been added by the operation
		/// </summary>
		Added, 
		/// <summary>
		/// Indicates that the current entry has been updated
		/// </summary>
		Updated, 
		/// <summary>
		/// Indicates that the operation failed.
		/// </summary>
		Failed 
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IBlogDataService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <remarks>
		/// By default, Entries where IsPublic is set to false will not be returned unless
		/// the CurrentPrincipal is in the "admin" role
		/// </remarks>
		/// <returns></returns>
		Entry GetEntry( string entryId );

//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="entryId"></param>
//		/// <remarks>
//		/// By default, Entries where IsPublic is set to false will not be returned unless
//		/// the CurrentPrincipal is in the "admin" role
//		/// </remarks>
//		/// <returns></returns>
//		string GetEntryTitle( string entryId );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <remarks>
		/// By default, Entries where IsPublic is set to false will not be returned unless
		/// the CurrentPrincipal is in the "admin" role
		/// </remarks>
		/// <returns></returns>
		Entry GetEntryForEdit( string entryId );

        EntryCollection GetEntries(bool fullContent);

		/// <summary>
		/// Returns an EntryCollection whose entries all fit the criteria
		/// specified by include.
		/// </summary>
		/// <param name="dayEntryCriteria">A delegate that specifies which days should be included.</param>
		/// <param name="entryCriteria">A delegate that specifies which entries should be included.</param>
		/// <param name="maxDays">The maximum number of days to include.</param>
		/// <param name="maxEntries">The maximum number of entries to return.</param>
		/// <returns></returns>
		EntryCollection GetEntries(
			Predicate<DayEntry> dayEntryCriteria, 
			Predicate<Entry> entryCriteria, 
			int maxDays, int maxEntries);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="start"></param>
		/// <param name="maxDays"></param>
		/// <param name="maxEntries"></param>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		EntryCollection GetEntriesForDay(DateTime start, DateTimeZone tz, string acceptLanguages, int maxDays, int maxEntries, string categoryName);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="summary"></param>
		/// <param name="tz"></param>
		/// <param name="acceptLanguages"></param>
		/// <returns></returns>
		EntryCollection GetEntriesForMonth(DateTime summary, DateTimeZone tz, string acceptLanguages);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		EntryCollection GetEntriesForCategory(string categoryName, string acceptLanguages);

		string GetCategoryTitle(string categoryurl);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		EntryCollection GetEntriesForUser(string user);
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		DateTime[] GetDaysWithEntries(DateTimeZone tz);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		DayEntry GetDayEntry(DateTime date );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		DayExtra GetDayExtra(DateTime date);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		void DeleteEntry( string entryId, CrosspostSiteCollection crosspostSites );
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <param name="trackingInfos"></param>
		/// <returns></returns>
		EntrySaveState SaveEntry( Entry entryId, params object[] trackingInfos );        
        
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		CategoryCacheEntryCollection GetCategories();
        
		/// <summary>
		/// 
		/// </summary>
		/// <param name="actions"></param>
		void RunActions(object[] actions);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tracking"></param>
		void AddTracking( Tracking tracking, params object[] actions );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <param name="trackingPermalink"></param>
		/// <param name="trackingType"></param>
		void DeleteTracking(string entryId, string trackingPermalink, TrackingType trackingType);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <returns></returns>
		TrackingCollection GetTrackingsFor( string entryId );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="comment"></param>
		void AddComment(Comment comment, params object [] actions );

		Comment GetCommentById( string entryId, string commentId);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <param name="commentId"></param>
		void ApproveComment( string entryId, string commentId );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <param name="commentId"></param>
		void DeleteComment( string entryId, string commentId );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <returns></returns>
		[Obsolete( "Use the overload to indicate you want allComments, or just the public; or use the GetPublicCommentsFor method.")]
		CommentCollection GetCommentsFor( string entryId );


		/// <summary>
		/// Gets the public comments for the entry.
		/// </summary>
		/// <param name="entryId">The entry id.</param>
		/// <returns>A collection of public comments for the entry.</returns>
		CommentCollection GetPublicCommentsFor( string entryId );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entryId"></param>
		/// <param name="allComments"></param>
		/// <returns></returns>
		CommentCollection GetCommentsFor( string entryId, bool allComments );
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		CommentCollection GetAllComments();
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		DateTime GetLastEntryUpdate();

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		DateTime GetLastCommentUpdate();

		StaticPage GetStaticPage( string pagename );
	}
}
