using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AdminController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public AdminController(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult Settings()
		{
			return View();
		}
    }
}
