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
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using NodaTime;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// WeblogCalendar is derived from the ASP.NET Calendar control.
	/// We need to do this, because we want to modify its style through
	/// our templates. Second reason is that only a day that has
	/// weblogs entries should be a link.
	/// The last reason is that we don't want "hard coded" style, which
	/// the base class Calendar does.
	/// For example, the color for the NextPrev links is hard coded to 'Black';
	/// unless you explicitely change the ForeColor property ;-)
	/// </summary>
	public class WeblogCalendar : System.Web.UI.WebControls.Calendar
	{
        DateTime[] daysWithEntries;

		/// <summary>
		/// The WeblogCalendar's constructor sets the CssClass properties.
		/// These styles are provided through the theme templates.
		/// </summary>
		public WeblogCalendar()
		{
            this.CssClass = "hCalendarStyle";
			this.DayHeaderStyle.CssClass = "hCalendarDayNameRow";
			this.DayStyle.CssClass = "hCalendarDay";
			this.SelectedDayStyle.CssClass = "hCalendarDayLinked";
			this.TodayDayStyle.CssClass = "hCalendarDayCurrent";
			this.TitleStyle.CssClass = "hCalendarMonthYearRow";
            this.NextPrevStyle.CssClass = "hCalendarNextPrevStyle";
            this.OtherMonthDayStyle.CssClass = "hCalendarOtherMonthStyle";
            this.SelectorStyle.CssClass = "hCalendarSelectorStyle";
            this.WeekendDayStyle.CssClass = "hCalendarWeekendStyle";

			//this.NextPrevFormat = NextPrevFormat.ShortMonth;
			this.ID = "weblogCalendar";

			this.DayRender += new DayRenderEventHandler(this.DayRenderHandler);
            this.Load += new EventHandler( this.WeblogCalendar_Load );
		}


        void WeblogCalendar_Load( object sender, EventArgs e )
        {
			
            SharedBasePage requestPage = this.Page as SharedBasePage;
            if ( requestPage.SiteConfig.AdjustDisplayTimeZone )
            {
                daysWithEntries = requestPage.DataService.GetDaysWithEntries(DateTimeZone.Utc);
				newtelligence.DasBlog.Util.WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
				this.TodaysDate = tz.ToLocalTime( DateTime.Now.ToUniversalTime() );
            }
            else
            {
                daysWithEntries = requestPage.DataService.GetDaysWithEntries(DateTimeZone.Utc);
            }

			// TSC: we have collected a date from the __EVENTARGUMENT if the __EVENTTARGET
			// was holding the name from our weblogCalender in the SharedBasePage,
			// so we supress the overwriting here
			if ( 
				( Parent.Page.Request.QueryString["date"] != null && 
				Parent.Page.Request.Params["__EVENTARGUMENT"] == null 
				) ||
				( Parent.Page.Request.QueryString["date"] != null &&
				Parent.Page.Request.Params["__EVENTTARGET"] != null &&
				Parent.Page.Request.Params["__EVENTTARGET"].IndexOf(this.ID) == -1 
				)
				)
			{
				try
				{
					string _mDate = Parent.Page.Request.QueryString["date"];
					this.VisibleDate = new System.DateTime(Convert.ToInt16(_mDate.Substring(0,4)),Convert.ToInt16(_mDate.Substring(5,2)), 1); 
				}
				catch
				{
					// supress
				}
			}
        }

        void DayRenderHandler(Object source, DayRenderEventArgs e) 
		{
            SharedBasePage requestPage = this.Page as SharedBasePage;

            e.Day.IsSelectable = false;

            foreach (DateTime day in daysWithEntries )
            {
                if (e.Day.Date == day )
                {
                    e.Cell.Controls.Clear();
                    HyperLink link = new HyperLink();
                    link.Text = e.Day.DayNumberText;
                    link.NavigateUrl = SiteUtilities.GetDayViewUrl(requestPage.SiteConfig,e.Day.Date);
                    e.Cell.Controls.Add(link);
                    e.Day.IsSelectable = true;
                    break;
                }
            }

			// Because we are using the CssClass property, we explicitely want to check
			// whether a day is both weekend _and_ lastmonth.
			// Otherwise this day would show up either
			// with 'hCalendarOtherMonthStyle' or with 'hCalendarWeekendStyle'.
			if( (e.Day.IsWeekend) && (e.Day.IsOtherMonth) )
			{
				e.Cell.CssClass = "hCalendarOtherMonthWeekendStyle";
			}
		}

		/// <summary>
		/// Override the Render method of the base class,
		/// because we don't want "hard coded" style.
		/// </summary>
		/// <param name="writer">System.Web.UI.HtmlTextWriter</param>
        protected override void Render(HtmlTextWriter writer)
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter( sw );

			// Render the calendar by calling the base class' method.
            base.Render(hw);

			// strip all the calendar formatting.
            writer.Write(Regex.Replace( sw.ToString(),"\\s*style=\"(.*?)\"",""));
        }
	}
}
