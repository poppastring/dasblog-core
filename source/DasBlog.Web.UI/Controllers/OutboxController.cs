using DasBlog.Managers.Interfaces;
using DasBlog.Services;
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

		public OutboxController(IActivityPubManager pubManager, IDasBlogSettings settings) : base(settings)
		{
			dasBlogSettings = settings;
			activityPubManager = pubManager;
		}

		[HttpGet]
		[Route("/users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			if(string.Compare(user, dasBlogSettings.SiteConfiguration.MastodonAccount, System.StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NoContent();
			}

			if(page)
			{
				activityPubManager.GetUserPage();
			}
			else
			{
				activityPubManager.GetUser();

			}

			return Json("");
		}
	}
}
