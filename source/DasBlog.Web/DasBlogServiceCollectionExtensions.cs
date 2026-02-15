using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using DasBlog.Services.Site;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using newtelligence.DasBlog.Runtime;
using System.IO;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web
{
	public static class DasBlogServiceCollectionExtensions
	{
		public static IServiceCollection AddDasBlogDataServices(this IServiceCollection services)
		{
			services
				.AddSingleton<ILoggingDataService>(sp =>
				{
					var settings = sp.GetRequiredService<IDasBlogSettings>();
					return LoggingDataServiceFactory.GetService(
						Path.Combine(settings.WebRootDirectory, settings.SiteConfiguration.LogDir));
				})
				.AddSingleton<IBlogDataService>(sp =>
				{
					var settings = sp.GetRequiredService<IDasBlogSettings>();
					var loggingService = sp.GetRequiredService<ILoggingDataService>();
					return BlogDataServiceFactory.GetService(
						Path.Combine(settings.WebRootDirectory, settings.SiteConfiguration.ContentDir), loggingService);
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
	}
}
