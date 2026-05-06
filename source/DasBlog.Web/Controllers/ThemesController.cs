using System;
using System.Linq;
using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class ThemesController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IFileSystemBinaryManager fileSystemBinaryManager;
		private readonly IThemeManager themeManager;
		private readonly IThemeContentValidator themeContentValidator;
		private readonly IMapper mapper;
		private readonly IOptionsMonitor<SiteConfig> siteConfigMonitor;
		private readonly ILogger<ThemesController> logger;

		public ThemesController(
			IDasBlogSettings dasBlogSettings,
			IFileSystemBinaryManager fileSystemBinaryManager,
			IThemeManager themeManager,
			IThemeContentValidator themeContentValidator,
			IMapper mapper,
			IOptionsMonitor<SiteConfig> siteConfigMonitor,
			ILogger<ThemesController> logger) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileSystemBinaryManager = fileSystemBinaryManager;
			this.themeManager = themeManager;
			this.themeContentValidator = themeContentValidator;
			this.mapper = mapper;
			this.siteConfigMonitor = siteConfigMonitor;
			this.logger = logger;
		}

		[HttpGet]
		[Route("/admin/themes")]
		public IActionResult Index()
		{
			var vm = new ThemeAdminListViewModel
			{
				Themes = themeManager.ListThemes(),
				ActiveTheme = dasBlogSettings.SiteConfiguration.Theme
			};
			return View("Index", vm);
		}

		[HttpGet]
		[Route("/admin/themes/create")]
		public IActionResult Create()
		{
			var vm = new CreateThemeViewModel
			{
				SourceTheme = "dasblog",
				AvailableSources = themeManager.ListThemes().Select(t => t.Name).ToList()
			};
			return View("Create", vm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/themes/create")]
		public IActionResult Create(CreateThemeViewModel model)
		{
			model.AvailableSources = themeManager.ListThemes().Select(t => t.Name).ToList();

			if (!ModelState.IsValid)
			{
				return View("Create", model);
			}

			try
			{
				themeManager.CreateTheme(model.Name, model.SourceTheme);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Theme '{0}' created from '{1}'", model.Name, model.SourceTheme));
				TempData["SuccessMessage"] = $"Theme '{model.Name}' created.";
				return RedirectToAction("Edit", new { name = model.Name });
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				return View("Create", model);
			}
		}

		[HttpGet]
		[Route("/admin/themes/edit/{name}")]
		public IActionResult Edit(string name)
		{
			try
			{
				var vm = new ThemeEditViewModel
				{
					Theme = themeManager.GetTheme(name),
					Files = themeManager.ListThemeFiles(name),
					SummaryModeEnabled = dasBlogSettings.SiteConfiguration.ShowItemSummaryInAggregatedViews
				};
				return View("Edit", vm);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return RedirectToAction("Index");
			}
		}

		[HttpGet]
		[Route("/admin/themes/edit/{name}/file")]
		public IActionResult EditFile(string name, string path)
		{
			try
			{
				// Lazily copy a materializable core file (e.g. Archive.cshtml)
				// from the site default into the theme folder the first time
				// the user clicks Edit on it.
				if (!themeManager.IsDefaultTheme(name)
					&& themeManager.IsMaterializableCoreFile(path))
				{
					if (themeManager.MaterializeCoreFile(name, path))
					{
						logger.LogInformation(new EventDataItem(EventCodes.Site, null,
							"Theme file '{0}/{1}' created from site default", name, path));
						TempData["SuccessMessage"] =
							$"Created {path} from the default site version. Edit and save to customize it.";
					}
				}

				var vm = new ThemeFileEditViewModel
				{
					ThemeName = name,
					RelativePath = path,
					Content = themeManager.ReadFile(name, path),
					IsDefaultTheme = themeManager.IsDefaultTheme(name),
					IsActiveTheme = themeManager.IsActiveTheme(name),
					Backups = themeManager.ListBackups(name, path)
				};
				vm.IsReadOnly = vm.IsDefaultTheme;
				return View("EditFile", vm);
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
				return RedirectToAction("Edit", new { name });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/themes/edit/{name}/file")]
		public IActionResult EditFile(string name, ThemeFileEditViewModel model)
		{
			model.ThemeName = name;
			model.IsDefaultTheme = themeManager.IsDefaultTheme(name);
			model.IsActiveTheme = themeManager.IsActiveTheme(name);
			model.IsReadOnly = model.IsDefaultTheme;

			if (model.IsReadOnly)
			{
				TempData["ErrorMessage"] = "This theme is locked from edits.";
				return RedirectToAction("Edit", new { name });
			}

			var validation = themeContentValidator.Validate(model.RelativePath, model.Content);
			if (!validation.IsValid)
			{
				foreach (var err in validation.Errors)
				{
					ModelState.AddModelError(string.Empty,
						$"Line {err.Line}{(err.Column > 0 ? ", col " + err.Column : string.Empty)}: {err.Message}");
				}
				logger.LogWarning(new EventDataItem(EventCodes.Site, null,
					"Theme file '{0}/{1}' rejected by pre-flight validation ({2} error(s)).",
					name, model.RelativePath, validation.Errors.Count));
				model.Backups = themeManager.ListBackups(name, model.RelativePath);
				return View("EditFile", model);
			}

			try
			{
				themeManager.WriteFile(name, model.RelativePath, model.Content);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Theme file '{0}/{1}' updated", name, model.RelativePath));
				TempData["SuccessMessage"] = $"Saved {model.RelativePath}. The previous version was kept and can be restored.";
				return RedirectToAction("EditFile", new { name, path = model.RelativePath });
			}
			catch (Exception ex)
			{
				model.Backups = themeManager.ListBackups(name, model.RelativePath);
				ModelState.AddModelError(string.Empty, ex.Message);
				return View("EditFile", model);
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/themes/edit/{name}/revert")]
		public IActionResult RevertFile(string name, string path, string backupId)
		{
			try
			{
				themeManager.RevertFile(name, path, backupId);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Theme file '{0}/{1}' reverted to backup '{2}'", name, path, backupId));
				TempData["SuccessMessage"] = $"Reverted {path} to a previous version.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("EditFile", new { name, path });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/themes/delete/{name}")]
		public IActionResult Delete(string name)
		{
			try
			{
				themeManager.DeleteTheme(name);
				logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Theme '{0}' deleted", name));
				TempData["SuccessMessage"] = $"Theme '{name}' deleted.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}
			return RedirectToAction("Index");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/themes/setactive/{name}")]
		public IActionResult SetActive(string name)
		{
			try
			{
				var theme = themeManager.GetTheme(name);
				var site = siteConfigMonitor.CurrentValue;
				site.Theme = theme.Name;

				if (!fileSystemBinaryManager.SaveSiteConfig(site))
				{
					TempData["ErrorMessage"] = "Unable to save site configuration.";
					logger.LogError(new EventDataItem(EventCodes.Error, null, "Unable to save site config when setting active theme"));
					return RedirectToAction("Index");
				}

				logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Active theme set to '{0}'", theme.Name));
				TempData["SuccessMessage"] = $"Active theme set to '{theme.Name}'.";
			}
			catch (Exception ex)
			{
				TempData["ErrorMessage"] = ex.Message;
			}

			return RedirectToAction("Index");
		}
	}
}
