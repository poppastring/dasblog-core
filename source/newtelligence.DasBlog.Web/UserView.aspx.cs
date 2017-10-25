namespace newtelligence.DasBlog.Web
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Core;

    public partial class UserView : SharedBasePage 
    {
        
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

        protected override PlaceHolder ContentPlaceHolder
        {
            get
            {
                return contentPlaceHolder;
            }
        }

        protected override EntryCollection LoadEntries()
        {
			SharedBasePage requestPage = this.Page as SharedBasePage;
			string userName = Request.QueryString["user"];

			return DataService.GetEntriesForUser(userName);			
		}

        public override string GetPageTemplate(string path)
        {
            return base.GetPageTemplate( path );
        }

        protected override Macros InitializeMacros()
        {
            return MacrosFactory.CreateMacrosInstance( this );
        }

        public override void ProcessItemTemplate( newtelligence.DasBlog.Runtime.Entry item, Control ContentPlaceHolder )
        {
            base.ProcessItemTemplate( item, ContentPlaceHolder );
        
        }

        public override string GetItemTemplate()
        {
            return base.GetItemTemplate();
        }
      }


}
