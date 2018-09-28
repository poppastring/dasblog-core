using System;
using System.IO;
using DasBlog.SmokeTest;
using DasBlog.Tests.FunctionalTests.BrowserBasedTests;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public abstract class TestSupportPlatform : IDisposable
	{
		protected IServiceCollection services = new ServiceCollection();
		public IServiceProvider ServiceProvider { get; private set; }
		protected ILogger<TestSupportPlatform> logger;
		private bool init;

		public TestSupportPlatform()
		{
			InjectDependenciesAll();
/*
			var loggerFactory = services.BuildServiceProvider()
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
*/
		}
		/// <summary>
		/// the derived class should call AddSingleton() and its peers on Services
		/// </summary>
		/// <param name="services">the one and only service collection
		///   loaded with logger, DasBlogSandBox, ScriptRunner and other common services</param>
		protected abstract void InjectDependencies(IServiceCollection services);

		protected abstract void CompleteSetupLocal();

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
				loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
				logger = loggerFactory.CreateLogger<TestSupportPlatform>();
				var dasBlogSandbox = this.ServiceProvider.GetService<IDasBlogSandbox>();
				dasBlogSandbox.Init();
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
			services.Configure<DasBlogISandboxOptions>(
				opts => opts.ContentRootPath =
					Path.Combine(Utils.GetProjectRootDirectory(), Constants.VanillaTestData));
			var repoPathEnvVar = Environment.GetEnvironmentVariable(Constants.DasBlogGitRepo);
			string repoPath;
			if (string.IsNullOrWhiteSpace(repoPathEnvVar))
			{
				repoPath = Utils.GetProjectRootDirectory();
			}
			else
			{
				repoPath = repoPathEnvVar;
			}
			services.Configure<GitVersionedFileServiceOptions>(
				opts => opts.GitRepo = repoPath);
			services.AddLogging();
			services.AddSingleton<IScriptRunner, ScriptRunner>();
			services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
			services.AddSingleton<IDasBlogSandbox, DasBlogSandbox>();
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
