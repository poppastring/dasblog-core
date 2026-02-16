using DasBlog.Services;
using DasBlog.Services.FileManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Web
{
	public static class DasBlogApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseDasBlogStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env, IDasBlogPathResolver paths)
		{
			app.UseStaticFiles();

			Action<StaticFileResponseContext> cacheControlPrepResponse = (ctx) =>
			{
				const int durationInSeconds = 60 * 60 * 24;
				ctx.Context.Response.Headers[HeaderNames.CacheControl] =
					"public,max-age=" + durationInSeconds;
				ctx.Context.Response.Headers["Expires"] = DateTime.UtcNow.AddHours(12).ToString("R");
			};

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(paths.BinariesPath),
				RequestPath = $"/{paths.BinariesUrlRelativePath}",
				OnPrepareResponse = cacheControlPrepResponse
			});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(paths.RadioStoriesFolderPath),
				RequestPath = "/content/radioStories",
				OnPrepareResponse = cacheControlPrepResponse
			});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Themes")),
				RequestPath = "/theme",
				OnPrepareResponse = cacheControlPrepResponse
			});

			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Themes")),
				RequestPath = "/themes",
				OnPrepareResponse = cacheControlPrepResponse
			});

			return app;
		}

		public static IApplicationBuilder UseDasBlogSecurityHeaders(this IApplicationBuilder app, IConfiguration configuration)
		{
			app.UseXContentTypeOptions();
			app.UseXfo(options => options.SameOrigin());
			app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());

			var securityScriptSources = configuration.GetSection("SecurityScriptSources")?.Value?.Split(";");
			var securityStyleSources = configuration.GetSection("SecurityStyleSources")?.Value?.Split(";");
			var defaultSources = configuration.GetSection("DefaultSources")?.Value?.Split(";");

			if (securityStyleSources != null && securityScriptSources != null && defaultSources != null)
			{
				app.UseCsp(options => options
					.DefaultSources(s => s.Self()
						.CustomSources(defaultSources)
						)
					.StyleSources(s => s.Self()
						.CustomSources(securityStyleSources)
						.UnsafeInline()
					)
					.ScriptSources(s => s.Self()
						.CustomSources(securityScriptSources)
						.UnsafeInline()
						.UnsafeEval()
					)
				);
			}

			app.Use(async (context, next) =>
			{
				context.Response.Headers["Permissions-Policy"] = "geolocation=(),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()";
				await next.Invoke();
			});

			return app;
		}

		public static IApplicationBuilder UseDasBlogEndpoints(this IApplicationBuilder app, IDasBlogSettings dasBlogSettings)
		{
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHealthChecks("/healthcheck");

				if (dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique)
				{
					endpoints.MapControllerRoute(
						"Original Post Format",
						"~/{year:int}/{month:int}/{day:int}/{posttitle}.aspx",
						new { controller = "BlogPost", action = "Post", posttitle = "" });

					endpoints.MapControllerRoute(
						"New Post Format",
						"~/{year:int}/{month:int}/{day:int}/{posttitle}",
						new { controller = "BlogPost", action = "Post", postitle = "" });
				}
				else
				{
					endpoints.MapControllerRoute(
						"Original Post Format",
						"~/{posttitle}.aspx",
						new { controller = "BlogPost", action = "Post", posttitle = "" });

					endpoints.MapControllerRoute(
						"New Post Format",
						"~/{posttitle}",
						new { controller = "BlogPost", action = "Post", postitle = "" });
				}

						endpoints.MapControllerRoute(
								name: "default", "~/{controller=Home}/{action=Index}/{id?}");
			});

			return app;
		}

		/// <summary>
		/// BlogDataService and DayEntry rely on the thread's CurrentPrincipal and its role to determine if users
		/// should be allowed edit and add posts.
		/// Unfortunately the asp.net team no longer favour an approach involving the current thread so
		/// much as I am loath to stick values on globalish type stuff going up and down the stack
		/// this is a light touch way of including the functionality and actually looks fairly safe.
		/// Hopefully, in the fullness of time we will beautify the legacy code and this can go.
		/// </summary>
		public static IApplicationBuilder UseDasBlogThreadPrincipal(this IApplicationBuilder app)
		{
			app.Use(PopulateThreadCurrentPrincipal);
			return app;
		}

		private static Task PopulateThreadCurrentPrincipal(HttpContext context, Func<Task> next)
		{
			IPrincipal existingThreadPrincipal = null;
			try
			{
				existingThreadPrincipal = Thread.CurrentPrincipal;
				Thread.CurrentPrincipal = context.User;
				return next();
			}
			finally
			{
				Thread.CurrentPrincipal = existingThreadPrincipal;
			}
		}
	}
}
