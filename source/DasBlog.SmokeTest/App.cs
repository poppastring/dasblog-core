using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.SmokeTest.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DasBlog.SmokeTest
{
	internal class App : IHostedService
	{
		private readonly ILogger<App> logger;
		private readonly IApplicationLifetime appLifetime;
		private readonly IDasBlogInstallation installation;
		private readonly WaitService waitService;
		private readonly ITester tester;

		public App(ILogger<App> logger, IApplicationLifetime appLifetime
			,IDasBlogInstallation installation, WaitService waitService
			,ITester tester)
		{
			this.logger = logger;
			this.appLifetime = appLifetime;
			this.installation = installation;
			this.waitService = waitService;
			this.tester = tester;
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
			logger.LogInformation($"Started {nameof(App)}");
			installation.Init();
			tester.Test();
//			waitService.StopWaiting();
		}

		private void WhenStopped()
		{
			logger.LogInformation($"Stopped {nameof(App)}");
		}

		private void WhenStopping()
		{
			logger.LogInformation($"Stopping {nameof(App)}");
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}