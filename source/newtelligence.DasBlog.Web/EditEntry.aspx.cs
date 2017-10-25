using System;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class EditEntry : SharedBasePage
	{


		protected override void OnInit(EventArgs e)
		{
			this.CategoryName  = "admin";
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

		protected override PlaceHolder ContentPlaceHolder
		{
			get { return contentPlaceHolder; }
		}

		protected override Macros InitializeMacros()
		{
			return MacrosFactory.CreateEditMacrosInstance(this);
		}

		protected override EntryCollection LoadEntries()
		{
			return new EntryCollection();
		}
	}
}