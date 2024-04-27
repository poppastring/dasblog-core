using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using Org.BouncyCastle.Ocsp;
using System.Net;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Produces("text/json")]
	public class ActivityPubController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;
		private readonly IBlogManager blogManager;
		private readonly IArchiveManager archiveManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IMapper mapper;
		private IMemoryCache memoryCache;
		private readonly ILogger<ActivityPubController> logger;

		public ActivityPubController(IActivityPubManager activityPubManager, IBlogManager blogManager,
								IArchiveManager archiveManager, IDasBlogSettings dasBlogSettings,
								IMemoryCache memoryCache, IMapper mapper, IHttpContextAccessor httpContextAccessor,
								ILogger<ActivityPubController> logger) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.activityPubManager = activityPubManager;
			this.blogManager = blogManager;
			this.archiveManager = archiveManager;
			this.mapper = mapper;
			this.memoryCache = memoryCache;
			this.httpContextAccessor = httpContextAccessor;
			this.logger = logger;
		}

		[HttpGet(".well-known/webfinger")]
		public ActionResult WebFinger()
		{
			var webfinger = activityPubManager.GetWebFinger();
			if (webfinger != null)
			{
				return Json(webfinger, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("@blog")]
		public IActionResult Actor()
		{
			var actor = activityPubManager.GetActor();
			if (actor != null)
			{
				return Json(actor, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("api/outbox")]
		public IActionResult Outbox()
		{
			// will contain references to all the posts from the blog
			if (!memoryCache.TryGetValue(CACHEKEY_ACTIVITYPUB, out EntryCollection entries))
			{	
				var languageFilter = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];
				var listofyears = archiveManager.GetDaysWithEntries().Select(i => i.Year).Distinct();

				entries = [];

				foreach (int year in listofyears)
				{
					entries.AddRange(
					archiveManager.GetEntriesForYear(new DateTime(year, 1, 1), languageFilter).OrderByDescending(x => x.CreatedUtc));
				}
				memoryCache.Set(CACHEKEY_ACTIVITYPUB, entries, SiteCacheSettings());
			}

			var outbox = activityPubManager.GenerateOutbox(entries);

			return Json(outbox, jsonSerializerOptions);
		}

		[HttpPost]
		[Route("api/inbox")]
        public async Task<IActionResult> Inbox()
        {
            // will receive POST requests each time someone follows/unfollows the blog, replies, deletes a comment, etc.
            // If we intend to follow other people, it will also receive the posts created in other instances?
            var requestbody = string.Empty;

            using (var reader = new StreamReader(httpContextAccessor.HttpContext.Request.Body))
            {
                requestbody = await reader.ReadToEndAsync();
            }

			var options = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			};

			InboxMessage? message;
			try
			{
				message = JsonSerializer.Deserialize<InboxMessage>(requestbody, options);
			}
			catch (Exception e)
			{
				logger.LogError(e.ToString());
				throw;
			}

			if (message?.IsDelete() ?? false)
			{
				throw new NotImplementedException("Delete not supported");
			}

			logger.LogInformation($"Received Activity: {requestbody}");

			var response = new JsonResult("") { StatusCode = (int)HttpStatusCode.OK, ContentType = "application/activity+json" };

			try
			{
				if (message?.IsFollow() ?? false)
				{
					// follow
				}
				else if (message?.IsUndoFollow() ?? false)
				{
					// Unfollow
					// get actor info
					// Send signed request to the actor's inbox				
				}
				else if (message?.IsCreateActivity() ?? false)
				{
					// add reply 
				}
				else
				{
					// unsupported activity
				}
			}
			catch (Exception e)
			{
				logger.LogError(e.ToString());
				throw;
			}

			return response;
        }

		[HttpGet]
		[Route("notes/{id}")]
		public IActionResult Notes(string id)
		{
			var notes = string.Empty;

			return Json(notes, jsonSerializerOptions);
		}

		[HttpGet]
		[Route("replies/{id}")]
		public IActionResult Replies(string id)
		{
			var replies = string.Empty;

			return Json(replies, jsonSerializerOptions);
		}

	}
}
