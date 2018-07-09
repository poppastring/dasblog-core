using System.Collections.Generic;
using System.Linq;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class UsersController : Controller
	{
		private readonly ILocalUserDataService _userService;
		public UsersController(ILocalUserDataService userService)
		{
			_userService = userService;
		}
		[Route("users")]
		public IActionResult Index()
		{
			ViewBag.SubViewName = "ViewUser";
			List<User> users = _userService.LoadUsers().ToList();
			var uvm = new UsersViewModel(users[0]);
			return View("Maintenance", uvm);
		}
		[HttpGet("/users/CreateEditDelete")]
		public IActionResult CreateEditDelete()
		{
			ViewBag.SubViewName = "ViewUser";
			List<User> users = _userService.LoadUsers().ToList();
			var uvm = new UsersViewModel(users[0]);
			return View("Maintenance", uvm);
		}
	}
}
