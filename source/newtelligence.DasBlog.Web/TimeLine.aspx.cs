using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class Timeline : SharedBasePage, IPageFormatInfo
	{

		public Timeline()
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
			base.TitleOverride = "Timeline";
			return new EntryCollection();
		}

		public Control Bodytext
		{
			get { return LoadControl("TimelineControl.ascx"); }
		}
	}
}