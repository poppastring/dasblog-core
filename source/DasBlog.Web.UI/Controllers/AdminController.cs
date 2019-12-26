using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	[Route("admin")]
	public class AdminController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IFileSystemBinaryManager fileSystemBinaryManager;
		private readonly IMapper mapper;

		public AdminController(IDasBlogSettings dasBlogSettings, IFileSystemBinaryManager fileSystemBinaryManager, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileSystemBinaryManager = fileSystemBinaryManager;
			this.mapper = mapper;
		}

		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		[Route("settings")]
		public IActionResult Settings()
		{
			var dbsvm = new DasBlogSettingsViewModel();
			dbsvm.MetaConfig = mapper.Map<MetaViewModel>(dasBlogSettings.MetaTags);
			dbsvm.SiteConfig = mapper.Map<SiteViewModel>(dasBlogSettings.SiteConfiguration);

			return View(dbsvm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("settings")]
		public IActionResult Settings(DasBlogSettingsViewModel settings)
		{
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

			if (!fileSystemBinaryManager.SaveMetaConfig(meta))
			{
				ModelState.AddModelError("", "Unable to save Meta configuration file.");
				return Settings(settings);
			}

			return Settings();
		}
	}
}
