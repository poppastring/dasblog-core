using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for Email.
	/// </summary>
	public partial class Email :SharedBasePage , IPageFormatInfo
	{
                
		override protected void OnInit(EventArgs e)
		{
			
			InitializeComponent();
			base.OnInit(e);

			resmgr = ApplicationResourceTable.Get();
		}
		
		private void InitializeComponent(){ }

       
		protected override PlaceHolder ContentPlaceHolder
		{
			get
			{
				return contentPlaceHolder;
			}
		}

		protected System.Resources.ResourceManager resmgr;

		protected override EntryCollection LoadEntries()
		{
			Entry e = new Entry();

			e.Title = resmgr.GetString("text_send_an_email");
			
			TitleOverride = e.Title;

			return new EntryCollection(new Entry[] { e });

		}

	
		public Control Bodytext
		{
			get
			{
				Control c = LoadControl("EmailBox.ascx");
				return c;
			}
		}    
	}
}
