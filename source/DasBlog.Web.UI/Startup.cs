using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
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
using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;
using Microsoft.Extensions.FileProviders;
using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Web
{
	public class Startup
	{
		public const string SITESECURITYCONFIG = @"Config\siteSecurity.config";
		private IHostingEnvironment _hostingEnvironment;
		private string _binariesPath;

		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
			.SetBasePath(env.ContentRootPath)
			.AddXmlFile(@"Config\site.config", optional: true, reloadOnChange: true)
			.AddXmlFile(@"Config\metaConfig.xml", optional: true, reloadOnChange: true)
			//.AddXmlFile(SITESECURITYCONFIG, optional: true, reloadOnChange: true)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			.AddEnvironmentVariables()
			;
			
			Configuration = builder.Build();

			_hostingEnvironment = env;
			_binariesPath = Configuration.GetValue<string>("binariesDir", "/").TrimStart('~').TrimEnd('/');
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddOptions();
			services.AddMemoryCache();

			services.Configure<SiteConfig>(Configuration);
			services.Configure<MetaTags>(Configuration);
//			services.Configure<SiteSecurityConfig>(Configuration);
			services.Configure<LocalUserDataOptions>(options
			  => options.Path = Path.Combine(_hostingEnvironment.ContentRootPath, SITESECURITYCONFIG));
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
				.AddTransient<ISiteRepairer, SiteRepairer>()
				.AddSingleton<ISiteSecurityConfig, SiteSecurityConfig>()
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
				.AddSingleton<IFileSystemBinaryManager, FileSystemBinaryManager>()
				.AddSingleton<ILocalUserDataService, LocalUserDataService>()
				;
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
			(var siteOk, string siteError) = RepairSite(app);
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/home/error");
			}

			if (!siteOk)
			{
				app.Run(async context => await context.Response.WriteAsync(siteError));
				return;
			}
			app.UseStaticFiles();
			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, _binariesPath.TrimStart('/'))),
				RequestPath = _binariesPath
			});
			app.UseAuthentication();
			app.Use(PopulateThreadCurrentPrincipalForMvc);
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
		/// <summary>
		/// BlogDataService and DayEntry rely on the thread's CurrentPrincipal and its role to determine if users
		/// should be allowed edit and add posts.
		/// Unfortunately the asp.net team no longer favour an approach involving the current thread so
		/// much as I am loath to stick values on globalish type stuff going up and down the stack
		/// this is a light touch way of including the functionality and actually looks fairly safe.
		/// Hopefully, in the fullness of time we will beautify the legacy code and this can go.
		/// </summary>
		/// <param name="context">provides the user data</param>
		/// <param name="next">standdard middleware - in this case MVC iteelf</param>
		/// <returns></returns>
		private Task PopulateThreadCurrentPrincipalForMvc(HttpContext context, Func<Task> next)
		{
			IPrincipal existingThreadPrincipal = null;
			try
			{
				existingThreadPrincipal = Thread.CurrentPrincipal;
				Thread.CurrentPrincipal = context.User;
				var rtn = next();
				return rtn;
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
