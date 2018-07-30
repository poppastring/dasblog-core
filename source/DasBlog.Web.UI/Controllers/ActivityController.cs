using System;
using System.Diagnostics;
using DasBlog.Core.Exceptions;
using DasBlog.Core.Services;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class ActivityController : DasBlogController
	{
		private readonly IActivityService activityService;

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
				return HandleError("Failed to display activity list"
				  , e);
			}
			
		}
	}
}
