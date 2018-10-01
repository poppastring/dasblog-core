using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Core.Extensions;
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
		public void TestLogger()
		{
			var loggerFactory = platform.ServiceProvider.GetService<ILoggerFactory>();
			var logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<InfrastructureTests>();
			logger.LogInformation("TestLogger running");
			Assert.True(true);
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

	internal class ScriptRunnerOptionsAccessor : IOptions<ScriptRunnerOptions>
	{
		public ScriptRunnerOptions Value { get; internal set; }
	}
	internal class OptionsAccessor<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; internal set; }
	}
}
