using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using DasBlog.Core.Common;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	[NoIndex]
	[Route("admin/authors/{email}/app-passwords")]
	public class AppPasswordsController : Controller
	{
		private const string GeneratedPasswordKey = "GeneratedAppPassword";
		private const string GeneratedPasswordNameKey = "GeneratedAppPasswordName";
		private readonly IUserService userService;
		private readonly ISiteSecurityConfig siteSecurityConfig;
		private readonly ISiteSecurityManager siteSecurityManager;
		private readonly UserManager<DasBlogUser> userManager;

		public AppPasswordsController(IUserService userService, ISiteSecurityConfig siteSecurityConfig,
			ISiteSecurityManager siteSecurityManager, UserManager<DasBlogUser> userManager)
		{
			this.userService = userService;
			this.siteSecurityConfig = siteSecurityConfig;
			this.siteSecurityManager = siteSecurityManager;
			this.userManager = userManager;
		}

		[HttpGet("")]
		public IActionResult Index(string email)
		{
			PreventCaching();
			var users = userService.GetAllUsers().ToList();
			var user = GetAuthorizedUser(users, email);
			if (user == null)
			{
				return NotFound();
			}

			var model = CreateViewModel(user);
			model.GeneratedPassword = TempData[GeneratedPasswordKey] as string;
			ViewData[GeneratedPasswordNameKey] = TempData[GeneratedPasswordNameKey];
			return View(model);
		}

		[HttpPost("create")]
		[ValidateAntiForgeryToken]
		public IActionResult Create(string email, AppPasswordsViewModel model)
		{
			PreventCaching();
			var users = userService.GetAllUsers().ToList();
			var user = GetAuthorizedUser(users, email);
			if (user == null)
			{
				return NotFound();
			}

			var name = model.NewPasswordName?.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				ModelState.AddModelError(nameof(model.NewPasswordName), "Enter a name for this app password.");
			}
			else if (user.AppPasswords?.Any(appPassword => string.Equals(appPassword.Name, name, StringComparison.OrdinalIgnoreCase)) == true)
			{
				ModelState.AddModelError(nameof(model.NewPasswordName), "An app password with that name already exists.");
			}

			if (!ModelState.IsValid)
			{
				var invalidModel = CreateViewModel(user);
				invalidModel.NewPasswordName = model.NewPasswordName;
				return View("Index", invalidModel);
			}

			var token = GenerateToken();
			user.AppPasswords ??= new List<AppPassword>();
			user.AppPasswords.Add(new AppPassword
			{
				Id = Guid.NewGuid().ToString("N"),
				Name = name,
				Hash = siteSecurityManager.HashPassword(token),
				Created = DateTime.UtcNow.ToString("O"),
				LastUsed = string.Empty,
				LastIp = string.Empty
			});

			var userIndex = users.FindIndex(existing => string.Equals(existing.EmailAddress, user.EmailAddress, StringComparison.OrdinalIgnoreCase));
			if (userIndex < 0)
			{
				return NotFound();
			}

			users[userIndex] = user;
			SaveUsers(users);

			TempData[GeneratedPasswordKey] = token;
			TempData[GeneratedPasswordNameKey] = name;
			return RedirectToAction(nameof(Index), new { email });
		}

		[HttpPost("revoke/{id}")]
		[ValidateAntiForgeryToken]
		public IActionResult Revoke(string email, string id)
		{
			PreventCaching();
			var users = userService.GetAllUsers().ToList();
			var user = GetAuthorizedUser(users, email);
			if (user == null)
			{
				return NotFound();
			}

			var appPassword = user.AppPasswords?.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
			if (appPassword == null)
			{
				return NotFound();
			}

			user.AppPasswords.Remove(appPassword);
			var userIndex = users.FindIndex(existing => string.Equals(existing.EmailAddress, user.EmailAddress, StringComparison.OrdinalIgnoreCase));
			users[userIndex] = user;
			SaveUsers(users);
			TempData["SuccessMessage"] = "App password revoked.";
			return RedirectToAction(nameof(Index), new { email });
		}

		private User GetAuthorizedUser(List<User> users, string email)
		{
			var user = users.FirstOrDefault(candidate =>
				string.Equals(candidate.EmailAddress, email, StringComparison.OrdinalIgnoreCase));
			if (user == null)
			{
				return null;
			}

			var currentUserId = userManager.GetUserId(User);
			return User.IsInRole("admin")
				|| string.Equals(user.EmailAddress, currentUserId, StringComparison.OrdinalIgnoreCase)
				? user
				: null;
		}

		private AppPasswordsViewModel CreateViewModel(User user)
		{
			return new AppPasswordsViewModel
			{
				AuthorName = user.DisplayName,
				LoginName = user.EmailAddress,
				AppPasswords = (user.AppPasswords ?? new List<AppPassword>())
					.Select(appPassword => new AppPasswordListItem
					{
						Id = appPassword.Id,
						Name = appPassword.Name,
						Created = appPassword.Created,
						LastUsed = appPassword.LastUsed,
						LastIp = appPassword.LastIp
					}).ToList()
			};
		}

		private void SaveUsers(List<User> users)
		{
			userService.SaveUsers(users);
			siteSecurityConfig.Users = users;
		}

		private void PreventCaching()
		{
			Response.Headers.CacheControl = "no-store, no-cache, max-age=0";
			Response.Headers.Pragma = "no-cache";
		}

		private static string GenerateToken()
		{
			var tokenBytes = RandomNumberGenerator.GetBytes(32);
			var token = Convert.ToBase64String(tokenBytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
			return "dbapp_" + token;
		}
	}
}
