using System;
using System.Web;
using System.Web.Mail;
using System.Xml;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;


namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for ClickThroughHandler.
	/// </summary>
	public class ClickThroughHandler : IHttpHandler 
	{
		public ClickThroughHandler()
		{
			
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
                          
            string targetUrl;
            string sourceId;

            if ( !siteConfig.EnableClickThrough )
            {
                context.Response.StatusCode = 503;
                context.Response.Status = "503 Service Unavailable";
                context.Response.End();
                return;
            }                                      

            sourceId = context.Request.QueryString["id"];
            targetUrl = context.Request.QueryString["url"];

            if ( targetUrl == null || targetUrl.Length == 0 ) 
            {
                context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
                return;
            }
            else
            {
                try
                {
                    ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                    IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService );
                    
                    Entry entry = dataService.GetEntry( sourceId );
                    if ( entry != null )
                    {
                        // we'll check whether the entry exists just to avoid trash in the DB
                        logService.AddClickThrough(
                            new LogDataItem(targetUrl,SiteUtilities.GetPermaLinkUrl(siteConfig, sourceId ),context.Request.UserAgent, context.Request.UserHostName));
                    }
                }
                catch( Exception exc )
                {
                    // absorb
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
                }
            }
            context.Response.Redirect(targetUrl);
        }
	}
}
