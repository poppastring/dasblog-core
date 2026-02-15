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

		bool SendTestEmail();
	}
}
