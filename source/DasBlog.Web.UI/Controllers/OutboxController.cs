using System.Linq;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	/// <summary>
	/// How clients can post Activities to this actor's outbox 
	/// A feed of whatever the user shares publicly.
	/// </summary>
	[Produces("text/json")]
	public class OutboxController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;

		public OutboxController(IActivityPubManager activityPubManager, IBlogManager blogManager, IDasBlogSettings dasBlogSettings, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.activityPubManager = activityPubManager;
			this.blogManager = blogManager;
			this.mapper = mapper;
		}

		[HttpGet]
		[Route("/users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			if(string.Compare(user, dasBlogSettings.SiteConfiguration.MastodonAccount, System.StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			if(page)
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
