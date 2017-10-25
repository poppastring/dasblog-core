using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.Services.Rss20;
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Web.Services
{
	public class TimelineHandler : IHttpHandler
	{
		public TimelineHandler(){}


        public void ProcessRequest( HttpContext context )
        {
            try
            {

                //Cache the sitemap for 8 hours...
                DataCache cache = CacheFactory.GetCache();
				string CacheKey = "TimelineXml";
				timeline root = cache[CacheKey] as timeline ;
				if (root == null) //we'll have to build it...
				{
					ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
					IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);
					SiteConfig siteConfig = SiteConfig.GetSiteConfig();

					root = new timeline();

					root.events = new eventCollection();

					int i = 0;

					//All Pages (stop after 750...it gets too big and the browser can't handle it...we'd need
					// to include dynamic paging
					EntryCollection entryCache = dataService.GetEntries(false); 
					//Fortunately this comes in ordered by post date, descending
					foreach(Entry e in entryCache)
					{
						if (e.IsPublic && (++i < 750))
						{
							//then add permalinks
                            string url = SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)e);
                            @event foo = new @event(e.CreatedLocalTime, false, TruncateDotDotDot(StripAllTags(e.Title), 50), url);
							foo.text += String.Format("<div align=\"right\"><a href=\"{0}\">More...</a></div>",url);
							root.events.Add(foo);
						}
					}
				
					cache.Insert(CacheKey,root,DateTime.Now.AddHours(8));
				}

				XmlSerializer x = new XmlSerializer(typeof(timeline));
				x.Serialize(HttpContext.Current.Response.OutputStream,root);
				HttpContext.Current.Response.ContentType = "text/xml";
			}
            catch(Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
            }
        }

		private string TruncateDotDotDot(string content, int maxlen)
		{
			string retVal = StripAllTags(content); //Important to strip tags FIRST or we could truncate in the middle of a TAG, rather than in the middle of content.
			if (retVal.Length > maxlen)
				retVal = retVal.Substring(0, Math.Min(maxlen, retVal.Length)) + "...";
			return retVal;
		}
		
		private string StripAllTags(string content)
		{
			string regex = "<(.|\n)+?>";
			System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);
			string retVal = content;
			try
			{
				retVal = reg.Replace(content, String.Empty);
			}
			catch
			{ //swallow anything that might go wrong
			}
			return retVal;
		}
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
	}
}
