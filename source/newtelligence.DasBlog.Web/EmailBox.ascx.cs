using System;
using System.Web;
using System.Web.UI;
using System.Text;
using System.Net;
using System.Net.Mail;

using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{


	/// <summary>
	///		Summary description for EmailBox.
	/// </summary>
	public partial class EmailBox :UserControl
	{

		protected System.Resources.ResourceManager resmgr;
		private SharedBasePage requestPage;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			requestPage = Page as SharedBasePage;
        
			// if you are commenting on your own blog, no need for Captha
			if (SiteSecurity.IsValidContributor())
			{
				CaptchaControl1.Enabled = CaptchaControl1.Visible = false;				
			}
			else
			{
				CaptchaControl1.Enabled = CaptchaControl1.Visible = requestPage.SiteConfig.EnableCaptcha;
			}

			resmgr = ApplicationResourceTable.Get();

			if (!IsPostBack)
			{
				if (Request.Cookies["name"] != null)
				{
					string nameStr = HttpUtility.UrlDecode(Request.Cookies["name"].Value, Encoding.UTF8);
					//truncate at 32 chars to avoid abuse...
					name.Text = nameStr.Substring(0,Math.Min(32,nameStr.Length));
				}

				if (Request.Cookies["email"] != null)
				{
					email.Text = HttpUtility.UrlDecode(Request.Cookies["email"].Value, Encoding.UTF8);
				}
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		
		protected void mailSend_Click(object sender, System.EventArgs e)
		{
			

			if (CaptchaControl1.Enabled && requestPage.SiteConfig.EnableCaptcha == true)
			{
				if (CaptchaControl1.UserValidated == false) 
				{
					return;
				}
			}
			
			if ( Page.IsValid )
			{
				if (rememberMe.Checked)
				{
					string path = HttpRuntime.AppDomainAppVirtualPath;
					
					//We don't encode the name so High Latin folks like Ren Drie?el don't break
					// the Http Input Validation stuff.
					HttpCookie cookieName = new HttpCookie("name",name.Text);
					cookieName.Path = path;
					Response.Cookies.Add(cookieName);

					HttpCookie cookieEmail = new HttpCookie("email", HttpUtility.HtmlEncode(email.Text));
					cookieEmail.Path = path;
					Response.Cookies.Add(cookieEmail);
                    
					Response.Cookies["name"].Expires = DateTime.MaxValue;
					Response.Cookies["email"].Expires = DateTime.MaxValue;
				}

                if ( requestPage.SiteConfig.SendCommentsByEmail && !String.IsNullOrEmpty(requestPage.SiteConfig.SmtpServer))
				{
					SendMailInfo defaultMailInfo = ComposeMail();
			
					requestPage.DataService.RunActions(new object[] { defaultMailInfo});
						
					string commentShort = defaultMailInfo.Message.Body.Replace("\n"," ");
					if ( commentShort.Length > 50 )
					{
						commentShort = commentShort.Substring(0,50)+"...";
					}
					requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.CommentEmail,
					commentShort ,  string.Format("{0} {1}", defaultMailInfo.Message.From,  HttpUtility.HtmlEncode(name.Text)  )));
				}
				Response.Redirect(SiteUtilities.GetStartPageUrl(requestPage.SiteConfig), false);
			}
		}


		private SendMailInfo ComposeMail()
		{
			

			MailMessage emailMessage = new MailMessage();

			if (requestPage.SiteConfig.NotificationEMailAddress != null && 
				requestPage.SiteConfig.NotificationEMailAddress.Length > 0 )
			{
				emailMessage.To.Add(requestPage.SiteConfig.NotificationEMailAddress);
			}
			else
			{
				emailMessage.To.Add(requestPage.SiteConfig.Contact);
			}

            string from = HttpUtility.HtmlEncode(email.Text);

			emailMessage.Subject = String.Format
				( "Weblog Mail from '{0} ({1})' on '{2}'"
				, HttpUtility.HtmlEncode(name.Text)
                , from
				, HttpUtility.HtmlEncode(requestPage.SiteConfig.Title));

			emailMessage.Body =  HttpUtility.HtmlEncode (comment.Text);
            emailMessage.IsBodyHtml = false;
			emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
			
			if (from != null && from.Length > 0 )
			{
				emailMessage.From = new MailAddress(from);
			}
			else
			{
				emailMessage.From = new MailAddress(requestPage.SiteConfig.Contact);
			}

			emailMessage.Headers.Add("Sender", requestPage.SiteConfig.Contact);
						
			// add the X-Originating-IP header
			string hostname = Dns.GetHostName();
			IPHostEntry ipHostEntry = Dns.GetHostEntry(hostname);
						
			if (ipHostEntry.AddressList.Length > 0)
			{
				emailMessage.Headers.Add("X-Originating-IP", ipHostEntry.AddressList[0].ToString());
			}
			SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, requestPage.SiteConfig.SmtpServer,
				requestPage.SiteConfig.EnableSmtpAuthentication,  requestPage.SiteConfig.UseSSLForSMTP, requestPage.SiteConfig.SmtpUserName, 
				requestPage.SiteConfig.SmtpPassword, requestPage.SiteConfig.SmtpPort);

			return sendMailInfo;
		}
	}
}
