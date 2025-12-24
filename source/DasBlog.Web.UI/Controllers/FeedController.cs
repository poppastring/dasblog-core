using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.Rss.Atom;
using DasBlog.Services.Rss.Rss20;
using DasBlog.Services.Rss.Rsd;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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
		[HttpGet("feed/rss/{category}"), HttpHead("feed/rss/{category}")]
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

			[HttpGet("feed/atom"), HttpHead("feed/atom")]
			public IActionResult Atom()
			{
				if (!memoryCache.TryGetValue(CACHEKEY_ATOM, out AtomRoot atom))
				{
					atom = subscriptionManager.GetAtom();

					memoryCache.Set(CACHEKEY_ATOM, atom, SiteCacheSettings());
				}

				return AtomContent(atom);
			}

			[HttpGet("feed/atom/{category}"), HttpHead("feed/atom/{category}")]
			public IActionResult AtomByCategory(string category)
			{
				if (!memoryCache.TryGetValue(CACHEKEY_ATOM + "_" + category, out AtomRoot atom))
				{
					atom = subscriptionManager.GetAtomCategory(category);

					if (atom.Entries?.Count > 0)
					{
						memoryCache.Set(CACHEKEY_ATOM + "_" + category, atom, SiteCacheSettings());
					}
				}

				if (atom.Entries?.Count == 0)
				{
					return NoContent();
				}

				return AtomContent(atom);
			}

			/// <summary>
			/// Serializes AtomRoot to XML with proper namespace handling (no xsi/xsd declarations)
			/// </summary>
			private ContentResult AtomContent(AtomRoot atom)
			{
				var serializer = new XmlSerializer(typeof(AtomRoot));
				var settings = new XmlWriterSettings
				{
					Indent = false,
					OmitXmlDeclaration = false,
					Encoding = new UTF8Encoding(false) // UTF-8 without BOM
				};

				using var memoryStream = new MemoryStream();
				using (var xmlWriter = XmlWriter.Create(memoryStream, settings))
				{
					serializer.Serialize(xmlWriter, atom, atom.Namespaces);
				}

				var xmlContent = Encoding.UTF8.GetString(memoryStream.ToArray());
				return Content(xmlContent, "application/atom+xml; charset=utf-8");
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

		[HttpGet("feed/rss/comments/{entryid}"), HttpHead("feed/rss/comments/{entryid}")]
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
					memoryCache.Remove(CACHEKEY_ATOM);
					memoryCache.Remove(CACHEKEY_FRONTPAGE);
				}
			}
		}
