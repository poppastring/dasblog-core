using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Web.Caching;
//using System.Collections.Specialized;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Web.Core
{
    public class IPBlackList : IHttpModule
    {
        private EventHandler onBeginRequest;

        public IPBlackList()
        {
            onBeginRequest = new EventHandler(this.HandleBeginRequest);
        }

        void IHttpModule.Dispose()
        {
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.BeginRequest += onBeginRequest;
        }

        //FIX: hardcoded filepath
        const string BLOCKEDIPSFILE = "SiteConfig/blockedips.config";
        const string BLOCKEDIPSCACHEKEY = "blockedips";

        public static HashSet<string> GetBlockedIPs(HttpContext context)
        {
            DataCache cache = CacheFactory.GetCache();

            HashSet<string> ips = (HashSet<string>)cache[BLOCKEDIPSCACHEKEY];
            if (ips == null)
            {
                ips = GetBlockedIPs(GetBlockedIPsFilePathFromCurrentContext(context));
                cache.Insert(BLOCKEDIPSCACHEKEY, ips, new CacheDependency(GetBlockedIPsFilePathFromCurrentContext(context)));
            }
            return ips;
        }

        private static string BlockedIPFileName = null;
        private static object blockedIPFileNameObject = new object();
        public static string GetBlockedIPsFilePathFromCurrentContext(HttpContext context)
        {
            if (BlockedIPFileName != null)
                return BlockedIPFileName;
            lock (blockedIPFileNameObject)
            {
                if (BlockedIPFileName == null)
                {
                    BlockedIPFileName = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + BLOCKEDIPSFILE).FullName;
                }
            }
            return BlockedIPFileName;
        }

        public static HashSet<string> GetBlockedIPs(string configPath)
        {
            HashSet<string> retval = new HashSet<string>();
            using (StreamReader sr = new StreamReader(configPath))
            {
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length != 0)
                    {
                        retval.Add(line);
                    }
                }
            }
            return retval;
        }

        private void HandleBeginRequest(object sender, EventArgs evargs)
        {
            HttpApplication app = sender as HttpApplication;

            if (app != null)
            {
                string IPAddr = app.Context.Request.ServerVariables["REMOTE_ADDR"];
                if (string.IsNullOrEmpty(IPAddr))
                {
                    return;
                }

                //Block the PHPBB worm.
                if (app.Context.Request.QueryString["rush"] != null)
                {
                    app.Context.Response.StatusCode = 404;
                    app.Context.Response.SuppressContent = true;
                    app.Context.Response.End();
                    return;
                }

                HashSet<string> badIPs = GetBlockedIPs(app.Context);
                if (badIPs != null && badIPs.Contains(IPAddr))
                {
                    app.Context.Response.StatusCode = 404;
                    app.Context.Response.SuppressContent = true;
                    app.Context.Response.End();
                    return;
                }
            }
        }
    }
}
