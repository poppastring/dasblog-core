using System;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using System.Collections.Generic;
using System.Globalization;

namespace newtelligence.DasBlog.Web {
    public partial class ArchivesControl : UserControl {
        private SharedBasePage requestPage;


        // temp storage
        private Dictionary<string, List<Entry>> sorted = new Dictionary<string, List<Entry>>(StringComparer.InvariantCultureIgnoreCase);
        private List<string> categories = new List<string>();

        const string NoCategory = "NOCATEGORY";

        public ArchivesControl()
            : base() {
        }

        private void EnsureCategory(string category) {

            if (!categories.Contains(category)) {
                categories.Add(category);
                sorted.Add(category, new List<Entry>());
            }
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            requestPage = this.Page as SharedBasePage;

            // get lite-entries
            EntryCollection entries = requestPage.DataService.GetEntries(false);

            // sort entries
            foreach (Entry entry in entries) {

                if (!entry.IsPublic) {
                    continue;
                }

                if (entry.Categories == null || entry.Categories.Length == 0) {
                    EnsureCategory(NoCategory);
                    sorted[NoCategory].Add(entry);
                } else {
                    string[] catArray = entry.GetSplitCategories();
                    foreach (string cat in catArray) {
                        EnsureCategory(cat);
                        sorted[cat].Add(entry);
                    }
                }
            }


            categories.Sort(StringComparer.InvariantCultureIgnoreCase);

            // Build the archive navigator
            StringBuilder catNav = new StringBuilder();
            catNav.Append("<p>");

            foreach (string cat in categories) {
                if (cat != NoCategory) {

                    catNav.AppendFormat("<a href=\"archives.aspx#{0}\">{1} ({2})</a> &#8226; ", HttpUtility.UrlEncode(cat), cat, sorted[cat].Count);
                }
            }

            // add the no-category category last
            if (categories.Contains(NoCategory)) {
                ResourceManager resmgr = ApplicationResourceTable.Get(); ;
                string translatedNone = resmgr.GetString("text_no_category");
                catNav.AppendFormat("<a href=\"archives.aspx#{0}\">{1} ({2})</a> &#8226; ", NoCategory, translatedNone, sorted[NoCategory].Count);
            }
            catNav.Append("</p>");
            contentPlaceHolder.Controls.Add(new LiteralControl(catNav.ToString()));

            // render the categories with the post titles

            foreach (string cat in categories) {

                // render No Category last
                if (cat == NoCategory) {
                    continue;
                }

                contentPlaceHolder.Controls.Add(RenderCategoryHeader(cat));

                contentPlaceHolder.Controls.Add(RenderCategory(sorted[cat]));
            }


            // "no category" category
            if (categories.Contains(NoCategory)) {
                ResourceManager resmgr = ApplicationResourceTable.Get(); ;
                string none = resmgr.GetString("text_no_category");

                // no link, since it's not possible to subscribe to
                contentPlaceHolder.Controls.Add(RenderCategoryHeader(NoCategory, none, false));

                contentPlaceHolder.Controls.Add(RenderCategory(sorted[NoCategory]));
            }

            DataBind();
        }


        private PlaceHolder RenderCategoryHeader(string cat) {
            return this.RenderCategoryHeader(cat, cat, true);
        }

        private PlaceHolder RenderCategoryHeader(string cat, string displayName, bool renderLink) {

            PlaceHolder header = new PlaceHolder();

                        // anchor for easy navigation
            string anchor = String.Format("<a name=\"{0}\"></a>", HttpUtility.UrlEncode(cat));
            header.Controls.Add(new LiteralControl(anchor));

            // category link
            if (renderLink) {
                HyperLink catLink = new HyperLink();
                catLink.CssClass = "date";
                catLink.Text = String.Format("{0} ({1})", displayName, sorted[cat].Count);
                catLink.NavigateUrl = SiteUtilities.GetCategoryViewUrl(requestPage.SiteConfig, cat);
                header.Controls.Add(catLink);
            } else {
                header.Controls.Add(new LiteralControl(String.Format("<span class=\"date\">{0} ({1})</span>", displayName, sorted[cat].Count)));
            }



            header.Controls.Add(new LiteralControl("&nbsp;"));

            if (renderLink) {
                // category feed
                HyperLink rssLink = new HyperLink();
                rssLink.CssClass = "categoryListXmlLinkStyle";
                rssLink.NavigateUrl = SiteUtilities.GetRssCategoryUrl(requestPage.SiteConfig, cat);
                System.Web.UI.WebControls.Image rssImg = new System.Web.UI.WebControls.Image();
                rssImg.ImageUrl = requestPage.GetThemedImageUrl("rssButton");
                rssLink.Controls.Add(rssImg);
                header.Controls.Add(rssLink);
            }

            header.Controls.Add(new LiteralControl("<hr size=\"1\">"));

            return header;
        }

        private PlaceHolder RenderCategory(List<Entry> entries) {

            // sort entries newest first
            entries.Sort(delegate(Entry x, Entry y) {
                return -1 * x.CreatedUtc.CompareTo(y.CreatedUtc);
            });

            PlaceHolder placeHolder = new PlaceHolder();

            placeHolder.Controls.Add(new LiteralControl("<table>"));

            foreach (Entry entry in entries) {
                placeHolder.Controls.Add(new LiteralControl("<tr><td>"));


                // render the post date

                placeHolder.Controls.Add(new LiteralControl(entry.CreatedUtc.ToString("d")));
               
                placeHolder.Controls.Add(new LiteralControl("</td><td>"));
                
                // render the entry title as link to the article
                HyperLink hl = new HyperLink();
                if (entry.Link != null) {
                    hl.NavigateUrl = entry.Link;
                } else {
                    hl.NavigateUrl = SiteUtilities.GetPermaLinkUrl(entry);
                }
    
                hl.Text = entry.Title;
                hl.CssClass = "TitleLinkStyle";
                placeHolder.Controls.Add(hl);

                placeHolder.Controls.Add(new LiteralControl("<td></tr>"));
            }

            placeHolder.Controls.Add(new LiteralControl("</table>"));
            placeHolder.Controls.Add(new LiteralControl("<br />"));

            return placeHolder;
        }

    }
}
