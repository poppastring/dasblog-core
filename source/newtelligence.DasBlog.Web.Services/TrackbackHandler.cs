using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Net.Mail;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;


namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for TrackbackHandler.
	/// </summary>
	public class TrackbackHandler : IHttpHandler 
	{
		public TrackbackHandler()
		{
			//
			// TODO: Add constructor logic here
			//
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
            string entryId;
            string title;
            string excerpt;
            string url;
            string blog_name;

            if ( !siteConfig.EnableTrackbackService )
            {
                context.Response.StatusCode = 503;
                context.Response.Status = "503 Service Unavailable";
                context.Response.End();
                return;
            }                                      

			// Try blocking them once, on the off chance they sent us a referrer
			string referrer = context.Request.UrlReferrer!=null?context.Request.UrlReferrer.AbsoluteUri:"";
			if (ReferralBlackList.IsBlockedReferrer(referrer))
			{
				if (siteConfig.EnableReferralUrlBlackList404s)
				{
					context.Response.StatusCode = 404;
					context.Response.End();
					return;
				}
			}

            entryId = context.Request.QueryString["guid"];

            if ( context.Request.HttpMethod == "POST" )
            {
                title = context.Request.Form["title"];
                excerpt= context.Request.Form["excerpt"];
                url = context.Request.Form["url"];
                blog_name = context.Request.Form["blog_name"];
            }
			/* GET is no longer in the Trackback spec. Keeping
			 * this arround for testing. Just uncomment.
            else if ( context.Request.HttpMethod == "GET" )
            {
                title = context.Request.QueryString["title"];
                excerpt= context.Request.QueryString["excerpt"];
                url = context.Request.QueryString["url"];
                blog_name = context.Request.QueryString["blog_name"];
            }
			*/
            else
            {
                context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
                return;
            }

            if ( url != null && url.Length > 0 )
            {
                try
                {
					// First line of defense, try blocking again with the URL they are tracking us back with
					if (ReferralBlackList.IsBlockedReferrer(url))
					{
						if (siteConfig.EnableReferralUrlBlackList404s)
						{
							context.Response.StatusCode = 404;
							context.Response.End();
							return;
						}
					}

					ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
					IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(),logService );
					
					Entry entry = dataService.GetEntry( entryId );
					
					if ( entry != null )
					{
						try
						{
							string requestBody = null;
							// see if this is a spammer
							HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
							webRequest.Method="GET";
							webRequest.UserAgent = SiteUtilities.GetUserAgent();

							HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;
								
							// now we want to get the page contents of the target body
							using (StreamReader requestReader = new StreamReader(response.GetResponseStream()))
							{
								requestBody = requestReader.ReadToEnd();
							}

							response.Close();

							// the source URL in the page could be URL encoded like the ClickThroughHandler does
							string urlEncodedBaseUrl = HttpUtility.UrlEncode(SiteUtilities.GetBaseUrl());

							// check to see if the source's page contains a link to us
							if (Regex.Match(requestBody, SiteUtilities.GetBaseUrl()).Success == false &&
								Regex.Match(requestBody, urlEncodedBaseUrl).Success == false)
							{	
								logService.AddEvent(new EventDataItem(
									EventCodes.TrackbackBlocked,
									context.Request.UserHostAddress + " because it did not contain a link",
									SiteUtilities.GetPermaLinkUrl(entryId),
									url,
									entry.Title
									));

								context.Response.StatusCode = 404;
								context.Response.End();
								return;
							}
						}
						catch
						{
							// trackback url is not even alive
							logService.AddEvent(new EventDataItem(
								EventCodes.TrackbackBlocked,
								context.Request.UserHostAddress + " because the server did not return a valid response",
								SiteUtilities.GetPermaLinkUrl(entryId),
								url,
								entry.Title
								));
							
							context.Response.StatusCode = 404;
							context.Response.End();
							return;
						}

						// if we've gotten this far, the trackback is real and valid
						Tracking t = new Tracking();
						t.PermaLink = url;
						t.Referer = context.Request.UrlReferrer != null ? context.Request.UrlReferrer.ToString() : String.Empty;
						t.RefererBlogName = blog_name;
						t.RefererExcerpt = excerpt;
						t.RefererTitle = title;
						t.RefererIPAddress = context.Request.UserHostAddress;
						t.TargetEntryId = entryId;
						t.TargetTitle = entry.Title;
						t.TrackingType = TrackingType.Trackback;

						ISpamBlockingService spamBlockingService = siteConfig.SpamBlockingService;
						if (spamBlockingService != null)
						{
							bool isSpam = false;
							try 
							{
								isSpam = spamBlockingService.IsSpam(t);
							}
							catch(Exception ex)
							{
								logService.AddEvent(new EventDataItem(EventCodes.Error, String.Format("The external spam blocking service failed for trackback from {0}. Original exception: {1}", t.PermaLink, ex), SiteUtilities.GetPermaLinkUrl(entryId)));
							}
							if (isSpam)
							{
								//TODO: maybe we can add a configuration option to moderate trackbacks.
								// For now, we'll just avoid saving suspected spam
								logService.AddEvent(new EventDataItem(
									EventCodes.TrackbackBlocked,
									context.Request.UserHostAddress + " because it was considered spam by the external blocking service.",
									SiteUtilities.GetPermaLinkUrl(entryId),
									url,
									entry.Title
									));
								context.Response.StatusCode = 404;
								context.Response.End();
								return;
							}
						}

						if ( siteConfig.SendTrackbacksByEmail &&
							siteConfig.SmtpServer != null && siteConfig.SmtpServer.Length > 0 )
						{
							MailMessage emailMessage = new MailMessage();
							if ( siteConfig.NotificationEMailAddress != null && 
								siteConfig.NotificationEMailAddress.Length > 0 )
							{
								emailMessage.To.Add(siteConfig.NotificationEMailAddress);
							}
							else
							{
								emailMessage.To.Add(siteConfig.Contact);
							}
							emailMessage.Subject = String.Format("Weblog trackback by '{0}' on '{1}'", t.PermaLink, t.TargetTitle);
							emailMessage.Body = String.Format("You were tracked back from\n{0}\r\non your weblog entry '{1}'\n({2}\r\n\r\nDelete Trackback: {3})", 
							                                  t.PermaLink, 
							                                  t.TargetTitle, 
							                                  SiteUtilities.GetPermaLinkUrl(entryId),
															  SiteUtilities.GetTrackbackDeleteUrl(entryId, t.PermaLink, t.TrackingType));


                            emailMessage.IsBodyHtml = false;
							emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
							emailMessage.From = new MailAddress(siteConfig.Contact);
							SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, siteConfig.SmtpServer,
                                siteConfig.EnableSmtpAuthentication, siteConfig.UseSSLForSMTP, siteConfig.SmtpUserName, siteConfig.SmtpPassword, siteConfig.SmtpPort);
							dataService.AddTracking(t, sendMailInfo );
						}
						else
						{
							dataService.AddTracking( t );
						}

						logService.AddEvent(
							new EventDataItem(
							EventCodes.TrackbackReceived,
							entry.Title,
							SiteUtilities.GetPermaLinkUrl(entryId),
							url));

						// return the correct Trackback response
						// http://www.movabletype.org/docs/mttrackback.html
						context.Response.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>0</error></response>");
						return;
					}
					
                }
				catch (System.Threading.ThreadAbortException ex)
				{
					// absorb
					ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,ex);
					return;
				}
                catch( Exception exc )
                {
                    // absorb
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);

					// return the correct Trackback response
					// http://www.movabletype.org/docs/mttrackback.html
					context.Response.Write("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><response><error>1</error><message>" + exc.ToString() + "</message></response>");
					return;
                }
            }
            
            if ( entryId != null && entryId.Length > 0 )
            {
                context.Response.Redirect(SiteUtilities.GetPermaLinkUrl(siteConfig,entryId));
            }
            else
            {
                context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
            }
        }
	}
}
