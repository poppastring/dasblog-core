using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
    public interface IBlogManager
    {
        Entry GetBlogPost(string posttitle, DateTime? postDate);
		StaticPage GetStaticPage(string posttitle);	
		Entry GetBlogPostByGuid(Guid postid);

		Entry GetEntryForEdit(string postid);

		EntryCollection GetFrontPagePosts(string acceptLanguageHeader);

        EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader);

		EntryCollection GetAllEntries();

		EntrySaveState CreateEntry(Entry entry);

		EntrySaveState UpdateEntry(Entry entry);

		void DeleteEntry(string postid);

		CategoryCacheEntryCollection GetCategories();

		CommentSaveState AddComment(string postid, Comment comment);

		CommentSaveState DeleteComment(string postid, string commentid);
		
		CommentSaveState ApproveComment(string postid, string commentid);

		CommentCollection GetComments(string postid, bool allComments);

		CommentCollection GetAllComments();

		List<Comment> GetCommentsFrontPage();

		List<Comment> GetCommentsForPage(int pageIndex);

		EntryCollection SearchEntries(string searchString, string acceptLanguageHeader);

		bool SendTestEmail();
	}
}
