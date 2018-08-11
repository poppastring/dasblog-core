using System.IO;
using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.SmokeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new HostBuilder()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureHostConfiguration(builder => { builder.AddEnvironmentVariables(prefix: "ASPNETCORE_"); })
				.ConfigureAppConfiguration((hostContext, builder) =>
				{
					builder.AddCommandLine(args);
					builder.AddJsonFile(
						Path.Combine(hostContext.HostingEnvironment.ContentRootPath, "appSettings.json"));
				}).ConfigureServices((hostContext, services) =>
				{
					services.Configure<BrowserOptions>(hostContext.Configuration);
					services.Configure<WebServerRunnerOptions>(hostContext.Configuration);
					services.AddSingleton<IWebServerRunner, WebServerRunner>();
					services.AddTransient<App>();
					services.AddSingleton<IBrowser, Browser>();
					services.AddTransient<ITester, Tester>();
					services.AddSingleton<IPublisher, Publisher>();
					services.AddSingleton<ITestExecutor, TestExecutor>();
				})
				.ConfigureLogging(
					(hostContext, logBuilder) => logBuilder.AddDebug().AddConsole()
					)
				.Build();
			App app = host.Services.GetService<App>();
			app.Run();
		}

	}
}
