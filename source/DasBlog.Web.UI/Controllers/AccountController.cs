using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private const string KEY_RETURNURL = "ReturnUrl";
		private readonly ILogger<AccountController> logger;
		private readonly IMapper mapper;
		private readonly SignInManager<DasBlogUser> signInManager;
		private readonly UserManager<DasBlogUser> userManager;

		public AccountController(
			UserManager<DasBlogUser> userManager,
			SignInManager<DasBlogUser> signInManager,
			IMapper mapper,
			ISiteSecurityManager siteSecurityManager
			, ILogger<AccountController> logger)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.userManager.PasswordHasher = new DasBlogPasswordHasher(siteSecurityManager);
			this.logger = logger;
			this.mapper = mapper;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			// TODO: https://go.microsoft.com/fwlink/?linkid=845470
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ViewData[KEY_RETURNURL] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData[KEY_RETURNURL] = returnUrl;
			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					logger.LogInformation((int)EventCodes.SecuritySuccess
					  , "{email} logged in successfully :: {url}", model.Email, Request.GetDisplayUrl());
					return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
				}
					logger.LogInformation((int)EventCodes.SecuritySuccess
					  , "{email} failed to log in :: {url}", model.Email, Request.GetDisplayUrl());

				ModelState.AddModelError(string.Empty, "The username and/or password is incorrect. Please try again.");
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Register(string returnUrl)
		{
			ViewData[KEY_RETURNURL] = returnUrl;

			return RedirectToAction("Index", "Home");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			if (ModelState.IsValid)
			{
				var user = mapper.Map<DasBlogUser>(model);
				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					// Success Notification
				}
				else
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError(item.Code, item.Description);
					}
				}
			}

			return View(model);
		}
	}
}
