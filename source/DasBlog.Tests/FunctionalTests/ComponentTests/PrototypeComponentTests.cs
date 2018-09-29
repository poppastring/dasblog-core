using System;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Constants = DasBlog.Tests.Support.Common.Constants;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using newtelligence.DasBlog.Runtime;

/*
 * THERE IS NO logging to the console - in the debugger the log appears in the detail of the test results
 * when run from the console the log appears in a log file assuming you provide the correct command line
 * dotnet xunit cli is required to get console output
 * If I do "dotnet xunit -diagnostics" it barfs with a reference to a missing Microsoft.Extensions.Options
 */
namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public class PrototypeComponentTests : IClassFixture<ComponentTestPlatform>, IDisposable
	{

		private ComponentTestPlatform platform;
		private ITestOutputHelper testOutputHelper;
		private ILogger<PrototypeComponentTests> logger;
		private IVersionedFileService versionedFileService;
		private IDasBlogSandbox dasBlogSandbox;
		public PrototypeComponentTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
					// this and others like it appear in the detail pane of Rider's test runner for
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
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
			this.testOutputHelper = testOutputHelper;
			dasBlogSandbox = platform.CreateSandbox(Constants.VanillaEnvironment);
		}

		[Fact]
		[Trait("Category", "ComponentTest")]
		public void SimpleTest()
		{
			Assert.True(true);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithUnmatchableData_ReturnsNull()
		{
			var sandbox = dasBlogSandbox;
			var blogManager = platform.CreateBlogManager(sandbox);
			EntryCollection entries = blogManager.SearchEntries("this cannot be found", null);
			Assert.Empty(entries);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithMatchableData_ReturnsOneRecord()
		{
			IDasBlogSandbox sandbox = dasBlogSandbox;
			sandbox = platform.CreateSandbox(Constants.VanillaEnvironment);
			sandbox.Init();
			var blogManager = platform.CreateBlogManager(sandbox);
			EntryCollection entries = blogManager.SearchEntries("put some text here", null);
			Assert.Single(entries);
		}


		public void Dispose()
		{
			dasBlogSandbox?.Terminate();
		}
	}

	// from https://stackoverflow.com/questions/46169169/net-core-2-0-configurelogging-xunit-test
	public class XunitLoggerProvider : ILoggerProvider
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}

		public ILogger CreateLogger(string categoryName)
			=> new XunitLogger(_testOutputHelper, categoryName);

		public void Dispose()
		{
		}
	}

	public class XunitLogger : ILogger
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private readonly string _categoryName;

		public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
		{
			_testOutputHelper = testOutputHelper;
			_categoryName = categoryName;
		}

		public IDisposable BeginScope<TState>(TState state)
			=> NoopDisposable.Instance;

		public bool IsEnabled(LogLevel logLevel)
			=> true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			_testOutputHelper.WriteLine($"{_categoryName} [{eventId}] {formatter(state, exception)}");
			if (exception != null)
				_testOutputHelper.WriteLine(exception.ToString());
		}

		private class NoopDisposable : IDisposable
		{
			public static NoopDisposable Instance = new NoopDisposable();
			public void Dispose()
			{ }
		}
	}
}
