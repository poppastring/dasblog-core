using System.Collections.Generic;
using System.Linq;
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
		private class ViewBagConfigurer
		{
			public void ConfigureViewBag(dynamic viewBag, string userAction)
			{
				System.Diagnostics.Debug.Assert(
					userAction == Constants.UsersCreateAction
					|| userAction == Constants.UsersEditAction
					|| userAction == Constants.UsersDeleteAction
					|| userAction == Constants.UsersViewAction);
				viewBag.Action = userAction;
				viewBag.SubViewName = ActionToSubView(userAction);
				switch (userAction)
				{
					case Constants.DeleteAction:
						viewBag.Linkability = "disabled";
						viewBag.Writability = "readonly";
						viewBag.Clickability = "disabled";
						break;
					case Constants.UsersViewAction:
						viewBag.Linkability = string.Empty;
						viewBag.Writability = "readonly";
						viewBag.Clickability = "disabled";
						break;
					default:
						viewBag.Linkability = "disabled";
						viewBag.Writability = string.Empty;
						viewBag.Clickability = string.Empty;
						break;
				}
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
				this.ControllerContext.RouteData.Values.Add("subAction", Constants.UsersCreateAction);
				return RedirectToAction(nameof(Maintenance));
						// might as weel encourage admin to start creating users
			}
			// make sure that there is an actual user associated with this email address
			// In many cases there won't be as the email will have been passed as null
			(var userFound, _) = _userService.FindFirstMatchingUser(u => u.EmailAddress == email);
			if (!userFound)
			{
				email = _userService.GetFirstUser().EmailAddress;
			}
			// we need to update the route data when the page is initially loaded.
			// the email address is automatically appended to the actions of forms that refer to this controller
			UpdateRouteData(email);

			return EditDeleteOrViewUser(Constants.UsersViewAction, email);
		}


		/// <summary>
		/// All requests for maintenance are processed through here
		/// </summary>
		/// <param name="subAction">Create, Edit, Delete, View</param>
		/// <param name="email">allows us to track user being modified</param>
		/// <returns>one way or another, the maintenance page.
		/// Will be null when the admin has not specified a user
		/// i.e. at first page load and after deletions</returns>
		[HttpGet("/users/Maintenance/{email?}")]
		public IActionResult Maintenance(string subAction, string email)
		{
			subAction = subAction ?? Constants.UsersViewAction;
			System.Diagnostics.Debug.Assert(email != null || subAction == Constants.UsersCreateAction);
			System.Diagnostics.Debug.Assert(
				subAction == Constants.UsersCreateAction
				|| subAction == Constants.UsersEditAction
				|| subAction == Constants.UsersDeleteAction
				|| subAction == Constants.UsersViewAction);
			email = email ?? string.Empty;
			if (subAction == Constants.UsersCreateAction)
			{
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersCreateAction);
				return View("Maintenance", _mapper.Map<UsersViewModel>(new User()));
			}
			else
			{
				return EditDeleteOrViewUser(subAction, email);
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
				return RedirectToAction(nameof(Index));
			}
			if (!ModelState.IsValid)
			{
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, userAction);
				return View("Maintenance", uvm);
			}
			switch (userAction)
			{
				case Constants.UsersCreateAction:
				case Constants.UsersEditAction:
					return SaveCreateOrEditUser(userAction, uvm, originalEmail);
				case Constants.UsersDeleteAction:
					return DeleteUser(uvm);
			}

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, userAction);
			return View("Maintenance", uvm);
		}

		private IActionResult SaveCreateOrEditUser(string userAction, UsersViewModel uvm, string originalEmail)
		{
			User user = _mapper.Map<User>(uvm);
			if (!ValidateUser(userAction, originalEmail, uvm, user))
			{
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, userAction);
				return View("Maintenance", uvm);
			}

			_userService.AddOrReplaceUser(user, originalEmail);
			_siteSecurityConfig.Refresh();
			UpdateRouteData(user.EmailAddress);
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// validate user after initial creation or subsequent edit
		/// </summary>
		/// <param name="userAction">Either UserCreateAction or UserEditAction </param>
		/// <param name="originalEmail">"" for creation, existing value on users for edit</param>
		/// <param name="uvm"></param>
		/// <param name="user">the user that has just been created or edited by the client</param>
		/// <returns>true if the user can be saved to the repo, otherwise false</returns>
		private bool ValidateUser(string userAction, string originalEmail, UsersViewModel uvm, User user)
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
			if (_userService.FindFirstMatchingUser(u => u.EmailAddress == user.EmailAddress).userFound)
			{
				ModelState.AddModelError(nameof(uvm.EmailAddress)
				  ,"This email address already exists - emails must be unique");
				return false;
			}

			return true;

		}
		// subroutine for http GET action
		private IActionResult EditDeleteOrViewUser(string subAction, string email)
		{
			System.Diagnostics.Debug.Assert(
				subAction == Constants.UsersEditAction
				|| subAction == Constants.UsersDeleteAction
				|| subAction == Constants.UsersViewAction);
			(var found, var user) = _userService.FindFirstMatchingUser(u => u.EmailAddress == email);
			if (!found)
			{
				return RedirectToAction(nameof(Index));
					// TODO show an error page
			}
			UsersViewModel uvm = _mapper.Map<UsersViewModel>(user);

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, subAction);
			return View("Maintenance", uvm);
		}

		// http post handler
		private IActionResult DeleteUser(UsersViewModel uvm)
		{
			if (_userService.DeleteUser(u => u.EmailAddress == uvm.EmailAddress))
			{
				_siteSecurityConfig.Refresh();
				this.ControllerContext.RouteData.Values.Remove("email");
				return RedirectToAction(nameof(Index));
			}
			else
			{
				ModelState.AddModelError(string.Empty
				  , $"Failed to delete the user {uvm.EmailAddress}.  The record may have already been deleted ");
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersDeleteAction);
				return View("Maintenance", uvm);
			}
		}
		private void UpdateRouteData(string email)
		{
			if (!this.ControllerContext.RouteData.Values.ContainsKey("email"))
			{
				this.ControllerContext.RouteData.Values.Add("email", "");
			}

			this.ControllerContext.RouteData.Values["email"] = email;
		}
	}
}
