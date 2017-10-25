namespace newtelligence.DasBlog.Web
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Web.Configuration;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.IO;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Core;
  
    public partial class EditUser : SharedBasePage , IPageFormatInfo
    {
                
        override protected void OnInit(EventArgs e)
        {
			this.CategoryName  = "admin";
            InitializeComponent();
            base.OnInit(e);
        }
		
        private void InitializeComponent()
        {    
        }
       
        protected override PlaceHolder ContentPlaceHolder
        {
            get
            {
                return contentPlaceHolder;
            }
        }

        protected override EntryCollection LoadEntries()
        {
            return new EntryCollection();
        }

        public Control Bodytext
        {
            get
            {
                return LoadControl("EditUserBox.ascx");
            }
        }    
    }
}
