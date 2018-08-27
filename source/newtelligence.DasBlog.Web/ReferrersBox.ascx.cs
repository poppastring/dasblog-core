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
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public partial class ReferrersBox : StatisticsListBase
    {
        const string CONSTREFERRERLIST = "Referrer Urls";
        const string CONSTINTERNETSEARCHESLIST = "Internet Searches";
        const string CONSTUSERAGENTSLIST = "User Agents";
        const string CONSTHITS = "Hits";
        const string CONSTSUMMARYROLLUP = "Summary";
        const string CONSTUSERDOMAINLIST = "User Domains";



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
            this.PreRender += new EventHandler(this.ReferrersBox_PreRender);

        }
        #endregion

        private void ReferrersBox_PreRender(object sender, EventArgs e)
        {
            Control root = contentPlaceHolder;

            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            string siteRoot = siteConfig.Root.ToUpper();

            Dictionary<string, int> referrerUrls = new Dictionary<string, int>();
            Dictionary<string, int> userAgents = new Dictionary<string, int>();
            Dictionary<string, int> searchUrls = new Dictionary<string, int>();
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
            logItems.AddRange(logService.GetReferralsForDay(localTime));

            if (siteConfig.AdjustDisplayTimeZone)
            {
                newtelligence.DasBlog.Util.WindowsTimeZone tz = siteConfig.GetConfiguredTimeZone();
                TimeSpan ts = tz.GetUtcOffset(DateTime.UtcNow);
                int offset = ts.Hours;

                if (offset < 0)
                {
                    logItems.AddRange(logService.GetReferralsForDay(localTime.AddDays(1)));
                }
                else
                {
                    logItems.AddRange(logService.GetReferralsForDay(localTime.AddDays(-1)));
                }
            }

            foreach (LogDataItem log in logItems)
            {
                bool exclude = false;
                if (log.UrlReferrer != null)
                {
                    exclude = log.UrlReferrer.ToUpper().StartsWith(siteRoot);

                    // Let Utils.ParseSearchString decide whether it's a search engine referrer.
                    if (SiteUtilities.ParseSearchString(log.UrlReferrer) != null)
                    {
                        exclude = true;

                        bool addToSearches = true;
                        if (siteConfig.AdjustDisplayTimeZone)
                        {
                            if (siteConfig.GetConfiguredTimeZone().ToLocalTime(log.RequestedUtc).Date != localTime.Date)
                            {
                                addToSearches = false;
                            }
                        }

                        if (addToSearches)
                        {
                            if (!searchUrls.ContainsKey(log.UrlReferrer))
                            {
                                searchUrls[log.UrlReferrer] = 0;
                            }

                            searchUrls[log.UrlReferrer] = searchUrls[log.UrlReferrer] + 1;
                        }
                    }
                }

                if (siteConfig.AdjustDisplayTimeZone)
                {
                    if (siteConfig.GetConfiguredTimeZone().ToLocalTime(log.RequestedUtc).Date != localTime.Date)
                    {
                        exclude = true;
                    }
                }

                if (!exclude)
                {
                    if (!referrerUrls.ContainsKey(log.UrlReferrer))
                    {
                        referrerUrls[log.UrlReferrer] = 0;
                    }

                    referrerUrls[log.UrlReferrer] = referrerUrls[log.UrlReferrer] + 1;

                    log.UserAgent = Server.HtmlEncode(log.UserAgent);
                    if (!userAgents.ContainsKey(log.UserAgent))
                    {
                        userAgents[log.UserAgent] = 0;
                    }

                    userAgents[log.UserAgent] = userAgents[log.UserAgent] + 1;

                    if (!userDomains.ContainsKey(log.UserDomain))
                    {
                        userDomains[log.UserDomain] = 0;
                    }

                    userDomains[log.UserDomain] = userDomains[log.UserDomain] + 1;
                }
            }

            Table rollupTable = new Table();
            rollupTable.CssClass = "statsTableStyle";
            TableRow row = new TableRow();
            row.CssClass = "statsTableHeaderRowStyle";
            row.Cells.Add(new TableCell());
            row.Cells.Add(new TableCell());
            row.Cells[0].CssClass = "statsTableHeaderColumnStyle";
            row.Cells[1].CssClass = "statsTableHeaderNumColumnStyle";
            row.Cells[0].Text = resmgr.GetString("text_activity_summary");
            row.Cells[1].Text = resmgr.GetString("text_activity_hits");
            rollupTable.Rows.Add(row);

            //SDH: I know this is gross, but I didn't want to totally rewrite this whole thing, I just wanted to get the rollup to work
            string total = String.Empty;
            Table internetSearchesTable = BuildStatisticsTable(GenerateSortedSearchStringItemList(searchUrls), resmgr.GetString("text_activity_internet_searches"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildSearchesRow), out total, null);
            BuildRow(total, rollupTable, resmgr.GetString("text_activity_internet_searches"));
            Table userDomainsTable = BuildStatisticsTable(GenerateSortedItemList(userDomains), resmgr.GetString("text_activity_user_domains"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildUserDomainRow), out total, null);
            Table userAgentsTable = BuildStatisticsTable(GenerateSortedItemList(userAgents), resmgr.GetString("text_activity_user_agent"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildAgentsRow), out total, null);
            Table referrerUrlsTable = BuildStatisticsTable(GenerateSortedItemList(referrerUrls), resmgr.GetString("text_activity_referrer_urls"), resmgr.GetString("text_activity_hits"), new StatisticsBuilderCallback(this.BuildReferrerRow), out total, null);
            BuildRow(total, rollupTable, resmgr.GetString("text_activity_referrer_urls"));

            root.Controls.Add(rollupTable);

            root.Controls.Add(internetSearchesTable);
            root.Controls.Add(referrerUrlsTable);
            root.Controls.Add(userDomainsTable);
            root.Controls.Add(userAgentsTable);

            //root.Controls.Add(BuildStatisticsTable(GenerateSortedItemList(userAgents), CONSTUSERAGENTSLIST, CONSTHITS, new StatisticsBuilderCallback(this.BuildAgentsRow), out total, null));

            DataBind();
        }
        private void BuildRow(string count, Table rollupTable, string title)
        {
            TableRow row = new TableRow();
            row.CssClass = "statsTableRowStyle";
            row.Cells.Add(new TableCell());
            row.Cells.Add(new TableCell());
            row.Cells[0].CssClass = "statsTableColumnStyle";
            row.Cells[1].CssClass = "statsTableNumColumnStyle";
            row.Cells[0].Text = System.Web.HttpUtility.HtmlEncode(title);
            row.Cells[1].Text = count;
            rollupTable.Rows.Add(row);
        }
    }
}
