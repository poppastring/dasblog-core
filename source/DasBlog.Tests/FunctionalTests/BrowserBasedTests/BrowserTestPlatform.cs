using System;
using System.IO;
using DasBlog.Tests.Automation.Dom;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DasBlog.Tests.Support.Common;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	public class BrowserTestPlatform : TestSupportPlatform
	{
		public IWebServerRunner Runner { get; private set; }
		public IBrowser Browser { get; private set; }
		public IPublisher Publisher { get; private set; }
		public ITestExecutor TestExecutor { get; private set; }
		public Pages Pages { get; private set; }

		/// <summary>
		/// completes the platform setup after the logger has been created although
		/// it's not clear that the instantiation has to take place after the logger has been configured.
		/// </summary>
		protected override void CompleteSetupLocal()
		{
			Runner = ServiceProvider.GetService<IWebServerRunner>();
			Browser = ServiceProvider.GetService<IBrowser>();
			TestExecutor = ServiceProvider.GetService<ITestExecutor>();
			Publisher = ServiceProvider.GetService<IPublisher>();
			Pages = new Pages(Browser);
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
				_ = e;
				// cannot do any logging here.  xunit logger throws an exception complaining that there
				// IDisposable no active test.
				// At the same time it refuses to let the console or debug logger work.
				throw;
			}
		}
		protected override string AppSettingsPathRelativeToProject { get; set; } =
			Constants.BrowserBasedTestsRelativeToProject;
	}
}
