using System.Diagnostics;
using DasBlog.Core.Exceptions;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	public abstract class DasBlogController : Controller
	{
		// avoid the exception handling middleware which would log the exception again
		public virtual IActionResult HandleError(string message, LoggedException ex)
		{
			return View(nameof(HomeController.Error)
			  ,new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
						// a bit of cargo-culting here - Activity is some sort of diagnostic thing
						// as presumably is TraceIdentifier
		}
	}
}
