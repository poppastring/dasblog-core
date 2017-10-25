using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Xml.XPath;
using System.Xml.Xsl;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	public partial class BlogRollControl : UserControl
	{

		public BlogRollControl()
		{
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			// Load up the OPML

            //FIX: fix hardcoded paths
			string opmlPath = SiteUtilities.MapPath("siteconfig\\blogroll.opml");
            string xsltPath = SiteUtilities.MapPath("siteconfig\\opml.xslt");

			XPathDocument xmlOpml = new XPathDocument(opmlPath);
            XslCompiledTransform xmlTransform = new XslCompiledTransform();

			// Load up the files
			xmlTransform.Load(xsltPath);

			// Transform
            using (StringWriter sw = new StringWriter()) {
                xmlTransform.Transform(xmlOpml, null, sw);

                contentPlaceHolder.Controls.Add(new LiteralControl(sw.ToString()));
            }

			DataBind();
		}

		protected override void OnInit(EventArgs e)
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
