using System.Linq;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DasBlog.Web.Controllers
{
    [Route("site")]
    public class SiteController : DasBlogController
    {
        private readonly ISiteManager siteManager;
		private readonly IBlogManager blogManager;
		private readonly IMemoryCache memoryCache;
		private readonly IMapper mapper;

		public SiteController(ISiteManager siteManager, IBlogManager blogManager, IMemoryCache memoryCache, IMapper mapper)
        {
			this.siteManager = siteManager;
			this.blogManager = blogManager;
			this.memoryCache = memoryCache;
			this.mapper = mapper;
		}

		[Produces("text/xml")]
		[Route("")]
        [HttpGet("map")]
        public ActionResult Map()
        {
            var sitemap = siteManager.GetGoogleSiteMap();

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

			return Ok(lpvm?.Posts.FirstOrDefault()?.Title);
		}
	}
}
