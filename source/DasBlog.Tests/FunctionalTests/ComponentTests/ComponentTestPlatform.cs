using System;
using System.Collections.Generic;
using DasBlog.Core;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using DasBlog.Core.Services;
using DasBlog.Managers;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public class ComponentTestPlatform : TestSupportPlatform
	{
		private ILogger<ComponentTestPlatform> logger;

		/// <summary>
		/// returns a fully functional blog manager object pointed at the environment provided
		/// </summary>
		/// <param name="environment">e.g. "Vanilla"</param>
		/// <returns>ready for anything</returns>
		public BlogManager CreateBlogManager(IDasBlogSandbox sandbox)
		{
			var dasBlogSettings = CreateDasBlogSettings(sandbox);
			ILogger<BlogManager> logger = ServiceProvider.GetService<ILoggerFactory>().CreateLogger<BlogManager>();
			return new BlogManager(dasBlogSettings, logger);
		}

		public IDasBlogSandbox CreateSandbox(string environment)
		{
			return ServiceProvider.GetService<IDasBlogSandboxFactory>().CreateSandbox(ServiceProvider, environment);
		}

		public ITestDataProcessor CreateTestDataProcessor(IDasBlogSandbox sandbox)
		{
			return ServiceProvider.GetService<ITestDataProcesorFactory>().CreateTestDataProcessor(sandbox);
		}

		private IDasBlogSettings CreateDasBlogSettings(IDasBlogSandbox sandbox)
		{
			(var siteConfigAccessor, var metaTagsAccessor) = GetOptions(sandbox);
			var dasBlogSettings = new DasBlogSettings(
				new HostingEnvironmentTest(sandbox.TestEnvironmentPath)
				, siteConfigAccessor
				, metaTagsAccessor
				, new SiteSecurityConfig()
				, null);
			return dasBlogSettings;
		}
		/// <summary>
		/// I have no idea how you get hold of options outside of DI
		/// and no time to find out
		/// </summary>
		/// <param name="sandbox">provides file system details</param>
		/// <returns>two sets of options required by DasBlogSettings</returns>
		private (IOptions<SiteConfig> siteConfigAccessor, IOptions<MetaTags> metaTagsAccessor) 
		  GetOptions(IDasBlogSandbox sandbox)
		{
			var Configuration = GetDasBlogAppConfiguration(sandbox);
			var localServices = new ServiceCollection();
			localServices.Configure<TimeZoneProviderOptions>(Configuration);
			localServices.Configure<SiteConfig>(Configuration);
			localServices.Configure<MetaTags>(Configuration);
			localServices.AddSingleton <OptionsExpsoer>();
			IServiceProvider localServiceProvider = localServices.BuildServiceProvider();
			var optionsExposer = localServiceProvider.GetService<OptionsExpsoer>();
			return (optionsExposer.SiteConfigAccessor, optionsExposer.MetatagsAccessor);
		}

		private IConfiguration GetDasBlogAppConfiguration(IDasBlogSandbox sandbox)
		{
			var staging = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
			ConfigurationBuilder configBuilder = new ConfigurationBuilder();
			configBuilder
				.SetBasePath(sandbox.TestEnvironmentPath)
				.AddXmlFile(@"Config/site.config", optional: true, reloadOnChange: true)
				.AddXmlFile(@"Config/metaConfig.xml", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{staging}json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				;
			return configBuilder.Build();
		}
		protected override void InjectDependencies(IServiceCollection services)
		{

		}

		protected override void CompleteSetupLocal()
		{
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			logger = loggerFactory.CreateLogger<ComponentTestPlatform>();
		}

	}

	internal class OptionsExpsoer
	{
		public OptionsExpsoer(IOptions<SiteConfig> siteConfigAccessor, IOptions<MetaTags> metaTagsAccessor)
		{
			this.SiteConfigAccessor = siteConfigAccessor;
			this.MetatagsAccessor = metaTagsAccessor;
		}

		public IOptions<MetaTags> MetatagsAccessor { get; }

		public IOptions<SiteConfig> SiteConfigAccessor { get; }
	}

	internal class HostingEnvironmentTest : IHostingEnvironment
	{
		public HostingEnvironmentTest(string path)
		{
			EnvironmentName = "Development";
			ApplicationName = "DasBlog.Web.UI";
			WebRootPath = path; ;
			WebRootFileProvider = null;
			ContentRootPath = path;
			ContentRootFileProvider = null;
		}

		public string EnvironmentName { get; set; }
		public string ApplicationName { get; set; }
		public string WebRootPath { get; set; }
		public IFileProvider WebRootFileProvider { get; set; }
		public string ContentRootPath { get; set; }
		public IFileProvider ContentRootFileProvider { get; set; }
	}

	internal class SiteSecurityConfig : ISiteSecurityConfig
	{
		public List<User> Users { get; set; }
		public void Refresh()
		{
			throw new NotImplementedException();
		}
	}
}
