using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.UnitTests.Controllers
{
	internal sealed class RegressionLogger<T> : ILogger<T>
	{
		internal sealed record Entry(LogLevel Level, EventId EventId, Exception Exception, Dictionary<string, object> State, string Message);
		public List<Entry> Entries { get; } = new();
		public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
		public bool IsEnabled(LogLevel logLevel) => true;
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			Entries.Add(new Entry(logLevel, eventId, exception,
				((IEnumerable<KeyValuePair<string, object>>)state).GroupBy(pair => pair.Key).ToDictionary(group => group.Key, group => group.First().Value), formatter(state, exception)));
		}
	}
}

