using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace DasBlog.Web.Controllers
{
	[Produces("text/json")]
	public class ActivityPubController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;

		public ActivityPubController(IActivityPubManager activityPubManager, IBlogManager blogManager, IDasBlogSettings dasBlogSettings, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.activityPubManager = activityPubManager;
			this.blogManager = blogManager;
			this.mapper = mapper;
		}

		[HttpGet(".well-known/webfinger")]
		public ActionResult WebFinger()
		{
			var webfinger = activityPubManager.WebFinger();
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
			var actor = activityPubManager.Actor();
			if (actor != null)
			{
				return Json(actor, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("api/outbox")]
		public IActionResult Outbox(string user, bool page)
		{
			// will contain references to all the posts from the blog
			var outbox = string.Empty;

			return Json(outbox, jsonSerializerOptions);
		}


		[HttpPost]
		[Route("api/inbox")]
		public IActionResult Inbox(string user, bool page)
		{
			// will receive POST requests each time someone follows/unfollows the blog, replies, deletes a comment, etc.
			// If we intend to follow other people, it will also receive the posts created in other instances?
			var inbox = string.Empty;

			return Json(inbox, jsonSerializerOptions);
		}
	}
}
