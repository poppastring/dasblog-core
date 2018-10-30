using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Core.Common;
using DasBlog.Core.Extensions;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using Constants = DasBlog.Tests.Support.Common.Constants;
using Utils = DasBlog.Tests.Support.Common.Utils;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	/// <summary>
	/// usage: dotnet test --logger trx;LogfileName=test_results.xml --results-directory ./test_results --filter Category=TestInfrastructureTest
	/// </summary>
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class InfrastructureTests : IClassFixture<InfrastructureTestPlatform>
	{
		private readonly InfrastructureTestPlatform platform;

		public InfrastructureTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			this.platform.CompleteSetup(testOutputHelper);
			LoggerValidator.Validate(platform.ServiceProvider);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void TestLogger()
		{
			var loggerFactory = platform.ServiceProvider.GetService<ILoggerFactory>();
			var logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<InfrastructureTests>();
			logger.LogInformation("TestLogger running");
			Assert.True(true);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void Test()
		{
			var scriptRunner = platform.ServiceProvider.GetService<IScriptRunner>();
			(var exitCode, var outputs) = scriptRunner.Run("TestScript", new Dictionary<string, string>(), false);
			Assert.Equal(0, exitCode);
			Assert.True(outputs.Length > 0);
		}

		[Fact(Skip="resurrect when a new timeout mechanism has been implemented")]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void Runner_WhenTimedOut_ThrowsExcption()
		{
			ScriptRunnerOptions opts = new ScriptRunnerOptions();
			opts.ScriptDirectory = Path.Combine(Utils.GetProjectRootDirectory(), Constants.ScriptsRelativePath);
			opts.ScriptTimeout = 1;		// one millisecond
			opts.ScriptExitTimeout = 10;
			var accessor = new OptionsAccessor<ScriptRunnerOptions> {Value = opts};
			ILogger<ScriptRunner> logger =
				platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<ScriptRunner>();
			IScriptPlatform scriptPlatform;
			if (new StdOSDetector().DetectOS() == Os.Windows)
			{
				scriptPlatform = new CmdScriptPlatform(platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<CmdScriptPlatform>());				
			}
			else
			{
				scriptPlatform = new BashScriptPlatform(platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<BashScriptPlatform>());				
			}
			ScriptRunner runner = new ScriptRunner(accessor, logger, scriptPlatform);
			Assert.Throws<Exception>(() => runner.Run("TestScript", runner.DefaultEnv, false));
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void DetectChangesScript_WhenCleanDirectory_ReturnsNothing()
		{
			var runner = platform.ServiceProvider.GetService<IScriptRunner>();
			(var exitCode, _) = runner.Run("DetectChanges", runner.DefaultEnv, false
//			  );
			  ,Path.Combine(Utils.GetProjectRootDirectory(), Constants.VanillaTestData));
			Assert.Equal(0, exitCode);
		}
	}

	internal class ScriptRunnerOptionsAccessor : IOptions<ScriptRunnerOptions>
	{
		public ScriptRunnerOptions Value { get; internal set; }
	}
}
