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

        EntryCollection GetFrontPagePosts();

        EntryCollection GetEntriesForPage(int pageIndex);

		void SaveEntry(Entry entry);

		void UpdateEntry(Entry entry);

		void DeleteEntry(string postid);

		CategoryCacheEntry GetCategories();

		string XmlRpcInvoke(Stream blob);
	}
}
