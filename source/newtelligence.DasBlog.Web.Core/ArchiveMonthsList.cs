using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace newtelligence.DasBlog.Web.Core
{
    /// <summary>
    /// Summary description for ArchiveMonthsList.
    /// </summary>
    public class ArchiveMonthsList : System.Web.UI.WebControls.WebControl
    {
        private List<DateTime> _monthList;
        private Dictionary<string, int> _monthTable;
        private SharedBasePage _requestPage;

        public ArchiveMonthsList()
        {
            this.Load += new EventHandler(this.ArchiveMonthList_Load);
        }

        void ArchiveMonthList_Load(object sender, EventArgs e)
        {
            DateTime[] daysWithEntries;
            _requestPage = this.Page as SharedBasePage;
            TimeZone timezone = null;

            daysWithEntries = _requestPage.DataService.GetDaysWithEntries(DateTimeZone.Utc);

            _monthTable = new Dictionary<string, int>();
            _monthList = new List<DateTime>();

            string languageFilter = Page.Request.Headers["Accept-Language"];
            foreach (DateTime date in daysWithEntries)
            {
                if (date <= DateTime.UtcNow)
                {
                    DateTime month = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                    string monthKey = month.ToString("MMMM, yyyy");
                    if (!_monthTable.ContainsKey(monthKey))
                    {
                        EntryCollection entries = _requestPage.DataService.GetEntriesForMonth(month, DateTimeZone.Utc, languageFilter);
                        if (entries != null)
                        {
                            _monthTable.Add(monthKey, entries.Count);
                            _monthList.Add(month);
                        }
                    }
                }
            }

            _monthList.Sort();
            _monthList.Reverse();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            HtmlGenericControl section = new HtmlGenericControl("div");
            section.Attributes["class"] = "archiveLinksContainerStyle";
            this.Controls.Add(section);

            Table list = new Table();
            list.CssClass = "archiveLinksTableStyle";
            section.Controls.Add(list);

            try
            {
                foreach (DateTime date in _monthList)
                {
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    //cell.CssClass ="archiveLinksCellStyle";
                    list.Rows.Add(row);
                    row.Cells.Add(cell);

                    HyperLink monthLink = new HyperLink();
                    //monthLink.CssClass = "archiveLinksLinkStyle";
                    string monthKey = date.ToString("MMMM, yyyy");
                    monthLink.Text = string.Format("{0} ({1})", monthKey, _monthTable[monthKey]);
                    monthLink.NavigateUrl = SiteUtilities.GetMonthViewUrl(_requestPage.SiteConfig, date);
                    cell.Controls.Add(monthLink);
                }
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, exc);
                section.Controls.Add(new LiteralControl("There was an error generating archive list<br />"));
            }

            section.RenderControl(writer);
        }
    }
}
