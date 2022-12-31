using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

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

			var webfinger = activityPubManager.WebFinger(resource);
			if (webfinger != null)
			{
				var wfvm = mapper.Map<WebFingerViewModel>(webfinger);
				wfvm.links = webfinger.Links.Select(entry => mapper.Map<WebFingerLinkViewModel>(entry)).ToList();

				return Json(wfvm, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("@{user}")]
		[Route("users/{user}")]
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

			var actor = new ActorViewModel
			{
				id = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}"),
				type = "Person",
				following = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/following"),
				followers = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers"),
				inbox = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/inbox"),
				outbox = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/outbox"),
				preferredUsername = mastodonAccount,
				name = dasBlogSettings.SiteConfiguration.Title,
				summary = dasBlogSettings.SiteConfiguration.Description,
				url = dasBlogSettings.RelativeToRoot($"@{mastodonAccount}"),
				published = DateTime.UtcNow
			};

			return Json(actor, jsonSerializerOptions);
		}

		[HttpGet]
		[Route("users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			string mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			if (page)
			{
				var fpentries = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"]).ToList();

				var userpage = activityPubManager.GetUserPage(fpentries);
				var upvm = mapper.Map<UserPageViewModel>(userpage);

				upvm.orderedItems = userpage.OrderItems.Select(entry => mapper.Map<OrderedItemViewModel>(entry)).ToArray();

				return Json(upvm, jsonSerializerOptions);
			}

			var userinfo = activityPubManager.GetUser();
			var uvm = mapper.Map<UserViewModel>(userinfo);

			return Json(uvm, jsonSerializerOptions);
		}

		[HttpGet]
		[Route("@{user}/statuses/{id}/activity")]
		public IActionResult ObjectStatusActivity(string user, string id)
		{
			return NotFound();
		}
	}
}
