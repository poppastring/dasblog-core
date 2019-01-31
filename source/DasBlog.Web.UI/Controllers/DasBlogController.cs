using DasBlog.Core.Exceptions;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Diagnostics;

namespace DasBlog.Web.Controllers
{
	public abstract class DasBlogController : Controller
	{
		protected const string CACHEKEY_RSS = "CACHEKEY_RSS";
		protected const string CACHEKEY_FRONTPAGE = "CACHEKEY_FRONTPAGE";

		// avoid the exception handling middleware which would log the exception again
		public virtual IActionResult HandleError(string message, LoggedException ex)
		{
			return View(nameof(HomeController.Error),
							new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
							// a bit of cargo-culting here - Activity is some sort of diagnostic thing
							// as presumably is TraceIdentifier
		}

		public virtual MemoryCacheEntryOptions SiteCacheSettings()
		{
			return new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(240));
		}
	}
}
