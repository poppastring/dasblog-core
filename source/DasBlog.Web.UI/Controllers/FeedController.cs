using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.Rss.Rss20;
using DasBlog.Services.Rss.Rsd;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DasBlog.Web.Controllers
{
    public class FeedController : DasBlogController
    {
        private IMemoryCache memoryCache;
        private readonly ISubscriptionManager subscriptionManager;
		private readonly IXmlRpcManager xmlRpcManager;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ILogger<FeedController> logger;

		public FeedController(ISubscriptionManager subscriptionManager, IXmlRpcManager xmlRpcManager, 
								IMemoryCache memoryCache, IDasBlogSettings dasBlogSettings, ILogger<FeedController> logger)
        {  
            this.subscriptionManager = subscriptionManager;
			this.xmlRpcManager = xmlRpcManager;
			this.memoryCache = memoryCache;
			this.dasBlogSettings = dasBlogSettings;
			this.logger = logger;
		}

		[Produces("text/xml")]
        [HttpGet("feed/rss"), HttpHead("feed/rss")]
        public IActionResult Rss()
        {
			if (!memoryCache.TryGetValue(CACHEKEY_RSS, out RssRoot rss))
			{
				rss = subscriptionManager.GetRss();

				memoryCache.Set(CACHEKEY_RSS, rss, SiteCacheSettings());
			}

			return Ok(rss);
        }

		[Produces("text/xml")]
		[HttpGet("feed/rss/{id}"), HttpHead("feed/rss/{id}")]
		public IActionResult RssItem(string id)
		{
			if (!memoryCache.TryGetValue(CACHEKEY_RSS+id, out RssItem rssItem))
			{
				rssItem = subscriptionManager.GetRssItem(id);

				memoryCache.Set(CACHEKEY_RSS+id, rssItem, SiteCacheSettings());
			}
			return Ok(rssItem);
		}

		[Produces("text/xml")]
		[HttpGet("feed/tags/{category}/rss"), HttpHead("feed/tags/{category}/rss")]
        public IActionResult RssByCategory(string category)
        {
			if (!memoryCache.TryGetValue(CACHEKEY_RSS + "_" + category, out RssRoot rss))
			{
				rss = subscriptionManager.GetRssCategory(category);

				if (rss.Channels[0]?.Items?.Count > 0)
				{
					memoryCache.Set(CACHEKEY_RSS + "_" + category, rss, SiteCacheSettings());
				}
			}

			if(rss.Channels[0]?.Items?.Count == 0)
			{
				return NoContent();
			}

			return Ok(rss);
        }

		[Produces("text/xml")]
		[HttpGet("feed/rsd")]
        public ActionResult Rsd()
        {
            RsdRoot rsd = null;

            rsd = subscriptionManager.GetRsd();

            return Ok(rsd);
        }

		[Produces("text/xml")]
		[HttpGet("feed/blogger")]
		public ActionResult Blogger()
		{
			// https://www.poppastring.com/blog/blogger.aspx
			// Implementation of Blogger XML-RPC Api
			// blogger
			// metaWebLog
			// mt

			return NoContent();
		}

		[Produces("text/xml")]
		[HttpPost("feed/blogger")]
		public async Task<IActionResult> BloggerPost()
		{
			var blogger = string.Empty;

			try
			{
				using (var mem = new MemoryStream())
				{
					await Request.Body.CopyToAsync(mem);
					blogger = xmlRpcManager.Invoke(mem);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(new EventDataItem(EventCodes.RSS, null, "FeedController.BloggerPost Error: {0}", ex.Message));
			}

			BreakSiteCache();

			return Content(blogger);
		}

		[HttpGet("feed/pingback")]
		public ActionResult PingBack()
		{
			return Ok();
		}

		[HttpGet("feed/rss/{entryid}/comments"), HttpHead("feed/rss/{entryid}/comments")]
		public ActionResult RssComments(string entryid)
		{
			return Ok();
		}

		[HttpGet("feed/trackback/{entryid}")]
		public ActionResult TrackBack(string entryid)
		{
			return Ok();
		}

		private void BreakSiteCache()
		{
			memoryCache.Remove(CACHEKEY_RSS);
			memoryCache.Remove(CACHEKEY_FRONTPAGE);
		}
	}
}
