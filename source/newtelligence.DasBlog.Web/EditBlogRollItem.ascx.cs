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
	///		Summary description for EditBlogRollItem.
	/// </summary>
	public partial class EditBlogRollItem : System.Web.UI.UserControl
	{
        protected OpmlOutlineCollection outlineCollection=null;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
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

        }
		#endregion

        [Bindable(true)][Browsable(true)]
        public string Description
        {
            get
            {
                return description.Text;
            }
            set
            {
                description.Text = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string Title
        {
            get
            {
                return htmlLink.Text;
            }
            set
            {
                htmlLink.Text = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string HtmlUrl
        {
            get
            {
                return htmlLink.NavigateUrl;
            }
            set
            {
                htmlLink.NavigateUrl = value;
            }
        }

        [Bindable(true)][Browsable(true)]
        public string XmlUrl
        {
            get
            {
                return rssLink.NavigateUrl;
            }
            set
            {
                rssLink.NavigateUrl = value;
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
            }
        }
	}
}
