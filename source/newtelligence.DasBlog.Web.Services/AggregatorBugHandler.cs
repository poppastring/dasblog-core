using System;
using System.Web;
using System.Web.Mail;
using System.Diagnostics;
using System.Xml;
using System.Drawing;
using System.Reflection;
using System.IO;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;


namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for AggregatorBugHandler.
	/// </summary>
	public class AggregatorBugHandler : IHttpHandler 
	{
        static byte[] aggBugBitmap;

        static AggregatorBugHandler()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            
            using ( Stream stm = asm.GetManifestResourceStream(asm.GetName().Name+".aggbug.gif") )
            {
                aggBugBitmap = new byte[stm.Length];
                stm.Read(aggBugBitmap,0,(int)stm.Length);
            }
        }

		public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        public void ProcessRequest( HttpContext context )
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
                          
            string sourceId;
            string referrerUrl;

            /*
             * This service is a bit diffent from the other handlers, because 
             * the "bugs" are out in the wild regardless of whether the service is
             * enabled. So the service must yield a valid result, regardless of
             * whetr
             */
            if ( siteConfig.EnableAggregatorBugging )
            {
                sourceId = context.Request.QueryString["id"];
                referrerUrl = context.Request.UrlReferrer!=null?context.Request.UrlReferrer.ToString():"";
                                
                try
                {
                    ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                    IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);
                    
                    Entry entry = dataService.GetEntry( sourceId );
                    if ( entry != null )
                    {
                        // we'll check whether the entry exists just to avoid trash in the DB
                        logService.AddAggregatorBugHit(
                            new LogDataItem(SiteUtilities.GetPermaLinkUrl(siteConfig, sourceId),referrerUrl,context.Request.UserAgent, context.Request.UserHostName));
                    }
					else
					{
						StackTrace st = new StackTrace();
						logService.AddEvent(new EventDataItem(EventCodes.Error, "Entry was not found: " + sourceId + " " + st.ToString(), ""));
					}
                }
                catch( Exception exc )
                {
                    // absorb
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
                }
            }

            context.Response.OutputStream.Write(aggBugBitmap,0,aggBugBitmap.Length);
            context.Response.ContentType="image/gif";
            context.Response.StatusCode=200;
            context.Response.End();
        }
	}
}
