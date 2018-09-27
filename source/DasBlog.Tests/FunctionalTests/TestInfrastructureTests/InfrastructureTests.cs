using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	/// <summary>
	/// usage: dotnet test --logger trx;LogfileName=test_results.xml --results-directory ./test_results --filter Category=TestInfrastructureTest
	/// </summary>
	public class InfrastructureTests : IClassFixture<InfrastructureTestPlatform>
	{
		private readonly InfrastructureTestPlatform platform;

		public InfrastructureTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			this.platform.CompleteSetup(testOutputHelper);
		}
		[Fact]
		[Trait("Category", "TestInfrastructureTest")]
		public void Test()
		{
			var scriptRunner = platform.ServiceProvider.GetService<IScriptRunner>();
			(var exitCode, var outputs, var errors) = scriptRunner.Run("TestScript.cmd", new Dictionary<string, string>());
			Assert.Equal(0, exitCode);
			Assert.True(outputs.Length > 0);
			Assert.True(errors.Length > 0);
		}

		[Fact]
		[Trait("Category", "TestInfrastructureTest")]
		public void Runner_WhenTimedOut_ThrowsExcption()
		{
			ScriptRunnerOptions opts = new ScriptRunnerOptions();
			opts.ScriptDirectory = Path.Combine(Utils.GetProjectRootDirectory(), Constants.ScriptsRelativePath);
			opts.ScriptTimeout = 1;		// one millisecond
			var accessor = new OptionsAccessor<ScriptRunnerOptions> {Value = opts};
			ILogger<ScriptRunner> logger =
				platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<ScriptRunner>();
			ScriptRunner runner = new ScriptRunner(accessor, logger);
			Assert.Throws<Exception>(() => runner.Run("TestScript.cmd", new Dictionary<string, string>()));
		}

		[Fact]
		[Trait("Category", "TestInfrastructureTest")]
		[Trait("Chosen", "1")]
		public void DetectChangesScript_WhenCleanDirectory_ReturnsNothing()
		{
			var runner = platform.ServiceProvider.GetService<IScriptRunner>();
			(var exitCode, var outputs, var errors) = runner.Run("DetectChanges.cmd", runner.DefaultEnv
//			  );
			  ,Path.Combine(Utils.GetProjectRootDirectory(), Constants.VanillaTestData));
			Assert.Equal(0, exitCode);
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
				opts =>
				{
					opts.ScriptDirectory =
						Path.Combine(Utils.GetProjectRootDirectory(),Constants.ScriptsRelativePath);
					opts.ScriptTimeout = Constants.DefaultScriptTimeout;
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptTimeout),
						out var envScriptTimeout))
					{
						opts.ScriptTimeout = envScriptTimeout;
					}
					if (Int32.TryParse(Environment.GetEnvironmentVariable(Constants.DasBlogTestScriptExitTimeout),
						out var envScriptExitTimeout))
					{
						opts.ScriptExitTimeout = envScriptExitTimeout;
					}
				});
			services.AddLogging();
			services.AddSingleton<IScriptRunner, ScriptRunner>();
			return services.BuildServiceProvider();
		}

		public void Dispose()
		{
		}
	}

	internal class ScriptRunnerOptionsAccessor : IOptions<ScriptRunnerOptions>
	{
		public ScriptRunnerOptions Value { get; internal set; }
	}
	internal class OptionsAccessor<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; internal set; }
	}
}
