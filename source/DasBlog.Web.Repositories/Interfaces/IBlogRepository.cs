using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasBlog.Web.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        Entry GetBlogPost(string postid);

		Entry GetEntryForEdit(string postid);

		EntryCollection GetFrontPagePosts();

        EntryCollection GetEntriesForPage(int pageIndex);

		EntrySaveState CreateEntry(Entry entry);

		EntrySaveState UpdateEntry(Entry entry);

		void DeleteEntry(string postid);

		CategoryCacheEntryCollection GetCategories();

		string XmlRpcInvoke(Stream blob);
	}
}
