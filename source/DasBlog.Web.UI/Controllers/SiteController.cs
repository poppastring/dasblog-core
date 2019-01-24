using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
    [Produces("text/xml")]
    [Route("site")]
    public class SiteController : DasBlogController
    {
        private ISiteManager siteManager;

        public SiteController(ISiteManager siteManager)
        {
			this.siteManager = siteManager;
        }

        [Route("")]
        [HttpGet("map")]
        public ActionResult Map()
        {
            var sitemap = siteManager.GetGoogleSiteMap();

            return Ok(sitemap);
        }
    }
}
