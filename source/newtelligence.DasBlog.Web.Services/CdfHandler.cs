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
// CDF support by (c) 2003 by Harry Pierson (http://www.devhawk.net)
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
using System.Collections;
using System.Web;
using System.Xml;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using System.Collections.Generic;
using NodaTime;

namespace newtelligence.DasBlog.Web.Services
{
	public class CdfHandler : IHttpHandler
	{
        public void ProcessRequest(HttpContext context)
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), loggingService);
            

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
			else
			{
				loggingService.AddReferral(
					new LogDataItem(
					context.Request.RawUrl,
					referrer,
					context.Request.UserAgent,
					context.Request.UserHostName));
			}
			context.Response.ContentType = "application/x-cdf";

            //Create the XmlWriter around the Response
            XmlTextWriter xw = new XmlTextWriter(context.Response.Output);
            xw.Formatting = Formatting.Indented;

            EntryCollection entriesAll = dataService.GetEntriesForDay(
                DateTime.Now.ToUniversalTime(), DateTimeZone.Utc, 
                null, siteConfig.RssDayCount, siteConfig.RssMainEntryCount, null);
			EntryCollection entries = new EntryCollection();
			foreach(Entry e in entriesAll)
			{
				if (e.IsPublic == true)
				{
					entries.Add(e);
				}
			}
            entries.Sort( new EntrySorter() );

            //Write out the boilerplate CDF
            xw.WriteStartDocument();

            xw.WriteStartElement("CHANNEL");
            xw.WriteAttributeString("HREF", SiteUtilities.GetBaseUrl(siteConfig));
			
			foreach(Entry current in entries)
			{
				xw.WriteAttributeString("LASTMOD", 
					FormatLastMod(current.ModifiedLocalTime));
			}
			/*IEnumerator enumerator = entries.GetEnumerator();
			if(enumerator.MoveNext())
			{
				xw.WriteAttributeString("LASTMOD", 
					FormatLastMod(((Entry)enumerator.Current).ModifiedLocalTime));
			}*/
			xw.WriteAttributeString("LEVEL", "1");
            xw.WriteAttributeString("PRECACHE", "YES");

            xw.WriteElementString("TITLE", siteConfig.Title);
            xw.WriteElementString("ABSTRACT", siteConfig.Subtitle);

            foreach (Entry entry in entries)
            {
                xw.WriteStartElement("ITEM");
                xw.WriteAttributeString("HREF", SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry));
                xw.WriteAttributeString("LASTMOD", FormatLastMod(entry.ModifiedLocalTime));
                xw.WriteAttributeString("LEVEL", "1");
                xw.WriteAttributeString("PRECACHE", "YES");

                xw.WriteElementString("TITLE", entry.Title);

                if (entry.Description != null && entry.Description.Length > 0)
                    xw.WriteElementString("ABSTRACT", entry.Description);

                xw.WriteEndElement();
            }
			
            xw.WriteEndElement(); //channel
            xw.WriteEndDocument();
        }

        //duplicated from SyndicationServiceBase
        protected class EntrySorter : IComparer<Entry>
        {
            public int Compare(Entry left, Entry right)
            {
                return right.CreatedUtc.CompareTo(left.CreatedUtc);
            }
        }

        private string FormatLastMod(DateTime date)
        {
            return string.Format("{0:yyyy-MM-ddTHH:mm}", date.ToUniversalTime());
        }

        public bool IsReusable { get { return true; } }
    }
}
