#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion


namespace newtelligence.DasBlog.Web
{
	using System;
	using System.Text.RegularExpressions;
	using System.IO;
	using System.Collections;
	using System.ComponentModel;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.SessionState;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using newtelligence.DasBlog.Runtime;
	using newtelligence.DasBlog.Web.Core;

	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public partial class ActivityBox : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Label labelTitle;
        protected System.Web.UI.HtmlControls.HtmlGenericControl editNavigatorLinks;
        protected System.Web.UI.HtmlControls.HtmlGenericControl editCrossPostSites;
		protected System.Resources.ResourceManager resmgr;
        protected SiteConfig siteConfig;
    
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (SiteSecurity.IsInRole("admin") == false) 
			{
				Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
			}
			
            //set the current page <li> tag to here
            string filePath = this.Request.FilePath;
			
            if (filePath.EndsWith("CrosspostReferrers.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                crosspost.Attributes["class"]="here";
                hyperlinkCrosspostReferrers.NavigateUrl= "";
            }
            else if (filePath.EndsWith("UserClickThroughs.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				userclickthrough.Attributes["class"]="here";
				hyperlinkUserClickThroughs.NavigateUrl= "";
			}
            else if (filePath.EndsWith("ClickThroughs.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                clickthrough.Attributes["class"]="here";
                hyperlinkClickThroughs.NavigateUrl= "";
            }
            else if (filePath.EndsWith("AggBugs.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                aggbugs.Attributes["class"]="here";
                hyperlinkAggBugs.NavigateUrl= "";
            }
            else if (filePath.EndsWith("Referrers.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                referrers.Attributes["class"]="here";
                hyperLinkReferrers.NavigateUrl= "";
            }
            else if (filePath.EndsWith("Eventlog.aspx", StringComparison.InvariantCultureIgnoreCase))
            {
                eventlog.Attributes["class"]="firstHere";
                hyperLinkEventlog.NavigateUrl= "";
            }

			if (this.Request.QueryString["date"] != null)
			{
				AddDateQueryString(hyperLinkEventlog);
				AddDateQueryString(hyperLinkReferrers);
				AddDateQueryString(hyperlinkAggBugs);
				AddDateQueryString(hyperlinkClickThroughs);
				AddDateQueryString(hyperlinkUserClickThroughs);
				AddDateQueryString(hyperlinkCrosspostReferrers);
			}
		}

		private void AddDateQueryString(HyperLink hyperLink)
		{
			if (hyperLink.NavigateUrl != String.Empty)
			{
				hyperLink.NavigateUrl = String.Format("{0}?date={1}",
					hyperLink.NavigateUrl,
					this.Request.QueryString["date"]);
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
            resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());
            SharedBasePage requestPage = Page as SharedBasePage;
            siteConfig = requestPage.SiteConfig;

			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.PreRender += new System.EventHandler(this.ActivityBox_PreRender);

		}
		#endregion

        protected void ActivityBox_PreRender(object sender, System.EventArgs e)
        {
            DataBind();
        }

	}
}
