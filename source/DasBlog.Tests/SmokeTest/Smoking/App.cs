using System.Threading;
using DasBlog.Tests.SmokeTest.Smoking.Interfaces;
using DasBlog.Tests.SmokeTest.Support.Interfaces;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.SmokeTest.Smoking
{
	internal class App
	{
		private readonly ILogger<App> logger;
		private readonly IDasBlogInstallation installation;
		private readonly ITester tester;
		private readonly IWebServerRunner runner;
		private readonly IPublisher publisher;

		public App(ILogger<App> logger
			,IDasBlogInstallation installation
			,ITester tester, IWebServerRunner runner, IPublisher publisher)
		{
			System.Diagnostics.Debug.Assert(tester != null);
			this.logger = logger;
			this.installation = installation;
			this.tester = tester;
			this.runner = runner;
			this.publisher = publisher;
		}
		public void Run()
		{
			try
			{
				logger.LogInformation($"Started {nameof(App)}");
				installation.Init();
				runner.RunDasBlog();
				Thread.Sleep(1000);
				tester.Test();
				publisher.Publish(tester.Results.Results);
				runner.Kill();
			}
			finally
			{
				tester.Dispose();
			}
		}
	}
}
