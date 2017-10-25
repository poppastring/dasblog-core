namespace newtelligence.DasBlog.Web
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using newtelligence.DasBlog.Web.Core;
    

	/// <summary>
	///		Summary description for EditNavigatorLinksBox.
	/// </summary>
	public partial class EditNavigatorLinksBox : System.Web.UI.UserControl
	{
        protected NavigationRoot navigationRoot;
        protected System.Web.UI.WebControls.RequiredFieldValidator validatorRFname;
        private string baseFileName="navigatorLinks.xml";
        protected System.Resources.ResourceManager resmgr;
        

        public string BaseFileName 
        {
            get
            {
                return baseFileName;
            }
            set
            {
                baseFileName = value;
            }
        }


        private void SaveList( string fileName )
        {
            using (StreamWriter s = new StreamWriter(fileName,false,System.Text.Encoding.UTF8))
            {
                XmlSerializer ser = new XmlSerializer(typeof(NavigationRoot));
                ser.Serialize(s,navigationRoot);
            }
        }

        private void LoadList( string fileName )
        {
            if (File.Exists(fileName))
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(NavigationRoot));
                    navigationRoot = (NavigationRoot)ser.Deserialize(s);
                }
            }
            else
            {
                navigationRoot = new NavigationRoot();
            }
            Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] = navigationRoot;
        }

        private void BindGrid()
        {
            navigatorLinksGrid.DataSource = navigationRoot.Items;
            navigatorLinksGrid.Columns[0].HeaderText = resmgr.GetString("text_navigatorlinks_title");
            DataBind();
        }

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (SiteSecurity.IsInRole("admin") == false) 
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());

            if ( !IsPostBack || 
                Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] == null )
            {
                SharedBasePage requestPage = Page as SharedBasePage;
                string fileName = Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName);
                LoadList( fileName );
            }
            else
            {
                navigationRoot = Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] as NavigationRoot;
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
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.navigatorLinksGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.navigatorLinksGrid_ItemCommand);
			this.navigatorLinksGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.navigatorLinksGrid_PageIndexChanged);
			this.navigatorLinksGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.navigatorLinksGrid_CancelCommand);
			this.navigatorLinksGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.navigatorLinksGrid_EditCommand);
			this.navigatorLinksGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.navigatorLinksGrid_UpdateCommand);
			this.navigatorLinksGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.navigatorLinksGrid_DeleteCommand);

		}
		#endregion

        private void navigatorLinksGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            
            navigatorLinksGrid.EditItemIndex = e.Item.ItemIndex;
            navigatorLinksGrid.DataBind();
        }

        private void navigatorLinksGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            navigationRoot.Items.RemoveAt(e.Item.DataSetIndex);
            SaveList( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));
            Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] = null;
            
            Response.Redirect( Page.Request.Url.AbsoluteUri, true );
        }

        private void navigatorLinksGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            navigatorLinksGrid.EditItemIndex = -1;
            navigatorLinksGrid.DataBind();
        }

        private void navigatorLinksGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            NavigationLink link = navigationRoot.Items[e.Item.DataSetIndex];
            link.Name = ((TextBox)e.Item.FindControl("textname")).Text;
            link.Url = ((TextBox)e.Item.FindControl("texturl")).Text;
            
            SaveList( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));
            Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] = null;
            
            Response.Redirect( Page.Request.Url.AbsoluteUri, true );
        }                                          

        private void navigatorLinksGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            navigatorLinksGrid.CurrentPageIndex = e.NewPageIndex;
            navigatorLinksGrid.DataBind();
        }

        private void navigatorLinksGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            if ( e.CommandName == "AddItem" )
            {
                NavigationLink newLink = new NavigationLink();
                newLink.Name = "New Entry";
                navigationRoot.Items.Insert(0,newLink);
                navigatorLinksGrid.CurrentPageIndex = 0;
                navigatorLinksGrid.EditItemIndex = 0;
                BindGrid();
            }
			else if ( e.CommandName == "MoveUp" && e.Item != null )
			{
				int position = e.Item.DataSetIndex;

				if ( position > 0 )
				{
					NavigationLink link = new NavigationLink();
					link.Name = navigationRoot.Items[position].Name;
					link.Url = navigationRoot.Items[position].Url;
					navigationRoot.Items.RemoveAt( position );
					navigationRoot.Items.Insert( position-1, link );
					navigatorLinksGrid.CurrentPageIndex = (position-1) / navigatorLinksGrid.PageSize;
					if ( navigatorLinksGrid.EditItemIndex == position )
					{
						navigatorLinksGrid.EditItemIndex = position - 1;
					}
					else if ( navigatorLinksGrid.EditItemIndex == position - 1)
					{
						navigatorLinksGrid.EditItemIndex = position;
					}	
				}
				SaveList( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));
				Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] = null;
            
				Response.Redirect( Page.Request.Url.AbsoluteUri, true );
			}
			else if ( e.CommandName == "MoveDown" && e.Item != null )
			{
				int position = e.Item.DataSetIndex;

				if ( position < navigationRoot.Items.Count-1 )
				{
					NavigationLink link = new NavigationLink();
					link.Name = navigationRoot.Items[position].Name;
					link.Url = navigationRoot.Items[position].Url;
					navigationRoot.Items.RemoveAt( position );
					navigationRoot.Items.Insert( position+1, link );
					navigatorLinksGrid.CurrentPageIndex = (position+1) / navigatorLinksGrid.PageSize;
					if ( navigatorLinksGrid.EditItemIndex == position )
					{
						navigatorLinksGrid.EditItemIndex = position + 1;
					}
					else if ( navigatorLinksGrid.EditItemIndex == position + 1)
					{
						navigatorLinksGrid.EditItemIndex = position;
					}
				}
				SaveList( Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),baseFileName));
				Session["newtelligence.DasBlog.Web.EditNavigatorLinksBox.NavigationRoot"] = null;
            
				Response.Redirect( Page.Request.Url.AbsoluteUri, true );
			}
        }        
	}
}
