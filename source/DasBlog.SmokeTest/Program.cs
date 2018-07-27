using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace DasBlog.SmokeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(Program).Assembly.Location));
					// the firefox and chromde drivers require the current directory
					// to be our binary directory.  The firefox/chrome nuget package
					// puts the driver there.
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
					services.AddSingleton<IWebRunner, WebRunner>();
					services.AddHostedService<SmokeTester>();
					services.AddSingleton<IBrowserFacade, BrowserFacade>();
					services.AddSingleton<ITester, Tester>();

					services.AddSingleton(waitService);
				}).ConfigureLogging((hostContext, logBuilder) =>
				{
					logBuilder.AddConsole();
					logBuilder.AddDebug();
				}).Build();
			host.Start();
			new Tester().Test();
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

	internal class Tester : ITester
	{
		public void Test()
		{
			var dir = Directory.GetCurrentDirectory();
			using (var driver = new FirefoxDriver())
			{
				driver.Navigate().GoToUrl("http://ibm.com");
			}
		}
	}

	public interface ITester
	{
		void Test();
	}

	internal class WaitService
	{
		private ManualResetEvent @event =  new ManualResetEvent(true);

		public void Wait()
		{
			@event.WaitOne();
		}

		public void StopWaiting()
		{
			@event.Set();
		}
	}

	internal class GitVersionedFileServiceOptions
	{
		public string GitRepo { get; set; }
	}

	internal class DasBlogInstallationOptions
	{
		public string ContentRootPath { get; set; }
	}

	internal class BrowserFacade : IBrowserFacade
	{
	}

	public interface IBrowserFacade
	{
	}

	internal class SmokeTester : IHostedService
	{
		private readonly ILogger<SmokeTester> logger;
		private readonly IApplicationLifetime appLifetime;
		private readonly IDasBlogInstallation installation;
		private readonly WaitService waitService;

		public SmokeTester(ILogger<SmokeTester> logger, IApplicationLifetime appLifetime
		  ,IDasBlogInstallation installation, WaitService waitService)
		{
			this.logger = logger;
			this.appLifetime = appLifetime;
			this.installation = installation;
			this.waitService = waitService;
		}
		public Task StartAsync(CancellationToken cancellationToken)
		{
			logger.LogInformation("Current directory at start up is {dir}", Directory.GetCurrentDirectory());
			
			appLifetime.ApplicationStarted.Register(WhenStarted);
			appLifetime.ApplicationStopped.Register(WhenStopped);
			appLifetime.ApplicationStopping.Register(WhenStopping);
			return Task.CompletedTask;
		}

		private void WhenStarted()
		{
			logger.LogInformation($"Started {nameof(SmokeTester)}");
			installation.Init();
//			waitService.StopWaiting();
		}

		private void WhenStopped()
		{
			logger.LogInformation($"Stopped {nameof(SmokeTester)}");
		}

		private void WhenStopping()
		{
			logger.LogInformation($"Stopping {nameof(SmokeTester)}");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}

	internal class DasBlogInstallation : IDasBlogInstallation
	{
		private readonly IVersionedFileService fileService;
		private readonly string path;
		private readonly ILogger<DasBlogInstallation> logger;
		public DasBlogInstallation(ILogger<DasBlogInstallation> logger
		  ,IVersionedFileService fileService, IOptions<DasBlogInstallationOptions> optionsAccessor)
		{
			this.logger = logger;
			this.fileService = fileService;
			this.path = optionsAccessor.Value.ContentRootPath;
		}
		public void Init()
		{
			(bool active, string errorMessage) = fileService.IsActive();
			if (!active)
			{
				logger.LogError(errorMessage);
			}

			(bool clean, string errorMessage2) = fileService.IsClean();
			if (!clean)
			{
				logger.LogError(errorMessage2);
			}
		}

		public void Terminate()
		{
			throw new NotImplementedException();
		}

		public string GetConfigPathAndFile()
		{
			throw new NotImplementedException();
		}

		public string GetContentDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetLogDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetWwwRootDirectoryPath()
		{
			throw new NotImplementedException();
		}
	}

	public interface IDasBlogInstallation
	{
		void Init();
		void Terminate();
		string GetConfigPathAndFile();
		string GetContentDirectoryPath();
		string GetLogDirectoryPath();
		string GetWwwRootDirectoryPath();
	}

	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		(bool clean, string errorMessage) IsClean();
		void Restore();
	}
}
