using System.Linq;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ActivityLogs;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
    [Route("site")]
    public class SiteController : DasBlogController
    {
        private readonly ISiteManager siteManager;
		private readonly IBlogManager blogManager;
		private readonly IMemoryCache memoryCache;
		private readonly IMapper mapper;
		private readonly ILogger<SiteController> logger;

		public SiteController(ISiteManager siteManager, IBlogManager blogManager, IMemoryCache memoryCache, IMapper mapper, ILogger<SiteController> logger)
        {
			this.siteManager = siteManager;
			this.blogManager = blogManager;
			this.memoryCache = memoryCache;
			this.mapper = mapper;
			this.logger = logger;
		}

		[Produces("text/xml")]
		[Route("")]
        [HttpGet("map")]
        public ActionResult Map()
        {
            var sitemap = siteManager.GetGoogleSiteMap();

			logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Site Map request"));

			return Ok(sitemap);
        }

		[Produces("text/plain")]
		[HttpGet("microsummary")]
		public ActionResult MicroSummary()
		{
			if (!memoryCache.TryGetValue(CACHEKEY_FRONTPAGE, out ListPostsViewModel lpvm))
			{
				lpvm = new ListPostsViewModel
				{
					Posts = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry))
								.Select(editentry => editentry).ToList()
				};

				memoryCache.Set(CACHEKEY_FRONTPAGE, lpvm, SiteCacheSettings());
			};

			logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Microsummary request"));

			return Ok(lpvm?.Posts.FirstOrDefault()?.Title);
		}
	}
}
