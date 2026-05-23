﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.Services
{
	/// <summary>
	/// Redirects all non-setup traffic to <c>/account/setup</c> when the site has
	/// no usable admin credentials, so a fresh install can't be browsed or driven
	/// until an operator establishes the initial admin user.
	/// </summary>
	public class FirstRunSetupMiddleware
	{
		private const string SetupPath = "/account/setup";

		private readonly RequestDelegate next;

		public FirstRunSetupMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context, IFirstRunService firstRunService)
		{
			if (firstRunService.IsSetupRequired() && ShouldGate(context.Request.Path))
			{
				context.Response.Redirect(SetupPath);
				return;
			}

			await next(context);
		}

		private static bool ShouldGate(PathString path)
		{
			var value = path.Value ?? string.Empty;

			if (value.StartsWith(SetupPath, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (value.StartsWith("/health", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (value.StartsWith("/home/error", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			return true;
		}
	}

	public static class FirstRunSetupMiddlewareExtensions
	{
		public static IApplicationBuilder UseFirstRunSetup(this IApplicationBuilder app)
		{
			return app.UseMiddleware<FirstRunSetupMiddleware>();
		}
	}
}
