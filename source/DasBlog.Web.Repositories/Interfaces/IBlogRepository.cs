using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        Entry GetBlogPost(string postid);

        EntryCollection GetFrontPagePosts();

        EntryCollection GetEntriesForPage(int pageIndex);
    }
}
