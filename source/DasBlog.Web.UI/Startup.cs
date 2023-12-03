using AutoMapper;
using Coravel;
using DasBlog.Core.Common;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using DasBlog.Services.Scheduler;
using DasBlog.Services.Site;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using DasBlog.Web.Settings;
using DasBlog.Web.Mappers;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using DasBlog.Web.TagHelpers.RichEdit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using reCAPTCHA.AspNetCore;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Net.Http.Headers;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DasBlog.Web
{
	public class Startup
	{
		private readonly string SiteSecurityConfigPath;
		private readonly string IISUrlRewriteConfigPath;
		private readonly string SiteConfigPath;
		private readonly string MetaConfigPath;
		private readonly string ThemeFolderPath;
		private readonly string LogFolderPath;
		private readonly string BinariesPath;
		private readonly string BinariesUrlRelativePath;
		private readonly string OEmbedProvidersPath;

		private readonly string DefaultSiteConfigPath;
		private readonly string DefaultMetaConfigPath;
		private readonly string DefaultOEmbedProvidersConfigPath;
		private readonly string DefaultSiteSecurityConfigPath;
		private readonly string DefaultIISUrlRewriteConfigPath;

		private readonly IWebHostEnvironment hostingEnvironment;
		

		public IConfiguration Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			hostingEnvironment = env;

			SiteSecurityConfigPath = Path.Combine("Config", $"siteSecurity.{env.EnvironmentName}.config");
			DefaultSiteSecurityConfigPath = Path.Combine("Config", "siteSecurity.config");
			IISUrlRewriteConfigPath = Path.Combine("Config", $"IISUrlRewrite.{env.EnvironmentName}.config");
			DefaultIISUrlRewriteConfigPath = Path.Combine("Config", "IISUrlRewrite.config");

			SiteConfigPath = Path.Combine("Config", $"site.{env.EnvironmentName}.config");
			DefaultSiteConfigPath = Path.Combine("Config", $"site.config");
			MetaConfigPath = Path.Combine("Config", $"meta.{env.EnvironmentName}.config");
			DefaultMetaConfigPath = Path.Combine("Config", $"meta.config");
			OEmbedProvidersPath = DefaultOEmbedProvidersConfigPath = Path.Combine("Config", $"oembed-providers.json");

			ConfigFileInitializationPrep();

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddXmlFile(DefaultSiteConfigPath, optional: false, reloadOnChange: true)
				.AddXmlFile(SiteConfigPath, optional: true, reloadOnChange: true)
				.AddXmlFile(DefaultMetaConfigPath, optional: false, reloadOnChange: true)
				.AddXmlFile(MetaConfigPath, optional: true, reloadOnChange: true)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddJsonFile(DefaultOEmbedProvidersConfigPath, optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();

			BinariesPath = new DirectoryInfo(Path.Combine(env.ContentRootPath, Configuration.GetValue<string>("BinariesDir"))).FullName;
			ThemeFolderPath = new DirectoryInfo(Path.Combine(env.ContentRootPath, "Themes", Configuration.GetSection("Theme").Value)).FullName;
			LogFolderPath = new DirectoryInfo(Path.Combine(env.ContentRootPath, Configuration.GetSection("LogDir").Value)).FullName;
			BinariesUrlRelativePath = "content/binary";
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddApplicationInsightsTelemetry();

			services.AddLogging(builder =>
			{
				builder.AddFile(opts => opts.LogDirectory = LogFolderPath);
			});

			services.AddOptions();
			services.AddHealthChecks().AddCheck<DasBlogHealthChecks>("health_check");
			services.AddMemoryCache();

			services.Configure<TimeZoneProviderOptions>(Configuration);
			services.Configure<SiteConfig>(Configuration);
			services.Configure<MetaTags>(Configuration);
			services.Configure<OEmbedProviders>(Configuration);
			services.AddSingleton<AppVersionInfo>();

			services.Configure<ConfigFilePathsDataOption>(options =>
			{
				options.SiteConfigFilePath = Path.Combine(hostingEnvironment.ContentRootPath, SiteConfigPath);
				options.MetaConfigFilePath = Path.Combine(hostingEnvironment.ContentRootPath, MetaConfigPath);
				options.OEmbedProvidersFilePath = Path.Combine(hostingEnvironment.ContentRootPath, OEmbedProvidersPath);
				options.SecurityConfigFilePath = Path.Combine(hostingEnvironment.ContentRootPath, SiteSecurityConfigPath);
				options.IISUrlRewriteFilePath = Path.Combine(hostingEnvironment.ContentRootPath, IISUrlRewriteConfigPath);
				options.ThemesFolder = ThemeFolderPath;
				options.BinaryFolder = BinariesPath;
				options.BinaryUrlRelative = string.Format("{0}/", BinariesUrlRelativePath);
			});

			services.Configure<ActivityRepoOptions>(options
			  => options.Path = LogFolderPath);

			//Important if you're using Azure, hosting on Nginx, or behind any reverse proxy
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.All;
				options.AllowedHosts = Configuration.GetValue<string>("AllowedHosts")?.Split(';').ToList<string>();
			});

			// Add identity types
			services
				.AddIdentity<DasBlogUser, DasBlogRole>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(Configuration.GetSection("IdentityOptions"));

			services.ConfigureApplicationCookie(options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromSeconds(10000);
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
					.AddCookie(options =>
					{
						options.LoginPath = "/account/login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
						options.LogoutPath = "/account/logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
						options.AccessDeniedPath = "/account/accessdenied"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
						options.SlidingExpiration = true;
						options.Cookie = new CookieBuilder
						{
							HttpOnly = true
						};
					});

			services.AddResponseCaching();

			services.Configure<RazorViewEngineOptions>(rveo =>
			{
				rveo.ViewLocationExpanders.Add(new DasBlogLocationExpander(Configuration.GetSection("Theme").Value));
			});

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromSeconds(1000);
			});

			services
				.AddHttpContextAccessor();

			services.AddScheduler();
			services.AddTransient<SiteEmailReport>();

			services
				.AddTransient<IDasBlogSettings, DasBlogSettings>()
				.AddTransient<IUserStore<DasBlogUser>, DasBlogUserStore>()
				.AddTransient<IRoleStore<DasBlogRole>, DasBlogUserRoleStore>()
				.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User)
				.AddTransient<ISiteRepairer, SiteRepairer>();

			services.AddScoped<IRichEditBuilder>(SelectRichEditor)
				.AddScoped<IBlogPostViewModelCreator, BlogPostViewModelCreator>();

			services
				.AddSingleton(hostingEnvironment.ContentRootFileProvider)
				.AddSingleton<IBlogManager, BlogManager>()
				.AddSingleton<IArchiveManager, ArchiveManager>()
				.AddSingleton<ICategoryManager, CategoryManager>()
				.AddSingleton<ISiteSecurityManager, SiteSecurityManager>()
				.AddSingleton<IXmlRpcManager, XmlRpcManager>()
				.AddSingleton<ISiteManager, SiteManager>()
				.AddSingleton<IActivityPubManager, ActivityPubManager>()
				.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
				.AddSingleton<SiteHttpContext>()
				.AddSingleton<IFileSystemBinaryManager, FileSystemBinaryManager>()
				.AddSingleton<IUserDataRepo, UserDataRepo>()
				.AddSingleton<ISiteSecurityConfig, SiteSecurityConfig>()
				.AddSingleton<IUserService, UserService>()
				.AddSingleton<IActivityService, ActivityService>()
				.AddSingleton<IActivityRepoFactory, ActivityRepoFactory>()
				.AddSingleton<IEventLineParser, EventLineParser>()
				.AddSingleton<ITimeZoneProvider, TimeZoneProvider>()
				.AddSingleton<ISubscriptionManager, SubscriptionManager>()
				.AddSingleton<IConfigFileService<MetaTags>, MetaConfigFileService>()
				.AddSingleton<IConfigFileService<OEmbedProviders>, OEmbedProvidersFileService>()
				.AddSingleton<IConfigFileService<SiteConfig>, SiteConfigFileService>()
				.AddSingleton<IConfigFileService<SiteSecurityConfigData>, SiteSecurityConfigFileService>();

			services.AddSingleton<IExternalEmbeddingHandler, ExternalEmbeddingHandler>();


			services
				.AddAutoMapper((serviceProvider, mapperConfig) =>
				{
					mapperConfig.AddProfile(new ProfilePost(serviceProvider.GetService<IDasBlogSettings>()));
					mapperConfig.AddProfile(new ProfileDasBlogUser(serviceProvider.GetService<ISiteSecurityManager>()));
					mapperConfig.AddProfile(new ProfileSettings());
					mapperConfig.AddProfile(new ProfileActivityPub());
					mapperConfig.AddProfile(new ProfileStaticPage());
				})
				.AddMvc()
				.AddXmlSerializerFormatters();

			services
				.AddControllersWithViews()
				.AddRazorRuntimeCompilation();

			services.AddRecaptcha(options =>
			{
				options.SiteKey = Configuration.GetSection("RecaptchaSiteKey").Value;
				options.SecretKey = Configuration.GetSection("RecaptchaSecretKey").Value;
			});

			services.Configure<CookiePolicyOptions>(options =>
			{
				bool.TryParse(Configuration.GetSection("CookieConsentEnabled").Value, out var flag);

				options.CheckConsentNeeded = context => flag;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDasBlogSettings dasBlogSettings)
		{
			(var siteOk, var siteError) = RepairSite(app);

			if (env.IsDevelopment() || env.IsStaging())
			{
				app.UseDeveloperExceptionPage();
				//app.UseBrowserLink();
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

			var options = new RewriteOptions()
				 .AddIISUrlRewrite(env.ContentRootFileProvider, IISUrlRewriteConfigPath);

			app.UseRewriter(options);
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

			app.UseStaticFiles();
			app.UseCookiePolicy();

			Action<StaticFileResponseContext> cacheControlPrepResponse = (ctx) =>
			{
				const int durationInSeconds = 60 * 60 * 24;
				ctx.Context.Response.Headers[HeaderNames.CacheControl] =
					"public,max-age=" + durationInSeconds;
				ctx.Context.Response.Headers["Expires"] = DateTime.UtcNow.AddHours(12).ToString("R");
			};

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(BinariesPath),
				RequestPath = string.Format("/{0}", BinariesUrlRelativePath),
				OnPrepareResponse = cacheControlPrepResponse
			});

			app.UseStaticFiles(new StaticFileOptions()
			{
				FileProvider = new PhysicalFileProvider(BinariesPath),
				RequestPath = string.Format("/{0}", BinariesUrlRelativePath),
				OnPrepareResponse = cacheControlPrepResponse
			});


			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "content/radioStories")),
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

			app.UseAuthentication();
			app.Use(PopulateThreadCurrentPrincipalForMvc);
			app.UseRouting();
			app.UseAuthorization();

			app.UseXContentTypeOptions();
			app.UseXXssProtection(options => options.EnabledWithBlockMode());
			app.UseXfo(options => options.SameOrigin());
			app.UseReferrerPolicy(opts => opts.NoReferrerWhenDowngrade());

			var SecurityScriptSources = Configuration.GetSection("SecurityScriptSources")?.Value?.Split(";");
			var SecurityStyleSources = Configuration.GetSection("SecurityStyleSources")?.Value?.Split(";");
			var DefaultSources = Configuration.GetSection("DefaultSources")?.Value?.Split(";");

			if (SecurityStyleSources != null && SecurityScriptSources != null && DefaultSources != null)
			{
				app.UseCsp(options => options
					.DefaultSources(s => s.Self()
						.CustomSources(DefaultSources)
						)
					.StyleSources(s => s.Self()
						.CustomSources(SecurityStyleSources)
						.UnsafeInline()
					)
					.ScriptSources(s => s.Self()
						   .CustomSources(SecurityScriptSources)
						.UnsafeInline()
						.UnsafeEval()
					)
				);
			}

			app.Use(async (context, next) =>
			{
				//being renamed/changed to this soon
				context.Response.Headers.Add("Permissions-Policy", "geolocation=(),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()");
				await next.Invoke();
			});

			app.UseLoggingAgent();

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

		private IRichEditBuilder SelectRichEditor(IServiceProvider serviceProvider)
		{
			var entryEditControl = serviceProvider.GetService<IDasBlogSettings>().SiteConfiguration.EntryEditControl.ToLower();
			IRichEditBuilder richEditBuilder;

			switch (entryEditControl)
			{
				case Constants.TinyMceEditor:
					richEditBuilder = new TinyMceBuilder(serviceProvider.GetService<IDasBlogSettings>());
					break;
				case Constants.NicEditEditor:
					richEditBuilder = new NicEditBuilder(serviceProvider.GetService<IDasBlogSettings>());
					break;
				case Constants.TextAreaEditor:
					richEditBuilder = new TextAreaBuilder();
					break;
				case Constants.FroalaEditor:
					richEditBuilder = new FroalaBuilder();
					break;
				default:
					throw new Exception($"Attempt to use unknown rich edit control, {entryEditControl}");
			}

			return richEditBuilder;
		}

		private void ConfigFileInitializationPrep()
		{
			if (!File.Exists(Path.Combine(hostingEnvironment.ContentRootPath, SiteSecurityConfigPath)))
			{
				File.Copy(Path.Combine(hostingEnvironment.ContentRootPath, DefaultSiteSecurityConfigPath), Path.Combine(hostingEnvironment.ContentRootPath, SiteSecurityConfigPath));
			}

			if (!File.Exists(Path.Combine(hostingEnvironment.ContentRootPath, IISUrlRewriteConfigPath)))
			{
				File.Copy(Path.Combine(hostingEnvironment.ContentRootPath, DefaultIISUrlRewriteConfigPath), Path.Combine(hostingEnvironment.ContentRootPath, IISUrlRewriteConfigPath));
			}
		}
	}
}
