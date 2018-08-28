using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using NodaTime;

namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for MicrosummaryHandler.
	/// </summary>
	public class MicrosummaryHandler : IHttpHandler
	{
		public MicrosummaryHandler()
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
			ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
			try
			{
				IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService );
				SiteConfig siteConfig = SiteConfig.GetSiteConfig();
				
				string languageFilter = context.Request.Headers["Accept-Language"];
				if ( SiteSecurity.IsInRole("admin") )
				{
					languageFilter = "";
				}

				EntryCollection entries =  dataService.GetEntriesForDay( DateTime.UtcNow, DateTimeZone.Utc, languageFilter,1,1,String.Empty );

				if(entries != null && entries.Count > 0)
				{
					Entry e = entries[0];
					context.Response.Write(e.Title);
				}
			}
			catch (Exception ex)
			{
				logService.AddEvent(new EventDataItem(EventCodes.Error,"Error generating Microsummary: " + ex.ToString(),String.Empty));
			}
		}
	}
}
