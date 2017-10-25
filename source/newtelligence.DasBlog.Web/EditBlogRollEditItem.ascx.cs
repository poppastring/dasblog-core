namespace newtelligence.DasBlog.Web
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
    using System.ComponentModel;
    using newtelligence.DasBlog.Web.Core;

	/// <summary>
	///		Summary description for EditBlogRollEditItem.
	/// </summary>
	public partial class EditBlogRollEditItem : System.Web.UI.UserControl
	{
        bool enteringEditMode = false;
        private OpmlOutlineCollection outlineCollection = new OpmlOutlineCollection();
        protected System.Resources.ResourceManager resmgr;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            
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
            this.buttonAddItem.Click += new System.Web.UI.ImageClickEventHandler(this.buttonAddItem_Click);
            this.blogRollGrid.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.blogRollGrid_PageIndexChanged);
            this.blogRollGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_CancelCommand);
            this.blogRollGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_EditCommand);
            this.blogRollGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_UpdateCommand);
            this.blogRollGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.blogRollGrid_DeleteCommand);
            this.blogRollGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.blogRollGrid_ItemDataBound);
            this.Init += new System.EventHandler(this.EditBlogRollEditItem_Init);

        }
		#endregion

        
        public void StoreCurrentPageIndexInViewState( int index )
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            requestPage.PageViewState[this.UniqueID+":cpi"] = index;
        }

        public int GetCurrentPageIndexFromViewState()
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            object cpi = requestPage.PageViewState[this.UniqueID+":cpi"];
            if ( cpi != null && cpi is int)
            {
                return (int)cpi;
            }
            else
            {
                return 0;
            }
        }

        public void StoreEditItemIndexInViewState( int index )
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            requestPage.PageViewState[this.UniqueID+":eii"] = index;            
        }

        public int GetEditItemIndexFromViewState()
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            object eii = requestPage.PageViewState[this.UniqueID+":eii"];
            if ( eii != null && eii is int)
            {
                return (int)eii;
            }
            else
            {
                return -1;
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            return base.OnBubbleEvent (source, args);
        }

        private void blogRollGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
        {
            StoreCurrentPageIndexInViewState(blogRollGrid.CurrentPageIndex = e.NewPageIndex);             
            Bind();
        }

        private void blogRollGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            StoreEditItemIndexInViewState(blogRollGrid.EditItemIndex = -1);
            Bind();
        }

        private void blogRollGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            outlineCollection.RemoveAt(e.Item.DataSetIndex);
            StoreEditItemIndexInViewState(blogRollGrid.EditItemIndex = -1);
            Bind();
        }

        private void blogRollGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            enteringEditMode = true;
            SharedBasePage requestPage = Page as SharedBasePage;
            StoreEditItemIndexInViewState(blogRollGrid.EditItemIndex = e.Item.ItemIndex);
            Bind();
        }

        private void blogRollGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            OpmlOutline row = outlineCollection[e.Item.DataSetIndex];
            EditBlogRollEditItem item = ((EditBlogRollEditItem)e.Item.FindControl("nestedEdit").Controls[0]);
            row.description = item.Description;
            row.title = item.Title;
            row.htmlUrl = item.HtmlUrl;
            row.xmlUrl = item.XmlUrl;
            StoreEditItemIndexInViewState(blogRollGrid.EditItemIndex = -1);
           OpmlOutline[] sortedOutlines = outlineCollection.ToArraySortedByTitle();
           for (int newIndex = 0; newIndex < sortedOutlines.Length; newIndex++)
            {
                if (row == sortedOutlines[newIndex])
                {
                    StoreCurrentPageIndexInViewState( blogRollGrid.CurrentPageIndex = newIndex / blogRollGrid.PageSize );
                    break;
                }
            }
            Bind();
        }

        private void blogRollGrid_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            
            if ( e.Item.DataSetIndex != -1 && 
                e.Item.DataSetIndex == blogRollGrid.EditItemIndex )
            {
                EditBlogRollEditItem nestedEdit = Page.LoadControl("EditBlogRollEditItem.ascx") as EditBlogRollEditItem;
                nestedEdit.ID = "innerEditor";
                e.Item.FindControl("nestedEdit").Controls.Add(nestedEdit);
                if ( enteringEditMode )
                {
                    // if we're just entering an editing session, make sure that
                    // the inner item's view state gets wiped, in case there's some
                    // state remembered for a control at this place already.
                    nestedEdit.StoreEditItemIndexInViewState(-1);
                }
                OpmlOutline outline = outlineCollection[e.Item.DataSetIndex];
                nestedEdit.Description = outline.description;
                nestedEdit.Title = outline.title;
                nestedEdit.XmlUrl = outline.xmlUrl;
                nestedEdit.HtmlUrl = outline.htmlUrl;
                nestedEdit.Outline = outline.outline;
            }
        }

        

        [Bindable(true)][Browsable(true)]
        public string Description
        {
            get
            {
                return textDescription.Text;
            }
            set
            {
                textDescription.Text = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string Title
        {
            get
            {
                return textTitle.Text;
            }
            set
            {
                textTitle.Text = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string HtmlUrl
        {
            get
            {
                return textHtmlUrl.Text;
            }
            set
            {
                textHtmlUrl.Text  = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string XmlUrl
        {
            get
            {
                return textXmlUrl.Text ;
            }
            set
            {
                textXmlUrl.Text = value;
            }
        }
        
        public OpmlOutlineCollection Outline
        {
            get
            {
                return outlineCollection;
            }
            set
            {
                outlineCollection = value;
                blogRollGrid.DataSource = outlineCollection;
                blogRollGrid.EditItemIndex = GetEditItemIndexFromViewState();
                blogRollGrid.CurrentPageIndex = GetCurrentPageIndexFromViewState();
                Bind();
            }
        }

        public void Bind()
        {
            if ( outlineCollection.Count == 0 )
            {
                multiItemPanel.Visible = false;
            }
            else
            {
                multiItemPanel.Visible = true;
            }
            blogRollGrid.DataBind();
            buttonAddItem.DataBind();
        }

        private void buttonAddItem_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            OpmlOutline newEntry = new OpmlOutline();
            newEntry.title = "New Entry";
            outlineCollection.Insert(0,newEntry);
            StoreEditItemIndexInViewState(blogRollGrid.EditItemIndex = 0);
            StoreCurrentPageIndexInViewState( blogRollGrid.CurrentPageIndex = 0 );
            Bind();
        }

        protected void EditBlogRollEditItem_Init(object sender, System.EventArgs e)
        {
            resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());
        }
	}
}
