using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class UserAdminController : DasBlogController
	{
		private readonly ILogger<UsersController> logger;
		private readonly IUserService userService;
		private readonly IMapper mapper;
		private readonly ISiteSecurityConfig siteSecurityConfig;

		public UserAdminController(IUserService userService, IMapper mapper, ISiteSecurityConfig siteSecurityConfig, ILogger<UsersController> logger)
		{
			this.logger = logger;
			this.userService = userService;
			this.mapper = mapper;
			this.siteSecurityConfig = siteSecurityConfig;
		}

		[HttpGet]
		[Route("/adminuser")]
		public IActionResult Index()
        {
			var uvm = mapper.Map<UsersViewModel>(userService.GetFirstUser());

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersViewMode);

			return View("Index", uvm);
		}

		[HttpGet]
		[Route("/adminuser/{email}")]
		public IActionResult EditUser(string email)
		{
			var uvm = mapper.Map<UsersViewModel>(userService.FindMatchingUser(email).user);

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersEditMode);

			return View("ViewEditUser", uvm);
		}

		[HttpPost]
		[Route("/adminuser/edit")]
		public IActionResult EditUser(UsersViewModel usersviewmodel)
		{
			var dasbloguser = mapper.Map<User>(usersviewmodel);

			userService.AddOrReplaceUser(dasbloguser, usersviewmodel.OriginalEmail);
			siteSecurityConfig.Refresh();

			var uvm = mapper.Map<UsersViewModel>(userService.GetFirstUser());

			return View("Index", uvm);
		}
	}
}
