using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AdminController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IFileSystemBinaryManager fileSystemBinaryManager;
		private readonly IMapper mapper;
		private readonly IBlogManager blogManager;
		private readonly IHostApplicationLifetime appLifetime;

		public AdminController(IDasBlogSettings dasBlogSettings, IFileSystemBinaryManager fileSystemBinaryManager, IMapper mapper,
								IBlogManager blogManager, IHostApplicationLifetime appLifetime) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileSystemBinaryManager = fileSystemBinaryManager;
			this.mapper = mapper;
			this.blogManager = blogManager;
			this.appLifetime = appLifetime;
		}

		[HttpGet]
		[Route("/admin")]
		[Route("/admin/settings")]
		public IActionResult Settings()
		{
			var dbsvm = new DasBlogSettingsViewModel();
			dbsvm.MetaConfig = mapper.Map<MetaViewModel>(dasBlogSettings.MetaTags);
			dbsvm.SiteConfig = mapper.Map<SiteViewModel>(dasBlogSettings.SiteConfiguration);

			return View(dbsvm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/settings")]
		public IActionResult Settings(DasBlogSettingsViewModel settings)
		{
			string savemessage = null;

			//save settings and reload...
			if (ModelState.ErrorCount > 0)
			{
				return Settings(settings);
			}

			var site = mapper.Map<SiteConfig>(settings.SiteConfig);
			var meta = mapper.Map<MetaTags>(settings.MetaConfig);

			site.ValidCommentTags = dasBlogSettings.SiteConfiguration.ValidCommentTags;
			site.SpamBlockingService = dasBlogSettings.SiteConfiguration.SpamBlockingService;
			site.CrosspostSites = dasBlogSettings.SiteConfiguration.CrosspostSites;
			site.PingServices = dasBlogSettings.SiteConfiguration.PingServices;

			if (!fileSystemBinaryManager.SaveSiteConfig(site))
			{
				ModelState.AddModelError("", "Unable to save Site configuration file.");
				return Settings(settings);
			}
			dasBlogSettings.SiteConfiguration = site;

			if (!fileSystemBinaryManager.SaveMetaConfig(meta))
			{
				ModelState.AddModelError("", "Unable to save Meta configuration file.");
				return Settings(settings);
			}
			dasBlogSettings.MetaTags = meta;

			TempData["MessageSaved"] = "Saved";

			return Settings();
		}
		public IActionResult TestEmail()
		{
			if (!blogManager.SendTestEmail())
			{
				ModelState.AddModelError("", "Unable to save Site configuration file.");
			}

			return RedirectToAction("Settings");
		}

		public IActionResult RestartSite()
		{
			appLifetime.StopApplication();

			return RedirectToAction("Settings");
		}

	}
}
