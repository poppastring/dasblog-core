using System;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial  class Search : System.Web.UI.UserControl
	{
		protected SharedBasePage requestPage;
		protected System.Resources.ResourceManager resmgr;

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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			requestPage = this.Page as SharedBasePage;
			resmgr = ((System.Resources.ResourceManager) ApplicationResourceTable.Get());
			DataBind();
		}
	}
}
