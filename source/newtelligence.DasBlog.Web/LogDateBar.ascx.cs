using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

// paulb: 2006-09-02
// cleaned up the code and added next/previous day links

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	///		Summary description for LogDateBar.
	/// </summary>
	public partial class LogDateBar : UserControl
	{

		// date navigation

		protected System.Resources.ResourceManager resmgr;

		protected override void OnLoad(EventArgs e) {
			base.OnLoad (e);

			resmgr = ApplicationResourceTable.Get();

			if (!Page.IsPostBack)
			{
				DateTime dt = new DateTime(DateTime.Now.Year, 1, 1);

				DateTime filteredDate = DateTime.UtcNow;

				string date = null;

				if (this.Request.QueryString["date"] != null)
				{
					date = this.Request.QueryString["date"];

					try
					{
						filteredDate = DateTime.ParseExact(date,"yyyy-MM-dd",CultureInfo.InvariantCulture);
					}
					catch (FormatException ex)
					{
						ErrorTrace.Trace(TraceLevel.Error, ex);
					}
				}

				// move the local/non-local date logic to one place

				if ( ((SharedBasePage)Page).SiteConfig.AdjustDisplayTimeZone && date == null) {
					newtelligence.DasBlog.Util.WindowsTimeZone tz = ((SharedBasePage)Page).SiteConfig.GetConfiguredTimeZone();
				filteredDate = tz.ToLocalTime(filteredDate);
				}

				// someone is trying to mess with us, let's play along
				if( filteredDate.Year > dt.Year || filteredDate.Year < dt.Year - 11){
					dt = new DateTime( filteredDate.Year, 1,1);
				}

				for (int i = 0; i > -12; i--)
				{
					this.DropDownListYear.Items.Add(dt.AddYears(i).ToString("yyyy"));
				}

				try{
					this.DropDownListYear.SelectedValue = filteredDate.Year.ToString();
				}catch( ArgumentOutOfRangeException ){
					//.. tried to set a year in the future, that's not in the list.
				}

				for (int i = 0; i < 12; i++)
				{
					this.DropDownListMonth.Items.Add(dt.AddMonths(i).ToString("MMMM"));
				}

				this.DropDownListMonth.SelectedIndex = filteredDate.Month - 1;

				int numDays = DateTime.DaysInMonth(Convert.ToInt32(this.DropDownListYear.SelectedItem.Text), this.DropDownListMonth.SelectedIndex + 1);
			
				for (int i = 1; i <= numDays; i++)
				{
					this.DropDownListDay.Items.Add(i.ToString());
				}

				this.DropDownListDay.SelectedIndex = filteredDate.Day - 1;

				// set the href for for the links
				
				DateTime nextDate= filteredDate.AddDays(1);
				DateTime previousDate = filteredDate.AddDays(-1);
                DateTime today = DateTime.Now;
				
				string requestUrl = this.Context.Request.Path;
				string dateFormat = "{0}?date={1:yyyy-MM-dd}";
				
				this.linkNext.HRef = String.Format( System.Globalization.CultureInfo.InvariantCulture, 
					dateFormat,
					requestUrl,
					nextDate);

				this.linkPrevious.HRef = String.Format( System.Globalization.CultureInfo.InvariantCulture, 
					dateFormat,
					requestUrl,
					previousDate);

                this.linkToday.HRef = String.Format(System.Globalization.CultureInfo.InvariantCulture,
                    dateFormat,
                    requestUrl,
                    today);
			}

			DataBind();
		}

		override protected void OnInit(EventArgs e)
		{
			this.buttonGo.Click += new System.EventHandler(this.buttonGo_Click);

			base.OnInit(e);
		}
		

		private void DropDownChanged()
		{
			string requestUrl = this.Context.Request.Path;
			string month = (this.DropDownListMonth.SelectedIndex + 1).ToString();
			string day = this.DropDownListDay.SelectedItem.Text;

			if (month.Length == 1) month = "0" + month;
			if (day.Length == 1) day = "0" + day;

			string responseUrl = String.Format("{0}?date={1}-{2}-{3}", 
				requestUrl, 
				this.DropDownListYear.SelectedItem,
				month, day);

			this.Context.Response.Redirect(responseUrl);
		}

		private void buttonGo_Click(object sender, System.EventArgs e)
		{
			this.DropDownChanged();
		}
	}
}
