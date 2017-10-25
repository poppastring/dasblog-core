using System;
using System.IO;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.TextEditors;

namespace newtelligence.DasBlog.Web
{
    /// <summary>                                             
    ///		Summary description for EditNavigatorLinksBox.
    /// </summary>
    public partial class EditContentFiltersBox : SharedBaseControl
    {
        [TransientPageState]
        public ContentFilterCollection contentFilters;
        protected RequiredFieldValidator validatorRFname;
        protected Panel Panel1;
        protected ResourceManager resmgr;

        private Core.EditControlAdapter editControl;

        private void SaveFilters()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig(SiteConfig.GetConfigFilePathFromCurrentContext());
            siteConfig.ApplyContentFiltersToWeb = checkFilterHtml.Checked;
            siteConfig.ApplyContentFiltersToRSS = checkFilterRSS.Checked;
            siteConfig.ContentFilters.Clear();
            siteConfig.ContentFilters.AddRange(contentFilters);
            XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));
            using (StreamWriter writer = new StreamWriter(SiteConfig.GetConfigFilePathFromCurrentContext()))
            {
                ser.Serialize(writer, siteConfig);
            }
        }

        protected string TruncateString(string s, int max)
        {
            if (s == null)
            {
                return "";
            }
            else if (s.Length < max)
            {
                return s;
            }
            else if (s.Length >= max)
            {
                return s.Substring(0, max) + "...";
            }
            return s;
        }

        private void LoadFilters()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig(SiteConfig.GetConfigFilePathFromCurrentContext());
            contentFilters = new ContentFilterCollection(siteConfig.ContentFilters);
            checkFilterHtml.Checked = siteConfig.ApplyContentFiltersToWeb;
            checkFilterRSS.Checked = siteConfig.ApplyContentFiltersToRSS;
        }

        private void BindGrid()
        {
            contentFiltersGrid.DataSource = contentFilters;
            DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SharedBasePage requestPage = this.Page as SharedBasePage;

            if (SiteSecurity.IsInRole("admin") == false)
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            resmgr = ((ResourceManager)ApplicationResourceTable.Get());

            if (!IsPostBack || contentFilters == null)
            {
                LoadFilters();
                UpdateTestBox();
            }

            BindGrid();
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            this.editControl = EditControlProvider.CreateEditControl<TinyMCEAdapter>();
            this.editControlHolder.Controls.Add(editControl.Control);
            this.editControl.Initialize();
            this.editControl.Width = Unit.Percentage(99d);
            this.editControl.Height = Unit.Pixel(200);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentFiltersGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.contentFiltersGrid_ItemCommand);
            this.contentFiltersGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.contentFiltersGrid_PageIndexChanged);
            this.contentFiltersGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.contentFiltersGrid_CancelCommand);
            this.contentFiltersGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.contentFiltersGrid_EditCommand);
            this.contentFiltersGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.contentFiltersGrid_UpdateCommand);
            this.contentFiltersGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.contentFiltersGrid_DeleteCommand);

        }
        #endregion

        private void contentFiltersGrid_EditCommand(object source, DataGridCommandEventArgs e)
        {
            contentFiltersGrid.EditItemIndex = e.Item.ItemIndex;
            DataBind();
            UpdateTestBox();
        }

        private void contentFiltersGrid_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            contentFilters.RemoveAt(e.Item.DataSetIndex);
            contentFiltersGrid.EditItemIndex = -1;
            DataBind();
            UpdateTestBox();
            changesAlert.Visible = true;
        }

        private void contentFiltersGrid_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            contentFiltersGrid.EditItemIndex = -1;
            DataBind();
            UpdateTestBox();
        }

        private void contentFiltersGrid_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            ContentFilter filter = contentFilters[e.Item.DataSetIndex];
            filter.Expression = ((TextBox)e.Item.FindControl("textExpression")).Text;
            filter.MapTo = ((TextBox)e.Item.FindControl("textMapTo")).Text;
            filter.IsRegEx = ((CheckBox)e.Item.FindControl("checkRegex")).Checked;
            contentFiltersGrid.EditItemIndex = -1;
            DataBind();
            UpdateTestBox();
            changesAlert.Visible = true;
        }

        private void contentFiltersGrid_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
        {
            contentFiltersGrid.CurrentPageIndex = e.NewPageIndex;
            DataBind();
            UpdateTestBox();
        }

        private void contentFiltersGrid_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            if (e.CommandName == "AddItem")
            {
                ContentFilter newFilter = new ContentFilter();
                newFilter.Expression = "New Expression";
                contentFilters.Insert(0, newFilter);
                contentFiltersGrid.CurrentPageIndex = 0;
                contentFiltersGrid.EditItemIndex = 0;
                UpdateTestBox();
                BindGrid();
            }
            else if (e.CommandName == "MoveUp" && e.Item != null)
            {
                int position = e.Item.DataSetIndex;

                if (position > 0)
                {
                    ContentFilter filter = contentFilters[position];
                    contentFilters.RemoveAt(position);
                    contentFilters.Insert(position - 1, filter);
                    contentFiltersGrid.CurrentPageIndex = (position - 1) / contentFiltersGrid.PageSize;
                    if (contentFiltersGrid.EditItemIndex == position)
                    {
                        contentFiltersGrid.EditItemIndex = position - 1;
                    }
                    else if (contentFiltersGrid.EditItemIndex == position - 1)
                    {
                        contentFiltersGrid.EditItemIndex = position;
                    }
                    changesAlert.Visible = true;
                }
                UpdateTestBox();
                BindGrid();
            }
            else if (e.CommandName == "MoveDown" && e.Item != null)
            {
                int position = e.Item.DataSetIndex;

                if (position < contentFilters.Count - 1)
                {
                    ContentFilter filter = contentFilters[position];
                    contentFilters.RemoveAt(position);
                    contentFilters.Insert(position + 1, filter);
                    contentFiltersGrid.CurrentPageIndex = (position + 1) / contentFiltersGrid.PageSize;
                    if (contentFiltersGrid.EditItemIndex == position)
                    {
                        contentFiltersGrid.EditItemIndex = position + 1;
                    }
                    else if (contentFiltersGrid.EditItemIndex == position + 1)
                    {
                        contentFiltersGrid.EditItemIndex = position;
                    }
                    changesAlert.Visible = true;
                }
                UpdateTestBox();
                BindGrid();

            }
        }

        private string Filter(string content)
        {
            if (contentFilters == null)
                return content;

            foreach (ContentFilter filter in contentFilters)
            {
                if (filter.IsRegEx)
                {
                    try
                    {
                        content = Regex.Replace(content, filter.Expression, filter.MapTo, RegexOptions.Singleline);
                    }
                    catch (Exception e)
                    {
                        content += String.Format("<br /><br />Error in filter {0}<br />{1}", HttpUtility.HtmlEncode(filter.Expression), HttpUtility.HtmlEncode(e.Message));
                        break;
                    }
                }
                else
                {
                    content = content.Replace(filter.Expression, filter.MapTo);
                }
            }

            return content;
        }

        private void UpdateTestBox()
        {
            previewPanel.Style.Add("padding", "5px");
            previewPanel.Style.Add("margin", "5px");

            previewPanel.Controls.Add(new LiteralControl(Filter(editControl.Text)));

            sampleContentBox.Visible = true;
        }

        protected void buttonFilterResults_Click(object sender, EventArgs e)
        {
            UpdateTestBox();
        }

        protected void buttonSaveChanges_Click(object sender, EventArgs e)
        {
            SaveFilters();
            UpdateTestBox();
            changesAlert.Visible = false;
        }
    }
}