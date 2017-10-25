using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Threading;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Runtime.Proxies;
using newtelligence.DasBlog.Web.Services.Rss20;

namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for XSSUpstreamer.
	/// </summary>
	public class XSSUpstreamer
	{
		static AutoResetEvent upstreamTrigger = new AutoResetEvent(false);
        string configPath;
        string contentPath;
        string logPath;
        
 
        public XSSUpstreamer(string configPath, string contentPath, string logPath)
		{
            this.configPath = configPath;
            this.contentPath = contentPath;
            this.logPath = logPath;
		}

        public static void TriggerUpstreaming()
        {
            upstreamTrigger.Set();
        }

        public void Run()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig( configPath );
            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(logPath);
            IBlogDataService dataService = BlogDataServiceFactory.GetService(contentPath, loggingService );
            

            loggingService.AddEvent( new EventDataItem( EventCodes.XSSServiceStart, "",""));

            do
            {
                try
                {
                    // reload on every cycle to get the current settings
                    siteConfig = SiteConfig.GetSiteConfig( configPath );
                    loggingService = LoggingDataServiceFactory.GetService(logPath);
                    dataService = BlogDataServiceFactory.GetService(contentPath, loggingService );
                    
                    SyndicationServiceImplementation syndicator = new SyndicationServiceImplementation(siteConfig, dataService, loggingService);

                    if ( siteConfig.EnableXSSUpstream &&
                         siteConfig.XSSUpstreamPassword != null &&
                         siteConfig.XSSUpstreamUsername != null )
                    {
                        try
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            StreamWriter streamWriter = new StreamWriter( memoryStream, System.Text.Encoding.UTF8 );
                            XmlSerializer ser = new XmlSerializer( typeof( RssRoot ) );
                            RssRoot root = syndicator.GetRss();
                            ser.Serialize( streamWriter, root );
                            
                            
                            
                            XmlStorageSystemClientProxy xssProxy = new XmlStorageSystemClientProxy();
                            if ( siteConfig.XSSUpstreamEndpoint != null && siteConfig.XSSUpstreamEndpoint.Length > 0 )
                            {
                                xssProxy.Url = siteConfig.XSSUpstreamEndpoint;
                            }

                            string rssFileName = "rss-dasblog.xml";
                            if ( siteConfig.XSSRSSFilename != null && siteConfig.XSSRSSFilename.Length > 0 )
                            {
                                rssFileName = siteConfig.XSSRSSFilename;
                            }

                            // the buffer is longer than the output and must be shortened
                            // by copying into a new buffer
                            byte[] rssBuffer = new byte[memoryStream.Position];
                            byte[] memoryStreamBuffer = memoryStream.GetBuffer();
                            for( int l=0;l<rssBuffer.Length;l++)
                            {
                                rssBuffer[l] = memoryStreamBuffer[l];
                            }


                            XSSSaveMultipleFilesReply reply =
                                xssProxy.SaveMultipleFiles(
                                    siteConfig.XSSUpstreamUsername, 
                                    XmlStorageSystemClientProxy.EncodePassword(siteConfig.XSSUpstreamPassword),
                                    new string[]{rssFileName},
                                    new byte[][]{rssBuffer});

                            if ( reply.flError )
                            {
                                loggingService.AddEvent(
                                    new EventDataItem( 
                                    EventCodes.XSSUpstreamError, reply.message, null, null));
                            }
                            else
                            {
                                loggingService.AddEvent(
                                    new EventDataItem( 
                                    EventCodes.XSSUpstreamSuccess, reply.message, rssFileName, xssProxy.Url));
                            }

                        }
                        catch( Exception e )
                        {
                            loggingService.AddEvent(
                                new EventDataItem( 
                                EventCodes.XSSUpstreamError, e.Message, e.StackTrace, null));
                        }
                     }

                    upstreamTrigger.WaitOne(TimeSpan.FromSeconds(siteConfig.XSSUpstreamInterval), false);

					// introduce a little time offset to limit competition for the files.
					Thread.Sleep(500);

                }
                catch( ThreadAbortException abortException )
                {
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info,abortException);
                    loggingService.AddEvent( new EventDataItem( EventCodes.XSSServiceShutdown, "",""));
                    break;
                }
                catch( Exception e )
                {
                    // if the siteConfig can't be read, stay running regardless 
                    // default wait time is 4 minutes in that case
                    Thread.Sleep( TimeSpan.FromSeconds(240));
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
                }
            }
            while ( true );
            loggingService.AddEvent( new EventDataItem( EventCodes.XSSServiceShutdown, "",""));
        }

	}
}