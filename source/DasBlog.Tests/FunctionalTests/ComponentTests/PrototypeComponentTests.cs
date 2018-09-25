using System;
using System.IO;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.SmokeTest;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
/*
 * THERE IS NO logging to the console - in the debugger the log appears in the detail of the test results
 * when run from the console the log appears in a log file assuming you provide the correct command line
 * dotnet xunit cli is required to get console output
 * If I do "dotnet xunit -diagnostics" it barfs with a reference to a missing Microsoft.Extensions.Options
 */
namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public class PrototypeComponentTests : IClassFixture<ComponentTestPlatform>
	{

		private ComponentTestPlatform platform;
		private ITestOutputHelper testOutputHelper;
		private ILogger<PrototypeComponentTests> logger;
		private IVersionedFileService versionedFileService;
		private IDasBlogInstallation dasBlogInstallation;
		public PrototypeComponentTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
			testOutputHelper.WriteLine("hello from component constructor");
					// this and others like it appear in the detail pane of Rider's test runner for
					// Running a successful test
					// Debugging a successful test
					// Running a failed test
					// Debugging a failed test
					// logger under for Run and Debug
			// I want to get hold of the xunit test results when running from the command line.
			// "dotnet test" isn't providing them as far as I can see from my limited investigations
			// Even after specifying RuntimeFramework as 2.1.4 in the csproj I got
			// "dotnet xunit" should be more fruitful but fails with the following issue:
			//   Could not load file or assembly 'Microsoft.Extensions.Options, Version=2.1.0.0,
			// There is a nuget package for M.E.O 2.1.1 but the install fails and is rolled back
			// There are no other v2 nuget packages except a preview 2.2.
			// "dotnet xunit" seems to be a no-go - did not investigate whether it solved
			// the original problem of locating and using test results.
			// 
			// It turns out that the following is not a bad start:
			// "dotnet test --logger trx;LogfileName=test_results.xml --results-directory ./test_results"
			//			test_results.xml will appear in <proj>/source/DasBlog.Tests/FunctionalTests/test_results
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
			this.testOutputHelper = testOutputHelper;
			this.logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PrototypeComponentTests>();
		}

	}

	public class ComponentTestPlatform : IDisposable
	{
		public IServiceProvider ServiceProvider { get; private set; }
		private ILogger<ComponentTestPlatform> logger;
		private bool init;

		public ComponentTestPlatform()
		{
			ServiceProvider = InjectDependencies();
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			logger = loggerFactory.CreateLogger<ComponentTestPlatform>();
		}

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
				var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
				loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
				var dasBlogInstallation = this.ServiceProvider.GetService<IDasBlogInstallation>();
				dasBlogInstallation.Init();
				init = true;
			}
		}
		private IServiceProvider InjectDependencies()
		{
			var services = new ServiceCollection();
			services.Configure<DasBlogInstallationOptions>(
				opts => opts.ContentRootPath =
					"c/projects/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla");
			var repoPathEnvVar = Environment.GetEnvironmentVariable(Constants.DasBlogGitRepo);
			string repoPath;
			if (string.IsNullOrWhiteSpace(repoPathEnvVar))
			{
				repoPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "../../../../../../"));
			}
			else
			{
				repoPath = repoPathEnvVar;
			}
			services.Configure<GitVersionedFileServiceOptions>(
				opts => opts.GitRepo = repoPath);
			services.AddLogging();
			services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
			services.AddSingleton<IDasBlogInstallation, DasBlogInstallation>();
			return services.BuildServiceProvider();
		}

		public void Dispose()
		{
		}
	}

	// from https://stackoverflow.com/questions/46169169/net-core-2-0-configurelogging-xunit-test
	public class XunitLoggerProvider : ILoggerProvider
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		public ILogger CreateLogger(string categoryName)
			=> new XunitLogger(_testOutputHelper, categoryName);

		public void Dispose()
		{ }
	}

	public class XunitLogger : ILogger
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly string _categoryName;

		public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
		{
			_testOutputHelper = testOutputHelper;
			_categoryName = categoryName;
		}

		public IDisposable BeginScope<TState>(TState state)
			=> NoopDisposable.Instance;

		public bool IsEnabled(LogLevel logLevel)
			=> true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			_testOutputHelper.WriteLine($"{_categoryName} [{eventId}] {formatter(state, exception)}");
			if (exception != null)
				_testOutputHelper.WriteLine(exception.ToString());
		}

		private class NoopDisposable : IDisposable
		{
			public static NoopDisposable Instance = new NoopDisposable();
			public void Dispose()
			{ }
		}
	}
}
