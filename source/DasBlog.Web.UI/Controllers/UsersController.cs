using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using AutoMapper;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using DasBlog.Core.Services;
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
		private readonly IUserService _userService;
		private readonly IMapper _mapper;
		private readonly ISiteSecurityConfig _siteSecurityConfig;
		public UsersController(IUserService userService, IMapper mapper, ISiteSecurityConfig siteSecurityConfig)
		{
			this._userService = userService;
			_mapper = mapper;
			_siteSecurityConfig = siteSecurityConfig;
		}
		/// <summary>
		/// Show the user identfied by email
		/// if no email is passed (e.g. when page is first displayed)
		/// then show the first user in the user repo
		/// or if there are no users (almost impossible) then
		/// redirect to the creation page
		/// </summary>
		/// <param name="email">typically null when the page is first displayed
		/// thereafter typically the email address of the last user edited
		/// or, as a default, the first user in the user repo</param>
		/// <returns>the user maintenance view either in view or create mode</returns>
		[Route("/users/{email?}")]
		public IActionResult Index(string email)
		{
			email = email ?? string.Empty;
			if (!_userService.HasUsers())
			{
				return RedirectToPage($"/users/Maintenance?submit={Constants.UsersCreateAction}");
						// might as weel encourage admin to start creating users
			}
			// make sure that there is an actual user associated with this email address
			// In many cases there won't be as the email will have been passed as null
			(var userFound, _) = _userService.FindFirstMatchingUser(u => u.EmailAddress == email);
			if (!userFound)
			{
				email = _userService.GetFirstUser().EmailAddress;
			}

			return EditDeleteOrViewUser(Constants.UsersViewAction, email);
		}
		/// <summary>
		/// All requests for maintenance are processed through here
		/// </summary>
		/// <param name="submit">Create, Edit, Delete, View</param>
		/// <param name="email">allows us to track user being modified</param>
		/// <returns>one way or another, the maintenance page.
		/// Will be null when the admin has not specified a user
		/// i.e. at first page load and after deletions</returns>
		[HttpGet("/users/Maintenance/{email?}")]
		public IActionResult Maintenance(string submit, string email)
		{
			//System.Diagnostics.Debug.Assert(email != null || submit == Constants.UsersCreateAction);
/*
			System.Diagnostics.Debug.Assert(
				submit == Constants.UsersCreateAction
				|| submit == Constants.UsersEditAction
				|| submit == Constants.UsersDeleteAction
				|| submit == Constants.UsersViewAction);
*/
			email = email ?? string.Empty;
			submit = submit ?? Constants.UsersViewAction;
			if (submit == Constants.UsersCreateAction) 
			{
				ViewBag.SubViewName = Constants.CreateUserSubView;
				ViewBag.Action = submit;
				return View("Maintenance", _mapper.Map<UsersViewModel>(new User()));
			}
			else
			{
				return EditDeleteOrViewUser(submit, email);
			}
		}
		/// <summary>
		/// handles user creation, edit and deletion
		/// </summary>
		/// <param name="submit">Save, Cancel or Delete</param>
		/// <param name="originalEmail">if the user's email has been modified by the edit then
		/// this enables us to find that user in the repo.  Null when this is called
		/// in response to a create operation</param>
		/// <param name="uvm"></param>
		/// <returns>either the page from which it came (on cancel or error) or the index</returns>
		[ValidateAntiForgeryToken]
		[HttpPost("/users/Maintenance/{email?}")]
		public IActionResult Maintenance(string submit, string originalEmail
		  ,UsersViewModel uvm)
		{
			System.Diagnostics.Debug.Assert(
			  submit == Constants.SaveAction
			  || submit == Constants.CancelAction
			  || submit == Constants.DeleteAction);
			ModelState.Remove(nameof(UsersViewModel.Password));
					// make sure that the password is not echoed back if validation fails
			string userAction;
			if (submit == Constants.DeleteAction)
			{
				userAction = Constants.UsersDeleteAction;
			}
			else
			{
				userAction = originalEmail == null ? Constants.UsersCreateAction : Constants.UsersEditAction;
			}

			originalEmail = originalEmail ?? string.Empty;
			if (submit == Constants.CancelAction)
			{
				this.Response.Headers.Add("Location", $"/users/{originalEmail}");
				return RedirectToPage($"/users/{originalEmail}");
			}
			if (!ModelState.IsValid)
			{
				ViewBag.SubViewName = ActionToSubView(userAction);
				ViewBag.Action = userAction;
				return View("Maintenance", uvm);
			}
			switch (userAction)
			{
				case Constants.UsersCreateAction:
				case Constants.UsersEditAction:
					return SaveCreateOrEditUser(userAction, uvm, originalEmail);
				case Constants.UsersDeleteAction:
					ViewBag.SubViewName = Constants.DeleteUserSubView;
					return DeleteUser(uvm);
			}

			ViewBag.Action = userAction;
			return View("Maintenance", uvm);
		}

		private IActionResult SaveCreateOrEditUser(string userAction, UsersViewModel uvm, string originalEmail)
		{
			User user = _mapper.Map<User>(uvm);
			List<User> users = _userService.GetAllUsers().ToList();
			if (!ValidateUser(userAction, originalEmail, users, uvm, user))
			{
				ViewBag.SubViewName = ActionToSubView(userAction);
				ViewBag.Action = userAction;
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
			return Index(user.EmailAddress);
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
		// subroutine for http GET action
		private IActionResult EditDeleteOrViewUser(string submit, string email)
		{
			System.Diagnostics.Debug.Assert(
				submit == Constants.UsersEditAction
				|| submit == Constants.UsersDeleteAction
				|| submit == Constants.UsersViewAction);
			(var found, var user) = _userService.FindFirstMatchingUser(u => u.EmailAddress == email);
			UsersViewModel uvm = _mapper.Map<UsersViewModel>(user);
			if (!found)
			{
				ViewBag.SubViewName = Constants.UsersErrorAction;
				return Index(null);
					// TODO show an error page
			}

			ViewBag.SubViewName = ActionToSubView(submit);
			if (ViewBag.SubViewName == Constants.ViewUserSubView
			  || ViewBag.SubViewName == Constants.DeleteUserSubView)
			{
				ViewBag.Writability = "readonly";
				ViewBag.Clickability = "disabled";
			}

			ViewBag.Action = submit;
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

		private IActionResult DeleteUser(UsersViewModel uvm)
		{
			List<User> users = _userService.GetAllUsers().ToList();
			int index = users.FindIndex(u => u.EmailAddress == uvm.EmailAddress);
			if (index != -1) 	// ignore -1 condition - presumably another admin just delete it
			{
				users.RemoveAt(index);
				_userService.SaveUsers(users);
				_siteSecurityConfig.Users = users;
			}

			ModelState.Remove("email");
//			return RedirectToPage("/users");
			return Index(null);
		}
	}
}
