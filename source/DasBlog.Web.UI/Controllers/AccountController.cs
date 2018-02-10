using System.Threading.Tasks;
using DasBlog.Web.UI.Models.AccountViewModels;
using DasBlog.Web.UI.Models.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Web.UI.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private const string KEY_RETURNURL = "ReturnUrl";
		private readonly UserManager<DasBlogUser> _userManager;
		private readonly SignInManager<DasBlogUser> _signInManager;
		private readonly ILogger _logger;

		public AccountController(
			UserManager<DasBlogUser> userManager,
			SignInManager<DasBlogUser> signInManager,
			ILoggerFactory loggerFactory)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = loggerFactory.CreateLogger<AccountController>();
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			await HttpContext.Authentication.SignOutAsync(IdentityConstants.ExternalScheme);

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
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					return RedirectToLocal(returnUrl);
				}
			}

			return View(model);
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction(nameof(HomeController.Index), "Home");
		}
	}
}
