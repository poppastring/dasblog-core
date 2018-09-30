using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

// from https://stackoverflow.com/questions/46169169/net-core-2-0-configurelogging-xunit-test
namespace DasBlog.Tests.FunctionalTests.Common
{
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
