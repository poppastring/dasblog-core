using System;
using System.Net.Http;
using DasBlog.Core.Services;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class ActivityController : Controller
	{
		private IActivityService activityService;

		public ActivityController(IActivityService activityService)
		{
			this.activityService = activityService;
		}

		[HttpGet]
		public IActionResult ActivityList()
		{
			return EventsByDate(DateTime.UtcNow);
		}
		[HttpGet(Name="/Activity/ActivityList/date")]
		public IActionResult EventsByDate(DateTime date)
		{
			var events = activityService.GetEventsForDay(date);
			ViewBag.Date = date.ToString("yyyy-MM-dd");
			ViewBag.NextDay = (date + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd");
			ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
			return View("ActivityList", events);
			
		}
	}
}
