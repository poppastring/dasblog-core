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


namespace newtelligence.DasBlog.Web
{
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.WebControls;

	/// <summary>
	/// A Calendar that is smart enough to know the entries that go along with it
	/// </summary>
	public class MonthViewCalendar : Calendar
	{
		public MonthViewCalendar() : base(){	}

		public EntryCollection EntriesThisMonth = null;
	}

    /// <summary>
    /// This MonthView module for dasBlog was written by Scott Hanselman (scott@hanselman.com) 
    /// </summary>
    public partial class MonthViewBox : System.Web.UI.UserControl 
    {
		SharedBasePage requestPage;
		protected System.Resources.ResourceManager resmgr;
		protected MonthViewCalendar calendarMonth = new MonthViewCalendar();
		private DateTime _month = DateTime.MinValue;

		public string GetUrlWithYear(int year)
		{
			return String.Format("{0}?year={1}",GetUrl(),year);
		}

		public string GetUrlWithMonth(DateTime month)
		{
			return String.Format("{0}?month={1:yyyy-MM}",GetUrl(),month.Date);
		}

		public string GetUrl()
		{
			return "monthview.aspx";
		}

        protected void Page_Load(object sender, System.EventArgs e)
        {
			resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());
			requestPage = Page as SharedBasePage;

			#region Setup Year Hyperlinks
			DateTime[] daysWithEntries = null;

			//Print out a list of all the years that have entries
			// with links to the Year View
			if ( requestPage.SiteConfig.AdjustDisplayTimeZone )
			{
				daysWithEntries = requestPage.DataService.GetDaysWithEntries(requestPage.SiteConfig.GetConfiguredTimeZone());
			}
			else
			{
				daysWithEntries = requestPage.DataService.GetDaysWithEntries(new newtelligence.DasBlog.Util.UTCTimeZone());
			}

            var years = new SortedSet<int>();
			foreach (DateTime date in daysWithEntries) years.Add(date.Year);
			foreach(int year in years.Reverse<int>())
			{
				HyperLink h = new HyperLink();
				h.NavigateUrl = GetUrlWithYear(year);
				h.Text = year.ToString();
				contentPlaceHolder.Controls.Add(h);

				Literal l = new Literal();
				l.Text = "&nbsp;";
				contentPlaceHolder.Controls.Add(l);
           	}
	
			Literal l2 = new Literal();
			l2.Text = "<br /><br />";
			contentPlaceHolder.Controls.Add(l2);
			#endregion

			#region Year View
			//I know this could be cleaner and the Year viewing code could better share the Month viewing code
			// but they are sufficiently different that I chose to keep them fairly separate
			if (Request.QueryString["year"] != null)
			{
				int year = int.Parse(Request.QueryString["year"]);
				int monthsInYear = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar.GetMonthsInYear(year);
				for(int i = 1; i <= monthsInYear; i++)
				{
					MonthViewCalendar c = new MonthViewCalendar();

					ApplyCalendarStyles(c);

					c.DayRender += new DayRenderEventHandler(calendarMonth_DayRender);
					c.VisibleMonthChanged += new MonthChangedEventHandler(calendarMonth_VisibleMonthChanged);
					c.SelectionChanged += new EventHandler(calendarMonth_SelectionChanged);
					c.PreRender += new EventHandler(calendarMonth_PreRender);

					//Don't show the Next/Prev for the Year Calendar 
					c.ShowNextPrevMonth = false;

					//Tell this Calendar to show a specific month
					c.VisibleDate = new DateTime(year, i, 1);

					contentPlaceHolder.Controls.Add(c);
				}
				requestPage.TitleOverride = year.ToString();
			}
			#endregion
			else //Month View
			#region Month View
			{
				//Setup the Event Handlers for the Calendar
				calendarMonth.DayRender += new DayRenderEventHandler(calendarMonth_DayRender);
				calendarMonth.VisibleMonthChanged += new MonthChangedEventHandler(calendarMonth_VisibleMonthChanged);
				calendarMonth.SelectionChanged += new EventHandler(calendarMonth_SelectionChanged);
				calendarMonth.PreRender += new EventHandler(calendarMonth_PreRender);

				ApplyCalendarStyles(calendarMonth);
			
				contentPlaceHolder.Controls.Add(calendarMonth);

				//Default to this month, otherwise parse out a yyyy-MM
				if ( Request.QueryString["month"] == null ) 
				{
					_month = DateTime.Now.Date;
				}
				else
				{
					try 
					{
						_month = DateTime.ParseExact(Request.QueryString["month"],"yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
					}
					catch 
					{
					}
				}
			
				//Set the title, and tell the calendar to show today
				requestPage.TitleOverride = _month.ToString("MMMM, yyyy");
				calendarMonth.VisibleDate = _month;
			}
			#endregion
		}

