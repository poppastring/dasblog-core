using System;
using System.Collections.Generic;
using System.Threading;
using DasBlog.Tests.Automation.Dom;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.FunctionalTests.IntegrationTests.Support;
using DasBlog.Tests.SmokeTest;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.Options;

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
	public class PrototypeBrowserBasedTests : IClassFixture<DasBlogTestWebApplicationFactory>
	{
		private DasBlogTestWebApplicationFactory webAppFactory;
		public PrototypeBrowserBasedTests(DasBlogTestWebApplicationFactory webApplicationFactory)
		{
			this.webAppFactory = webApplicationFactory;
			webAppFactory.CreateClient();
		}
		[Fact]
		public void MinimalTest()
		{
			IWebServerRunner runner = null;
			IBrowser browser = null;
			try
			{
				var serviceProvider = InjectDependencies();
				runner = serviceProvider.GetService<IWebServerRunner>();
				browser = serviceProvider.GetService<IBrowser>();
				var testExecutor = serviceProvider.GetService<ITestExecutor>();
				var publisher = serviceProvider.GetService<IPublisher>();
				serviceProvider
					.GetService<ILoggerFactory>()
					.AddConsole(LogLevel.Debug)
					.AddDebug(LogLevel.Debug);
				var pages = new Pages(browser);
				runner.RunDasBlog();
				browser.Init();
				List<TestStep> testSteps = new List<TestStep>
				{
					new TestStep(() => pages.Login.Goto()),
					new TestStep(() => pages.Login.IsDisplayed()),
					new TestStep(() => pages.Login.LoginButton != null),
					new TestStep(() => pages.Login.LoginButton.Click()),
					new TestStep(() =>
						pages.Login.PasswordValidation.Text.ToLower().Contains("the password field is required")),
					new TestStep(() => pages.Login.IsDisplayed())
				};
				var results = new TestResults();
				testExecutor.Execute(testSteps, results);
				publisher.Publish(results.Results);
				Assert.True(results.TestPassed);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
			finally
			{
				browser?.Dispose();
				runner?.Kill();
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
			services.AddLogging();
			services.AddSingleton<IWebServerRunner, WebServerRunner>();
//			services.AddTransient<App>();
			services.AddSingleton<IBrowser, Browser>();
			services.AddSingleton<IPublisher, Publisher>();
			services.AddSingleton<ITestExecutor, TestExecutor>();
			return services.BuildServiceProvider();
		}
	}
}
