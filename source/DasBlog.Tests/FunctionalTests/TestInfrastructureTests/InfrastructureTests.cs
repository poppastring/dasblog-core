using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	public class InfrastructureTests : IClassFixture<InfrastructureTestPlatform>
	{
		private readonly InfrastructureTestPlatform platform;

		public InfrastructureTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			this.platform.CompleteSetup(testOutputHelper);			
		}
		[Fact]
		public void Test()
		{
			var scriptRunner = platform.ServiceProvider.GetService<IScriptRunner>();
			scriptRunner.Run("TestScript.cmd", new Dictionary<string, string>());
		}
	}
	public class InfrastructureTestPlatform : IDisposable
	{
		public IServiceProvider ServiceProvider { get; private set; }
		private ILogger<InfrastructureTestPlatform> logger;
		private bool init;

		public InfrastructureTestPlatform()
		{
			ServiceProvider = InjectDependencies();
			var loggerFactory = ServiceProvider
				.GetService<ILoggerFactory>();
			loggerFactory.AddConsole(LogLevel.Debug)
				.AddDebug(LogLevel.Debug);
			logger = loggerFactory.CreateLogger<InfrastructureTestPlatform>();
		}

		/// <summary>
		/// completes the platform setup by activating the logger
		/// TODO look into how the test output helper is supposed to be injected into class fixture
		/// </summary>
		/// <param name="testOutputHelper">did not get injected into this object's constructor
		///   so we have to shoehorn it in here</param>
		public void CompleteSetup(ITestOutputHelper testOutputHelper)
		{
			if (!init)
			{
				var loggerFactory = ServiceProvider.GetService<ILoggerFactory>();
				loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
				init = true;
			}
		}
		private IServiceProvider InjectDependencies()
		{
			var services = new ServiceCollection();
			services.Configure<ScriptRunnerOptions>(
				opts => opts.ScriptDirectory =
					Path.Combine(Utils.GetProjectRootDirectory(), "source/DasBlog.Tests/Support/DasBlog.Tests.Support/Scripts/"));
			services.AddLogging();
			services.AddSingleton<IScriptRunner, ScriptRunner>();
			return services.BuildServiceProvider();
		}

		public void Dispose()
		{
		}
	}
}
