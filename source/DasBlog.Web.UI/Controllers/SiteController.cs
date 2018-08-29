using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Managers.Interfaces;

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
            urlset sitemap = siteManager.GetGoogleSiteMap();

            return Ok(sitemap);
        }
    }
}
