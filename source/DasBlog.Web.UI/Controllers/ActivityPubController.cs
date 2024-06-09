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

namespace DasBlog.Web.Controllers
{
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
			jsonSerOptions =  new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };
		}

		[HttpGet(".well-known/webfinger")]
		[Produces("application/json")]
		public ActionResult WebFinger()
		{
			var webfinger = activityPubManager.GetWebFinger();
			if (webfinger != null)
			{
				return Ok(webfinger);
			}

			return NoContent();
		}

		[HttpGet("@blog")]
		[Produces("application/activity+json")]
		public IActionResult Actor()
		{
			var actor = activityPubManager.GetActor();
			if (actor != null)
			{
				return Ok(actor);
			}

			return NoContent();
		}

		[HttpGet("api/users/blog")]
		[Produces("application/activity+json")]
		public IActionResult BlogUser()
		{
			return Actor();
		}

		[HttpGet("api/outbox")]
		[Produces("application/activity+json")]
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
			
			return Ok(outbox);
		}

		[HttpGet("api/notes/{id}")]
		[Produces("application/activity+json")]
		public IActionResult Note(string id)
		{
			if (!string.IsNullOrEmpty(id))
			{
				var entry = blogManager.GetEntryForEdit(id);
				if (entry != null)
				{
					var outbox = activityPubManager.GetNote(entry);

					return Ok(outbox);
				}
			}

			return NotFound();
		}

		[HttpPost("api/inbox")]
		[Produces("application/activity+json")]
		public async Task<IActionResult> Inbox()
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
	
			try
			{
				if(message.Actor == null)
				{
					return Unauthorized();
				}

				if (message?.IsFollow() ?? false)
				{
					// Add to follower list

					await activityPubManager.Follow(message);
				}
				else if (message?.IsUndoFollow() ?? false)
				{
					// Remove from follower list

					await activityPubManager.Unfollow(message, requestbody);
				}
				else if (message?.IsCreateActivity() ?? false)
				{
					var status = AddCommentFromMessage(message);
					if (status != CommentSaveState.Added)
					{
						return StatusCode(500);
					}
				}
				else if (message?.IsLikeRequest() ?? false)
				{
					AddCommentFromMessage(message);

					await activityPubManager.Like(message);
				}
				else if (message?.IsEchoRequest() ?? false)
				{
					return Accepted();
				}
				else
				{
					return StatusCode(500);
				}
			}
			catch (Exception e)
			{
				logger.LogError(e.ToString());
				throw;
			}

			return Accepted();
        }

		[HttpGet("api/replies/{postid}")]
		[Produces("application/activity+json")]
		public IActionResult Replies(string postid)
		{
			var comments = blogManager.GetComments(postid, true)
					.Where(x => x.IsActivityPubReply == true).ToList();

			var replies = activityPubManager.GetReplies(postid, comments);

			return Ok(replies);
		}

		private CommentSaveState AddCommentFromMessage(InboxMessage message)
		{
			string postid = message.Context.ToString();

			var comment = new Comment
			{
				IsActivityPubLike = message.IsLikeRequest(),
				IsActivityPubReply = message.IsCreateActivity(),
				ActivityPubUrl = message.Object.Id.ToString(),
				IsPublic = false
			};

			return blogManager.AddComment(postid, comment);
		}
	}
}
