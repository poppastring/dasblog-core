using System;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Collections;
using System.Web;
using System.Web.Mail;
using System.Web.Security;
using newtelligence.DasBlog.Runtime.Proxies;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Services;

namespace newtelligence.DasBlog.Web
{
	/// <summary>
	/// Summary description for Global.
	/// </summary>
	public class Global : HttpApplication
	{
		private XSSUpstreamer xssUpstreamer = null;
		private Thread xssUpstreamerThread = null;
		private MailToWeblog mailToWeblog = null;
		private Thread mailToWeblogThread = null;
		private ReportMailer reportMailer = null;
		private Thread reportMailerThread = null;
		private bool identityLoaded = false;

		public Global()
		{
			InitializeComponent();
		}

		protected void Application_Start(Object sender, EventArgs e)
		{
			//We clear out the Cache on App Restart...
            CacheFactory.GetCache().Clear();

			ILoggingDataService loggingService = null;

			loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
			loggingService.AddEvent(new EventDataItem(EventCodes.ApplicationStartup, "", ""));

			SiteConfig siteConfig = SiteConfig.GetSiteConfig(SiteConfig.GetConfigFilePathFromCurrentContext());
			
			//if (siteConfig.EnableReportMailer)
			{
				reportMailer = new ReportMailer(
					SiteConfig.GetConfigFilePathFromCurrentContext(),
					SiteConfig.GetContentPathFromCurrentContext(),
					SiteConfig.GetLogPathFromCurrentContext()
					);

				reportMailerThread = new Thread(new ThreadStart(reportMailer.Run));
				reportMailerThread.Name = "ReportMailer";
				reportMailerThread.IsBackground = true;
				reportMailerThread.Start();

			}

			if (siteConfig.EnablePop3)
			{
				mailToWeblog = new MailToWeblog(
					SiteConfig.GetConfigFilePathFromCurrentContext(),
					SiteConfig.GetContentPathFromCurrentContext(),
					SiteConfig.GetBinariesPathFromCurrentContext(),
					SiteConfig.GetLogPathFromCurrentContext(),
					new Uri(new Uri(SiteConfig.GetSiteConfig().Root), SiteConfig.GetSiteConfig().BinariesDirRelative)
					);

				mailToWeblogThread = new Thread(new ThreadStart(mailToWeblog.Run));
				mailToWeblogThread.Name = "MailToWeblog";
				mailToWeblogThread.IsBackground = true;
				mailToWeblogThread.Start();
			}

			if (siteConfig.EnableXSSUpstream)
			{
				xssUpstreamer = new XSSUpstreamer(
					SiteConfig.GetConfigFilePathFromCurrentContext(),
					SiteConfig.GetContentPathFromCurrentContext(),
					SiteConfig.GetLogPathFromCurrentContext()
					);

				xssUpstreamerThread = new Thread(new ThreadStart(xssUpstreamer.Run));
				xssUpstreamerThread.Name = "XSSUpstreamer";
				xssUpstreamerThread.IsBackground = true;
				xssUpstreamerThread.Start();
			}

			/*
			if (siteConfig.EnableMovableTypeBlackList)
			{
				ReferralBlackListFactory.AddBlacklist(new MovableTypeBlacklist(), Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(), "blacklist.txt"));
			}
			*/

			if (siteConfig.EnableReferralUrlBlackList && siteConfig.ReferralUrlBlackList.Length != 0)
			{
				ReferralBlackListFactory.AddBlacklist(new ReferralUrlBlacklist(), siteConfig.ReferralUrlBlackList);
			}
		}

		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		System.Text.RegularExpressions.Regex oldAtom = new System.Text.RegularExpressions.Regex("SyndicationServiceExperimental.asmx",System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			if (identityLoaded == false)
			{
				Impersonation.ApplicationIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
				identityLoaded = true;
			}

			HttpRequest request = HttpContext.Current.Request;
			HttpResponse response = HttpContext.Current.Response;
					
			//Custom 301 Permanent Redirect for requests for the order Atom 0.3 feed
			if (oldAtom.IsMatch(request.Path))
			{
				if (request.RequestType == "GET")
				{
					response.StatusCode = 301;
					response.Status = "301 Moved Permanently";
					response.RedirectLocation = oldAtom.Replace(request.Path,"SyndicationService.asmx");
					response.End();
				}
			}
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{
			//Only needed on ASP.NET 1.1
			if(System.Environment.Version.Major < 2)
			{
				foreach(string cookie in Response.Cookies)
				{
					const string HTTPONLY = ";HttpOnly";
					string path = Response.Cookies[cookie].Path;
					if (path.EndsWith(HTTPONLY) == false)
					{
						//force HttpOnly to be added to the cookie problem
						Response.Cookies[cookie].Path += HTTPONLY;
					}
				}
			}
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{
			if (Request.IsAuthenticated == true)
			{
				HttpCookie authenCookie = HttpContext.Current.Request.Cookies.Get(FormsAuthentication.FormsCookieName);
				if (authenCookie == null)
				{
					FormsAuthentication.SignOut();
					HttpContext.Current.User = null;
					return;
				}
				FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authenCookie.Value);
				FormsIdentity id = new FormsIdentity(ticket);
				UserToken token = SiteSecurity.GetToken(ticket.Name);
				if (token != null)
				{
					GenericPrincipal principal = new GenericPrincipal(id, new string[] {token.Role});
					HttpContext.Current.User = principal;
				}
				else
				{
					FormsAuthentication.SignOut();
					HttpContext.Current.User = null;
				}
			}
		}

		protected void Application_Error(Object sender, EventArgs e)
		{
		}

		protected void Session_End(Object sender, EventArgs e)
		{
		}

		protected void Application_End(Object sender, EventArgs e)
		{
			if (mailToWeblogThread != null)
			{
				mailToWeblogThread.Abort();
				mailToWeblogThread.Join();
			}

			if (reportMailerThread != null)
			{
				reportMailerThread.Abort();
				reportMailerThread.Join();
			}

			if (xssUpstreamerThread != null)
			{
				xssUpstreamerThread.Abort();
				xssUpstreamerThread.Join();
			}
		}

		#region Web Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Global
			// 
			this.BeginRequest += new EventHandler(this.Application_BeginRequest);
			this.Error += new EventHandler(this.Application_Error);
			this.AuthenticateRequest += new EventHandler(this.Application_AuthenticateRequest);
			this.EndRequest += new EventHandler(this.Application_EndRequest);

		}

		#endregion
	}
}
