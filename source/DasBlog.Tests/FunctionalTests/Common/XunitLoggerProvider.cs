using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

// from https://stackoverflow.com/questions/46169169/net-core-2-0-configurelogging-xunit-test
namespace DasBlog.Tests.FunctionalTests.Common
{
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
}
