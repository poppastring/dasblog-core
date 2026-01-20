using System;
using System.Linq;
using DasBlog.Core.Exceptions;
using DasBlog.Services.ActivityLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	[Route("admin/log")]
	public class ActivityController : DasBlogController
	{
		private readonly IActivityService activityService;

		public ActivityController(IActivityService activityService)
		{
			this.activityService = activityService;
		}

		[HttpGet]
		[Route("")]
		public IActionResult Index()
		{
			return EventsByDate(DateTime.Today);
		}

		[HttpGet]
		[Route("{date:datetime}")]
		public IActionResult EventsByDate(DateTime date)
		{
			try
			{
				var events = activityService.GetEventsForDay(date);

				ViewBag.Date = date.ToString("yyyy-MM-dd");
				ViewBag.NextDay = (date + new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd");
				ViewBag.PreviousDay = (date - new TimeSpan(1, 0, 0, 0)).ToString("yyyy-MM-dd");
				ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
				return View("ActivityList", events);
			}
			catch (LoggedException e)
			{
				return HandleError("Failed to display activity list", e);
			}
		}
	}
}
