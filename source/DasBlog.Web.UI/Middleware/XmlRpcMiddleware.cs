using CookComputing.XmlRpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;
using System;
using System.IO;
using System.Threading.Tasks;
using DasBlog.Web.UI.Core.XmlRpc.MoveableType;
using DasBlog.Web.UI.Core;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;
using System.Reflection;

namespace DasBlog.Web.UI.Middleware
{
    public class XmlRpcMiddleware : XmlRpcServerProtocol, IMovableType
    {
        private readonly IDasBlogSettings _dasBlogSettings;

        public XmlRpcMiddleware(RequestDelegate next, IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await HandleHttpRequest(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // Need to log the problem somewhere????
            }
        }

        public async Task HandleHttpRequest(HttpContext context)
        {
            if (context.Request.Method == "GET")
            {
                XmlRpcServiceAttribute xmlRpcServiceAttribute = (XmlRpcServiceAttribute)Attribute.GetCustomAttribute(base.GetType(), typeof(XmlRpcServiceAttribute));
                if (xmlRpcServiceAttribute != null && !xmlRpcServiceAttribute.AutoDocumentation)
                {
                    HandleUnsupportedMethod(context);
                }
                else
                {
                    bool autoDocVersion = true;
                    if (xmlRpcServiceAttribute != null)
                    {
                        autoDocVersion = xmlRpcServiceAttribute.AutoDocVersion;
                    }
                    await HandleGET(context, autoDocVersion);
                }
            }
            else if (context.Request.Method != "POST")
            {
                HandleUnsupportedMethod(context);
            }
            else
            {
                Stream src = base.Invoke(context.Request.Body);
                Stream outputStream = context.Response.Body;
                Util.CopyStream(src, outputStream);
                outputStream.Flush();
                context.Response.ContentType = "text/xml";
            }
        }

        protected async Task HandleGET(HttpContext context, bool autoDocVersion)
        {
            // Microsoft.AspNetCore.Html.

            // HtmlTextWriter wrtr = new HtmlTextWriter(context.Response.Body);
            // XmlRpcDocWriter.WriteDoc(wrtr, base.GetType(), autoDocVersion);

            context.Response.StatusCode = StatusCodes.Status200OK;
        }

        protected void HandleUnsupportedMethod(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
        }

        Category[] IMovableType.mt_getCategoryList(string blogid, string username, string password)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                // throw new ServiceDisabledException();
            }
            UserToken token = SiteSecurity.Login(username, password);
            if (token == null)
            {
                throw new System.Security.SecurityException();
            }
            return InternalGetCategoryList();
        }

        Category[] IMovableType.mt_getPostCategories(string postid, string username, string password)
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

            Category[] mcats = InternalGetCategoryList();
            Entry entry = dataService.GetEntry(postid);
            if (entry != null)
            {
                List<Category> acats = new List<Category>();
                string[] cats = entry.GetSplitCategories();
                if (cats.Length > 0)
                {
                    foreach (string cat in cats)
                    {
                        foreach (Category mcat in mcats)
                        {
                            if (cat == mcat.categoryId)
                            {
                                Category cpcat = mcat;
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

        PostTitle[] IMovableType.mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts)
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
            EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), new Util.UTCTimeZone(), null, SiteConfig.GetSiteConfig().RssDayCount, numberOfPosts, null);
            List<PostTitle> arrayList = new List<PostTitle>();
            foreach (Entry entry in entries)
            {
                PostTitle post = new PostTitle();
                post.title = noNull(entry.Title);
                post.dateCreated = entry.CreatedUtc;
                post.postid = noNull(entry.EntryId);
                post.userid = username;
                arrayList.Add(post);
            }

            return arrayList.ToArray();
        }

        TrackbackPing[] IMovableType.mt_getTrackbackPings(string postid)
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }

            List<TrackbackPing> arrayList = new List<TrackbackPing>();
            foreach (Tracking trk in dataService.GetTrackingsFor(postid))
            {
                if (trk.TrackingType == TrackingType.Trackback)
                {
                    TrackbackPing tp = new TrackbackPing();
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

        bool IMovableType.mt_setPostCategories(string postid, string username, string password, Category[] categories)
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

            Entry entry = dataService.GetEntryForEdit(postid);
            if (entry != null)
            {
                string cats = "";
                foreach (Category mcat in categories)
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

        TextFilter[] IMovableType.mt_supportedTextFilters()
        {
            if (!_dasBlogSettings.SiteConfiguration.EnableBloggerApi)
            {
                throw new ServiceDisabledException();
            }

            TextFilter tf = new TextFilter();
            tf.key = "default";
            tf.@value = "default";
            return new TextFilter[] { tf };
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

        private Category InternalGetFrontPageCategory()
        {
            Category mcat = new Category();
            mcat.categoryId = "Front Page";
            mcat.categoryName = "Front Page";
            //mcat.isPrimary = true;
            return mcat;
        }

        private Category[] InternalGetCategoryList()
        {
            List<Category> arrayList = new List<Category>();
            CategoryCacheEntryCollection categories = dataService.GetCategories();
            if (categories.Count == 0)
            {
                arrayList.Add(InternalGetFrontPageCategory());
            }
            else
            {
                foreach (CategoryCacheEntry catEntry in categories)
                {
                    Category category = new Category();
                    category.categoryId = noNull(catEntry.Name);
                    category.categoryName = noNull(catEntry.Name);
                    arrayList.Add(category);
                }
            }
            return arrayList.ToArray();
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
