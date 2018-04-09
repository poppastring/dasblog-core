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
    public class SiteController : Controller
    {
        private ISiteRepository _siteRepository;

        public SiteController(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [Route("")]
        [HttpGet("map")]
        public ActionResult Map()
        {
            urlset sitemap = _siteRepository.GetGoogleSiteMap();

            return Ok(sitemap);
        }
    }
}