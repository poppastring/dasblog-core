using System.IO;
using DasBlog.SmokeTest.Interfaces;
using Microsoft.Extensions.Configuration;
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
				.ConfigureHostConfiguration(builder =>
				{
					builder.AddEnvironmentVariables(prefix: "ASPNETCORE_");
				})
				.ConfigureAppConfiguration((hostContext, builder) =>
				{
					builder.AddCommandLine(args);
				}).ConfigureServices((hostContext, services) =>
				{
					services.Configure<DasBlogInstallationOptions>(options =>
						ConfigureDasBlogInstallation(options, hostContext.Configuration));
					services.Configure<GitVersionedFileServiceOptions>(options =>
						ConfigureGitPath(options, hostContext.Configuration));
					services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
					services.AddSingleton<IDasBlogInstallation, DasBlogInstallation>();
					services.AddSingleton<IWebServerRunner, WebServerRunner>();
					services.AddHostedService<App>();
					services.AddSingleton<IBrowser, Browser>();
					services.AddSingleton<ITester, Tester>();

					services.AddSingleton(waitService);
				}).ConfigureLogging((hostContext, logBuilder) =>
				{
					logBuilder.AddConsole();
					logBuilder.AddDebug();
				}).Build();
			host.Start();
			waitService.Wait();
			host.StopAsync();
//			sm.StopAsync(new CancellationToken());
		}

		private static void ConfigureGitPath(GitVersionedFileServiceOptions options, IConfiguration config)
		{
			if (string.IsNullOrEmpty(config[nameof(options.GitRepo)]))
			{
				string root = Path.GetFullPath(typeof(Program).Assembly.Location);
				options.GitRepo = Path.Combine(root.Replace( Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
				  , "../../../../../..");
			}
			else
			{
				options.GitRepo = config[nameof(options.GitRepo)];
			}
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

	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		(bool clean, string errorMessage) IsClean();
		void Restore();
	}
}
