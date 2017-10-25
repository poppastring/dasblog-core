using System;
using System.Resources;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class TimelineControl : UserControl
    {
		
        public TimelineControl()
        {
        }
		
	
		
        protected void Page_Load(object sender, EventArgs e)
        {
			
        }

		override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }
		
        private void InitializeComponent()
        {
            this.Load += new EventHandler(this.Page_Load);
        }
    }
}
