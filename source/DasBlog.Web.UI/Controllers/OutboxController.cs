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
		private readonly IMapper mapper;

		public OutboxController(IActivityPubManager activityPubManager, IDasBlogSettings dasBlogSettings, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.activityPubManager = activityPubManager;
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
				var userpage = activityPubManager.GetUserPage();
				var upvm = mapper.Map<UserPageViewModel>(userpage);
				upvm.orderedItems = userpage.OrderItems.Select(entry => mapper.Map<OrderedItemViewModel>(entry)).ToArray();

				return Json(upvm, jsonSerializerOptions);
			}

			var userinfo = activityPubManager.GetUser();
			var uvm = mapper.Map<UserViewModel>(userinfo);

			return Json(uvm, jsonSerializerOptions);
		}
	}
}
