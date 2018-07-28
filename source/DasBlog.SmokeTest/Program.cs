using System.Diagnostics;
using System.IO;
using DasBlog.SmokeTest.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DasBlog.SmokeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			WaitService waitService = new WaitService();
			var host = new HostBuilder()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.ConfigureHostConfiguration(builder =>
				{
					builder.AddEnvironmentVariables(prefix: "ASPNETCORE_");
				})
				.ConfigureAppConfiguration((hostContext, builder) =>
				{
					builder.AddCommandLine(args);
					builder.AddJsonFile(Path.Combine(hostContext.HostingEnvironment.ContentRootPath, "appSettings.json"));
				}).ConfigureServices((hostContext, services) =>
				{
					services.Configure<DasBlogInstallationOptions>(options =>
						ConfigureDasBlogInstallation(options, hostContext.Configuration));
					services.Configure<BrowserOptions>(hostContext.Configuration);
					services.Configure<WebServerRunnerOptions>(hostContext.Configuration);
					services.AddSingleton<IVersionedFileService, NoopVersionedFileService>();
					services.AddSingleton<IDasBlogInstallation, DasBlogInstallation>();
					services.AddSingleton<IWebServerRunner, WebServerRunner>();
					services.AddHostedService<App>();
					services.AddSingleton<IBrowser, Browser>();
					services.AddSingleton<ITester, Tester>();
					services.AddSingleton<IPublisher, Publisher>();

					services.AddSingleton(waitService);
					services.AddSingleton<ITestExecutor, TestExecutor>();
				}).ConfigureLogging((hostContext, logBuilder) =>
				{
					logBuilder.AddConsole();
					logBuilder.AddDebug();
				}).Build();
			host.Start();
			waitService.Wait();
			host.StopAsync();
			Process.GetCurrentProcess().Kill();
					// tragic - but I can't sus how to close down the process
					// I think the hosted service, App, will not exit - despite having tried Environment.Exit(0)
		}

		private static void ConfigureDasBlogInstallation(DasBlogInstallationOptions options, IConfiguration config)
		{
			if (string.IsNullOrEmpty(config[nameof(options.ContentRootPath)]))
			{
				string root = Path.GetFullPath(typeof(Program).Assembly.Location);
				options.ContentRootPath = Path.Combine(root.Replace( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				  , "../../../../DasBlog.Web.UI");
			}
			else
			{
				options.ContentRootPath = config[nameof(options.ContentRootPath)];
			}
		}
	}
}
