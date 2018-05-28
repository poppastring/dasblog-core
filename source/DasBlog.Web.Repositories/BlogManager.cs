using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using DasBlog.Managers.Interfaces;
using DasBlog.Core;
using newtelligence.DasBlog.Util;
using Blogger = DasBlog.Core.XmlRpc.Blogger;
using MoveableType = DasBlog.Core.XmlRpc.MoveableType;
using MetaWeblog = DasBlog.Core.XmlRpc.MetaWeblog;
using DasBlog.Core.Security;
using DasBlog.Core.Exceptions;
using System.Security;
using System.IO;
using CookComputing.XmlRpc;
using System.Reflection;
using System.Xml.Serialization;
using newtelligence.DasBlog.Web.Services.Rss20;

namespace DasBlog.Managers
{
    [XmlRpcService(Name = "DasBlog Blogger Access Point", Description = "Implementation of Blogger XML-RPC Api")]
    public class BlogManager : IBlogManager, MoveableType.IMovableType, Blogger.IBlogger, MetaWeblog.IMetaWeblog
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private ISiteSecurityManager _siteSecurity;
        private readonly IDasBlogSettings _dasBlogSettings;

        public BlogManager(IDasBlogSettings settings, ISiteSecurityManager siteSecurityRepository)
        {
            _dasBlogSettings = settings;
            _siteSecurity = siteSecurityRepository;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        public Entry GetBlogPost(string postid)
        {
            return _dataService.GetEntry(postid);
        }

		public Entry GetEntryForEdit(string postid)
		{
			return _dataService.GetEntryForEdit(postid);
		}

		public EntryCollection GetFrontPagePosts()
        {
            DateTime fpDayUtc;
            TimeZone tz;

            //Need to insert the Request.Headers["Accept-Language"];
            string languageFilter = "en-US"; // Request.Headers["Accept-Language"];
            fpDayUtc = DateTime.UtcNow.AddDays(_dasBlogSettings.SiteConfiguration.ContentLookaheadDays);

            if (_dasBlogSettings.SiteConfiguration.AdjustDisplayTimeZone)
            {
                tz = WindowsTimeZone.TimeZones.GetByZoneIndex(_dasBlogSettings.SiteConfiguration.DisplayTimeZoneIndex);
            }
            else
            {
                tz = new UTCTimeZone();
            }

            return _dataService.GetEntriesForDay(fpDayUtc, TimeZone.CurrentTimeZone,
                                languageFilter,
                                _dasBlogSettings.SiteConfiguration.FrontPageDayCount, _dasBlogSettings.SiteConfiguration.FrontPageEntryCount, string.Empty);
        }

        public EntryCollection GetEntriesForPage(int pageIndex)
        {
            Predicate<Entry> pred = null;

            //Shallow copy as we're going to modify it...and we don't want to modify THE cache.
            EntryCollection cache = _dataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

            // remove the posts from the front page
            EntryCollection fp = GetFrontPagePosts();

            cache.RemoveRange(0, fp.Count);

            int entriesPerPage = _dasBlogSettings.SiteConfiguration.EntriesPerPage;

            // compensate for frontpage
            if ((pageIndex - 1) * entriesPerPage < cache.Count)
            {
                // Remove all entries before the current page's first entry.
                int end = (pageIndex - 1) * entriesPerPage;
                cache.RemoveRange(0, end);

                // Remove all entries after the page's last entry.
                if (cache.Count - entriesPerPage > 0)
                {
                    cache.RemoveRange(entriesPerPage, cache.Count - entriesPerPage);
                    // should match
                    bool postCount = cache.Count <= entriesPerPage;
                }

                return _dataService.GetEntries(null, EntryCollectionFilter.DefaultFilters.IsInEntryIdCacheEntryCollection(cache),
                    Int32.MaxValue,
                    Int32.MaxValue);
            }

            return new EntryCollection();
        }

		public EntrySaveState CreateEntry(Entry entry)
		{
			return InternalSaveEntry(entry, null, null);
		}

		public EntrySaveState UpdateEntry(Entry entry)
		{
			return InternalSaveEntry(entry, null, null);
		}

		public void DeleteEntry(string postid)
		{
			_dataService.DeleteEntry(postid, null);
		}

		private EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList)
		{

			EntrySaveState rtn = EntrySaveState.Failed;
			// we want to prepopulate the cross post collection with the crosspost footer
			if (_dasBlogSettings.SiteConfiguration.EnableCrossPostFooter && _dasBlogSettings.SiteConfiguration.CrossPostFooter != null 
				&& _dasBlogSettings.SiteConfiguration.CrossPostFooter.Length > 0)
			{
				foreach (CrosspostInfo info in crosspostList)
				{
					info.CrossPostFooter = _dasBlogSettings.SiteConfiguration.CrossPostFooter;
				}
			}

			// now save the entry, passign in all the necessary Trackback and Pingback info.
			try
			{
				// if the post is missing a title don't publish it
				if (entry.Title == null || entry.Title.Length == 0)
				{
					entry.IsPublic = false;
				}

				// if the post is missing categories, then set the categories to empty string.
				if (entry.Categories == null)
					entry.Categories = "";

				rtn = _dataService.SaveEntry(entry, 
					(_dasBlogSettings.SiteConfiguration.PingServices.Count > 0) ?
						new WeblogUpdatePingInfo(_dasBlogSettings.SiteConfiguration.Title, _dasBlogSettings.GetBaseUrl(), _dasBlogSettings.GetBaseUrl(), _dasBlogSettings.RsdUrl, _dasBlogSettings.SiteConfiguration.PingServices) : null,
					(entry.IsPublic) ?
						trackbackList : null,
					_dasBlogSettings.SiteConfiguration.EnableAutoPingback && entry.IsPublic ?
						new PingbackInfo(
							_dasBlogSettings.GetPermaLinkUrl(entry.EntryId),
							entry.Title,
							entry.Description,
							_dasBlogSettings.SiteConfiguration.Title) : null,
					crosspostList);

				//TODO: SendEmail(entry, siteConfig, logService);

			}
			catch (Exception ex)
			{
				//TODO: Do something with this????
				// StackTrace st = new StackTrace();
				// logService.AddEvent(new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), ""));
			}

			// we want to invalidate all the caches so users get the new post
			// TODO: BreakCache(entry.GetSplitCategories());

			return rtn;
		}

