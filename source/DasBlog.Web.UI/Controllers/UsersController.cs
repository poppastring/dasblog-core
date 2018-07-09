using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Web.Common;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class UsersController : Controller
	{
		private readonly ILocalUserDataService _userService;
		private readonly IMapper _mapper;
		public UsersController(ILocalUserDataService userService, IMapper mapper)
		{
			_userService = userService;
			_mapper = mapper;
		}
		[Route("users")]
		public IActionResult Index()
		{
			ViewBag.SubViewName = "ViewUser";
			List<User> users = _userService.LoadUsers().ToList();
			UsersViewModel uvm = _mapper.Map<UsersViewModel>(new User());
			if (users.Count > 0)
			{
				uvm = _mapper.Map<UsersViewModel>(users[0]);			
			}
			return View("Maintenance", uvm);
		}
		[HttpGet("/users/CreateEditDelete/{email?}")]
		public IActionResult CreateEditDelete(string submit, string email)
		{
			submit = submit ?? Constants.UsersViewAction;
			if (submit == Constants.UsersCreateAction)
			{
				ViewBag.SubViewName = "CreateUser";
				return View("Maintenance", _mapper.Map<UsersViewModel>(new User()));
			}
			else
			{
				return EditDeleteView(submit, email);
			}
		}

		[HttpPost("/users/CreateEditDelete/{email?}")]
		public IActionResult CreateEditDelete(string submit, string originalEmail, UsersViewModel uvm)
		{
			if (submit == Constants.CancelAction)
			{
				return Index();
			}
			if (!ModelState.IsValid)
			{
				return View("Maintenance", uvm);
			}

			SaveNewUser(uvm);
			return Index();
		}

		private void SaveNewUser(UsersViewModel uvm)
		{
			User user = _mapper.Map<User>(uvm);
			List<User> users = _userService.LoadUsers().ToList();
			users.Add(user);
			_userService.SaveUsers(users);
		}

		private IActionResult EditDeleteView(string submit, string email)
		{
			List<User> users = _userService.LoadUsers().ToList();
			var user = users.FirstOrDefault(u => u.EmailAddress == email);
			UsersViewModel uvm;
			if (user == null)
			{
				ViewBag.SubViewName = Constants.UsersErrorAction;
				uvm = _mapper.Map<UsersViewModel>(new User());
			}
			else
			{
				uvm = _mapper.Map<UsersViewModel>(user);				
			}
			switch (submit)
			{
				case Constants.UsersViewAction:
					ViewBag.SubViewName = Constants.ViewUserSubView;
					break;
				case Constants.UsersEditAction:
					ViewBag.SubViewName = Constants.EditUserSubView;
					break;
				case Constants.UsersDeleteAction:
					ViewBag.SubViewName = Constants.DeleteUserSubView;
					break;
			}
			return View("Maintenance", uvm);
		}
	}
}
