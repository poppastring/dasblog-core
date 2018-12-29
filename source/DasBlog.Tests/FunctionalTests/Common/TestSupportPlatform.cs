using System;
using System.IO;
using DasBlog.Core.Common;
using DasBlog.SmokeTest;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Constants = DasBlog.Tests.Support.Common.Constants;
using SupportUtils = DasBlog.Tests.Support.Common.Utils;

namespace DasBlog.Tests.FunctionalTests.Common
{
	/// <summary>
	/// Hypothesis on order of object creation:
	/// 1. XUnit discovers test e.g. PrototypeBrowserBasedTests
	/// 2. XUnit inspects constructor and discovers requirement for BrowserBasedTestPlatform
	/// 3. CLR instantiates base class, TestSupportPlatform
	/// 4.   TestSupportPlatform constructor executes InjectDependenciesAll
	/// 5.   TestSupportPlatform.InjectDependenciesAll registers, ScriptRunner, VersionedFileSystem and other common dependencies
	/// 6.   TestSupportPlatform.InjectDependenciesAll registers logger (with no provider)
	/// 7.   TestSupportPlatform.InjectDependenciesAll calls overriden method InjectDependencies on derived platform (which should have been created somewhere above)
	/// 8.     DerivedPlatform.InjectDependencies registers specialised dependencies (e.g. Browser)
	/// 9.   TestSupportPlatform.InjectDependenciesAll calls Services.BuildServiceProvider
	/// 10.XUnit further inspects constructor and discovers requirement for ITestOutputHelper
	/// 11.XUnit instantiates its implementation of ITestOutputHelper
	/// 12.CLR instantiates test passing the newly instantiated ITestHelper and TestSupportPlatform
	/// 13.test constructor calls CompleteSetup on the TestSupportPlatform.
	/// 14.TestSupportPlatform.CompleteSetup is executed with ITestOutputHelper as a parameter
	/// 15.TestSupportPlatform.CompleteSetup uses the its ServiceProvider to get the ILoggerFactory and adds a provider
	/// 	based on ITestOutputHelper
	/// 16.TestSupportPlatform.CompleteSetup calls DerivedPlatform.CompleteSetupLocal
	/// 17.DerivedPlatform.CompleteSetupLocal uses ServiceProvider to instantiated specialised objects such as Browser
	/// good to go.
	/// </summary>
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
						Path.Combine(SupportUtils.GetProjectRootDirectory(),Constants.ScriptsRelativePath);
					opts.ScriptTimeout = Constants.DefaultScriptTimeout;
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptTimeout),
						out var envScriptTimeout))
					{
						opts.ScriptTimeout = envScriptTimeout;
					}
					opts.ScriptExitTimeout = Constants.DefaultScriptExitDelay;
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptExitDelay),
						out var envScriptExitTimeout))
					{
						opts.ScriptExitTimeout = envScriptExitTimeout;
					}
				});
			var repoPathEnvVar = Environment.GetEnvironmentVariable(Constants.DasBlogGitRepo);
			services.Configure<GitVersionedFileServiceOptions>(
				opts =>
				{
					opts.GitRepoDirectory = SupportUtils.GetProjectRootDirectory();
					opts.TestDataDirectroy = Constants.TestDataDirectory;
				});
			if (new DasBlog.Core.Common.StdOSDetector().DetectOS() == Os.Windows)
			{
				services.AddSingleton<IScriptPlatform, CmdScriptPlatform>();
			}
			else
			{
				services.AddSingleton<IScriptPlatform, BashScriptPlatform>();				
			}
			services.AddSingleton<IScriptRunner, ScriptRunner>();
			services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
			services.AddSingleton<IDasBlogSandboxFactory, DasBlogSandboxFactory>();
			services.AddSingleton<ITestDataProcesorFactory, TestDataProcesorFactory>();
			ConfigurationBuilder configBuilder = new ConfigurationBuilder();
			configBuilder.AddJsonFile(
				Path.Combine(SupportUtils.GetProjectRootDirectory(), Constants.FunctionalTestsRelativeToProject, "appsettings.json")
				, optional: false, reloadOnChange: false);
			// the derived platform app settings will have precedence over the more general ones
			// by dint of being added to the configuration later
			configBuilder.AddJsonFile(
				Path.Combine(SupportUtils.GetProjectRootDirectory(), AppSettingsPathRelativeToProject, "appsettings.json")
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