		private void BreakCache(string[] categories)
		{
			newtelligence.DasBlog.Web.Core.DataCache cache = newtelligence.DasBlog.Web.Core.CacheFactory.GetCache();

			// break the caching
			cache.Remove("BlogCoreData");
			cache.Remove("Rss::" + _dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + _dasBlogSettings.SiteConfiguration.RssEntryCount.ToString());

			foreach (string category in categories)
			{
				string CacheKey = "Rss:" + category + ":" + _dasBlogSettings.SiteConfiguration.RssDayCount.ToString() + ":" + _dasBlogSettings.SiteConfiguration.RssEntryCount.ToString();
				cache.Remove(CacheKey);
			}
		}

		public string XmlRpcInvoke(Stream requestStream)
        {
            try
            {
                XmlRpcSerializer xmlRpcSerializer = new XmlRpcSerializer();

                XmlRpcServiceAttribute xmlRpcServiceAttribute = (XmlRpcServiceAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(XmlRpcServiceAttribute));
                if (xmlRpcServiceAttribute != null)
                {
                    if (xmlRpcServiceAttribute.XmlEncoding != null)
                    {
                        xmlRpcSerializer.XmlEncoding = Encoding.GetEncoding(xmlRpcServiceAttribute.XmlEncoding);
                    }
                    xmlRpcSerializer.UseIntTag = xmlRpcServiceAttribute.UseIntTag;
                    xmlRpcSerializer.UseStringTag = xmlRpcServiceAttribute.UseStringTag;
                    xmlRpcSerializer.UseIndentation = xmlRpcServiceAttribute.UseIndentation;
                    xmlRpcSerializer.Indentation = xmlRpcServiceAttribute.Indentation;
                }

                var bodyStream = new StreamReader(requestStream);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

                XmlRpcRequest request = xmlRpcSerializer.DeserializeRequest(bodyStream, this.GetType());
                XmlRpcResponse response = this.Invoke(request);
                Stream stream = new MemoryStream();
                xmlRpcSerializer.SerializeResponse(stream, response);
                stream.Seek(0L, SeekOrigin.Begin);


                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                XmlRpcFaultException faultEx = (!(ex is XmlRpcException)) ? ((!(ex is XmlRpcFaultException)) ? new XmlRpcFaultException(0, ex.Message) : ((XmlRpcFaultException)ex)) : new XmlRpcFaultException(0, ((XmlRpcException)ex).Message);
                XmlRpcSerializer xmlRpcSerializer2 = new XmlRpcSerializer();
                Stream stream2 = new MemoryStream();
                xmlRpcSerializer2.SerializeFaultResponse(stream2, faultEx);
                stream2.Seek(0L, SeekOrigin.Begin);

                StreamReader reader2 = new StreamReader(stream2);
                return reader2.ReadToEnd();
            }
        }

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = _dataService.GetEntry(postid);

