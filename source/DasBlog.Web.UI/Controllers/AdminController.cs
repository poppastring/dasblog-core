using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AdminController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;

		public AdminController(IDasBlogSettings dasBlogSettings, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.mapper = mapper;
		}

		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Settings()
		{
			var dbsvm = new DasBlogSettingsViewModel();
			dbsvm.MetaConfig = mapper.Map<MetaViewModel>(dasBlogSettings.MetaTags);
			dbsvm.SiteConfig = mapper.Map<SiteViewModel>(dasBlogSettings.SiteConfiguration);

			return View(dbsvm);
		}

		[HttpPost]
		public IActionResult Settings(DasBlogSettingsViewModel settings)
		{
			//save settings and reload...
			if (ModelState.ErrorCount > 0)
			{
				return View(settings);
			}

			return Settings();
		}
	}
}
