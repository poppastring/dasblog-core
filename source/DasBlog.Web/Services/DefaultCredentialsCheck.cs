using System;
using System.Linq;
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
		private const string DEFAULT_EMAIL = "myemail@myemail.com";
		private const string DEFAULT_PASSWORD_HASH = "19-A2-85-41-44-B6-3A-8F-76-17-A6-F2-25-01-9B-12";

		private readonly IUserService userService;
		private readonly IHostEnvironment hostEnvironment;

		public DefaultCredentialsCheck(IUserService userService, IHostEnvironment hostEnvironment)
		{
			this.userService = userService;
			this.hostEnvironment = hostEnvironment;
		}

		public bool IsUsingDefaults()
		{
			// Skip the warning in Development - devs intentionally use defaults
			if (hostEnvironment.IsDevelopment())
			{
				return false;
			}

			var users = userService.GetAllUsers();
			return users.Any(u =>
				string.Equals(u.EmailAddress, DEFAULT_EMAIL, StringComparison.OrdinalIgnoreCase)
				|| u.Password == DEFAULT_PASSWORD_HASH);
		}
	}
}


