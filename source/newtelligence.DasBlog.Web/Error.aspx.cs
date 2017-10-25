using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class Error : SharedBasePage, IPageFormatInfo
	{
		protected override PlaceHolder ContentPlaceHolder
		{
			get { return contentPlaceHolder; }
		}

		#region IPageFormatInfo Members
		public Control Bodytext
		{
			get { return LoadControl("ErrorBox.ascx"); }
		}
		#endregion

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		void InitializeComponent()
		{
			Load += new EventHandler(Error_Load);
		}

		void Error_Load(object sender, EventArgs e)
		{
			CategoryName = "error";
			Response.StatusCode = 404;
		}

		protected override EntryCollection LoadEntries()
		{
			return new EntryCollection();
		}
	}
}