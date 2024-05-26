using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using System;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Encodings.Web;

namespace DasBlog.Web.Controllers
{
	[Produces("application/activity+json")]
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
		private readonly JsonSerializerOptions jsonSerOptions; 

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
			jsonSerOptions =  new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
		}

		[HttpGet(".well-known/webfinger")]
		public ActionResult WebFinger()
		{
			var webfinger = activityPubManager.GetWebFinger();
			if (webfinger != null)
			{
				return Json(webfinger, jsonSerOptions);
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
				return Json(actor, jsonSerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("api/outbox")]
		public IActionResult Outbox()
		{
			return Ok();
			
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
			
			return Json(outbox, jsonSerOptions);
		}

		[HttpPost]
		[Route("api/inbox")]
        public async Task<JsonResult> Inbox()
        {
			// return Accepted();

			// will receive POST requests each time someone follows/unfollows the blog, replies, deletes a comment, etc.
			// If we intend to follow other people, it will also receive the posts created in other instances?
			var requestbody = string.Empty;
			var requestheader = httpContextAccessor.HttpContext.Request.Headers;

			using (var reader = new StreamReader(httpContextAccessor.HttpContext.Request.Body))
            {
                requestbody = await reader.ReadToEndAsync();
            }

			InboxMessage? message;
			try
			{
				message = JsonSerializer.Deserialize<InboxMessage>(requestbody, jsonSerOptions);
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

			var response = new JsonResult(string.Empty) { StatusCode = (int)HttpStatusCode.Accepted, ContentType = "application/activity+json" };

			try
			{
				if(message.Actor == null)
				{
					return new JsonResult(string.Empty) { StatusCode = (int)HttpStatusCode.Unauthorized };
				}

				if (message?.IsFollow() ?? false)
				{
					await activityPubManager.Follow(message);
				}
				else if (message?.IsUndoFollow() ?? false)
				{
					await activityPubManager.Unfollow(message, requestbody);
				}
				else if (message?.IsCreateActivity() ?? false)
				{
					await activityPubManager.AddReply(message);
				}
				else if (message?.IsLikeRequest() ?? false)
				{
					await activityPubManager.Like(message);
				}
				else if (message?.IsEchoRequest() ?? false)
				{
					return new JsonResult(string.Empty) { StatusCode = (int)HttpStatusCode.Accepted };
				}
				else
				{
                    return new JsonResult(string.Empty) { StatusCode = (int)HttpStatusCode.InternalServerError };
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
		[Route("api/notes/{id}")]
		public IActionResult Notes(string id)
		{
			var notes = string.Empty;

			return Json(notes, jsonSerOptions);
		}

		[HttpGet]
		[Route("api/replies/{id}")]
		public IActionResult Replies(string id)
		{
			var replies = string.Empty;

			return Json(replies, jsonSerOptions);
		}

	}
}
