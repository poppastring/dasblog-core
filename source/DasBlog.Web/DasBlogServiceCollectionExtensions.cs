using AutoMapper;
using Coravel;
using DasBlog.Core.Common;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using DasBlog.Services.Scheduler;
using DasBlog.Services.Site;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using DasBlog.Web.Mappers;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using DasBlog.Web.Settings;
using DasBlog.Web.TagHelpers.RichEdit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using reCAPTCHA.AspNetCore;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace DasBlog.Web
{
	public static class DasBlogServiceCollectionExtensions
	{
		public static IServiceCollection AddDasBlogDataServices(this IServiceCollection services)
		{
			services
				.AddSingleton<ILoggingDataService>(sp =>
				{
					var paths = sp.GetRequiredService<IDasBlogPathResolver>();
					return LoggingDataServiceFactory.GetService(paths.LogFolderPath);
				})
				.AddSingleton<IBlogDataService>(sp =>
				{
					var paths = sp.GetRequiredService<IDasBlogPathResolver>();
					var loggingService = sp.GetRequiredService<ILoggingDataService>();
					return BlogDataServiceFactory.GetService(paths.ContentFolderPath, loggingService);
				});

			return services;
		}

		public static IServiceCollection AddDasBlogManagers(this IServiceCollection services)
		{
			services
				.AddSingleton<IBlogManager, BlogManager>()
				.AddSingleton<ICommentManager, CommentManager>()
				.AddSingleton<ISearchManager, SearchManager>()
				.AddSingleton<IArchiveManager, ArchiveManager>()
				.AddSingleton<ICategoryManager, CategoryManager>()
				.AddSingleton<ISiteSecurityManager, SiteSecurityManager>()
				.AddSingleton<IXmlRpcManager, XmlRpcManager>()
				.AddSingleton<ISiteManager, SiteManager>()
				.AddSingleton<IActivityPubManager, ActivityPubManager>()
				.AddSingleton<IFileSystemBinaryManager, FileSystemBinaryManager>()
				.AddSingleton<ISubscriptionManager, SubscriptionManager>();

			return services;
		}

		public static IServiceCollection AddDasBlogServices(this IServiceCollection services, IWebHostEnvironment env)
		{
			services
				.AddSingleton<IDasBlogSettings, DasBlogSettings>()
				.AddSingleton(env.ContentRootFileProvider)
				.AddSingleton<SiteHttpContext>()
				.AddSingleton<IUserDataRepo, UserDataRepo>()
				.AddSingleton<ISiteSecurityConfig, SiteSecurityConfig>()
				.AddSingleton<IUserService, UserService>()
				.AddSingleton<IActivityService, ActivityService>()
				.AddSingleton<IActivityRepoFactory, ActivityRepoFactory>()
				.AddSingleton<IEventLineParser, EventLineParser>()
				.AddSingleton<ITimeZoneProvider, TimeZoneProvider>()
				.AddSingleton<IConfigFileService<MetaTags>, MetaConfigFileService>()
				.AddSingleton<IConfigFileService<OEmbedProviders>, OEmbedProvidersFileService>()
				.AddSingleton<IConfigFileService<SiteConfig>, SiteConfigFileService>()
				.AddSingleton<IConfigFileService<SiteSecurityConfigData>, SiteSecurityConfigFileService>()
				.AddSingleton<IExternalEmbeddingHandler, ExternalEmbeddingHandler>();

			services				
				.AddTransient<IUserStore<DasBlogUser>, DasBlogUserStore>()
				.AddTransient<IRoleStore<DasBlogRole>, DasBlogUserRoleStore>()
				.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User)
				.AddTransient<ISiteRepairer, SiteRepairer>();

			return services;
		}

		public static IServiceCollection AddDasBlogConfiguration(this IServiceCollection services,
			IConfiguration configuration, IDasBlogPathResolver pathResolver)
		{
			services.AddApplicationInsightsTelemetry();

			services.AddLogging(builder =>
			{
				builder.AddFile(opts => opts.LogDirectory = pathResolver.LogFolderPath);
			});

			services.AddOptions();
			services.AddHealthChecks().AddCheck<DasBlogHealthChecks>("health_check");
			services.AddMemoryCache();

			services.Configure<TimeZoneProviderOptions>(configuration);
			services.Configure<SiteConfig>(configuration);
			services.Configure<MetaTags>(configuration);
			services.Configure<OEmbedProviders>(configuration);
			services.AddSingleton<AppVersionInfo>();

			services.AddSingleton<IDasBlogPathResolver>(pathResolver);

			services.Configure<ConfigFilePathsDataOption>(options =>
			{
				options.SiteConfigFilePath = pathResolver.SiteConfigFilePath;
				options.MetaConfigFilePath = pathResolver.MetaConfigFilePath;
				options.OEmbedProvidersFilePath = pathResolver.OEmbedProvidersFilePath;
				options.SecurityConfigFilePath = pathResolver.SecurityConfigFilePath;
				options.IISUrlRewriteFilePath = pathResolver.IISUrlRewriteFilePath;
				options.ThemesFolder = pathResolver.ThemeFolderPath;
				options.BinaryFolder = pathResolver.BinariesPath;
				options.BinaryUrlRelative = $"{pathResolver.BinariesUrlRelativePath}/";
			});

			services.Configure<ActivityRepoOptions>(options
			  => options.Path = pathResolver.LogFolderPath);

			//Important if you're using Azure, hosting on Nginx, or behind any reverse proxy
			services.Configure<ForwardedHeadersOptions>(options =>
			{
				options.ForwardedHeaders = ForwardedHeaders.All;
				options.AllowedHosts = configuration.GetValue<string>("AllowedHosts")?.Split(';').ToList<string>();
			});

			return services;
		}

		public static IServiceCollection AddDasBlogSecurity(this IServiceCollection services, IConfiguration configuration)
		{
			services
				.AddIdentity<DasBlogUser, DasBlogRole>()
				.AddDefaultTokenProviders();

			services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));

			services.ConfigureApplicationCookie(options =>
			{
				options.ExpireTimeSpan = TimeSpan.FromSeconds(10000);
			});

			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
					.AddCookie(options =>
					{
						options.LoginPath = "/account/login";
						options.LogoutPath = "/account/logout";
						options.AccessDeniedPath = "/account/accessdenied";
						options.SlidingExpiration = true;
						options.Cookie = new CookieBuilder
						{
							HttpOnly = true
						};
					});

			services.Configure<CookiePolicyOptions>(options =>
			{
				bool.TryParse(configuration.GetSection("CookieConsentEnabled").Value, out var flag);

				options.CheckConsentNeeded = context => flag;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

			services.AddRecaptcha(options =>
			{
				options.SiteKey = configuration.GetSection("RecaptchaSiteKey").Value;
				options.SecretKey = configuration.GetSection("RecaptchaSecretKey").Value;
			});

			return services;
		}

		public static IServiceCollection AddDasBlogWebServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddResponseCaching();

			services.Configure<RazorViewEngineOptions>(rveo =>
			{
				rveo.ViewLocationExpanders.Add(new DasBlogLocationExpander(configuration.GetSection("Theme").Value));
			});

			services.AddSession(options =>
			{
				options.IdleTimeout = TimeSpan.FromSeconds(1000);
			});

			services.AddHttpContextAccessor();

			services.AddScheduler();
			services.AddTransient<SiteEmailReport>();

			services.AddScoped<IRichEditBuilder>(SelectRichEditor)
				.AddScoped<IBlogPostViewModelCreator, BlogPostViewModelCreator>();

			services
				.AddAutoMapper((serviceProvider, mapperConfig) =>
				{
					mapperConfig.AddProfile(new ProfilePost(serviceProvider.GetService<IDasBlogSettings>()));
					mapperConfig.AddProfile(new ProfileDasBlogUser(serviceProvider.GetService<ISiteSecurityManager>()));
					mapperConfig.AddProfile(new ProfileSettings());
					mapperConfig.AddProfile(new ProfileActivityPub());
					mapperConfig.AddProfile(new ProfileStaticPage());
				}, Array.Empty<Assembly>())
				.AddMvc()
				.AddXmlSerializerFormatters();

			services
				.AddControllersWithViews()
				.AddRazorRuntimeCompilation();

			return services;
		}

		private static IRichEditBuilder SelectRichEditor(IServiceProvider serviceProvider)
		{
			var entryEditControl = serviceProvider.GetService<IDasBlogSettings>().SiteConfiguration.EntryEditControl.ToLower();

			return entryEditControl switch
			{
				Constants.TinyMceEditor => new TinyMceBuilder(serviceProvider.GetService<IDasBlogSettings>()),
				Constants.NicEditEditor => new NicEditBuilder(serviceProvider.GetService<IDasBlogSettings>()),
				Constants.TextAreaEditor => new TextAreaBuilder(),
				Constants.FroalaEditor => new FroalaBuilder(),
				_ => throw new Exception($"Attempt to use unknown rich edit control, {entryEditControl}")
			};
		}
	}
}
