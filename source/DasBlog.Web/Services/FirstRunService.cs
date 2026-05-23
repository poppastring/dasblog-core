﻿using System.Linq;
using DasBlog.Services.Users;

namespace DasBlog.Web.Services
{
	public interface IFirstRunService
	{
		/// <summary>
		/// Returns true when the site has no usable admin credentials and the
		/// operator must complete the first-run setup flow before logging in.
		/// </summary>
		bool IsSetupRequired();
	}

	public class FirstRunService : IFirstRunService
	{
		private readonly IUserService userService;

		public FirstRunService(IUserService userService)
		{
			this.userService = userService;
		}

		public bool IsSetupRequired()
		{
			var users = userService.GetAllUsers()?.ToList();
			if (users == null || users.Count == 0)
			{
				return true;
			}

			// If every stored user has an empty password, the operator hasn't
			// completed setup yet (this is the state the shipped template uses).
			return users.All(u => string.IsNullOrWhiteSpace(u.Password));
		}
	}
}
