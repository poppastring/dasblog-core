﻿using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface IBlogManager
    {
        Entry GetBlogPost(string postid);

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
	}
}
