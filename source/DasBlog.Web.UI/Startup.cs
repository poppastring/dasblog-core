using System;
using System.Security.Principal;
using DasBlog.Web.Core;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.Repositories;
using DasBlog.Web.Repositories.Interfaces;
using DasBlog.Web.UI.Models.Identity;
using DasBlog.Web.UI.Settings;
using DasBlog.Web.UI.ViewsEngine;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DasBlog.Web.UI.Mappers;

namespace DasBlog.Web.UI
{
	public class Startup
	{
		private IHostingEnvironment _hostingEnvironment;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddXmlFile(@"Config\site.config", optional: true, reloadOnChange: true)
			.AddXmlFile(@"Config\metaConfig.xml", optional: true, reloadOnChange: true)
			.AddXmlFile(@"Config\siteSecurity.config", optional: true, reloadOnChange: true)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			.AddEnvironmentVariables();

			Configuration = builder.Build();

			_hostingEnvironment = env;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddMemoryCache();

			services.Configure<SiteConfig>(Configuration);
			services.Configure<MetaTags>(Configuration);
			services.Configure<SiteSecurityConfig>(Configuration);

			// Add identity types
			services
				.AddIdentity<DasBlogUser, DasBlogRole>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = true;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 6;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 10;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = true;
			});

			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.Cookie.Expiration = TimeSpan.FromDays(150);
				options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
				options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
				options.AccessDeniedPath = "/Account/AccessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
				options.SlidingExpiration = true;
			});

			services.Configure<RazorViewEngineOptions>(rveo =>
			{
				rveo.ViewLocationExpanders.Add(new DasBlogLocationExpander(Configuration.GetSection("DasBlogSettings")["Theme"]));
			});

			services
				.AddTransient<IDasBlogSettings, DasBlogSettings>()
				.AddTransient<IUserStore<DasBlogUser>, DasBlogUserStore>()
				.AddTransient<IRoleStore<DasBlogRole>, DasBlogUserRoleStore>()
				.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User);

			services
				.AddSingleton<IBlogRepository, BlogRepository>()
				.AddSingleton<ISubscriptionRepository, SubscriptionRepository>()
				.AddSingleton<IArchiveRepository, ArchiveRepository>()
				.AddSingleton<ICategoryRepository, CategoryRepository>()
				.AddSingleton<ISiteSecurityRepository, SiteSecurityRepository>()
				.AddSingleton<ISiteRepository, SiteRepository>()
				.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services
				.AddAutoMapper(typeof(ProfileDasBlogUser))
				.AddMvc()
				.AddXmlSerializerFormatters();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"Original Post Format",
					"{posttitle}.aspx",
					new { controller = "Home", action = "Post", posttitle = "" });

				routes.MapRoute(
					"New Post Format",
					"{posttitle}",
					new { controller = "Home", action = "Post", id = "" });

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});

			RewriteOptions options = new RewriteOptions()
				 .AddIISUrlRewrite(env.ContentRootFileProvider, @"Config\IISUrlRewrite.xml");

			app.UseRewriter(options);
		}
	}
}
