using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DasBlog.Tests.Support.Common;
using DasBlog.SmokeTest;
using DasBlog.Tests.Automation.Dom;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.Options;
using Xunit.Abstractions;
using Constants = DasBlog.Tests.Support.Common.Constants;

/*
 * THERE IS NO logging to the console - in the debugger the log appears in the detail of the test results
 * when run from the console the log appears in a log file assuming you provide the correct command line
 * dotnet xunit cli is required to get console output
 * If I do "dotnet xunit -diagnostics" it barfs with a reference to a missing Microsoft.Extensions.Options
 */
namespace DasBlog.Tests.FunctionalTests
{
	public class BrowserOptionsAccessor : IOptions<BrowserOptions>
	{
		public BrowserOptionsAccessor(BrowserOptions opts)
		{
			Value = opts;
		}
		public BrowserOptions Value { get; }
	}
	public class PrototypeBrowserBasedTests : IClassFixture<BrowserTestPlatform>
	{

		private BrowserTestPlatform platform;
		private ITestOutputHelper testOutputHelper;
		private ILogger<PrototypeBrowserBasedTests> logger;
		private IVersionedFileService versionedFileService;
		private IDasBlogInstallation dasBlogInstallation;
		public PrototypeBrowserBasedTests(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
		{
			testOutputHelper.WriteLine("hello from browser constructor");
					// the above message and others like it appear in the detail pane of Rider's test runner for
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
			browserTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = browserTestPlatform;
			this.testOutputHelper = testOutputHelper;
			this.logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PrototypeBrowserBasedTests>();
		}

		[Fact(Skip="")]
		public void MinimalTest()
		{
//			Thread.Sleep(5000);
			try
			{
				logger.LogError("logging starts here");
				List<TestStep> testSteps = new List<TestStep>
				{
					new TestStep(() => platform.Pages.Login.Goto()),
					new TestStep(() => platform.Pages.Login.IsDisplayed()),
					new TestStep(() => platform.Pages.Login.LoginButton != null),
					new TestStep(() => platform.Pages.Login.LoginButton.Click()),
					new TestStep(() =>
						platform.Pages.Login.PasswordValidation.Text.ToLower().Contains("the password field is required")),
					new TestStep(() => platform.Pages.Login.IsDisplayed())
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				throw;
			}
			finally
			{
			}
		}
		[Fact(Skip="")]
		public void MinimalTest2()
		{
			Thread.Sleep(5000);
			try
			{
				logger.LogError("logging starts here");
				List<TestStep> testSteps = new List<TestStep>
				{
					new TestStep(() => platform.Pages.Login.Goto()),
					new TestStep(() => platform.Pages.Login.IsDisplayed()),
					new TestStep(() => platform.Pages.Login.LoginButton != null),
					new TestStep(() => platform.Pages.Login.LoginButton.Click()),
					new TestStep(() =>
						platform.Pages.Login.PasswordValidation.Text.ToLower().Contains("the password field is required")),
					new TestStep(() => platform.Pages.Login.IsDisplayed())
				};
				var results = new TestResults();
				platform.TestExecutor.Execute(testSteps, results);
				platform.Publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				throw;
			}
			finally
			{
			}
		}
	}

	public class BrowserTestPlatform : IDisposable
	{
		public IWebServerRunner Runner { get; private set; }
		public IServiceProvider ServiceProvider { get; private set; }
		public IBrowser Browser { get; private set; }
		public IPublisher Publisher { get; private set; }
		public ITestExecutor TestExecutor { get; private set; }
		public Pages Pages { get; private set; }
		private ILogger<BrowserTestPlatform> logger;
		private bool init;

		public BrowserTestPlatform()
		{
			ServiceProvider = InjectDependencies();
			Runner = ServiceProvider.GetService<IWebServerRunner>();
			Browser = ServiceProvider.GetService<IBrowser>();
			TestExecutor = ServiceProvider.GetService<ITestExecutor>();
			Publisher = ServiceProvider.GetService<IPublisher>();
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			logger = loggerFactory.CreateLogger<BrowserTestPlatform>();
			Pages = new Pages(Browser);
		}

		/// <summary>
		/// completes the platform setup by activating the logger
		/// TODO look into how the test output helper is supposed to be injected into class fixture
		/// </summary>
		/// <param name="testOutputHelper">did not get injected</param>
		public void CompleteSetup(ITestOutputHelper testOutputHelper)
		{
			if (!init)
			{
				var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
				loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
				var dasBlogInstallation = this.ServiceProvider.GetService<IDasBlogInstallation>();
				dasBlogInstallation.Init();
				this.Runner.RunDasBlog();
				this.Browser.Init();
				init = true;
			}
		}
		private IServiceProvider InjectDependencies()
		{
			var services = new ServiceCollection();
			services.Configure<BrowserOptions>(options =>
			{
				options.HomeUrl = "http://localhost:5000/";
				options.Driver = "firefox";
			});
			services.Configure<DasBlogInstallationOptions>(
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
			services.AddSingleton<IWebServerRunner, WebServerRunner>();
			services.AddSingleton<IBrowser, Browser>();
			services.AddSingleton<IPublisher, Publisher>();
			services.AddSingleton<ITestExecutor, TestExecutor>();
			services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
			services.AddSingleton<IDasBlogInstallation, DasBlogInstallation>();
			return services.BuildServiceProvider();
		}

		public void Dispose()
		{
			try
			{
				Runner?.Kill();
				Browser?.Dispose();
			}
			catch (Exception e)
			{
				// cannot do any logging here.  xunit logger throws an exception complaining that there
				// IDisposable no active test.
				// At the same time it refuses to let the console or debug logger work.
				throw;
			}
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
