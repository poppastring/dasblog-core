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
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class ClickThroughsBox : StatisticsListBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
		}

		#region Web Form Designer generated code
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new EventHandler(this.Page_Load);
			this.PreRender += new EventHandler(this.ClickThroughsBox_PreRender);

		}
		#endregion

		private void BuildClickThroughsRow(TableRow row, StatisticsItem item, object objDataService)
		{
			HyperLink link = new HyperLink();
			IBlogDataService dataService = objDataService as IBlogDataService;

			string[] urls = item.identifier.Split('°');
			string text = SiteUtilities.ClipString(urls[0], 80);
			link.Text = text;
			link.NavigateUrl = urls[0];

			HyperLink hlLocal = new HyperLink();
			hlLocal.NavigateUrl = urls[1];
			text = SiteUtilities.ClipString(urls[1], 80);
			if ((hlLocal.NavigateUrl != null && (hlLocal.NavigateUrl.Length > 0)))
			{
				int idx;
				string id;

				// urls in the log have been written using URL Rewriting
				if (hlLocal.NavigateUrl.IndexOf("guid,") > -1)
				{
					string guid = hlLocal.NavigateUrl.Substring(0, hlLocal.NavigateUrl.Length - 5);
					idx = guid.IndexOf("guid,");
					id = guid.Substring(idx + 5);
				}
				else
				{
					idx = hlLocal.NavigateUrl.IndexOf("guid=");
					id = hlLocal.NavigateUrl.Substring(idx + 5);
				}

				Entry entry = dataService.GetEntry(id);
		
				if (entry != null && entry.Title != null && entry.Title.Length > 0)
				{
					text = SiteUtilities.ClipString(entry.Title, 80);
				}
			}

			hlLocal.Text = text;

			row.Cells[0].Controls.Add(link);
			row.Cells[0].Controls.Add(new LiteralControl("<br />from:"));
			row.Cells[0].Controls.Add(hlLocal);
			row.Cells[1].Text = item.count.ToString();
		}

		private void ClickThroughsBox_PreRender(object sender, EventArgs e)
		{
			Control root = contentPlaceHolder;

			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
			IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

            Dictionary<string, int> clickThroughUrls = new Dictionary<string, int>();
            Dictionary<string, int> userAgents = new Dictionary<string, int>();
            Dictionary<string, int> userDomains = new Dictionary<string, int>();

			// get the user's local time
			DateTime utcTime = DateTime.UtcNow;
			DateTime localTime = siteConfig.GetConfiguredTimeZone().ToLocalTime(utcTime);
			
			if (Request.QueryString["date"] != null)
			{
				try
				{
					DateTime popUpTime = DateTime.ParseExact(Request.QueryString["date"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
					utcTime = new DateTime(popUpTime.Year, popUpTime.Month, popUpTime.Day, utcTime.Hour, utcTime.Minute, utcTime.Second);
					localTime = new DateTime(popUpTime.Year, popUpTime.Month, popUpTime.Day, localTime.Hour, localTime.Minute, localTime.Second);
				}
				catch (FormatException ex)
				{
					ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, ex);
				}
			}

			LogDataItemCollection logItems = new LogDataItemCollection();
			logItems.AddRange(logService.GetClickThroughsForDay(localTime));

			if (siteConfig.AdjustDisplayTimeZone)
			{
				newtelligence.DasBlog.Util.WindowsTimeZone tz = siteConfig.GetConfiguredTimeZone();
				TimeSpan ts = tz.GetUtcOffset(DateTime.UtcNow);
				int offset = ts.Hours;

				if (offset < 0)
				{
					logItems.AddRange(logService.GetClickThroughsForDay(localTime.AddDays(1)));
				}
				else
				{
					logItems.AddRange(logService.GetClickThroughsForDay(localTime.AddDays(-1)));
				}
			}

			foreach (LogDataItem log in logItems)
			{
				bool exclude = false;

				if (siteConfig.AdjustDisplayTimeZone)
				{
					if (siteConfig.GetConfiguredTimeZone().ToLocalTime(log.RequestedUtc).Date != localTime.Date)
					{
						exclude = true;
					}
				}

				if (!exclude)
				{
					string key = log.UrlRequested + "°" + log.UrlReferrer;
					if (!clickThroughUrls.ContainsKey(key))
					{
						clickThroughUrls[key] = 0;
					}
					clickThroughUrls[key] = clickThroughUrls[key] + 1;

					if (!userAgents.ContainsKey(log.UserAgent))
					{
						userAgents[log.UserAgent] = 0;
					}
					userAgents[log.UserAgent] = userAgents[log.UserAgent] + 1;

					if(!userDomains.ContainsKey(log.UserDomain))
					{
						userDomains[log.UserDomain] = 0;
					}
					userDomains[log.UserDomain] = userDomains[log.UserDomain] + 1;
				}
			}

            root.Controls.Add(BuildStatisticsTable(GenerateSortedItemList(clickThroughUrls), resmgr.GetString("text_activity_click_throughs"), resmgr.GetString("text_activity_clicks"), new StatisticsBuilderCallback(this.BuildClickThroughsRow), dataService));
            root.Controls.Add(BuildStatisticsTable(GenerateSortedItemList(userDomains), resmgr.GetString("text_activity_user_domains"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildUserDomainRow), dataService));
            root.Controls.Add(BuildStatisticsTable(GenerateSortedItemList(userAgents), resmgr.GetString("text_activity_user_agent"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildAgentsRow), dataService));

			DataBind();
		}
	}
}
