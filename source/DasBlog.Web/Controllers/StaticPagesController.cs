using System;
using DasBlog.Core.Common;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class StaticPagesController : DasBlogBaseController
	{
		private readonly IStaticPageManager staticPageManager;
		private readonly ILogger<StaticPagesController> logger;

		public StaticPagesController(
			IDasBlogSettings dasBlogSettings,
			IStaticPageManager staticPageManager,
			ILogger<StaticPagesController> logger) : base(dasBlogSettings)
		{
			this.staticPageManager = staticPageManager;
			this.logger = logger;
		}

		[HttpGet]
		[Route("/admin/staticpages")]
		public IActionResult Index()
		{
			var vm = new StaticPageListViewModel
			{
				Pages = staticPageManager.ListPages()
			};
			return View("Index", vm);
		}

		[HttpGet]
		[Route("/admin/staticpages/edit/{name}")]
		public IActionResult Edit(string name)
		{
			try
			{
				var info = staticPageManager.GetPage(name);
				var vm = new StaticPageEditViewModel
				{
					Name = info.Name,
					DisplayTitle = info.DisplayTitle,
					PublicUrlPath = info.PublicUrlPath,
					Content = staticPageManager.Read(name),
					Exists = info.Exists,
					Backups = staticPageManager.ListBackups(name)
				};
				return View("Edit", vm);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/staticpages/edit/{name}")]
		public IActionResult Edit(string name, StaticPageEditViewModel model)
		{
			model.Name = name;

			try
			{
				var info = staticPageManager.GetPage(name);
				model.DisplayTitle = info.DisplayTitle;
				model.PublicUrlPath = info.PublicUrlPath;

				staticPageManager.Write(name, model.Content);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null,
					"Static page '{0}' saved", name));
				TempData["SuccessMessage"] =
					$"Saved {info.DisplayTitle}. The previous version was kept and can be restored.";
				return RedirectToAction("Edit", new { name });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				model.Backups = staticPageManager.ListBackups(name);
				model.Exists = staticPageManager.GetPage(name).Exists;
				return View("Edit", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/staticpages/delete/{name}")]
		public IActionResult Delete(string name)
		{
			try
			{
				staticPageManager.Delete(name);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null,
					"Static page '{0}' deleted", name));
				TempData["SuccessMessage"] = $"Deleted '{name}'.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/staticpages/revert/{name}")]
		public IActionResult Revert(string name, string backupId)
		{
			try
			{
				staticPageManager.Revert(name, backupId);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null,
					"Static page '{0}' reverted to backup '{1}'", name, backupId));
				TempData["SuccessMessage"] = $"Reverted '{name}' to a previous version.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("Edit", new { name });
		}
	}
}
