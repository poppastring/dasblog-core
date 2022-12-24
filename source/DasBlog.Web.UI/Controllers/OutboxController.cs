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
	public class OutboxController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public OutboxController(IDasBlogSettings settings) : base(settings)
		{
			dasBlogSettings = settings;
		}

		//users/mdownie/outbox?page=true
		[HttpGet]
		[Route("/users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			return Json("");
		}
	}
}
