
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
	public class AuthorController : DasBlogController
	{
		private readonly ILogger<AuthorController> logger;
		private readonly IUserService userService;
		private readonly IMapper mapper;
		private readonly ISiteSecurityConfig siteSecurityConfig;

		public AuthorController(IUserService userService, IMapper mapper, ISiteSecurityConfig siteSecurityConfig, ILogger<AuthorController> logger)
		{
			this.logger = logger;
			this.userService = userService;
			this.mapper = mapper;
			this.siteSecurityConfig = siteSecurityConfig;
		}

		[HttpGet]
		[Route("/authors")]
		public IActionResult Index()
        {
			var uvm = mapper.Map<AuthorViewModel>(userService.GetFirstUser());

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersViewMode);

			logger.LogInformation(new EventDataItem(EventCodes.ViewUser, null, "View author: {0}", uvm.DisplayName));

			return RedirectToAction(uvm.OriginalEmail, "authors");
		}

		[HttpGet]
		[Route("/authors/{email}")]
		public IActionResult EditAuthor(string email)
		{
			var uvm = mapper.Map<AuthorViewModel>(userService.FindMatchingUser(email).user);

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersEditMode);

			logger.LogInformation(new EventDataItem(EventCodes.ViewUser, null, "View author: {0}", uvm.DisplayName));

			return View("ViewEditAuthor", uvm);
		}

		public IActionResult EditAuthor(AuthorViewModel authorviewmodel)
		{
			return View("ViewEditAuthor", authorviewmodel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/authors")]
		public IActionResult UpdateAuthor(AuthorViewModel authorviewmodel)
		{
			authorviewmodel.Active = true;
			authorviewmodel.Ask = true;
			authorviewmodel.NotifyOnAllComment = true;
			authorviewmodel.NotifyOnNewPost = true;
			authorviewmodel.NotifyOnOwnComment = true;

			ModelState.Clear();

			if (string.IsNullOrWhiteSpace(authorviewmodel.Password))
			{ 
				ModelState.AddModelError("", "Invalid Password.");
			}

			if (string.IsNullOrWhiteSpace(authorviewmodel.DisplayName))
			{
				ModelState.AddModelError("", "Invalid Display Name.");
			}

			if (string.IsNullOrWhiteSpace(authorviewmodel.EmailAddress))
			{
				ModelState.AddModelError("", "Invalid Email Address.");
			}

			if (ModelState.ErrorCount > 0)
			{
				return View("ViewEditAuthor", authorviewmodel);
			}

			var dasbloguser = mapper.Map<User>(authorviewmodel);

			userService.AddOrReplaceUser(dasbloguser, authorviewmodel.OriginalEmail);
			siteSecurityConfig.Users = userService.GetAllUsers().ToList();

			var uvm = mapper.Map<AuthorViewModel>(userService.GetFirstUser());

			logger.LogInformation(new EventDataItem(EventCodes.EditUser, null, "Edit User: {0}", uvm.DisplayName));

			return Index();
		}
	}
}
