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

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	///		Summary description for ThemeBar.
	/// </summary>
	public partial class ThemeBar : UserControl
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			SharedBasePage page = Page as SharedBasePage;
			if ( !page.SiteConfig.EnableStartPageCaching ) 
			{
				ThemeDictionary themes;
				LinkButton linkButton;

				linkButton = new LinkButton();
				linkButton.Text = "Reset";
				linkButton.CausesValidation = false;
				linkButton.CommandArgument="";
				linkButton.Command += new CommandEventHandler(this.HandleCommands);
				content.Controls.Add(linkButton);
                            
				themes = (ThemeDictionary) page.DataCache["Themes"];

				if (themes != null)
				{
					foreach( BlogTheme theme in themes.Values )
					{
						content.Controls.Add( new LiteralControl("&nbsp;|&nbsp;"));
						linkButton = new LinkButton();
						linkButton.Text = theme.Title;
						linkButton.CausesValidation = false;
						linkButton.CommandArgument=theme.Name;
						linkButton.Command += new CommandEventHandler(this.HandleCommands);
						content.Controls.Add(linkButton);
					}
				}
				else
				{
					this.Visible = false;
				}
			}
			else
			{
				this.Visible = false;
			}
		}

		void HandleCommands( object sender, CommandEventArgs e )
		{
			SharedBasePage page = Page as SharedBasePage;
			if ( (string)e.CommandArgument == "" )
			{
				page.UserTheme = "";
			}
			else
			{
				page.UserTheme = (string)e.CommandArgument;
			}
			page.Redirect(page.Request.RawUrl);
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
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion
	}
}
