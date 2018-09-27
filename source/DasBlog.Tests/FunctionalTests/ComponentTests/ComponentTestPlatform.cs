using System;
using System.IO;
using DasBlog.SmokeTest;
using DasBlog.Tests.FunctionalTests.TestInfrastructureTests;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public class ComponentTestPlatform : TestSupportPlatform
	{
		private ILogger<ComponentTestPlatform> logger;
		private bool init;

		protected override void InjectDependencies(IServiceCollection services)
		{
			// nothing to do
		}

		protected override void CompleteSetupLocal(ITestOutputHelper testOutputHelper)
		{
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			logger = loggerFactory.CreateLogger<ComponentTestPlatform>();
		}

	}
}
