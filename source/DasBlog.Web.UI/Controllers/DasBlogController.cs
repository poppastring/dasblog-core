using DasBlog.Core.Common;
using DasBlog.Core.Exceptions;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DasBlog.Web.Controllers
{
	public abstract class DasBlogController : Controller
	{
		protected const string CACHEKEY_RSS = "CACHEKEY_RSS";
		protected const string CACHEKEY_FRONTPAGE = "CACHEKEY_FRONTPAGE";
		protected const string CACHEKEY_ARCHIVE = "CACHEKEY_ARCHIVE";

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

		protected class ViewBagConfigurer
		{
			public void ConfigureViewBag(dynamic viewBag, string maintenanceMode)
			{
				viewBag.Action = maintenanceMode;
				switch (maintenanceMode)
				{
					case Constants.UsersViewMode:
						viewBag.Linkability = string.Empty;
						viewBag.Writability = "readonly";
						viewBag.Clickability = "disabled";
						break;
					default:
						viewBag.Linkability = "disabled";
						viewBag.Writability = string.Empty;
						viewBag.Clickability = string.Empty;
						break;
				}
			}

			private IDictionary<string, string> mapActionToView = new Dictionary<string, string>
			{
				{Constants.UsersCreateMode, Constants.CreateUserSubView},
				{Constants.UsersEditMode, Constants.EditUserSubView},
				{Constants.UsersDeleteMode, Constants.DeleteUserSubView},
				{Constants.UsersViewMode, Constants.ViewUserSubView}
			};

			/// <summary>
			/// simple mapping
			/// </summary>
			/// <param name="Action">e.g. UsersEditAction</param>
			/// <returns>EditUsersSubView</returns>
			private string ActionToSubView(string action) => mapActionToView[action];

		}

	}
}
