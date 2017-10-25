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
using System.Globalization;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    /// Summary description for WebForm1.
    /// </summary>
    public partial class EventlogBox : UserControl
    {
        protected ResourceManager resmgr;

        protected void Page_Load(object sender, EventArgs e)
        {
            resmgr = ApplicationResourceTable.Get();
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
            this.PreRender += new EventHandler(this.EventlogBox_PreRender);

        }
        #endregion

        private void EventlogBox_PreRender(object sender, EventArgs e)
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            Control root = contentPlaceHolder;
            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());

            Table table = new Table();
            table.CssClass = "statsTableStyle";

            TableRow row = new TableRow();
            row.CssClass = "statsTableHeaderRowStyle";
            row.Cells.Add(new TableCell());
            row.Cells.Add(new TableCell());
            row.Cells.Add(new TableCell());
            row.Cells[0].CssClass = "statsTableDateColumnStyle";
            row.Cells[1].CssClass = "statsTableNumColumnStyle";
            row.Cells[2].CssClass = "statsTableColumnStyle";
            row.Cells[0].Text = "<b>" + resmgr.GetString("text_time") + "</b>";
            row.Cells[1].Text = "<b>" + resmgr.GetString("text_message_code") + "</b>";
            row.Cells[2].Text = "<b>" + resmgr.GetString("text_message_text") + "</b>";
            table.Rows.Add(row);

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

            EventDataItemCollection logItems = new EventDataItemCollection();
            logItems.AddRange(logService.GetEventsForDay(localTime));

            if (siteConfig.AdjustDisplayTimeZone)
            {
                newtelligence.DasBlog.Util.WindowsTimeZone tz = siteConfig.GetConfiguredTimeZone();
                TimeSpan ts = tz.GetUtcOffset(DateTime.UtcNow);
                int offset = ts.Hours;

                if (offset < 0)
                {
                    logItems.AddRange(logService.GetEventsForDay(localTime.AddDays(1)));
                }
                else
                {
                    logItems.AddRange(logService.GetEventsForDay(localTime.AddDays(-1)));
                }
            }

            EventDataItem[] sortedLogItems = logItems.ToSortedArray();

            foreach (EventDataItem eventItem in sortedLogItems)
            {
                if (siteConfig.AdjustDisplayTimeZone)
                {
                    if (siteConfig.GetConfiguredTimeZone().ToLocalTime(eventItem.EventTimeUtc).Date != localTime.Date)
                    {
                        continue;
                    }
                }

                row = new TableRow();
                row.CssClass = "statsTableRowStyle";

                switch (eventItem.EventCode)
                {
                    case ((int)EventCodes.Error):
                    case ((int)EventCodes.PingbackServerError):
                    case ((int)EventCodes.PingWeblogsError):
                    case ((int)EventCodes.Pop3ServerError):
                    case ((int)EventCodes.SmtpError):
                        row.CssClass = "statsTableRowStyleError";
                        break;
                    case ((int)EventCodes.SecurityFailure):
                        row.CssClass = "statsTableRowStyleSecurityFailure";
                        break;
                    case ((int)EventCodes.TrackbackBlocked):
                    case ((int)EventCodes.ReferralBlocked):
                    case ((int)EventCodes.ItemReferralBlocked):
                    case ((int)EventCodes.CommentBlocked):
                    case ((int)EventCodes.PingbackBlocked):
                        row.CssClass = "statsTableRowStyleBlocked";
                        break;
                    default:
                        break;
                }

                row.Cells.Add(new TableCell());
                row.Cells.Add(new TableCell());
                row.Cells.Add(new TableCell());
                row.Cells[0].CssClass = "statsTableDateColumnStyle";
                row.Cells[1].CssClass = "statsTableNumColumnStyle";
                row.Cells[2].CssClass = "statsTableColumnStyle";

                if (siteConfig.AdjustDisplayTimeZone)
                {
                    row.Cells[0].Text = siteConfig.GetConfiguredTimeZone().ToLocalTime(eventItem.EventTimeUtc).ToString("yyyy-MM-dd HH:mm:ss tt");
                }
                else
                {
                    row.Cells[0].Text = eventItem.EventTimeUtc.ToString("yyyy-MM-dd HH:mm:ss tt") + " UTC";
                }

                row.Cells[1].Text = eventItem.EventCode.ToString();
                row.Cells[2].Text = eventItem.HtmlMessage;

                table.Rows.Add(row);
            }

            root.Controls.Add(table);

            DataBind();
        }
    }
}