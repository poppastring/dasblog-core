using DasBlog.Tests.FunctionalTests.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.BrowserBasedTests
{
	[Trait("Category", "SkipWhenLiveUnitTesting")]
	public class BrowserBasedTestsBase : IClassFixture<BrowserTestPlatform>
	{
		protected BrowserTestPlatform platform;
		protected ITestOutputHelper testOutputHelper;
		protected ILogger<PrototypeBrowserBasedTests> logger;
		public BrowserBasedTestsBase(ITestOutputHelper testOutputHelper, BrowserTestPlatform browserTestPlatform)
		{
			// the above message and others like it appear in the detail pane of Rider's test runner for
			// Running a successful test
			// Debugging a successful test
			// Running a failed test
			// Debugging a failed test
			// logger under for Run and Debug
			// I want to get hold of the xunit test results when running from the command line.
			// "dotnet test" isn't providing them as far as I can see from my limited investigations
			// Even after specifying RuntimeFramework as 2.1.4 in the csproj I got
			// "dotnet xunit" should be more fruitful but fails with the following issue:
			//   Could not load file or assembly 'Microsoft.Extensions.Options, Version=2.1.0.0,
			// There is a nuget package for M.E.O 2.1.1 but the install fails and is rolled back
			// There are no other v2 nuget packages except a preview 2.2.
			// "dotnet xunit" seems to be a no-go - did not investigate whether it solved
			// the original problem of locating and using test results.
			// 
			// It turns out that the following is not a bad start:
			// "dotnet test --logger trx;LogfileName=test_results.xml --results-directory ./test_results"
			//			test_results.xml will appear in <proj>/source/DasBlog.Tests/FunctionalTests/test_results
			browserTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = browserTestPlatform;
			LoggerValidator.Validate(platform.ServiceProvider);
			this.testOutputHelper = testOutputHelper;
			this.logger = platform.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PrototypeBrowserBasedTests>();
		}
		
	}
}
