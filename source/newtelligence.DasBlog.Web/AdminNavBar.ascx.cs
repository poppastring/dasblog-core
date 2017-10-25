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
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.Security;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for AdminNavBar.
	/// </summary>
	public partial  class AdminNavBar : System.Web.UI.UserControl
	{
		protected System.Resources.ResourceManager resmgr;
		protected SiteConfig siteConfig;

		
				
		protected void Page_Load(object sender, System.EventArgs e)
		{
			siteConfig = SiteConfig.GetSiteConfig();
			
			resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());

			//set the current page <li> tag to here
            string filePath = this.Request.FilePath;

            if (filePath.EndsWith("EditCrossPostSites.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editCrossPostSites.Attributes["class"]="here";
				hyperLinkEditCrossPostSites.NavigateUrl= "";
			}
            else if (filePath.EndsWith("EditNavigatorLinks.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editNavigatorLinks.Attributes["class"]="here";
				hyperLinkEditNavigatorLinks.NavigateUrl= "";
			}
			else if (filePath.EndsWith("EditBlogRoll.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editBlogRoll.Attributes["class"]="here";
				hyperLinkEditBlogRoll.NavigateUrl= "";
			}
			else if (filePath.EndsWith("EditContentFilters.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editContentFilters.Attributes["class"]="here";
				hyperLinkEditContentFilters.NavigateUrl= "";
			}
			else if (filePath.EndsWith("EditConfig.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editConfig.Attributes["class"]="here";
				hyperLinkEditConfig.NavigateUrl= "";
			}
			else if (filePath.EndsWith("Referrers.aspx", StringComparison.InvariantCultureIgnoreCase)||
                     filePath.EndsWith("Eventlog.aspx", StringComparison.InvariantCultureIgnoreCase)||
                     filePath.EndsWith("AggBugs.aspx", StringComparison.InvariantCultureIgnoreCase)||
                     filePath.EndsWith("ClickThroughs.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				activity.Attributes["class"]="here";
				hyperLinkActivity.NavigateUrl= "";
			}
			else if (filePath.EndsWith("EditEntry.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editEntry.Attributes["class"]="firstHere";
				hyperLinkEditEntry.NavigateUrl= "";
			}				
			else if (filePath.EndsWith("EditUser.aspx", StringComparison.InvariantCultureIgnoreCase))
			{
				editUser.Attributes["class"]="here";
				hyperLinkEditUser.NavigateUrl= "";
			}
			
			// The only tab that should be visible for a contributor is the
			// "Add Entry" and "User Settings" tab
			if (SiteSecurity.IsInRole("contributor"))
			{
				this.editConfig.Visible = false;
				this.editCrossPostSites.Visible = false;
				this.editContentFilters.Visible = false;
				this.editBlogRoll.Visible = false;
				this.editNavigatorLinks.Visible = false;
				this.activity.Visible = false;
			}
			DataBind();
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
