using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Controllers;
using DasBlog.Web.Identity;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Controllers
{
	public class AccountControllerTwoFactorTests
	{
		[Fact]
		public async Task Login_Post_RedirectsToTwoFactorChallengeWhenPasswordRequiresTwoFactor()
		{
			var signInManager = BuildSignInManager();
			signInManager
				.Setup(s => s.PasswordSignInAsync("admin@example.com", "password", true, false))
				.ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.TwoFactorRequired);

			var controller = new AccountController(
				userManager: null,
				signInManager: signInManager.Object,
				mapper: null,
				logger: Mock.Of<ILogger<AccountController>>(),
				settings: Mock.Of<IDasBlogSettings>(),
				firstRunService: Mock.Of<IFirstRunService>(),
				userService: Mock.Of<IUserService>(),
				siteSecurityManager: Mock.Of<ISiteSecurityManager>(),
				siteSecurityConfig: Mock.Of<ISiteSecurityConfig>());

			var result = await controller.Login(new LoginViewModel
			{
				Email = "admin@example.com",
				Password = "password",
				RememberMe = true
			}, "/admin/settings");

			var redirect = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("LoginWith2fa", redirect.ActionName);
			Assert.Equal("/admin/settings", redirect.RouteValues["returnUrl"]);
			Assert.Equal(true, redirect.RouteValues["rememberMe"]);
		}

		private static Mock<SignInManager<DasBlogUser>> BuildSignInManager()
		{
			var userManager = new Mock<UserManager<DasBlogUser>>(
				Mock.Of<IUserStore<DasBlogUser>>(),
				Mock.Of<IOptions<IdentityOptions>>(),
				Mock.Of<IPasswordHasher<DasBlogUser>>(),
				new List<IUserValidator<DasBlogUser>>(),
				new List<IPasswordValidator<DasBlogUser>>(),
				Mock.Of<ILookupNormalizer>(),
				new IdentityErrorDescriber(),
				Mock.Of<IServiceProvider>(),
				Mock.Of<ILogger<UserManager<DasBlogUser>>>());

			return new Mock<SignInManager<DasBlogUser>>(
				userManager.Object,
				Mock.Of<IHttpContextAccessor>(),
				Mock.Of<IUserClaimsPrincipalFactory<DasBlogUser>>(),
				Options.Create(new IdentityOptions()),
				Mock.Of<ILogger<SignInManager<DasBlogUser>>>(),
				Mock.Of<IAuthenticationSchemeProvider>(),
				Mock.Of<IUserConfirmation<DasBlogUser>>());
		}
	}
}
