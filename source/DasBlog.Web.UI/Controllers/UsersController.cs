using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Web.Attributes;
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
			public void ConfigureViewBag(dynamic viewBag, string maintenanceMode)
			{
				System.Diagnostics.Debug.Assert(
					maintenanceMode == Constants.UsersCreateMode
					|| maintenanceMode == Constants.UsersEditMode
					|| maintenanceMode == Constants.UsersDeleteMode
					|| maintenanceMode == Constants.UsersViewMode);
				viewBag.Action = maintenanceMode;
				viewBag.SubViewName = ActionToSubView(maintenanceMode);
				switch (maintenanceMode)
				{
					case Constants.DeleteAction:
						viewBag.Linkability = "disabled";
						viewBag.Writability = "readonly";
						viewBag.Clickability = "disabled";
						break;
					case Constants.UsersViewMode:
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
				{Constants.UsersCreateMode, Constants.CreateUserSubView}
				,{Constants.UsersEditMode, Constants.EditUserSubView}
				,{Constants.UsersDeleteMode, Constants.DeleteUserSubView}
				,{Constants.UsersViewMode, Constants.ViewUserSubView}
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
				this.ControllerContext.RouteData.Values.Add("maintenanceMode", Constants.UsersCreateMode);
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

			return EditDeleteOrViewUser(Constants.UsersViewMode, email);
		}


		/// <summary>
		/// All requests for maintenance are processed through here
		/// </summary>
		/// <param name="maintenanceMode">Create, Edit, Delete, View</param>
		/// <param name="email">allows us to track user being modified</param>
		/// <returns>one way or another, the maintenance page.
		/// Will be null when the admin has not specified a user
		/// i.e. at first page load and after deletions</returns>
		[HttpGet("/users/Maintenance/{email?}")]
		public IActionResult Maintenance(string maintenanceMode, string email)
		{
			maintenanceMode = maintenanceMode ?? Constants.UsersViewMode;
			System.Diagnostics.Debug.Assert(email != null || maintenanceMode == Constants.UsersCreateMode);
			System.Diagnostics.Debug.Assert(
				maintenanceMode == Constants.UsersCreateMode
				|| maintenanceMode == Constants.UsersEditMode
				|| maintenanceMode == Constants.UsersDeleteMode
				|| maintenanceMode == Constants.UsersViewMode);
			email = email ?? string.Empty;
			if (maintenanceMode == Constants.UsersCreateMode)
			{
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersCreateMode);
				return View("Maintenance", _mapper.Map<UsersViewModel>(new User()));
			}
			else
			{
				return EditDeleteOrViewUser(maintenanceMode, email);
			}
		}
		/// <summary>
		/// handles user creation, edit and deletion
		/// </summary>
		/// <param name="submitAction">Save, Cancel or Delete</param>
		/// <param name="originalEmail">if the user's email has been modified by the edit then
		/// this enables us to find that user in the repo.  Null when this is called
		/// in response to a create operation</param>
		/// <param name="uvm"></param>
		/// <returns>either the page from which it came (on cancel or error) or the index</returns>
		[ValidateAntiForgeryToken]
		[HttpPost("/users/Maintenance/{email?}")]
		public IActionResult Maintenance(string submitAction, string originalEmail
		  ,UsersViewModel uvm)
		{
			System.Diagnostics.Debug.Assert(
			  submitAction == Constants.SaveAction
			  || submitAction == Constants.CancelAction
			  || submitAction == Constants.DeleteAction);
			string maintenanceMode;
			if (submitAction == Constants.DeleteAction)
			{
				maintenanceMode = Constants.UsersDeleteMode;
			}
			else
			{
				maintenanceMode = originalEmail == null ? Constants.UsersCreateMode : Constants.UsersEditMode;
			}

			originalEmail = originalEmail ?? string.Empty;
			if (submitAction == Constants.CancelAction)
			{
				return RedirectToAction(nameof(Index));
			}

			if (IsLoggedInUserRecord(uvm))
			{
				if (maintenanceMode == Constants.UsersEditMode && uvm.Role != Role.Admin
				  || maintenanceMode == Constants.UsersDeleteMode)
				{
					ModelState.AddModelError(string.Empty
					  , "You cannot delete your own user record or change the role");
				}
			}
			if (!ModelState.IsValid)
			{
				if (!string.IsNullOrWhiteSpace(uvm.Password))
				{
					uvm.Password = string.Empty;
					// if the password field is blannk and therefore invalid then
					// we need to keep it to show the error message.
					// if the password field is not blank and some other field
					// is invalid then we need to remove the password field
					// to ensure the form returns an empty password field
					// This will need to change if we introduce more exacting password rules
					ModelState.Remove(nameof(UsersViewModel.Password));
					// make sure that the password is not echoed back if validation fails

				}
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, maintenanceMode);
				return View("Maintenance", uvm);
			}
			ModelState.Remove(nameof(UsersViewModel.Password));
				// make sure that the password is not echoed back if validation fails
			switch (maintenanceMode)
			{
				case Constants.UsersCreateMode:
				case Constants.UsersEditMode:
					return SaveCreateOrEditUser(maintenanceMode, uvm, originalEmail);
				case Constants.UsersDeleteMode:
					return DeleteUser(uvm);
			}

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, maintenanceMode);
			return View("Maintenance", uvm);
		}

		private bool IsLoggedInUserRecord(UsersViewModel uvm)
		{
			var loggedInUserEmail = this.HttpContext.User.Identities.First().Name;
			return loggedInUserEmail == uvm.EmailAddress;
		}

		private IActionResult SaveCreateOrEditUser(string maintenanceMode, UsersViewModel uvm, string originalEmail)
		{
			User user = _mapper.Map<User>(uvm);
			if (!ValidateUser(maintenanceMode, originalEmail, uvm, user))
			{
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, maintenanceMode);
				return View("Maintenance", uvm);
			}

			_userService.AddOrReplaceUser(user, originalEmail);
			_siteSecurityConfig.Refresh();
			UpdateRouteData(user.EmailAddress);
			return RedirectToAction(nameof(Index));
		}

		/// <summary>
		/// validate user after initial creation or subsequent edit
		/// 1. email must be unique within user repo
		/// 2. password must be non-empty
		/// </summary>
		/// <param name="maintenanceMode">Either UserCreateAction or UserEditAction </param>
		/// <param name="originalEmail">"" for creation, existing value in users repo for edit</param>
		/// <param name="uvm"></param>
		/// <param name="user">the user that has just been created or edited by the client</param>
		/// <returns>true if the user can be saved to the repo, otherwise false</returns>
		private bool ValidateUser(string maintenanceMode, string originalEmail, UsersViewModel uvm, User user)
		{
			System.Diagnostics.Debug.Assert(maintenanceMode == Constants.UsersCreateMode
			  || maintenanceMode == Constants.UsersEditMode);
			System.Diagnostics.Debug.Assert(
			  maintenanceMode == Constants.UsersCreateMode && originalEmail == string.Empty
			  || maintenanceMode == Constants.UsersEditMode && originalEmail != string.Empty);
			bool rtn = true;
			if (string.IsNullOrEmpty(uvm.Password))
			{
				ModelState.AddModelError(nameof(uvm.Password)
				  , "The password must contain some characters");
				rtn = false;
			}
			if ( user.EmailAddress != originalEmail
			  && _userService.FindFirstMatchingUser(u => u.EmailAddress == user.EmailAddress).userFound)
			{
				ModelState.AddModelError(nameof(uvm.EmailAddress)
				  ,"This email address already exists - emails must be unique");
				rtn = false;
			}

			return rtn;

		}
		// subroutine for http GET action
		private IActionResult EditDeleteOrViewUser(string maintenanceMode, string email)
		{
			System.Diagnostics.Debug.Assert(
				maintenanceMode == Constants.UsersEditMode
				|| maintenanceMode == Constants.UsersDeleteMode
				|| maintenanceMode == Constants.UsersViewMode);
			(var found, var user) = _userService.FindFirstMatchingUser(u => u.EmailAddress == email);
			if (!found)
			{
				return RedirectToAction(nameof(Index));
					// TODO show an error page
			}
			UsersViewModel uvm = _mapper.Map<UsersViewModel>(user);

			new ViewBagConfigurer().ConfigureViewBag(ViewBag, maintenanceMode);
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
				new ViewBagConfigurer().ConfigureViewBag(ViewBag, Constants.UsersDeleteMode);
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
