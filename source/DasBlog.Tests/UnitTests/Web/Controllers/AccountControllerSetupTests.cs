﻿using System.Collections.Generic;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Controllers
{
	public class AccountControllerSetupTests
	{
		private static AccountController Build(
			bool setupRequired,
			IUserService userService = null,
			ISiteSecurityManager siteSecurityManager = null,
			ISiteSecurityConfig siteSecurityConfig = null)
		{
			var firstRun = new Mock<IFirstRunService>();
			firstRun.Setup(s => s.IsSetupRequired()).Returns(setupRequired);

			userService ??= Mock.Of<IUserService>(s => s.GetAllUsers() == new List<User>());
			siteSecurityManager ??= Mock.Of<ISiteSecurityManager>(
				s => s.HashPassword(It.IsAny<string>()) == "HASH");
			siteSecurityConfig ??= Mock.Of<ISiteSecurityConfig>();

			var settings = Mock.Of<IDasBlogSettings>();
			var logger = Mock.Of<ILogger<AccountController>>();

			return new AccountController(
				userManager: null,
				signInManager: null,
				mapper: null,
				logger: logger,
				settings: settings,
				firstRunService: firstRun.Object,
				userService: userService,
				siteSecurityManager: siteSecurityManager,
				siteSecurityConfig: siteSecurityConfig);
		}

		[Fact]
		public void Setup_Get_ReturnsNotFoundWhenNotRequired()
		{
			var controller = Build(setupRequired: false);

			var result = controller.Setup();

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void Setup_Post_ReturnsNotFoundWhenNotRequired()
		{
			var controller = Build(setupRequired: false);

			var result = controller.Setup(new SetupViewModel());

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void Setup_Post_ReturnsViewWhenModelInvalid()
		{
			var controller = Build(setupRequired: true);
			controller.ModelState.AddModelError("Password", "required");

			var result = controller.Setup(new SetupViewModel());

			Assert.IsType<ViewResult>(result);
		}

		[Fact]
		public void Setup_Post_CreatesAdminAndRedirectsToLogin()
		{
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns(new List<User>());

			var saved = new List<(User user, string original)>();
			userService
				.Setup(s => s.AddOrReplaceUser(It.IsAny<User>(), It.IsAny<string>()))
				.Callback<User, string>((u, original) =>
				{
					saved.Add((u, original));
					userService.Setup(s => s.GetAllUsers()).Returns(new List<User> { u });
				});

			var security = new Mock<ISiteSecurityManager>();
			security.Setup(s => s.HashPassword("strong-pwd-1!")).Returns("HASHED");

			var config = new Mock<ISiteSecurityConfig>();
			config.SetupProperty(c => c.Users, new List<User>());

			var controller = Build(
				setupRequired: true,
				userService: userService.Object,
				siteSecurityManager: security.Object,
				siteSecurityConfig: config.Object);

			var model = new SetupViewModel
			{
				Email = "  new@example.com  ",
				DisplayName = "  Admin Person  ",
				Password = "strong-pwd-1!",
				ConfirmPassword = "strong-pwd-1!"
			};

			var result = controller.Setup(model);

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Login", redirect.ActionName);

			Assert.Single(saved);
			Assert.Equal("new@example.com", saved[0].user.EmailAddress);
			Assert.Equal("Admin Person", saved[0].user.DisplayName);
			Assert.Equal("HASHED", saved[0].user.Password);
			Assert.Equal(Role.Admin, saved[0].user.Role);
			Assert.True(saved[0].user.Active);
			Assert.Equal(string.Empty, saved[0].original);

			// Cache must be synced so the freshly-saved admin is visible to subsequent logins.
			Assert.Single(config.Object.Users);
			Assert.Equal("new@example.com", config.Object.Users[0].EmailAddress);
		}

		[Fact]
		public void Setup_Post_ReplacesExistingAdminPreservingOriginalEmail()
		{
			var existing = new User
			{
				Role = Role.Admin,
				EmailAddress = "old@example.com",
				DisplayName = "Old Admin",
				Password = string.Empty
			};
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns(new List<User> { existing });

			var saved = new List<(User user, string original)>();
			userService
				.Setup(s => s.AddOrReplaceUser(It.IsAny<User>(), It.IsAny<string>()))
				.Callback<User, string>((u, original) => saved.Add((u, original)));

			var security = new Mock<ISiteSecurityManager>();
			security.Setup(s => s.HashPassword(It.IsAny<string>())).Returns("HASHED");

			var controller = Build(
				setupRequired: true,
				userService: userService.Object,
				siteSecurityManager: security.Object);

			var model = new SetupViewModel
			{
				Email = "new@example.com",
				DisplayName = "New Admin",
				Password = "strong-pwd-1!",
				ConfirmPassword = "strong-pwd-1!"
			};

			controller.Setup(model);

			Assert.Single(saved);
			Assert.Equal("old@example.com", saved[0].original);
			Assert.Equal("new@example.com", saved[0].user.EmailAddress);
			Assert.Equal("New Admin", saved[0].user.DisplayName);
		}
	}
}
