#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using System;
using System.Net.Mail;
using CookComputing.XmlRpc;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Services
{
	public interface IPingback
	{
		//http://hixie.ch/specs/pingback/pingback
        [XmlRpcMethod("pingback.ping",
                Description="pings this weblog")]
        [return: XmlRpcReturnValue(
                        Description="a string")]
        string ping(
            string sourceUri,
            string targetUri);
	}

    [XmlRpcService(Name="DasBlog Pingback Access Point", 
                   Description="Implementation of Pingback XML-RPC Api")]
    public class PingbackAPI : XmlRpcService, IPingback
    {

        IBlogDataService dataService;
        ILoggingDataService logDataService;
        SiteConfig siteConfig = SiteConfig.GetSiteConfig();

        public PingbackAPI()
        {
            siteConfig = SiteConfig.GetSiteConfig();
            logDataService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logDataService);
            
        }

        public string ping(
            string sourceUri,
            string targetUri)
        {

            if ( !siteConfig.EnablePingbackService )
            {
                throw new ServiceDisabledException();
            }

            string returnValue="0";


			if (ReferralBlackList.IsBlockedReferrer(sourceUri))
			{
				if (siteConfig.EnableReferralUrlBlackList404s)
				{
					this.Context.Response.StatusCode = 404;
					this.Context.Response.End();
					throw new XmlRpcFaultException(404, "not found");
				}
			}


            try
            {
                string entryId=null;
                
				// OmarS: need to rewrite the URL so w can find the entryId
                Uri uriTargetUri = new Uri( SiteUtilities.MapUrl(targetUri) );
                string query = uriTargetUri.Query;
                if ( query.Length>0 && query[0]=='?')
                {
                    query = query.Substring(1);
                }
                else
                {
                    return returnValue;
                }

                string[] queryItems = query.Split('&');
                if ( queryItems == null )
                    return returnValue;

                foreach( string queryItem in queryItems )
                {
                    string[] keyvalue = queryItem.Split('=');
                    if ( keyvalue.Length==2)
                    {
                        string key=keyvalue[0];
                        string @value=keyvalue[1];

                        if ( key == "guid")
                        {
                            entryId = @value;
                            break;
                        }
                    }
                }

                if ( entryId != null )
                {
                    Entry entry = dataService.GetEntry(entryId);
                    if ( entry != null )
                    {
                        Tracking t = new Tracking();
                        t.PermaLink = sourceUri;
						t.Referer = this.Context.Request.UrlReferrer != null ? this.Context.Request.UrlReferrer.ToString() : String.Empty;
                        t.RefererBlogName = sourceUri;
                        t.RefererExcerpt = String.Empty;
                        t.RefererTitle = sourceUri;
                        t.TargetEntryId = entryId;
                        t.TargetTitle = entry.Title;
                        t.TrackingType = TrackingType.Pingback;
                        t.RefererIPAddress = this.Context.Request.UserHostAddress;

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
								logDataService.AddEvent(new EventDataItem(EventCodes.Error, String.Format("The external spam blocking service failed for pingback from {0}. Original exception: {1}", sourceUri, ex), targetUri));
							}
							if (isSpam)
							{
								//TODO: May provide moderation in the future. For now we just ignore the pingback
								logDataService.AddEvent(new EventDataItem(
									EventCodes.PingbackBlocked,
									"Pingback blocked from " + sourceUri + " because it was considered spam by the external blocking service.",
									targetUri, sourceUri));
								System.Web.HttpContext.Current.Response.StatusCode = 404;
								System.Web.HttpContext.Current.Response.End();
								throw new XmlRpcFaultException(404, "not found");
							}
						}

                        if ( siteConfig.SendPingbacksByEmail &&
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
                            emailMessage.Subject = String.Format("Weblog pingback by '{0}' on '{1}'", sourceUri, t.TargetTitle);
                            emailMessage.Body = String.Format("You were pinged back by\n{0}\r\non your weblog entry '{1}'\n({2}\r\n\r\nDelete Trackback: {3})", 
                                                              sourceUri, 
                                                              t.TargetTitle, 
                                                              SiteUtilities.GetPermaLinkUrl(entry),
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
                            dataService.AddTracking(t);
                        }
                        
                        logDataService.AddEvent(
                            new EventDataItem(EventCodes.PingbackReceived,entry.Title,targetUri,sourceUri));
                        returnValue = sourceUri;
                    }
                }
            }
            catch( Exception e )
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
                return "0";
            }
            return returnValue;
        }
    }
}
