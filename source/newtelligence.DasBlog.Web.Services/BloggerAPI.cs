#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using CookComputing.XmlRpc;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.Services.Blogger;
using newtelligence.DasBlog.Web.Services.MetaWeblog;
using newtelligence.DasBlog.Web.Services.MovableType;
using NodaTime;

namespace newtelligence.DasBlog.Web.Services
{
    [XmlRpcService(Name = "DasBlog Blogger Access Point", Description = "Implementation of Blogger XML-RPC Api")]
    public class BloggerAPI : XmlRpcService, IBlogger, IMovableType, IMetaWeblog
    {
        SiteConfig siteConfig;
        IBlogDataService dataService;
        ILoggingDataService logService;

        public BloggerAPI()
        {
            this.siteConfig = SiteConfig.GetSiteConfig();
            this.logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            this.dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);
        }

        private string noNull(string s)
        {
            if (s == null)
                return "";
            else
                return s;
        }

        bool IBlogger.blogger_deletePost(string appKey, string postid, string username, string password, bool publish)
        {
            // TODO: NLS - This doesn't seem to work through w.bloggar
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }

            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }


            SiteUtilities.DeleteEntry(postid, siteConfig, this.logService, this.dataService);

            return true;
        }

        bool IBlogger.blogger_editPost(string appKey, string postid, string username, string password, string content, bool publish)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                FillEntryFromBloggerPost(entry, content, username);
                entry.IsPublic = publish;
                entry.Syndicated = publish;
                SiteUtilities.SaveEntry(entry, siteConfig, this.logService, this.dataService);
            }
            return true;
        }

        newtelligence.DasBlog.Web.Services.Blogger.Category[] IBlogger.blogger_getCategories(string blogid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            List<newtelligence.DasBlog.Web.Services.Blogger.Category> arrayList = new List<newtelligence.DasBlog.Web.Services.Blogger.Category>();
            CategoryCacheEntryCollection categories = dataService.GetCategories();
            if (categories.Count == 0)
            {
                newtelligence.DasBlog.Web.Services.Blogger.Category bcat = new newtelligence.DasBlog.Web.Services.Blogger.Category();
                bcat.categoryid = "Front Page";
                bcat.description = "Front Page";
                bcat.htmlUrl = SiteUtilities.GetCategoryViewUrl(bcat.categoryid);
                bcat.rssUrl = SiteUtilities.GetRssCategoryUrl(bcat.categoryid);
                bcat.title = noNull(bcat.description);
                arrayList.Add(bcat);
            }
            else foreach (CategoryCacheEntry cat in categories)
                {
                    newtelligence.DasBlog.Web.Services.Blogger.Category bcat = new newtelligence.DasBlog.Web.Services.Blogger.Category();
                    bcat.categoryid = noNull(cat.Name);
                    bcat.description = noNull(cat.Name);
                    bcat.htmlUrl = SiteUtilities.GetCategoryViewUrl(cat.Name);
                    bcat.rssUrl = SiteUtilities.GetRssCategoryUrl(cat.Name);
                    bcat.title = noNull(cat.Name);
                    arrayList.Add(bcat);
                }
            return arrayList.ToArray();
        }

        newtelligence.DasBlog.Web.Services.Blogger.Post IBlogger.blogger_getPost(string appKey, string postid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry entry = dataService.GetEntry(postid);
            if (entry != null)
            {
                newtelligence.DasBlog.Web.Services.Blogger.Post post = new newtelligence.DasBlog.Web.Services.Blogger.Post();
                FillBloggerPostFromEntry(entry, ref post);
                return post;
            }
            else
            {
                return new newtelligence.DasBlog.Web.Services.Blogger.Post();
            }
        }

        newtelligence.DasBlog.Web.Services.Blogger.Post[] IBlogger.blogger_getRecentPosts(string appKey,
                                                           string blogid,
                                                           string username,
                                                           string password,
                                                           int numberOfPosts)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), DateTimeZone.Utc, null, SiteConfig.GetSiteConfig().RssDayCount, numberOfPosts, null);
            List<newtelligence.DasBlog.Web.Services.Blogger.Post> arrayList = new List<newtelligence.DasBlog.Web.Services.Blogger.Post>();
            foreach (Entry entry in entries)
            {
                newtelligence.DasBlog.Web.Services.Blogger.Post post = new newtelligence.DasBlog.Web.Services.Blogger.Post();
                FillBloggerPostFromEntry(entry, ref post);
                arrayList.Add(post);
            }
            return arrayList.ToArray();
        }

        string IBlogger.blogger_getTemplate(string appKey, string blogid, string username, string password, string templateType)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return "";
        }

        newtelligence.DasBlog.Web.Services.Blogger.UserInfo IBlogger.blogger_getUserInfo(string appKey, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            User user = SiteSecurity.GetUser(username);
            newtelligence.DasBlog.Web.Services.Blogger.UserInfo userInfo = new newtelligence.DasBlog.Web.Services.Blogger.UserInfo();

            userInfo.email = noNull(user.EmailAddress);
            userInfo.url = noNull(siteConfig.Root);
            userInfo.firstname = "";
            userInfo.lastname = "";
            userInfo.nickname = noNull(user.DisplayName);
            return userInfo;
        }

        newtelligence.DasBlog.Web.Services.Blogger.BlogInfo[] IBlogger.blogger_getUsersBlogs(string appKey, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            BlogInfo[] blogs = new BlogInfo[1];
            BlogInfo blog = new BlogInfo();
            blog.blogid = "0";
            blog.blogName = noNull(siteConfig.Title);
            blog.url = noNull(siteConfig.Root);
            blogs[0] = blog;
            return blogs;
        }

        string IBlogger.blogger_newPost(
            string appKey,
            string blogid,
            string username,
            string password,
            string content,
            bool publish)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry newPost = new Entry();
            newPost.Initialize();
            FillEntryFromBloggerPost(newPost, content, username);
            newPost.IsPublic = publish;
            newPost.Syndicated = publish;


            SiteUtilities.SaveEntry(newPost, siteConfig, this.logService, this.dataService);

            return newPost.EntryId;
        }

        bool IBlogger.blogger_setTemplate(string appKey, string blogid, string username, string password, string template, string templateType)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return false;
        }

        /// <summary>Fills a DasBlog entry from a Blogger Post structure.</summary>
        /// <param name="entry">DasBlog entry to fill.</param>
        /// <param name="content">Content string of the Blogger post</param>
        /// <param name="username">Username of the author</param>
        private void FillEntryFromBloggerPost(Entry entry, string content, string username)
        {
            const string TitleStart = "<title>";
            const string TitleStop = "</title>";

            // The title can optionally be sent inline with the content as an XML fragment.  Make sure to parse it out correctly
            // Can't use normal XML processing tools because it's not well formed.  Have to do this the old fashioned way.
            string title = "";
            string lowerContent = content.ToLower();

            int nTitleStart = lowerContent.IndexOf(TitleStart);
            if (nTitleStart >= 0)
            {
                nTitleStart += TitleStart.Length;
                int nTitleStop = lowerContent.IndexOf(TitleStop, nTitleStart);

                title = content.Substring(nTitleStart, nTitleStop - nTitleStart);
                content = content.Substring(nTitleStop + TitleStop.Length);
            }

            //newPost.CreatedUtc = DateTime.Now.ToUniversalTime();

            entry.Title = title;
            entry.Description = "";
            entry.Content = content;
            entry.Author = username;
        }

        private void FillBloggerPostFromEntry(Entry entry, ref newtelligence.DasBlog.Web.Services.Blogger.Post post)
        {
            post.content = noNull(string.Format("<title>{0}</title>{1}", entry.Title, entry.Content));
            post.dateCreated = entry.CreatedUtc;
            post.postid = noNull(entry.EntryId);
            post.userid = noNull(entry.Author);
        }

        private newtelligence.DasBlog.Web.Services.MovableType.Category InternalGetFrontPageCategory()
        {
            newtelligence.DasBlog.Web.Services.MovableType.Category mcat = new newtelligence.DasBlog.Web.Services.MovableType.Category();
            mcat.categoryId = "Front Page";
            mcat.categoryName = "Front Page";
            //mcat.isPrimary = true;
            return mcat;
        }

        private newtelligence.DasBlog.Web.Services.MovableType.Category[] InternalGetCategoryList()
        {
            List<newtelligence.DasBlog.Web.Services.MovableType.Category> arrayList = new List<newtelligence.DasBlog.Web.Services.MovableType.Category>();
            CategoryCacheEntryCollection categories = dataService.GetCategories();
            if (categories.Count == 0)
            {
                arrayList.Add(InternalGetFrontPageCategory());
            }
            else
            {
                foreach (CategoryCacheEntry catEntry in categories)
                {
                    newtelligence.DasBlog.Web.Services.MovableType.Category category = new newtelligence.DasBlog.Web.Services.MovableType.Category();
                    category.categoryId = noNull(catEntry.Name);
                    category.categoryName = noNull(catEntry.Name);
                    //category.isPrimary=false;
                    arrayList.Add(category);
                }
            }
            return arrayList.ToArray();
        }

        // MoveableType
        newtelligence.DasBlog.Web.Services.MovableType.Category[] IMovableType.mt_getCategoryList(string blogid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return InternalGetCategoryList();
        }

        newtelligence.DasBlog.Web.Services.MovableType.Category[] IMovableType.mt_getPostCategories(string postid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            newtelligence.DasBlog.Web.Services.MovableType.Category[] mcats = InternalGetCategoryList();
            Entry entry = dataService.GetEntry(postid);
            if (entry != null)
            {
                List<newtelligence.DasBlog.Web.Services.MovableType.Category> acats = new List<newtelligence.DasBlog.Web.Services.MovableType.Category>();
                string[] cats = entry.GetSplitCategories();
                if (cats.Length > 0)
                {
                    foreach (string cat in cats)
                    {
                        foreach (newtelligence.DasBlog.Web.Services.MovableType.Category mcat in mcats)
                        {
                            if (cat == mcat.categoryId)
                            {
                                newtelligence.DasBlog.Web.Services.MovableType.Category cpcat = mcat;
                                cpcat.isPrimary = (acats.Count == 0);
                                acats.Add(cpcat);
                                break;
                            }
                        }
                    }
                }
                if (acats.Count == 0)
                {
                    acats.Add(InternalGetFrontPageCategory());
                }
                return acats.ToArray();
            }
            else
            {
                return null;
            }
        }

        newtelligence.DasBlog.Web.Services.MovableType.PostTitle[] IMovableType.mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), DateTimeZone.Utc, null, SiteConfig.GetSiteConfig().RssDayCount, numberOfPosts, null);
            List<newtelligence.DasBlog.Web.Services.MovableType.PostTitle> arrayList = new List<newtelligence.DasBlog.Web.Services.MovableType.PostTitle>();
            foreach (Entry entry in entries)
            {
                newtelligence.DasBlog.Web.Services.MovableType.PostTitle post = new newtelligence.DasBlog.Web.Services.MovableType.PostTitle();
                post.title = noNull(entry.Title);
                post.dateCreated = entry.CreatedUtc;
                post.postid = noNull(entry.EntryId);
                post.userid = username;
                arrayList.Add(post);
            }
            return arrayList.ToArray();
        }

        newtelligence.DasBlog.Web.Services.MovableType.TrackbackPing[] IMovableType.mt_getTrackbackPings(string postid)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            List<newtelligence.DasBlog.Web.Services.MovableType.TrackbackPing> arrayList = new List<newtelligence.DasBlog.Web.Services.MovableType.TrackbackPing>();
            foreach (Tracking trk in dataService.GetTrackingsFor(postid))
            {
                if (trk.TrackingType == TrackingType.Trackback)
                {
                    newtelligence.DasBlog.Web.Services.MovableType.TrackbackPing tp = new newtelligence.DasBlog.Web.Services.MovableType.TrackbackPing();
                    tp.pingIP = "";
                    tp.pingTitle = noNull(trk.RefererTitle);
                    tp.pingURL = noNull(trk.PermaLink);
                    arrayList.Add(tp);
                }
            }
            return arrayList.ToArray();
        }

        bool IMovableType.mt_publishPost(string postid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return true;
        }

        bool IMovableType.mt_setPostCategories(string postid, string username, string password, newtelligence.DasBlog.Web.Services.MovableType.Category[] categories)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                string cats = "";
                foreach (newtelligence.DasBlog.Web.Services.MovableType.Category mcat in categories)
                {
                    if (cats.Length > 0)
                        cats += ";";
                    cats += mcat.categoryId;
                }
                entry.Categories = cats;
                dataService.SaveEntry(entry);
                // give the XSS upstreamer a hint that things have changed
                XSSUpstreamer.TriggerUpstreaming();
                return true;
            }
            else
            {
                return false;
            }
        }

        string[] IMovableType.mt_supportedMethods()
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            List<string> arrayList = new List<string>();
            foreach (MethodInfo method in GetType().GetMethods())
            {
                if (method.IsDefined(typeof(XmlRpcMethodAttribute), true))
                {
                    XmlRpcMethodAttribute attr = method.GetCustomAttributes(typeof(XmlRpcMethodAttribute), true)[0] as XmlRpcMethodAttribute;
                    arrayList.Add(attr.Method);
                }
            }
            return arrayList.ToArray();

        }

        newtelligence.DasBlog.Web.Services.MovableType.TextFilter[] IMovableType.mt_supportedTextFilters()
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            TextFilter tf = new TextFilter();
            tf.key = "default";
            tf.@value = "default";
            return new newtelligence.DasBlog.Web.Services.MovableType.TextFilter[] { tf };
        }

        // Metaweblog
        bool IMetaWeblog.metaweblog_editPost(string postid, string username, string password, newtelligence.DasBlog.Web.Services.MetaWeblog.Post post, bool publish)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                entry.Author = username;
                TrackbackInfoCollection trackbackList = FillEntryFromMetaWeblogPost(entry, post);

                entry.IsPublic = publish;
                entry.Syndicated = publish;

                // give the XSS upstreamer a hint that things have changed
                //FIX: XSSUpstreamer.TriggerUpstreaming();

                SiteUtilities.SaveEntry(entry, trackbackList, siteConfig, this.logService, this.dataService);
            }
            return true;
        }

        newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo[] IMetaWeblog.metaweblog_getCategories(string blogid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            List<newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo> arrayList = new List<newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo>();
            CategoryCacheEntryCollection categories = dataService.GetCategories();
            if (categories.Count == 0)
            {
                newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo bcat = new newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo();
                bcat.categoryid = "Front Page";
                bcat.description = "Front Page";
                bcat.htmlUrl = SiteUtilities.GetCategoryViewUrl(bcat.categoryid);
                bcat.rssUrl = SiteUtilities.GetRssCategoryUrl(bcat.categoryid);
                bcat.title = noNull(bcat.description);
                arrayList.Add(bcat);
            }
            else
            {
                foreach (CategoryCacheEntry cat in categories)
                {
                    newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo bcat = new newtelligence.DasBlog.Web.Services.MetaWeblog.CategoryInfo();
                    bcat.categoryid = noNull(cat.Name);
                    bcat.description = noNull(cat.Name);
                    bcat.htmlUrl = SiteUtilities.GetCategoryViewUrl(cat.Name);
                    bcat.rssUrl = SiteUtilities.GetRssCategoryUrl(cat.Name);
                    bcat.title = noNull(cat.Name);
                    arrayList.Add(bcat);
                }
            }
            return arrayList.ToArray();
        }

        newtelligence.DasBlog.Web.Services.MetaWeblog.Post IMetaWeblog.metaweblog_getPost(string postid, string username, string password)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry entry = dataService.GetEntry(postid);
            if (entry != null)
            {
                return MetaWeblog.Post.Create(entry);
            }
            else
            {
                return new newtelligence.DasBlog.Web.Services.MetaWeblog.Post();
            }
        }

        newtelligence.DasBlog.Web.Services.MetaWeblog.Post[] IMetaWeblog.metaweblog_getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), DateTimeZone.Utc, null, SiteConfig.GetSiteConfig().RssDayCount, numberOfPosts, null);
            List<newtelligence.DasBlog.Web.Services.MetaWeblog.Post> arrayList = new List<newtelligence.DasBlog.Web.Services.MetaWeblog.Post>();
            foreach (Entry entry in entries)
            {
                arrayList.Add(MetaWeblog.Post.Create(entry));
            }
            return arrayList.ToArray();
        }

        string IMetaWeblog.metaweblog_newPost(string blogid, string username, string password, newtelligence.DasBlog.Web.Services.MetaWeblog.Post post, bool publish)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry newPost = new Entry();
            newPost.Initialize();
            newPost.Author = username;

            TrackbackInfoCollection trackbackList = FillEntryFromMetaWeblogPost(newPost, post);

            newPost.IsPublic = publish;
            newPost.Syndicated = publish;
            // give the XSS upstreamer a hint that things have changed
            //FIX: XSSUpstreamer.TriggerUpstreaming();

            SiteUtilities.SaveEntry(newPost, trackbackList, siteConfig, this.logService, this.dataService);

            return newPost.EntryId;
        }

        /// <summary>Fills a DasBlog entry from a MetaWeblog Post structure.</summary>
        /// <param name="entry">DasBlog entry to fill.</param>
        /// <param name="post">MetaWeblog post structure to fill from.</param>
        /// <returns>TrackbackInfoCollection of posts to send trackback pings.</returns>
        private TrackbackInfoCollection FillEntryFromMetaWeblogPost(Entry entry, newtelligence.DasBlog.Web.Services.MetaWeblog.Post post)
        {
            // W.Bloggar doesn't pass in the DataCreated, 
            // so we have to check for that
            if (post.dateCreated != DateTime.MinValue)
            {
                entry.CreatedUtc = post.dateCreated.ToUniversalTime();
            }

            //Patched to avoid html entities in title
            entry.Title = HttpUtility.HtmlDecode(post.title);
            entry.Content = post.description;
            entry.Description = noNull(post.mt_excerpt);

            // If mt_allow_comments is null, then the sender did not specify.  Use default dasBlog behavior in that case
            if (post.mt_allow_comments != null)
            {
                int nAllowComments = Convert.ToInt32(post.mt_allow_comments);
                if (nAllowComments == 0 || nAllowComments == 2)
                    entry.AllowComments = false;
                else
                    entry.AllowComments = true;
            }

            if (post.categories != null && post.categories.Length > 0)
            {
                // handle categories
                string categories = "";

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                bool needSemi = false;
                post.categories = RemoveDups(post.categories, true);
                foreach (string category in post.categories)
                {
                    //watch for "" as a category
                    if (category.Length > 0)
                    {
                        if (needSemi) sb.Append(";");
                        sb.Append(category);
                        needSemi = true;
                    }
                }
                categories = sb.ToString();

                if (categories.Length > 0)
                    entry.Categories = categories;
            }

            // We'll always return at least an empty collection
            TrackbackInfoCollection trackbackList = new TrackbackInfoCollection();

            // Only MT supports trackbacks in the post
            if (post.mt_tb_ping_urls != null)
            {
                foreach (string trackbackUrl in post.mt_tb_ping_urls)
                {
                    trackbackList.Add(new TrackbackInfo(
                        trackbackUrl,
                        SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry),
                        entry.Title,
                        entry.Description,
                        siteConfig.Title));
                }
            }
            return trackbackList;
        }

        private string[] RemoveDups(string[] items, bool sort)
        {
            List<string> noDups = new List<string>();
            for (int i = 0; i < items.Length; i++)
            {
                if (!noDups.Contains(items[i].Trim()))
                {
                    noDups.Add(items[i].Trim());
                }
            }
            if (sort) noDups.Sort();  //sorts list alphabetically
            string[] uniqueItems = new String[noDups.Count];
            noDups.CopyTo(uniqueItems);
            return uniqueItems;
        }
        /// <summary>
        /// newMediaObject implementation : Xas
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        UrlInfo IMetaWeblog.metaweblog_newMediaObject(object blogid, string username, string password, MediaType enc)
        {
            if (!siteConfig.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            // Get the binary data
            string strPath = Path.Combine(SiteConfig.GetBinariesPathFromCurrentContext(), enc.name);

            // check if the name of the media type includes a subdirectory we need to create
            FileInfo fileInfo = new FileInfo(strPath);
            if (fileInfo.Directory.Exists == false && fileInfo.Directory.FullName != SiteConfig.GetBinariesPathFromCurrentContext())
            {
                fileInfo.Directory.Create();
            }

            try
            {
                using (FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(enc.bits);
                    }
                }
            }
            catch (Exception e)
            {
                throw new XmlRpcException(e.ToString());
            }

            string path = Path.Combine(siteConfig.BinariesDirRelative, enc.name);
            UrlInfo urlInfo = new UrlInfo();
            urlInfo.url = SiteUtilities.RelativeToRoot(siteConfig, path); ;
            return urlInfo;
        }
    }
}
