﻿using System.Threading;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.SmokeTest
{
	internal class App
	{
		private readonly ILogger<App> logger;
		private readonly ITester tester;
		private readonly IWebServerRunner runner;
		private readonly IPublisher publisher;

		public App(ILogger<App> logger
			,ITester tester, IWebServerRunner runner, IPublisher publisher)
		{
			System.Diagnostics.Debug.Assert(tester != null);
			this.logger = logger;
			this.tester = tester;
			this.runner = runner;
			this.publisher = publisher;
		}
		public void Run()
		{
			try
			{
				logger.LogInformation($"Started {nameof(App)}");
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