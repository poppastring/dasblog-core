using System;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
    public interface IBlogManager
    {
        Entry GetBlogPost(string posttitle, DateTime? postDate);

		Entry GetBlogPostByGuid(Guid postid);

		Entry GetEntryForEdit(string postid);

		EntryCollection GetFrontPagePosts(string acceptLanguageHeader);

        EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader);

		EntrySaveState CreateEntry(Entry entry);

		EntrySaveState UpdateEntry(Entry entry);

		void DeleteEntry(string postid);

		CategoryCacheEntryCollection GetCategories();

		CommentSaveState AddComment(string postid, Comment comment);

		CommentSaveState DeleteComment(string postid, string commentid);
		
		CommentSaveState ApproveComment(string postid, string commentid);

		CommentCollection GetComments(string postid, bool allComments);

	    EntryCollection SearchEntries(string searchString, string acceptLanguageHeader);

		void SendCommentEmail(string name, string email, string homepage,string content, string postitle, string entryid);
    }
}
