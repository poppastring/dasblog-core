using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;
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
		[Route("/users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			if (string.Compare(user, dasBlogSettings.SiteConfiguration.MastodonAccount, System.StringComparison.InvariantCultureIgnoreCase) != 0)
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
		[Route("/users/@{user}")]
		public IActionResult Actor(string user)
		{
			return NotFound();
		}

		[HttpGet]
		[Route("/@{user}/{id}")]
		public IActionResult ObjectSearch(string user, string id)
		{
			return NotFound();
		}

		[HttpGet]
		[Route("/@{user}/statuses/{id}/activity")]
		public IActionResult ObjectStatusActivity(string user, string id)
		{
			return NotFound();
		}
	}
}
