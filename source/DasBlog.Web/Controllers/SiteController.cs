using System.Linq;
using System.Text;
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

			return Ok(sitemap);
		}

		[Produces("application/xml")]
		[HttpGet("/sitemap.xml")]
		public ActionResult SitemapXml()
		{
			var sitemap = siteManager.GetGoogleSiteMap();

			return Ok(sitemap);
		}

		[HttpGet("/robots.txt")]
		public ActionResult RobotsTxt()
		{
			var scheme = Request.Scheme;
			var host = Request.Host.ToUriComponent();
			var pathBase = Request.PathBase.HasValue ? Request.PathBase.ToUriComponent() : string.Empty;
			var sitemapUrl = $"{scheme}://{host}{pathBase}/sitemap.xml";

			var sb = new StringBuilder();
			sb.Append("User-agent: *\n");
			sb.Append("Disallow: ").Append(pathBase).Append("/account/login\n");
			sb.Append('\n');
			sb.Append("Sitemap: ").Append(sitemapUrl).Append('\n');

			return Content(sb.ToString(), "text/plain", Encoding.UTF8);
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