		private void calendarMonth_DayRender(object sender, DayRenderEventArgs e)
		{
			SharedBasePage requestPage = this.Page as SharedBasePage;
			MonthViewCalendar eventSource = sender as MonthViewCalendar;

			// This is a standard text label with just the day.  This may get tossed later when 
			// we add a HyperLink for those days that have entries.  Otherwise, this serves to 
			// override the default LinkButton/PostBack behavior.
			e.Cell.Controls.Clear();
			Literal day = new Literal();
			day.Text = e.Day.DayNumberText;
			e.Cell.Controls.Add(day);

			//For every entry this month, see if we have to render it in this day.
			// I'm sure there are cleaner and faster ways to get this, but I'm using OutputCaching
			// in the ASCX page, so I don't feel so bad.  It's inefficient, but it's largely cached.
			bool controlsCleared = false;
			foreach(Entry entry in eventSource.EntriesThisMonth)
			{
				if (e.Day.Date == entry.CreatedLocalTime.Date)
				{
					if (controlsCleared == false)
					{
						controlsCleared = true;
						//Override the default LinkButton and PostBack stuff and 
						// replace with a simpler HyperLink Control.
						e.Cell.Controls.Clear();
						HyperLink dayLink = new HyperLink();
						dayLink.Text= e.Day.DayNumberText;
						dayLink.NavigateUrl = SiteUtilities.GetDayViewUrl(requestPage.SiteConfig,e.Day.Date);
						e.Cell.Controls.Add(dayLink);
					}
					e.Day.IsSelectable = false;
					Literal lit = new Literal();
					lit.Text = "<br />";
					e.Cell.Controls.Add(lit);

					HyperLink link = new HyperLink();
					link.Text = (entry.Title != null ? entry.Title : TruncateDotDotDot(entry.Content));
                    link.NavigateUrl = SiteUtilities.GetPermaLinkUrl(requestPage.SiteConfig, (ITitledEntry)entry);
					e.Cell.Controls.Add(link);
					e.Day.IsSelectable = true;

					//Poorman's spacer
					Literal lit2 = new Literal();
					lit2.Text = "<br />-<br />";
					e.Cell.Controls.Add(lit2);

				}
			}

			// Because we are using the CssClass property, we explicitely want to check
			// whether a day is both weekend _and_ lastmonth.
			// Otherwise this day would show up either
			// with 'hCalendarOtherMonthStyle' or with 'hCalendarWeekendStyle'.
			if( (e.Day.IsWeekend) && (e.Day.IsOtherMonth) )
			{
				e.Cell.CssClass = "lCalendarOtherMonthWeekendStyle";
			}
		}

		private void calendarMonth_SelectionChanged(object sender, EventArgs e)
		{
			//If we post back because of a Select Day click then redirect to the Day view with that day.
			// TODO: It'd be cleaner to override the Day rendering and make the link direct
			// and avoid this silly postback.
			SharedBasePage requestPage = this.Page as SharedBasePage;
			Response.Redirect(SiteUtilities.GetDayViewUrl(requestPage.SiteConfig,calendarMonth.SelectedDate));
		}

		
		/// <summary>
		/// Gets the entries for this month that we will be rendering
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void calendarMonth_PreRender(object sender, EventArgs e)
		{
			Control root = contentPlaceHolder;
			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			string languageFilter = Request.Headers["Accept-Language"];
			MonthViewCalendar eventSource = sender as MonthViewCalendar;

			requestPage = Page as SharedBasePage;
			
			// if we adjust for time zones (we don't show everything in UTC), we get the entries
			// using the configured time zone.
			eventSource.EntriesThisMonth = new EntryCollection();
			if ( siteConfig.AdjustDisplayTimeZone )
			{
				//Store away these Entries for the DayRender step...
				eventSource.EntriesThisMonth = requestPage.DataService.GetEntriesForMonth( eventSource.VisibleDate, siteConfig.GetConfiguredTimeZone(), languageFilter);
			}
			else
			{
				//Store away these Entries for the DayRender step...
				eventSource.EntriesThisMonth = requestPage.DataService.GetEntriesForMonth( eventSource.VisibleDate, new Util.UTCTimeZone(), languageFilter);
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			StringWriter sw = new StringWriter();
			HtmlTextWriter hw = new HtmlTextWriter( sw );

			// Render the calendar by calling the base class' method.
			base.Render(hw);

			// strip all the calendar formatting.
			writer.Write(Regex.Replace( sw.ToString(),"\\s*style=\"(.*?)\"",""));
		}

		private void calendarMonth_VisibleMonthChanged(object sender, MonthChangedEventArgs e)
		{
			//If we post back because of a Next or Prev month operation, rediect to that month.
			// TODO: It'd be cleaner to override the Next/Prev button rendering and make the link direct
			// and avoid this silly postback.
			Response.Redirect(GetUrlWithMonth(calendarMonth.VisibleDate.Date));
		}

		private const int MAXCONTENTLEN = 50;
		private string TruncateDotDotDot(string content)
		{
			string retVal = StripAllTags(content); //Important to strip tags FIRST or we could truncate in the middle of a TAG, rather than in the middle of content.
			if (retVal.Length > MAXCONTENTLEN) 
				retVal = retVal.Substring(0,Math.Min(MAXCONTENTLEN,retVal.Length)) + "...";
			return retVal;
		}

		private string StripAllTags(string content)
		{
			string regex = "<(.|\n)+?>";
			System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.IgnoreCase;
			System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(regex, options);
			string retVal = content;
			try
			{
				retVal = reg.Replace(content,String.Empty);
			}
			catch
			{ //swallow anything that might go wrong
			}
			return retVal;
		}

		private void ApplyCalendarStyles(Calendar c)
		{
			//Setup the CSS classes for the Calendar
			c.CssClass = "lCalendarStyle";
			c.DayHeaderStyle.CssClass = "lCalendarDayNameRow";
			c.DayStyle.CssClass = "lCalendarDay";
			c.SelectedDayStyle.CssClass = "lCalendarDayLinked";
			c.TodayDayStyle.CssClass = "lCalendarDayCurrent";
			c.TitleStyle.CssClass = "lCalendarMonthYearRow";
			c.NextPrevStyle.CssClass = "lCalendarNextPrevStyle";
			c.OtherMonthDayStyle.CssClass = "lCalendarOtherMonthStyle";
			c.SelectorStyle.CssClass = "lCalendarSelectorStyle";
			c.WeekendDayStyle.CssClass = "lCalendarWeekendStyle";

			//Force a few of my preferred settings.  I don't feel like messing with CSS
			c.DayStyle.VerticalAlign = VerticalAlign.Top;
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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
		}
		#endregion
	}
}
