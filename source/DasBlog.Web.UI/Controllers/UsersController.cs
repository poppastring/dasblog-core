using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using AutoMapper;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Web.Common;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class UsersController : Controller
	{
		private readonly ILocalUserDataService _userService;
		private readonly IMapper _mapper;
		private readonly ISiteSecurityConfig _siteSecurityConfig;
		public UsersController(ILocalUserDataService userService, IMapper mapper, ISiteSecurityConfig siteSecurityConfig)
		{
			_userService = userService;
			_mapper = mapper;
			_siteSecurityConfig = siteSecurityConfig;
		}
		[Route("users")]
		public IActionResult Index()
		{
			ViewBag.SubViewName = Constants.ViewUserSubView;
			ViewBag.Writability = "readonly";
			ViewBag.Clickability = "disabled";
			List<User> users = _userService.LoadUsers().ToList();
			UsersViewModel uvm = _mapper.Map<UsersViewModel>(new User());
			if (users.Count == 0)
			{
				return RedirectToPage($"/users/CreateEditDeleteView?submit={Constants.UsersCreateAction}");
						// might as weel encourage admin to start creating users
			}
			else
			{
				uvm = _mapper.Map<UsersViewModel>(users[0]);
			}
			return View("Maintenance", uvm);
		}
		[HttpGet("/users/CreateEditDeleteView/{email?}")]
		public IActionResult CreateEditDeleteView(string submit, string email)
		{
			submit = submit ?? Constants.UsersViewAction;
			if (submit == Constants.UsersCreateAction)
			{
				ViewBag.SubViewName = Constants.CreateUserSubView;
				ViewBag.CreateEditDeleteInProgress = true;
				return View("Maintenance", _mapper.Map<UsersViewModel>(new User()));
			}
			else
			{
				return EditDeleteView(submit, email);
			}
		}
		[ValidateAntiForgeryToken]
		[HttpPost("/users/CreateEditDeleteView/{email?}")]
		public IActionResult CreateEditDeleteView(string submit, string originalEmail, UsersViewModel uvm)
		{
			ModelState.Remove(nameof(UsersViewModel.Password));
					// make sure that the password is not echoed back if validation fails
			var userAction = originalEmail == null ? Constants.UsersCreateAction : Constants.UsersEditAction;
			originalEmail = originalEmail ?? string.Empty;
			if (submit == Constants.CancelAction)
			{
				return RedirectToAction(nameof(Index));
			}
			if (!ModelState.IsValid)
			{
				ViewBag.SubViewName = ActionToSubView(userAction);
				return View("Maintenance", uvm);
			}
			switch (userAction)
			{
				case Constants.UsersCreateAction:
				case Constants.UsersEditAction:
					return SaveCreateOrEditUser(userAction, uvm, originalEmail);
				case Constants.UsersDeleteAction:
					ViewBag.SubViewName = Constants.DeleteUserSubView;
					return RedirectToAction(nameof(Index));
			}
			return View("Maintenance", uvm);
		}

		private IActionResult SaveCreateOrEditUser(string userAction, UsersViewModel uvm, string originalEmail)
		{
			User user = _mapper.Map<User>(uvm);
			List<User> users = _userService.LoadUsers().ToList();
			if (!ValidateUser(userAction, originalEmail, users, uvm, user))
			{
				ViewBag.SubViewName = ActionToSubView(userAction);
				return View("Maintenance", uvm);
			}

			var index = users.FindIndex(u => u.EmailAddress == originalEmail);
			if (index == -1)
			{
				users.Add(user);
			}
			else
			{
				users[index] = user;
			}
			_userService.SaveUsers(users);
			_siteSecurityConfig.Refresh();
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// validate user after initial creation or subsequent edit
		/// </summary>
		/// <param name="userAction">Either UserCreateAction or UserEditAction </param>
		/// <param name="originalEmail">"" for creation, existing value on users for edit</param>
		/// <param name="users">all the users in the repo</param>
		/// <param name="uvm"></param>
		/// <param name="user">the user that has just been created or edited by the client</param>
		/// <returns>true if the user can be saved to the repo, otherwise false</returns>
		private bool ValidateUser(string userAction, string originalEmail, List<User> users, UsersViewModel uvm, User user)
		{
			System.Diagnostics.Debug.Assert(userAction == Constants.UsersCreateAction
			  || userAction == Constants.UsersEditAction);
			System.Diagnostics.Debug.Assert(
			  userAction == Constants.UsersCreateAction && originalEmail == string.Empty
			  || userAction == Constants.UsersEditAction && originalEmail != string.Empty);
			if (user.EmailAddress == originalEmail)
			{
				return true;
			}
			if (users.Any(u => u.EmailAddress == user.EmailAddress))
			{
				ModelState.AddModelError(nameof(uvm.EmailAddress)
				  ,"This email address already exists - emails must be unique");
				return false;
			}

			return true;

		}
		private IActionResult EditDeleteView(string submit, string email)
		{
			System.Diagnostics.Debug.Assert(
				submit == Constants.UsersEditAction
				|| submit == Constants.UsersDeleteAction
				|| submit == Constants.UsersViewAction);
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

			ViewBag.SubViewName = ActionToSubView(submit);
			if (ViewBag.SubViewName == Constants.ViewUserSubView)
			{
				ViewBag.Writability = "readonly";
				ViewBag.Clickability = "disabled";
				ViewBag.CreateEditDeleteInProgress = false;
			}
			else
			{
				ViewBag.CreateEditDeleteInProgress = true;
			}
			return View("Maintenance", uvm);
		}
		
		private IDictionary<string, string> mapActionToView = new Dictionary<string, string>
		{
			{Constants.UsersCreateAction, Constants.CreateUserSubView}
			,{Constants.UsersEditAction, Constants.EditUserSubView}
			,{Constants.UsersDeleteAction, Constants.DeleteUserSubView}
			,{Constants.UsersViewAction, Constants.ViewUserSubView}
		};

		/// <summary>
		/// simple mapping
		/// </summary>
		/// <param name="Action">e.g. UsersEditAction</param>
		/// <returns>EditUsersSubView</returns>
		private string ActionToSubView(string action) => mapActionToView[action];
	}
}
