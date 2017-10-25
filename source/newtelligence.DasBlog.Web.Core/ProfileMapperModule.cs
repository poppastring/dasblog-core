using System;
using System.Web;

namespace newtelligence.DasBlog.Web.Core
{
	public class ProfileMapperModule : IHttpModule
	{
        static readonly string[] HttpHandlerStrings = new string[] {"CaptchaImage.aspx", "aggbug.ashx", "blogger.aspx", "pingback.aspx", "trackback.aspx", "get_aspx_ver.aspx"};
        private EventHandler onBeginRequest;

		public ProfileMapperModule()
		{
			onBeginRequest = new EventHandler( HandleBeginRequest );
		}

		public void Init( HttpApplication context )
		{
			context.BeginRequest += onBeginRequest;
		}

		public void Dispose()
		{
		}

		private void HandleBeginRequest( object sender, EventArgs evargs )
		{
			HttpApplication app = sender as HttpApplication;
			string requestUrl = "";

            if ( app != null )
            {
                requestUrl = app.Context.Request.Url.PathAndQuery;
                if ( requestUrl.IndexOf(",") == -1 && requestUrl.IndexOf("?") == -1 && requestUrl.EndsWith(".aspx") )
                {
                    if ( app.Context.Request.Url.Segments.Length >= 2 )
                    {
                        string title     = app.Context.Request.Url.Segments[app.Context.Request.Url.Segments.Length - 1];
                        string directory = app.Context.Request.Url.Segments[app.Context.Request.Url.Segments.Length - 2];

                        if ( Array.IndexOf(HttpHandlerStrings, title) == -1 && directory.ToUpper() == "PROFILES/" )
                        {
                            title = title.Replace(".aspx", "");
                            title = title.Replace(" ", "");
                            title = title.Replace("%2b", " ");
                            title = title.Replace("%20", " "); 
                            title = title.Replace("+", " ");
                            title = title.Replace("-", " ");

                            User user = SiteSecurity.GetUserByDisplayName( title );

                            if ( user != null )
                            {
                                requestUrl = "~/Profile.aspx?user=" + user.Name;
                                app.Context.RewritePath( requestUrl );
                            }
                            else
                            {
                                app.Response.Redirect( "FormatPage.aspx?path=SiteConfig/pageerror.format.html", true );

                            }
                        }
                    }
                }
            }
		}
	}
}