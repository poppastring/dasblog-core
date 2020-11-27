
using System.Linq;
using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Core.Security;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class UsersController : DasBlogController
	{
		private readonly ILogger<UsersController> logger;
		private readonly IUserService userService;
		private readonly IMapper mapper;
		private readonly ISiteSecurityConfig siteSecurityConfig;

		public UsersController(IUserService userService, IMapper mapper, ISiteSecurityConfig siteSecurityConfig, ILogger<UsersController> logger)
		{
			this.logger = logger;
			this.userService = userService;
			this.mapper = mapper;
			this.siteSecurityConfig = siteSecurityConfig;
		}

		[HttpGet]
		[Route("/users")]
		public IActionResult Index()
        {
			var uvm = mapper.Map<UsersViewModel>(userService.GetFirstUser());

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersViewMode);

			logger.LogInformation(new EventDataItem(EventCodes.ViewUser, null, "View User: {0}", uvm.DisplayName));

			return RedirectToAction(uvm.OriginalEmail, "users");
		}

		[HttpGet]
		[Route("/users/{email}")]
		public IActionResult EditUser(string email)
		{
			var uvm = mapper.Map<UsersViewModel>(userService.FindMatchingUser(email).user);

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersEditMode);

			logger.LogInformation(new EventDataItem(EventCodes.ViewUser, null, "View User: {0}", uvm.DisplayName));

			return View("ViewEditUser", uvm);
		}

		public IActionResult EditUser(UsersViewModel userviewmodel)
		{
			return View("ViewEditUser", userviewmodel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/users")]
		public IActionResult UpdateUser(UsersViewModel usersviewmodel)
		{
			usersviewmodel.Active = true;
			usersviewmodel.Ask = true;
			usersviewmodel.NotifyOnAllComment = true;
			usersviewmodel.NotifyOnNewPost = true;
			usersviewmodel.NotifyOnOwnComment = true;

			ModelState.Clear();

			if (string.IsNullOrWhiteSpace(usersviewmodel.Password))
			{ 
				ModelState.AddModelError("", "Invalid Password.");
			}

			if (string.IsNullOrWhiteSpace(usersviewmodel.DisplayName))
			{
				ModelState.AddModelError("", "Invalid Display Name.");
			}

			if (string.IsNullOrWhiteSpace(usersviewmodel.EmailAddress))
			{
				ModelState.AddModelError("", "Invalid Email Address.");
			}

			if (ModelState.ErrorCount > 0)
			{
				return View("ViewEditUser", usersviewmodel);
			}

			var dasbloguser = mapper.Map<User>(usersviewmodel);

			userService.AddOrReplaceUser(dasbloguser, usersviewmodel.OriginalEmail);
			siteSecurityConfig.Users = userService.GetAllUsers().ToList();

			var uvm = mapper.Map<UsersViewModel>(userService.GetFirstUser());

			logger.LogInformation(new EventDataItem(EventCodes.EditUser, null, "Edit User: {0}", uvm.DisplayName));

			return Index();
		}
	}
}
