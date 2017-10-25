using newtelligence.DasBlog.Web.Core.Amp;
using System;
using System.Web;

namespace newtelligence.DasBlog.Web.Core
{
    public class AmpifyModule : IHttpModule
    {
        private AmpifyPageFilter _pageFilter;

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
        }

        private void Context_BeginRequest(object sender, EventArgs e)
        {
            SiteConfig config = SiteConfig.GetSiteConfig();

            HttpApplication app = sender as HttpApplication;
            if (app != null && SiteUtilities.IsAMPage())
            {
                _pageFilter = new AmpifyPageFilter(app.Response.Filter);
                app.Response.Filter = _pageFilter;
            }
        }

        public void Dispose()
        {

        }
    }
}
