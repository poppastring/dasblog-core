using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{

	#region Blogroll.aspx
	public partial class Blogroll : SharedBasePage, IPageFormatInfo
	{

		public Blogroll()
		{
			errorHandlingOff = true;
		}

		protected override void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		private void InitializeComponent()
		{
		}

		protected override PlaceHolder ContentPlaceHolder
		{
			get { return contentPlaceHolder; }
		}

		protected override EntryCollection LoadEntries()
		{
			base.TitleOverride = "Blogroll";
			return new EntryCollection();
		}

		public Control Bodytext
		{
			get { return LoadControl("BlogRollControl.ascx"); }
		}
	}
	#endregion
}