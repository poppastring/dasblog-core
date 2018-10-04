using System;
using System.IO;
using DasBlog.SmokeTest;
using DasBlog.Tests.FunctionalTests.BrowserBasedTests;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public abstract class TestSupportPlatform : IDisposable
	{
		protected IServiceCollection services = new ServiceCollection();
		public IServiceProvider ServiceProvider { get; private set; }
		private ILogger<TestSupportPlatform> logger;
		private bool init;

		public TestSupportPlatform()
		{
			InjectDependenciesAll();
		}
		/// <summary>
		/// the derived class should call AddSingleton() and its peers on Services
		/// </summary>
		/// <param name="services">the one and only service collection
		///   loaded with logger, DasBlogSandBox, ScriptRunner and other common services</param>
		protected abstract void InjectDependencies(IServiceCollection services);

		protected abstract void CompleteSetupLocal();
		/// <summary>
		/// the path where the appsettings.json file is located for each derived platfrom
		/// e.g. "source/DasBlog.Tests/FunctionTests/ComponentTests"
		/// </summary>
		protected abstract string AppSettingsPathRelativeToProject { get; set; }

		/// <summary>
		/// completes the platform setup by activating the logger
		/// TODO look into how the test output helper is supposed to be injected into class fixture
		/// </summary>
		/// <param name="testOutputHelper">did not get injected into this object's constructor
		///   so we have to shoehorn it in here</param>
		public void CompleteSetup(ITestOutputHelper testOutputHelper)
		{
			if (!init)
			{
				init = true;
				var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
				loggerFactory.AddProvider(new BrowserBasedTests.XunitLoggerProvider(testOutputHelper));
				logger = loggerFactory.CreateLogger<TestSupportPlatform>();
				CompleteSetupLocal();
			}
		}
		private void InjectDependenciesAll()
		{
			services.Configure<ScriptRunnerOptions>(
				opts =>
				{
					opts.ScriptDirectory =
						Path.Combine(Utils.GetProjectRootDirectory(),Constants.ScriptsRelativePath);
					opts.ScriptTimeout = Constants.DefaultScriptTimeout;
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptTimeout),
						out var envScriptTimeout))
					{
						opts.ScriptTimeout = envScriptTimeout;
					}
					opts.ScriptExitTimeout = Constants.DefaultScriptExitTimeout;
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptExitTimeout),
						out var envScriptExitTimeout))
					{
						opts.ScriptExitTimeout = envScriptExitTimeout;
					}
				});
			var repoPathEnvVar = Environment.GetEnvironmentVariable(Constants.DasBlogGitRepo);
			services.Configure<GitVersionedFileServiceOptions>(
				opts =>
				{
					opts.GitRepoDirectory = Utils.GetProjectRootDirectory();
					opts.TestDataDirectroy = Constants.TestDataDirectory;
				});
			services.AddSingleton<IScriptRunner, ScriptRunner>();
			services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
			services.AddSingleton<IDasBlogSandboxFactory, DasBlogSandboxFactory>();
			services.AddSingleton<ITestDataProcesorFactory, TestDataProcesorFactory>();
			ConfigurationBuilder configBuilder = new ConfigurationBuilder();
			configBuilder.AddJsonFile(
				Path.Combine(Utils.GetProjectRootDirectory(), Constants.FunctionalTestsRelativeToProject, "appsettings.json")
				, optional: false, reloadOnChange: false);
			// the derived platform app settings will have precedence over the more general ones
			// by dint of being added to the configuration later
			configBuilder.AddJsonFile(
				Path.Combine(Utils.GetProjectRootDirectory(), AppSettingsPathRelativeToProject, "appsettings.json")
				, optional: false, reloadOnChange: false);
			IConfigurationRoot config = configBuilder.Build();
			services.Configure<ILoggerFactory>(config);
			services.AddLogging(
				opts =>
				{
					opts.AddConfiguration(config.GetSection("Logging"));
					opts.SetMinimumLevel(LogLevel.Information);
				});
			InjectDependencies(services);	// add in the derived class's dependencies
			ServiceProvider = services.BuildServiceProvider();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
