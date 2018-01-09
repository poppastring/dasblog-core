using System;
using System.Collections.Generic;
using System.Text;
using CookComputing.XmlRpc;
using MovableType = newtelligence.DasBlog.Web.Services.MovableType;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Services;
using newtelligence.DasBlog.Web;
using System.Reflection;

namespace DasBlog.Web.Core
{
    [XmlRpcService(Name = "DasBlog Blogger Access Point", Description = "Implementation of Blogger XML-RPC Api")]
    public class BloggerAPI : XmlRpcService, MovableType.IMovableType
    {
        private IBlogDataService _dataService;
        private ILoggingDataService _loggingDataService;
        private readonly IDasBlogSettings _dasBlogSettings;

        public BloggerAPI(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
            _loggingDataService = LoggingDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.LogDir);
            _dataService = BlogDataServiceFactory.GetService(_dasBlogSettings.WebRootDirectory + _dasBlogSettings.SiteConfiguration.ContentDir, _loggingDataService);
        }

        private MovableType.Category[] InternalGetCategoryList()
        {
            List<MovableType.Category> arrayList = new List<MovableType.Category>();
            CategoryCacheEntryCollection categories = _dataService.GetCategories();
            if (categories.Count == 0)
            {
                arrayList.Add(InternalGetFrontPageCategory());
            }
            else
            {
                foreach (CategoryCacheEntry catEntry in categories)
                {
                    MovableType.Category category = new MovableType.Category();
                    category.categoryId = noNull(catEntry.Name);
                    category.categoryName = noNull(catEntry.Name);
                    //category.isPrimary=false;
                    arrayList.Add(category);
                }
            }
            return arrayList.ToArray();
        }

        MovableType.Category[] MovableType.IMovableType.mt_getCategoryList(string blogid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
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

        MovableType.Category[] MovableType.IMovableType.mt_getPostCategories(string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            MovableType.Category[] mcats = InternalGetCategoryList();
            Entry entry = _dataService.GetEntry(postid);
            if (entry != null)
            {
                List<MovableType.Category> acats = new List<MovableType.Category>();
                string[] cats = entry.GetSplitCategories();
                if (cats.Length > 0)
                {
                    foreach (string cat in cats)
                    {
                        foreach (MovableType.Category mcat in mcats)
                        {
                            if (cat == mcat.categoryId)
                            {
                                MovableType.Category cpcat = mcat;
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

        MovableType.PostTitle[] MovableType.IMovableType.mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            EntryCollection entries = _dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), TimeZone.CurrentTimeZone, null, 
                _dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
            List<MovableType.PostTitle> arrayList = new List<MovableType.PostTitle>();
            foreach (Entry entry in entries)
            {
                MovableType.PostTitle post = new MovableType.PostTitle();
                post.title = noNull(entry.Title);
                post.dateCreated = entry.CreatedUtc;
                post.postid = noNull(entry.EntryId);
                post.userid = username;
                arrayList.Add(post);
            }
            return arrayList.ToArray();
        }

        MovableType.TrackbackPing[] MovableType.IMovableType.mt_getTrackbackPings(string postid)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            List<MovableType.TrackbackPing> arrayList = new List<MovableType.TrackbackPing>();
            foreach (Tracking trk in _dataService.GetTrackingsFor(postid))
            {
                if (trk.TrackingType == TrackingType.Trackback)
                {
                    MovableType.TrackbackPing tp = new MovableType.TrackbackPing();
                    tp.pingIP = "";
                    tp.pingTitle = noNull(trk.RefererTitle);
                    tp.pingURL = noNull(trk.PermaLink);
                    arrayList.Add(tp);
                }
            }
            return arrayList.ToArray();
        }

        bool MovableType.IMovableType.mt_publishPost(string postid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
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

        bool MovableType.IMovableType.mt_setPostCategories(string postid, string username, string password, MovableType.Category[] categories)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            Entry entry = _dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                string cats = "";
                foreach (MovableType.Category mcat in categories)
                {
                    if (cats.Length > 0)
                        cats += ";";
                    cats += mcat.categoryId;
                }
                entry.Categories = cats;
                _dataService.SaveEntry(entry);
                // give the XSS upstreamer a hint that things have changed
                XSSUpstreamer.TriggerUpstreaming();
                return true;
            }
            else
            {
                return false;
            }
        }

        string[] MovableType.IMovableType.mt_supportedMethods()
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
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

        MovableType.TextFilter[] MovableType.IMovableType.mt_supportedTextFilters()
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }
            MovableType.TextFilter tf = new MovableType.TextFilter();
            tf.key = "default";
            tf.@value = "default";
            return new MovableType.TextFilter[] { tf };
        }

        private MovableType.Category InternalGetFrontPageCategory()
        {
            MovableType.Category mcat = new MovableType.Category();
            mcat.categoryId = "Front Page";
            mcat.categoryName = "Front Page";
            //mcat.isPrimary = true;
            return mcat;
        }

        private string noNull(string s)
        {
            if (s == null)
                return "";
            else
                return s;
        }
    }
}
