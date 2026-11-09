using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.FileManagement;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;

namespace DasBlog.Web
{
	public static class DasBlogApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseDasBlogStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env, IDasBlogPathResolver paths)
		{
			app.UseDasBlogLegacyAssets();
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

		public static IApplicationBuilder UseDasBlogLegacyAssets(this IApplicationBuilder app)
		{
			return app.Use(async (context, next) =>
			{
				if (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method))
				{
					string cdnPath = null;
					PathString remaining;
					if (context.Request.Path.StartsWithSegments("/lib/bootstrap/5.3.3", out remaining))
						cdnPath = "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.3";
					else if (context.Request.Path.StartsWithSegments("/lib/bootswatch/5.3.3", out remaining))
						cdnPath = "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.3";
					else if (context.Request.Path.StartsWithSegments("/lib/font-awesome/6.7.2", out remaining))
						cdnPath = "https://cdn.jsdelivr.net/gh/FortAwesome/Font-Awesome@6.7.2";

					if (cdnPath != null && remaining.HasValue && remaining != new PathString("/"))
					{
						context.Response.Redirect(cdnPath + remaining.ToUriComponent() + context.Request.QueryString.ToUriComponent());
						return;
					}
				}

				await next.Invoke();
			});
		}

		public static IApplicationBuilder UseDasBlogSecurityHeaders(this IApplicationBuilder app, IConfiguration configuration)
		{
			app.UseXContentTypeOptions();
			app.UseXfo(options => options.SameOrigin());
			app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());

			var assetCdnSources = new[] { "https://cdnjs.cloudflare.com", "https://cdn.jsdelivr.net" };
			var securityScriptSources = GetSecuritySources(configuration, "SecurityScriptSources",
				assetCdnSources.Concat(new[] { "https://ajax.aspnetcdn.com" }).ToArray());
			var securityStyleSources = GetSecuritySources(configuration, "SecurityStyleSources",
				assetCdnSources.Concat(new[] { "https://fonts.googleapis.com" }).ToArray());
			var defaultSources = GetSecuritySources(configuration, "DefaultSources");
			var fontSources = GetSecuritySources(configuration, "DefaultSources",
				assetCdnSources.Concat(new[] { "https://fonts.gstatic.com" }).ToArray());
			var imageSources = GetSecuritySources(configuration, "DefaultSources", "https://cdn.jsdelivr.net");

			app.UseCsp(options => options
				.DefaultSources(s =>
				{
					s.Self();
					if (defaultSources.Length > 0)
						s.CustomSources(defaultSources);
				})
				.StyleSources(s => s.Self()
					.CustomSources(securityStyleSources)
					.UnsafeInline()
				)
				.ScriptSources(s => s.Self()
					.CustomSources(securityScriptSources)
					.UnsafeInline()
					.UnsafeEval()
				)
				.FontSources(s => s.Self()
					.CustomSources(fontSources)
				)
				.ImageSources(s => s.Self()
					.CustomSources(imageSources)
				)
			);

			app.Use(async (context, next) =>
			{
				context.Response.Headers["Permissions-Policy"] = "geolocation=(),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()";
				await next.Invoke();
			});

			return app;
		}

		private static string[] GetSecuritySources(IConfiguration configuration, string key, params string[] requiredSources)
		{
			var configuredSources = configuration[key]?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				?? Array.Empty<string>();
			return requiredSources.Concat(configuredSources).Distinct(StringComparer.Ordinal).ToArray();
		}

		public static IApplicationBuilder UseDasBlogNoIndexPolicy(this IApplicationBuilder app)
		{
			app.Use(async (context, next) =>
			{
				var noIndexEndpoint = context.GetEndpoint()?.Metadata.GetMetadata<Controllers.NoIndexAttribute>() != null;

				context.Response.OnStarting(() =>
				{
					if (noIndexEndpoint || context.Response.StatusCode >= StatusCodes.Status400BadRequest)
					{
						context.Response.Headers["X-Robots-Tag"] = "noindex, nofollow";
					}

					return Task.CompletedTask;
				});

				await next.Invoke();
			});

			return app;
		}

		public static IApplicationBuilder UseDasBlogEndpoints(this IApplicationBuilder app)
		{
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHealthChecks("/healthcheck");

				endpoints.MapControllers();

				endpoints.MapControllerRoute(
					"Unique Original Post Format",
					"~/{year:int}/{month:int}/{day:int}/{posttitle}.aspx",
					new { controller = "BlogPost", action = "Post" });

				endpoints.MapControllerRoute(
					"Unique New Post Format",
					"~/{year:int}/{month:int}/{day:int}/{posttitle}",
					new { controller = "BlogPost", action = "Post" });

				endpoints.MapControllerRoute(
					"Original Post Format",
					"~/{posttitle}.aspx",
					new { controller = "BlogPost", action = "Post" });

				endpoints.MapControllerRoute(
					"New Post Format",
					"~/{posttitle}",
					new { controller = "BlogPost", action = "Post" });

				endpoints.MapControllerRoute(
					name: "default", "~/{controller=Home}/{action=Index}/{id?}");

				endpoints.MapFallbackToController("NotFoundPage", "BlogPost");
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
