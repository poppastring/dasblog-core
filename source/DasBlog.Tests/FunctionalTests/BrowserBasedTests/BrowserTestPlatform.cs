using System;
using System.IO;
using DasBlog.Core.Extensions;
using DasBlog.SmokeTest;
using DasBlog.Tests.Automation.Dom;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.FunctionalTests.TestInfrastructureTests;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	public class BrowserTestPlatform : TestSupportPlatform
	{
		public IWebServerRunner Runner { get; private set; }
//		public IServiceProvider ServiceProvider { get; private set; }
		public IBrowser Browser { get; private set; }
		public IPublisher Publisher { get; private set; }
		public ITestExecutor TestExecutor { get; private set; }
		public Pages Pages { get; private set; }
//		private ILogger<BrowserTestPlatform> logger;
		private bool init;

		/// <summary>
		/// completes the platform setup by activating the logger
		/// TODO look into how the test output helper is supposed to be injected into class fixture
		/// </summary>
		/// <param name="testOutputHelper">did not get injected</param>
		protected override void CompleteSetupLocal(ITestOutputHelper testOutputHelper)
		{
//			var loggerFactory = ServiceProvider
//				.GetService<ILoggerFactory>();
/*
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
*/
//			logger = loggerFactory.CreateLogger<BrowserTestPlatform>();
			Runner = ServiceProvider.GetService<IWebServerRunner>();
			Browser = ServiceProvider.GetService<IBrowser>();
			TestExecutor = ServiceProvider.GetService<ITestExecutor>();
			Publisher = ServiceProvider.GetService<IPublisher>();
			Pages = new Pages(Browser);
			logger.LogInformation("good to go");
			//var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
			this.Runner.RunDasBlog();
			this.Browser.Init();
		}
		protected override void InjectDependencies(IServiceCollection services)
		{
			services.Configure<BrowserOptions>(options =>
			{
				options.HomeUrl = "http://localhost:5000/";
				options.Driver = "firefox";
			});
			services.AddSingleton<IWebServerRunner, WebServerRunner>();
			services.AddSingleton<IBrowser, Browser>();
			services.AddSingleton<IPublisher, Publisher>();
			services.AddSingleton<ITestExecutor, TestExecutor>();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					Runner?.Kill();
					Browser?.Dispose();
				}
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
}
