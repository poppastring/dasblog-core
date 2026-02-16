using AutoMapper;
using Coravel;
using DasBlog.Core.Common;
using DasBlog.Services;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Scheduler;
using DasBlog.Services.Site;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Web
{
	public class Startup
	{
		private readonly IWebHostEnvironment hostingEnvironment;
		private readonly IDasBlogPathResolver pathResolver;

		public IConfiguration Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			hostingEnvironment = env;

			DasBlogPathResolver.EnsureDefaultConfigFiles(env.ContentRootPath, env.EnvironmentName);

			var defaultSiteConfigPath = Path.Combine("Config", "site.config");
			var siteConfigPath = Path.Combine("Config", $"site.{env.EnvironmentName}.config");
			var defaultMetaConfigPath = Path.Combine("Config", "meta.config");
			var metaConfigPath = Path.Combine("Config", $"meta.{env.EnvironmentName}.config");
			var oembedProvidersPath = Path.Combine("Config", "oembed-providers.json");

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddXmlFile(defaultSiteConfigPath, optional: false, reloadOnChange: true)
				.AddXmlFile(siteConfigPath, optional: true, reloadOnChange: true)
				.AddXmlFile(defaultMetaConfigPath, optional: false, reloadOnChange: true)
				.AddXmlFile(metaConfigPath, optional: true, reloadOnChange: true)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddJsonFile(oembedProvidersPath, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			pathResolver = new DasBlogPathResolver(env.ContentRootPath, env.EnvironmentName, Configuration);
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDasBlogConfiguration(Configuration, pathResolver);
			services.AddDasBlogSecurity(Configuration);
			services.AddDasBlogWebServices(Configuration);

			services.AddDasBlogDataServices();
			services.AddDasBlogManagers();
			services.AddDasBlogServices(hostingEnvironment);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDasBlogSettings dasBlogSettings, IDasBlogPathResolver paths)
		{
			(var siteOk, var siteError) = RepairSite(app);

			if (env.IsDevelopment() || env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/home/error");
			}

			if (env.IsStaging() || env.IsProduction())
			{
				app.UseHsts(options => options.MaxAge(days: 30));
			}

			app.ApplicationServices.UseScheduler(scheduler =>
			{
				scheduler
					.Schedule<SiteEmailReport>()
					.DailyAt(23, 19)
					.Zoned(TimeZoneInfo.Local);
			});

			if (!siteOk)
			{
				app.Run(async context => await context.Response.WriteAsync(siteError));
				return;
			}

			var rewriteOptions = new RewriteOptions()
				 .AddIISUrlRewrite(env.ContentRootFileProvider, paths.IISUrlRewriteRelativePath);

			app.UseRewriter(rewriteOptions);
			app.UseRouting();

			//if you've configured it at /blog or /whatever, set that pathbase so ~ will generate correctly
			var path = "/";
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.Root))
			{
				var rootUri = new Uri(dasBlogSettings.SiteConfiguration.Root);
				path = rootUri.AbsolutePath;
			}

			//Deal with path base and proxies that change the request path
			if (path != "/")
			{
				app.Use((context, next) =>
				{
					context.Request.PathBase = new PathString(path);
					return next.Invoke();
				});
			}

			app.UseForwardedHeaders();
			app.UseDasBlogStaticFiles(env, paths);
			app.UseCookiePolicy();

			app.UseAuthentication();
			app.Use(PopulateThreadCurrentPrincipalForMvc);
			app.UseRouting();
			app.UseAuthorization();

			app.UseDasBlogSecurityHeaders(Configuration);
			app.UseDasBlogEndpoints(dasBlogSettings);

			app.UseHttpContext();
		}

		/// <summary>
		/// BlogDataService and DayEntry rely on the thread's CurrentPrincipal and its role to determine if users
		/// should be allowed edit and add posts.
		/// Unfortunately the asp.net team no longer favour an approach involving the current thread so
		/// much as I am loath to stick values on globalish type stuff going up and down the stack
		/// this is a light touch way of including the functionality and actually looks fairly safe.
		/// Hopefully, in the fullness of time we will beautify the legacy code and this can go.
		/// </summary>
		private Task PopulateThreadCurrentPrincipalForMvc(HttpContext context, Func<Task> next)
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

		private static (bool result, string errorMessage) RepairSite(IApplicationBuilder app)
		{
			var sr = app.ApplicationServices.GetService<ISiteRepairer>();
			return sr.RepairSite();
		}
	}
}
