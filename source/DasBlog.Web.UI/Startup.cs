﻿using System;
using System.Security.Principal;
using DasBlog.Core.Configuration;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Identity;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DasBlog.Web.Mappers;
using DasBlog.Core;
using DasBlog.Services;
using DasBlog.Services.Interfaces;

namespace DasBlog.Web
{
	public class Startup
	{
		public const string SITESECURITYCONFIG = @"Config\siteSecurity.config";
		private IHostingEnvironment _hostingEnvironment;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddXmlFile(@"Config\site.config", optional: true, reloadOnChange: true)
			.AddXmlFile(@"Config\metaConfig.xml", optional: true, reloadOnChange: true)
			.AddXmlFile(SITESECURITYCONFIG, optional: true, reloadOnChange: true)
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
				options.LoginPath = "/account/login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
				options.LogoutPath = "/account/logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
				options.AccessDeniedPath = "/account/accessDenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
				options.SlidingExpiration = true;
				options.Cookie = new CookieBuilder
				{
					HttpOnly = true,
					Expiration = TimeSpan.FromDays(30),
				};
			});

			services.Configure<RazorViewEngineOptions>(rveo =>
			{
				rveo.ViewLocationExpanders.Add(new DasBlogLocationExpander(Configuration.GetSection("DasBlogSettings")["Theme"]));
			});

			services
				.AddTransient<IDasBlogSettings, DasBlogSettings>()
				.AddTransient<IUserStore<DasBlogUser>, DasBlogUserStore>()
				.AddTransient<IRoleStore<DasBlogRole>, DasBlogUserRoleStore>()
				.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User)
				;

			services
				.AddSingleton(_hostingEnvironment.ContentRootFileProvider)
				.AddSingleton<IBlogManager, BlogManager>()
				.AddSingleton<ISubscriptionManager, SubscriptionManager>()
				.AddSingleton<IArchiveManager, ArchiveManager>()
				.AddSingleton<ICategoryManager, CategoryManager>()
				.AddSingleton<ISiteSecurityManager, SiteSecurityManager>()
				.AddSingleton<IXmlRpcManager, XmlRpcManager>()
				.AddSingleton<ISiteManager, SiteManager>()
				.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
				.AddSingleton<IPrincipalService, PrincipalService>();
			services
				.AddAutoMapper(mapperConfig =>
				{
					mapperConfig.AddProfile(new ProfilePost(services.BuildServiceProvider().GetService<IDasBlogSettings>()));
					mapperConfig.AddProfile(typeof(ProfileDasBlogUser));
				})
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
				app.UseExceptionHandler("/home/error");
			}
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					"Original Post Format",
					"{posttitle}.aspx",
					new { controller = "BlogPost", action = "Post", posttitle = "" });

				routes.MapRoute(
					"New Post Format",
					"{posttitle}",
					new { controller = "BlogPost", action = "Post", postitle = ""  });

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
