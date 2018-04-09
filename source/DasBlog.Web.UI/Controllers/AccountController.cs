using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		private const string KEY_RETURNURL = "ReturnUrl";
		private readonly UserManager<DasBlogUser> _userManager;
		private readonly SignInManager<DasBlogUser> _signInManager;
		private readonly IMapper _mapper;
		private readonly ILogger _logger;

		public AccountController(
			UserManager<DasBlogUser> userManager,
			SignInManager<DasBlogUser> signInManager,
			IMapper mapper,
			ILoggerFactory loggerFactory)
		{
			_userManager = userManager;
			_userManager.PasswordHasher = new DasBlogPasswordHasher();

			_signInManager = signInManager;
			_mapper = mapper;
			_logger = loggerFactory.CreateLogger<AccountController>();
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
				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					return RedirectToLocal(returnUrl);
				}
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Register(string returnUrl)
		{
			ViewData[KEY_RETURNURL] = returnUrl;

			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData[KEY_RETURNURL] = returnUrl;

			if (ModelState.IsValid)
			{
				var user = _mapper.Map<DasBlogUser>(model);
				var result = await _userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
					return RedirectToLocal(returnUrl);
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
