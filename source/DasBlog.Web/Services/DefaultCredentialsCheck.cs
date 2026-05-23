using System;
using System.Linq;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.Users;
using Microsoft.Extensions.Hosting;

namespace DasBlog.Web.Services
{
	public interface IDefaultCredentialsCheck
	{
		bool IsUsingDefaults();
	}

	public class DefaultCredentialsCheck : IDefaultCredentialsCheck
	{
		internal const string DEFAULT_EMAIL = "myemail@myemail.com";
		internal const string DEFAULT_PASSWORD = "admin";

		private readonly IUserService userService;
		private readonly IHostEnvironment hostEnvironment;
		private readonly ISiteSecurityManager siteSecurityManager;

		public DefaultCredentialsCheck(
			IUserService userService,
			IHostEnvironment hostEnvironment,
			ISiteSecurityManager siteSecurityManager)
		{
			this.userService = userService;
			this.hostEnvironment = hostEnvironment;
			this.siteSecurityManager = siteSecurityManager;
		}

		public bool IsUsingDefaults()
		{
			// Skip the warning in Development - devs intentionally use defaults
			if (hostEnvironment.IsDevelopment())
			{
				return false;
			}

			var users = userService.GetAllUsers();
			if (users == null)
			{
				return true;
			}

			return users.Any(u =>
				string.Equals(u.EmailAddress, DEFAULT_EMAIL, StringComparison.OrdinalIgnoreCase)
				|| string.IsNullOrWhiteSpace(u.Password)
				|| siteSecurityManager.VerifyHashedPassword(u.Password, DEFAULT_PASSWORD));
		}
	}
}


