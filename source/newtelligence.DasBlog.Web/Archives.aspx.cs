using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class Archives : SharedBasePage, IPageFormatInfo
	{

		public Archives()
		{
			
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
			base.TitleOverride = "Archives";
			return new EntryCollection();
		}

		public Control Bodytext
		{
			get { return LoadControl("ArchivesControl.ascx"); }
		}
	}
}