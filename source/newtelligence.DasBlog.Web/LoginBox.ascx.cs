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
using System.Web.Security;
using newtelligence.DasBlog.Web.Core;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace newtelligence.DasBlog.Web
{

	/// <summary>
	///		Summary description for LoginBox.
	/// </summary>
	public partial  class LoginBox : System.Web.UI.UserControl
	{
		protected System.Resources.ResourceManager resmgr;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			resmgr = ApplicationResourceTable.Get();
			DataBind();

			if (Page.FindControl("LoginBox") == null)
			{
				this.ID = "LoginBox";

				//				string pageID = "mainForm";
				//				string parendID = this.ID+"_";

				// OmarS: form elements can no longer be accessed by name due to XHTML
				// rules. This ensures we will work on Whidbey
				/*
					string usernameID = "document." + pageID + "." + parendID + "username.value";
					string passwordID = "document." + pageID + "." + parendID + "password.value";
					string challengeID = "document." + pageID + "." + parendID + "challenge.value";
					*/

				// PaulB: Changed the Javascript to use document.getElementById, and the WebControl.ClientID property to make 
				// sure we will work on future versions.
				/*
					string usernameID = String.Format("document.forms[\"{0}\"].elements[\"{1}\"].value", pageID, parendID + "username");
					string passwordID = String.Format("document.forms[\"{0}\"].elements[\"{1}\"].value", pageID, parendID + "password");
					string challengeID = String.Format("document.forms[\"{0}\"].elements[\"{1}\"].value", pageID, parendID + "challenge");
					*/
				
				// form the script that is to be registered at client side.
				String scriptString = "<script type=\"text/javascript\" language=\"JavaScript\" src=\"scripts/md5.js\"></script>\n";
				scriptString += "<script type=\"text/javascript\" language=\"javascript\">\n";
				scriptString += "	function doChallengeResponse() {\n";
				// uncomment to debug in IE.
				// IE -> Tools -> Internet Options... -> Advanced -> Browsing
				// Disable Script Debugging should be unchecked
				// scriptString += "	debugger;\n";
				scriptString += "	password = document.getElementById('" + password.ClientID + "').value;\n";
				scriptString += "	if (password)	{\n";
				scriptString += "		password = net_md5(password);	// this makes it superchallenged!!\n";
				// get the value for the challenge
				scriptString += "		challenge = document.getElementById('" + challenge.ClientID + "').value;\n";
				// get the value for the username
				scriptString += "		username = document.getElementById('" + username.ClientID + "').value;\n";
				// create a challenge
				scriptString += "		str = challenge + password + username;\n";
				scriptString += "		str = net_md5(str);\n";
				// prepare the challenge and the password for the postback
				scriptString += "		document.getElementById('" + challenge.ClientID + "').value = str;\n";
				scriptString += "		document.getElementById('" + password.ClientID + "').value = '';\n";
				scriptString += "	}\n";
				scriptString += "}\n";
				scriptString += "</script>";

				if(!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"clientScript"))
				{
					// RyanG: We shouldn't enable the password hashing function if the user has this feature
					// disabled, otherwise the login never works if encryption is disabled.
					/*
						Page.RegisterClientScriptBlock("clientScript", scriptString);
						doSignIn.Attributes.Add("onclick", "doChallengeResponse();");
					*/
					
					if (SiteConfig.EncryptLoginPassword) 
					{
						Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"clientScript", scriptString);
						doSignIn.Attributes.Add("onclick", "doChallengeResponse();");
					}
				}
			}
			
			if (!Page.IsPostBack)
			{
				FormsAuthentication.SignOut();
				challenge.Value = Session.SessionID.ToString();
				ViewState["challenge"] = challenge.Value;
			}

            OpenIdLogin1.Visible = SiteConfig.AllowOpenIdAdmin;
		}

        private SiteConfig SiteConfig 
        {
            get 
            {
                return ((SharedBasePage)Page).SiteConfig;
            }
        }


		override protected void OnInit(EventArgs e)
		{
			base.OnInit(e);

            this.Load += new EventHandler(Page_Load);
            doSignIn.Click +=new EventHandler(doSignIn_Click);
            OpenIdLogin1.LoggedIn +=new EventHandler<OpenIdEventArgs>(OpenIdLogin1_LoggedIn);
		}
		

		public void SetAuthCookie(string tokenName, string userName)
		{
			FormsAuthentication.SetAuthCookie(userName, rememberCheckbox.Checked);
		}

        /// <summary>
        /// Fired upon login.
        /// Note, that straight after login, forms auth will redirect the user to their original page. So this page may never be rendererd.
        /// </summary>
        protected void OpenIdLogin1_LoggedIn(object sender, OpenIdEventArgs e)
        {
            // only allow the openid validation when it's enabled in the siteconfig
            if (SiteConfig.AllowOpenIdAdmin) 
            {
                e.Cancel = true; //Need to cancel or the control will log us in for free. Eek!
                UserToken token = SiteSecurity.Login(e.Response);
                if (token != null) {
                    SetAuthCookie(token.Name, token.Name);
                    Response.Redirect(SiteUtilities.GetAdminPageUrl(), true);
                }
            }
        }

        // handles the button click
		protected void doSignIn_Click(object sender, System.EventArgs e)
		{

			if (SiteConfig.EncryptLoginPassword)
			{
				string viewStateChallenge = ViewState["challenge"] as string;
                if (viewStateChallenge == null) { throw new ArgumentException("Password Challenge was null in ViewState!"); }
				UserToken token = SiteSecurity.Login(username.Text, challenge.Value, viewStateChallenge.ToString());
				if (token != null)
				{
					SetAuthCookie(token.Name, username.Text);
					Response.Redirect(SiteUtilities.GetAdminPageUrl(), true);
				}
				else
				{
					challenge.Value = Session.SessionID.ToString();
					ViewState["challenge"] = challenge.Value;
				}
			}
			else
			{
				UserToken token = SiteSecurity.Login(username.Text, password.Text);
				if (token != null)
				{
					SetAuthCookie(token.Name, username.Text);
					Response.Redirect(SiteUtilities.GetAdminPageUrl(), true);
				}
			}
		}
	}
}
