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
using QRCoder;
using System;
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
		private readonly IDasBlogSettings settings;
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
			this.settings = settings;
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

				if (result.RequiresTwoFactor)
				{
					return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, rememberMe = model.RememberMe });
				}

				logger.LogInformation(new EventDataItem(EventCodes.SecurityFailure, null, 
												"{email} failed to log in", model.Email));

				ModelState.AddModelError(string.Empty, "The username and/or password is incorrect. Please try again.");
			}

			return View(model);
		}

		[HttpGet("account/login-with-2fa")]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
		{
			SetNoStoreHeaders();

			var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return NotFound();
			}

			DefaultPage("Two-factor authentication");
			return View(new LoginWith2faViewModel { RememberMe = rememberMe, ReturnUrl = returnUrl });
		}

		[HttpPost("account/login-with-2fa")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[EnableRateLimiting("login")]
		public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model)
		{
			SetNoStoreHeaders();

			var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				DefaultPage("Two-factor authentication");
				return View(model);
			}

			var code = StripAuthenticatorCode(model.TwoFactorCode);
			var result = await signInManager.TwoFactorAuthenticatorSignInAsync(code, model.RememberMe, model.RememberMachine);

			if (result.Succeeded)
			{
				logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
					"{email} completed two-factor authentication", user.Email));

				return LocalRedirect(model.ReturnUrl ?? Url.Action("Index", "Home"));
			}

			logger.LogInformation(new EventDataItem(EventCodes.SecurityFailure, null,
				"{email} failed two-factor authentication", user.Email));

			ModelState.AddModelError(string.Empty, "The authenticator code is invalid.");
			DefaultPage("Two-factor authentication");
			return View(model);
		}

		[HttpGet("account/login-with-recovery-code")]
		[AllowAnonymous]
		public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
		{
			SetNoStoreHeaders();

			var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return NotFound();
			}

			DefaultPage("Recovery code");
			return View(new LoginWithRecoveryCodeViewModel { ReturnUrl = returnUrl });
		}

		[HttpPost("account/login-with-recovery-code")]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		[EnableRateLimiting("login")]
		public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model)
		{
			SetNoStoreHeaders();

			var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
			if (user == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				DefaultPage("Recovery code");
				return View(model);
			}

			var result = await signInManager.TwoFactorRecoveryCodeSignInAsync(model.RecoveryCode.Trim());
			if (result.Succeeded)
			{
				logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
					"{email} logged in with a recovery code", user.Email));

				return LocalRedirect(model.ReturnUrl ?? Url.Action("Index", "Home"));
			}

			logger.LogInformation(new EventDataItem(EventCodes.SecurityFailure, null,
				"{email} failed recovery code login", user.Email));

			ModelState.AddModelError(string.Empty, "The recovery code is invalid or has already been used.");
			DefaultPage("Recovery code");
			return View(model);
		}

		[HttpGet("account/security/enable-authenticator")]
		[RequireHttps]
		public async Task<IActionResult> EnableAuthenticator()
		{
			SetNoStoreHeaders();

			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			DefaultPage("Enable authenticator");
			return View(await BuildEnableAuthenticatorViewModel(user));
		}

		[HttpPost("account/security/enable-authenticator")]
		[RequireHttps]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel model)
		{
			SetNoStoreHeaders();

			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			if (!ModelState.IsValid)
			{
				DefaultPage("Enable authenticator");
				return View(await BuildEnableAuthenticatorViewModel(user));
			}

			var code = StripAuthenticatorCode(model.Code);
			var isValid = await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
			if (!isValid)
			{
				ModelState.AddModelError(nameof(model.Code), "The verification code is invalid.");
				DefaultPage("Enable authenticator");
				return View(await BuildEnableAuthenticatorViewModel(user));
			}

			await userManager.SetTwoFactorEnabledAsync(user, true);
			var recoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToList();
			await signInManager.RefreshSignInAsync(user);

			logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
				"{email} enabled two-factor authentication", user.Email));

			DefaultPage("Recovery codes");
			return View("ShowRecoveryCodes", new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes });
		}

		[HttpGet("account/security/disable-2fa")]
		public IActionResult Disable2fa()
		{
			DefaultPage("Disable two-factor authentication");
			return View();
		}

		[HttpPost("account/security/disable-2fa")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Disable2faConfirmed()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			await userManager.SetTwoFactorEnabledAsync(user, false);
			await signInManager.RefreshSignInAsync(user);

			logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
				"{email} disabled two-factor authentication", user.Email));

			TempData["SuccessMessage"] = "Two-factor authentication has been disabled.";
			return RedirectToAction(nameof(EnableAuthenticator));
		}

		[HttpGet("account/security/reset-recovery-codes")]
		public IActionResult ResetRecoveryCodes()
		{
			DefaultPage("Reset recovery codes");
			return View();
		}

		[HttpPost("account/security/reset-recovery-codes")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetRecoveryCodesConfirmed()
		{
			var user = await userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound();
			}

			if (!await userManager.GetTwoFactorEnabledAsync(user))
			{
				return RedirectToAction(nameof(EnableAuthenticator));
			}

			var recoveryCodes = (await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10)).ToList();

			logger.LogInformation(new EventDataItem(EventCodes.SecuritySuccess, null,
				"{email} reset two-factor recovery codes", user.Email));

			DefaultPage("Recovery codes");
			return View("ShowRecoveryCodes", new RecoveryCodesViewModel { RecoveryCodes = recoveryCodes });
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

		private async Task<EnableAuthenticatorViewModel> BuildEnableAuthenticatorViewModel(DasBlogUser user)
		{
			var key = await userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(key))
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
				key = await userManager.GetAuthenticatorKeyAsync(user);
			}

			var issuer = settings.SiteConfiguration?.Title ?? "DasBlog";
			var authenticatorUri = GenerateQrCodeUri(user.Email, key, issuer);

			return new EnableAuthenticatorViewModel
			{
				SharedKey = FormatKey(key),
				AuthenticatorUri = authenticatorUri,
				QrCodeSvg = GenerateQrCodeSvg(authenticatorUri),
				IsTwoFactorEnabled = await userManager.GetTwoFactorEnabledAsync(user)
			};
		}

		private static string GenerateQrCodeUri(string email, string unformattedKey, string issuer)
		{
			return string.Format(
				"otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
				Uri.EscapeDataString(issuer),
				Uri.EscapeDataString(email),
				Uri.EscapeDataString(unformattedKey));
		}

		private static string GenerateQrCodeSvg(string authenticatorUri)
		{
			using var generator = new QRCodeGenerator();
			using var data = generator.CreateQrCode(authenticatorUri, QRCodeGenerator.ECCLevel.Q);
			var qrCode = new SvgQRCode(data);
			return qrCode.GetGraphic(4);
		}

		private static string FormatKey(string unformattedKey)
		{
			if (string.IsNullOrEmpty(unformattedKey))
			{
				return string.Empty;
			}

			var result = string.Join(" ", Enumerable.Range(0, (unformattedKey.Length + 3) / 4)
				.Select(i => unformattedKey.Substring(i * 4, Math.Min(4, unformattedKey.Length - i * 4))));
			return result.ToLowerInvariant();
		}

		private static string StripAuthenticatorCode(string code)
		{
			return code?.Replace(" ", string.Empty).Replace("-", string.Empty);
		}

		private void SetNoStoreHeaders()
		{
			Response.Headers["Cache-Control"] = "no-store, no-cache, max-age=0";
			Response.Headers["Pragma"] = "no-cache";
			Response.Headers["Expires"] = "0";
		}
	}
}
