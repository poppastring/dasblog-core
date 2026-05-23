using AutoMapper;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AccountController : DasBlogBaseController
	{
		private const string KEY_RETURNURL = "ReturnUrl";
		private readonly ILogger<AccountController> logger;
		private readonly IMapper mapper;
		private readonly SignInManager<DasBlogUser> signInManager;
		private readonly UserManager<DasBlogUser> userManager;
		private readonly IFirstRunService firstRunService;
		private readonly IUserService userService;
		private readonly ISiteSecurityManager siteSecurityManager;
		private readonly ISiteSecurityConfig siteSecurityConfig;
		private const string LOGIN = "Login";

		public AccountController(UserManager<DasBlogUser> userManager, SignInManager<DasBlogUser> signInManager,
							IMapper mapper, ILogger<AccountController> logger, IDasBlogSettings settings,
							IFirstRunService firstRunService, IUserService userService,
							ISiteSecurityManager siteSecurityManager, ISiteSecurityConfig siteSecurityConfig)
							: base(settings)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.logger = logger;
			this.mapper = mapper;
			this.firstRunService = firstRunService;
			this.userService = userService;
			this.siteSecurityManager = siteSecurityManager;
			this.siteSecurityConfig = siteSecurityConfig;
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			// TODO: https://go.microsoft.com/fwlink/?linkid=845470
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ViewData[KEY_RETURNURL] = returnUrl;
			DefaultPage(LOGIN);
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[EnableRateLimiting("login")]
		public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
		{
			ViewData[KEY_RETURNURL] = returnUrl;
			if (ModelState.IsValid)
			{
				// Note: lockoutOnFailure is intentionally false. The login email is publicly
				// visible (it's the author byline on posts), so per-account lockout would let any
				// anonymous visitor lock the admin out by deliberately failing logins. Brute-force
				// is mitigated by the per-IP "login" rate-limit policy applied to this action.
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

				if (result.Succeeded)
				{
					logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null, 
												"{email} logged in successfully", model.Email));

					return LocalRedirect(returnUrl ?? Url.Action("Index", "Home"));
				}
				logger.LogInformation(new EventDataItem(EventCodes.SecurityFailure, null, 
												"{email} failed to log in", model.Email));

				ModelState.AddModelError(string.Empty, "The username and/or password is incorrect. Please try again.");
			}

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			var userName = HttpContext.User.Identity?.Name ?? "Unknown";

			await signInManager.SignOutAsync();

			logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null, "{email} logged out successfully", userName));

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Register(string returnUrl)
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
				if (!result.Succeeded)
				{
					foreach (var item in result.Errors)
					{
						ModelState.AddModelError(item.Code, item.Description);
					}
				}
			}

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Setup()
		{
			if (!firstRunService.IsSetupRequired())
			{
				return NotFound();
			}

			DefaultPage("Setup");
			return View(new SetupViewModel());
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public IActionResult Setup(SetupViewModel model)
		{
			if (!firstRunService.IsSetupRequired())
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var users = userService.GetAllUsers().ToList();
			var admin = users.FirstOrDefault(u => u.Role == Role.Admin);
			var originalEmail = admin?.EmailAddress ?? string.Empty;

			if (admin == null)
			{
				admin = new User { Role = Role.Admin, Active = true };
			}

			admin.EmailAddress = model.Email.Trim();
			admin.DisplayName = model.DisplayName.Trim();
			admin.Active = true;
			admin.Password = siteSecurityManager.HashPassword(model.Password);

			userService.AddOrReplaceUser(admin, originalEmail);

			// Refresh the cached SecurityConfiguration.Users list so the freshly-saved
			// admin is visible to DasBlogUserStore.FindByNameAsync on the next login.
			siteSecurityConfig.Users = userService.GetAllUsers().ToList();

			logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
				"First-run setup completed for {email}", admin.EmailAddress));

			return RedirectToAction(nameof(Login));
		}
	}
}