			if (entry != null)
			{
				// Are comments allowed

				_dataService.AddComment(comment);

				est = CommentSaveState.Added;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentSaveState DeleteComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = _dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				_dataService.DeleteComment(postid, commentid);

				est = CommentSaveState.Deleted;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentSaveState ApproveComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;
			Entry entry = _dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				_dataService.ApproveComment(postid, commentid);

				est = CommentSaveState.Approved;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentCollection GetComments(string postid, bool allComments)
		{
			return _dataService.GetCommentsFor(postid, allComments);
		}

		private XmlRpcResponse Invoke(XmlRpcRequest request)
        {
            MethodInfo methodInfo = null;
            methodInfo = (((object)request.mi == null) ? base.GetType().GetMethod(request.method) : request.mi);
            object retValue;
            try
            {
                retValue = methodInfo.Invoke((object)this, request.args);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }
                throw ex;
            }
            return new XmlRpcResponse(retValue);
        }

        public MoveableType.Category[] mt_getCategoryList(string blogid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new Core.Exceptions.ServiceDisabledException();
            }
			Core.Security.UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }
            return InternalGetCategoryList();
        }

        public MoveableType.Category[] mt_getPostCategories(string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }
            MoveableType.Category[] mcats = InternalGetCategoryList();
            Entry entry = _dataService.GetEntry(postid);
            if (entry != null)
            {
                List<MoveableType.Category> acats = new List<MoveableType.Category>();
                string[] cats = entry.GetSplitCategories();
                if (cats.Length > 0)
                {
                    foreach (string cat in cats)
                    {
                        foreach (MoveableType.Category mcat in mcats)
                        {
                            if (cat == mcat.categoryId)
                            {
                                MoveableType.Category cpcat = mcat;
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

        public MoveableType.PostTitle[] mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }
            EntryCollection entries = _dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), TimeZone.CurrentTimeZone, null,
                _dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
            List<MoveableType.PostTitle> arrayList = new List<MoveableType.PostTitle>();
            foreach (Entry entry in entries)
            {
                MoveableType.PostTitle post = new MoveableType.PostTitle();
                post.title = NoNull(entry.Title);
                post.dateCreated = entry.CreatedUtc;
                post.postid = NoNull(entry.EntryId);
                post.userid = username;
                arrayList.Add(post);
            }
            return arrayList.ToArray();
        }

        public MoveableType.TrackbackPing[] mt_getTrackbackPings(string postid)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            List<MoveableType.TrackbackPing> arrayList = new List<MoveableType.TrackbackPing>();
            foreach (Tracking trk in _dataService.GetTrackingsFor(postid))
            {
                if (trk.TrackingType == TrackingType.Trackback)
                {
                    MoveableType.TrackbackPing tp = new MoveableType.TrackbackPing();
                    tp.pingIP = "";
                    tp.pingTitle = NoNull(trk.RefererTitle);
                    tp.pingURL = NoNull(trk.PermaLink);
                    arrayList.Add(tp);
                }
            }
            return arrayList.ToArray();
        }

        public bool mt_publishPost(string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }

            // TODO: Fill out the Publish Post

            return true;
        }

        public bool mt_setPostCategories(string postid, string username, string password, MoveableType.Category[] categories)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }
            Entry entry = _dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                string cats = "";
                foreach (MoveableType.Category mcat in categories)
                {
                    if (cats.Length > 0)
                        cats += ";";
                    cats += mcat.categoryId;
                }
                entry.Categories = cats;
                _dataService.SaveEntry(entry);

                // TODO: give the XSS upstreamer a hint that things have changed
                //  XSSUpstreamer.TriggerUpstreaming();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string[] mt_supportedMethods()
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            List<string> arrayList = new List<string>();
            arrayList.Add("mt.getCategoryList");
            arrayList.Add("mt.getPostCategories");
            arrayList.Add("mt.getRecentPostTitles");
            arrayList.Add("mt.getTrackbackPings");
            arrayList.Add("mt.publishPost");
            arrayList.Add("mt.setPostCategories");
            arrayList.Add("mt.supportedMethods");
            arrayList.Add("mt.supportedTextFilters");

            return arrayList.ToArray();
        }

        public MoveableType.TextFilter[] mt_supportedTextFilters()
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            MoveableType.TextFilter tf = new MoveableType.TextFilter();
            tf.key = "default";
            tf.@value = "default";
            return new MoveableType.TextFilter[] { tf };
        }

        public MoveableType.Category[] InternalGetCategoryList()
        {
            List<MoveableType.Category> arrayList = new List<MoveableType.Category>();
            CategoryCacheEntryCollection categories = _dataService.GetCategories();
            if (categories.Count == 0)
            {
                arrayList.Add(InternalGetFrontPageCategory());
            }
            else
            {
                foreach (CategoryCacheEntry catEntry in categories)
                {
                    MoveableType.Category category = new MoveableType.Category();
                    category.categoryId = NoNull(catEntry.Name);
                    category.categoryName = NoNull(catEntry.Name);
                    //category.isPrimary=false;
                    arrayList.Add(category);
                }
            }
            return arrayList.ToArray();
        }

        private MoveableType.Category InternalGetFrontPageCategory()
        {
            MoveableType.Category mcat = new MoveableType.Category();
            mcat.categoryId = "Front Page";
            mcat.categoryName = "Front Page";
            //mcat.isPrimary = true;
            return mcat;
        }

        private string NoNull(string s)
        {
            if (s == null)
                return string.Empty;
            else
                return s;
        }

        public bool blogger_deletePost(string appKey, string postid, string username, string password, [XmlRpcParameter(Description = "Where applicable, this specifies whether the blog should be republished after the post has been deleted.")] bool publish)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }

            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            // TODO: Figure this out
            // SiteUtilities.DeleteEntry(postid, siteConfig, this.logService, this.dataService);

            return true;
        }

        public bool blogger_editPost(string appKey, string postid, string username, string password, string content, bool publish)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = _dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                FillEntryFromBloggerPost(entry, content, username);
                entry.IsPublic = publish;
                entry.Syndicated = publish;

                // TODO: Figure this out
                // SiteUtilities.SaveEntry(entry, siteConfig, this.logService, this.dataService);
            }
            return true;
        }

        public Blogger.Category[] blogger_getCategories(string blogid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            List<Blogger.Category> arrayList = new List<Blogger.Category>();
            CategoryCacheEntryCollection categories = _dataService.GetCategories();
            if (categories.Count == 0)
            {
                Blogger.Category bcat = new Blogger.Category();
                bcat.categoryid = "Front Page";
                bcat.description = "Front Page";
                bcat.htmlUrl = _dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
                bcat.rssUrl = _dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
                bcat.title = NoNull(bcat.description);
                arrayList.Add(bcat);
            }
            else foreach (CategoryCacheEntry cat in categories)
                {
                    Blogger.Category bcat = new Blogger.Category();
                    bcat.categoryid = NoNull(cat.Name);
                    bcat.description = NoNull(cat.Name);
                    bcat.htmlUrl = _dasBlogSettings.GetCategoryViewUrl(cat.Name);
                    bcat.rssUrl = _dasBlogSettings.GetCategoryViewUrl(cat.Name);  //Should this be GetRssCategoryUrl()
                    bcat.title = NoNull(cat.Name);
                    arrayList.Add(bcat);
                }
            return arrayList.ToArray();
        }

        public Blogger.Post blogger_getPost(string appKey, string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry entry = _dataService.GetEntry(postid);
            if (entry != null)
            {
                Blogger.Post post = new Blogger.Post();
                FillBloggerPostFromEntry(entry, ref post);
                return post;
            }
            else
            {
                return new Blogger.Post();
            }
        }

        public Blogger.Post[] blogger_getRecentPosts(string appKey, string blogid, string username, string password, int numberOfPosts)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }

            EntryCollection entries = _dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), _dasBlogSettings.GetConfiguredTimeZone(), 
                                            null, _dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
            List<Blogger.Post> arrayList = new List<Blogger.Post>();
            foreach (Entry entry in entries)
            {
                Blogger.Post post = new Blogger.Post();
                FillBloggerPostFromEntry(entry, ref post);
                arrayList.Add(post);
            }
            return arrayList.ToArray();
        }

        public string blogger_getTemplate(string appKey, string blogid, string username, string password, string templateType)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return "";
        }

        public Blogger.UserInfo blogger_getUserInfo(string appKey, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new SecurityException();
            }

            User user = _siteSecurity.GetUser(username);
            Blogger.UserInfo userInfo = new Blogger.UserInfo();

            userInfo.email = NoNull(user.EmailAddress);
            userInfo.url = NoNull(_dasBlogSettings.SiteConfiguration.Root);
            userInfo.firstname = "";
            userInfo.lastname = "";
            userInfo.nickname = NoNull(user.DisplayName);
            return userInfo;
        }

        public Blogger.BlogInfo[] blogger_getUsersBlogs(string appKey, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Blogger.BlogInfo[] blogs = new Blogger.BlogInfo[1];
            Blogger.BlogInfo blog = new Blogger.BlogInfo();
            blog.blogid = "0";
            blog.blogName = NoNull(_dasBlogSettings.SiteConfiguration.Title);
            blog.url = NoNull(_dasBlogSettings.SiteConfiguration.Root);
            blogs[0] = blog;
            return blogs;
        }

        public string blogger_newPost(string appKey, string blogid, string username, string password, string content, bool publish)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry newPost = new Entry();
            newPost.Initialize();
            FillEntryFromBloggerPost(newPost, content, username);
            newPost.IsPublic = publish;
            newPost.Syndicated = publish;

            // SiteUtilities.SaveEntry(newPost, siteConfig, this.logService, this.dataService);

            return newPost.EntryId;
        }

        public bool blogger_setTemplate(string appKey, string blogid, string username, string password, string template, string templateType)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return false;
        }

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

        private void FillBloggerPostFromEntry(Entry entry, ref Blogger.Post post)
        {
            post.content = NoNull(string.Format("<title>{0}</title>{1}", entry.Title, entry.Content));
            post.dateCreated = entry.CreatedUtc;
            post.postid = NoNull(entry.EntryId);
            post.userid = NoNull(entry.Author);
        }

        public bool metaweblog_editPost(string postid, string username, string password, MetaWeblog.Post post, bool publish)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = _dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                entry.Author = username;
                TrackbackInfoCollection trackbackList = FillEntryFromMetaWeblogPost(entry, post);

                entry.IsPublic = publish;
                entry.Syndicated = publish;

                // TODO: Figure out how to save here
                // SiteUtilities.SaveEntry(entry, trackbackList, siteConfig, this.logService, this.dataService);
            }
            return true;
        }

        public MetaWeblog.CategoryInfo[] metaweblog_getCategories(string blogid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            List<MetaWeblog.CategoryInfo> arrayList = new List<MetaWeblog.CategoryInfo>();
            CategoryCacheEntryCollection categories = _dataService.GetCategories();
            if (categories.Count == 0)
            {
                MetaWeblog.CategoryInfo bcat = new MetaWeblog.CategoryInfo();
                bcat.categoryid = "Front Page";
                bcat.description = "Front Page";
                bcat.htmlUrl = _dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
                bcat.rssUrl = _dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);  // TODO should this be GetRssCategoryUrl
                bcat.title = NoNull(bcat.description);
                arrayList.Add(bcat);
            }
            else
            {
                foreach (CategoryCacheEntry cat in categories)
                {
                    MetaWeblog.CategoryInfo bcat = new MetaWeblog.CategoryInfo();
                    bcat.categoryid = NoNull(cat.Name);
                    bcat.description = NoNull(cat.Name);
                    bcat.htmlUrl = _dasBlogSettings.GetCategoryViewUrl(cat.Name);
                    bcat.rssUrl = _dasBlogSettings.GetCategoryViewUrl(cat.Name);   // TODO should this be GetRssCategoryUrl
                    bcat.title = NoNull(cat.Name);
                    arrayList.Add(bcat);
                }
            }
            return arrayList.ToArray();
        }

        public MetaWeblog.Post metaweblog_getPost(string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            Entry entry = _dataService.GetEntry(postid);
            if (entry != null)
            {
                return this.Create(entry);
            }
            else
            {
                return new MetaWeblog.Post();
            }
        }

        public MetaWeblog.Post[] metaweblog_getRecentPosts(string blogid, string username, string password, int numberOfPosts)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            EntryCollection entries = _dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), _dasBlogSettings.GetConfiguredTimeZone(), null, 
                                                        _dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
            List<MetaWeblog.Post> arrayList = new List<MetaWeblog.Post>();
            foreach (Entry entry in entries)
            {
                arrayList.Add(this.Create(entry));
            }
            return arrayList.ToArray();
        }

        public string metaweblog_newPost(string blogid, string username, string password, MetaWeblog.Post post, bool publish)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
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

            // TODO: Figure out how to SaveEntry here
            // SiteUtilities.SaveEntry(newPost, trackbackList, siteConfig, this.logService, this.dataService);

            return newPost.EntryId;
        }

        public MetaWeblog.UrlInfo metaweblog_newMediaObject(object blogid, string username, string password, MetaWeblog.MediaType enc)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = _siteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }

            // Get the binary data
            string strPath = Path.Combine(_dasBlogSettings.RelativeToRoot(_dasBlogSettings.SiteConfiguration.BinariesDir), enc.name);

            // check if the name of the media type includes a subdirectory we need to create
            FileInfo fileInfo = new FileInfo(strPath);
            if (fileInfo.Directory.Exists == false && fileInfo.Directory.FullName != _dasBlogSettings.RelativeToRoot(_dasBlogSettings.SiteConfiguration.BinariesDir))
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

            string path = Path.Combine(_dasBlogSettings.SiteConfiguration.BinariesDirRelative, enc.name);
            MetaWeblog.UrlInfo urlInfo = new MetaWeblog.UrlInfo();
            urlInfo.url = _dasBlogSettings.RelativeToRoot(path);
            return urlInfo;
        }

        /// <summary>Fills a DasBlog entry from a MetaWeblog Post structure.</summary>
        /// <param name="entry">DasBlog entry to fill.</param>
        /// <param name="post">MetaWeblog post structure to fill from.</param>
        /// <returns>TrackbackInfoCollection of posts to send trackback pings.</returns>
        private TrackbackInfoCollection FillEntryFromMetaWeblogPost(Entry entry, MetaWeblog.Post post)
        {
            // W.Bloggar doesn't pass in the DataCreated, 
            // so we have to check for that
            if (post.dateCreated != DateTime.MinValue)
            {
                entry.CreatedUtc = post.dateCreated.ToUniversalTime();
            }

            //Patched to avoid html entities in title
            entry.Title = post.title; // TODO: Find out how to decode this...  HttpUtility.HtmlDecode(post.title);
            entry.Content = post.description;
            entry.Description = NoNull(post.mt_excerpt);

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
                        "", //TODO: Replace SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry)
                        entry.Title,
                        entry.Description,
                        _dasBlogSettings.SiteConfiguration.Title));
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

        private MetaWeblog.Post Create(Entry entry)
        {
            if (entry == null) throw new ArgumentNullException("entry");

            MetaWeblog.Post post = new MetaWeblog.Post();
            post.description = entry.Content ?? "";
            post.mt_excerpt = entry.Description ?? "";
            post.dateCreated = entry.CreatedUtc;
            post.title = entry.Title ?? "";
            post.link = post.permalink = _dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
            post.postid = entry.EntryId ?? "";
            post.categories = entry.GetSplitCategories();
            return post;
        }

		public CategoryCacheEntryCollection GetCategories()
		{
			return _dataService.GetCategories();
		}
	}
}
