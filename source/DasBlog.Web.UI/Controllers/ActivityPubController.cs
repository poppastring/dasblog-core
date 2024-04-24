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
		public ActionResult WebFinger(string resource)
		{
			if (resource.StartsWith("acct:"))
			{
				resource = resource.Remove(0, 5);
			}

			var webfinger = activityPubManager.WebFinger();
			if (webfinger != null)
			{
				return Json(webfinger, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("@{user}")]
		public IActionResult Actor(string user)
		{
			var mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			var actor = activityPubManager.Actor();

			return Json(actor, jsonSerializerOptions);
		}

		[HttpGet]
		[Route("api/outbox")]
		public IActionResult Outbox(string user, bool page)
		{
			var outbox = string.Empty;

			return Json(outbox, jsonSerializerOptions);
		}


		[HttpPost]
		[Route("api/inbox")]
		public IActionResult Inbox(string user, bool page)
		{
			var inbox = string.Empty;

			return Json(inbox, jsonSerializerOptions);
		}
	}
}
