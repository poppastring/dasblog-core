using DasBlog.Core;
using DasBlog.Core.Security;
using DasBlog.Managers;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using System;
using System.Collections.Generic;
using System.IO;

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
			IOptions<BlogManagerOptions> optsAccessor = BuildBlogManagerOptions(sandbox);
			var modifiableOptsAccessor = BuildBlogManagerModifiableOptions(sandbox);
			var bm = new BlogManager(logger, dasBlogSettings);
			var cacheFixer = ServiceProvider.GetService<ICacheFixer>();
			cacheFixer.InvalidateCache(bm);
			return bm;
		}

		private IOptions<BlogManagerOptions> BuildBlogManagerOptions(IDasBlogSandbox sandbox)
		{
			var configuration = GetDasBlogAppConfiguration(sandbox);
			return new OptionsWrapper<BlogManagerOptions>(
			  new OptionsBuilder<BlogManagerOptions>().Build(configuration));
		}
		private IOptionsMonitor<BlogManagerModifiableOptions> BuildBlogManagerModifiableOptions(IDasBlogSandbox sandbox)
		{
			var configuration = GetDasBlogAppConfiguration(sandbox);
			return new FakeOptionsMonitor<BlogManagerModifiableOptions>(
			  new OptionsBuilder<BlogManagerModifiableOptions>().Build(configuration));
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
		private (IOptions<SiteConfig> siteConfigAccessor, IOptions<MetaTags> metaTagsAccessor) 
		  GetOptions(IDasBlogSandbox sandbox)
		{
			var configuration = GetDasBlogAppConfiguration(sandbox);
			return (new OptionsWrapper<SiteConfig>(new OptionsBuilder<SiteConfig>().Build(configuration))
				, new OptionsWrapper<MetaTags>(new OptionsBuilder<MetaTags>().Build(configuration)));
		}

		private IConfiguration GetDasBlogAppConfiguration(IDasBlogSandbox sandbox)
		{
			var staging = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
			ConfigurationBuilder configBuilder = new ConfigurationBuilder();
			configBuilder
				.SetBasePath(sandbox.TestEnvironmentPath)
				.AddXmlFile(@"Config/site.config", optional: true, reloadOnChange: true)
				.AddXmlFile(@"Config/metaConfig.xml", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables()
				;
			return configBuilder.Build();
		}
		protected override void InjectDependencies(IServiceCollection services)
		{
			services.AddSingleton<ICacheFixer, CacheFixer>();
		}

		protected override void CompleteSetupLocal()
		{
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			logger = loggerFactory.CreateLogger<ComponentTestPlatform>();
		}
		protected override string AppSettingsPathRelativeToProject { get; set; } =
			Constants.ComponentTestsRelativeToProject;

	}

	/// <summary>
	/// This builds the options instance and configures it from the configuration passed into Build()
	/// I have no idea how you get hold of options outside of DI
	/// and no time to find out
	/// </summary>
	internal class OptionsBuilder<TOptions> where TOptions : class, new()
	{
		private class OptionsExpsoer<U>  where U : class, new()
		{
			public OptionsExpsoer(IOptions<U> accessor)
			{
				this.Accessor = accessor;
			}

			public IOptions<U> Accessor { get; }
		}
		public TOptions Build(IConfiguration configuration)
		{
			var opts = new TOptions();
			configuration.Bind(opts);
			configuration.GetSection("PingServices").Bind(opts);
			return opts;
/*
			var localServices = new ServiceCollection();
			localServices.Configure<TOptions>(configuration);
			localServices.AddSingleton<OptionsExpsoer<TOptions>>();
			IServiceProvider localServiceProvider = localServices.BuildServiceProvider();
			return localServiceProvider.GetService<OptionsExpsoer<TOptions>>().Accessor;
*/
		}
	}

	internal class HostingEnvironmentTest : IWebHostEnvironment
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
