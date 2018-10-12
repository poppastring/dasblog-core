using System;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public class FakeOptionsMonitor<TOptions> : IOptionsMonitor<TOptions>
	{
		private TOptions options;

		public FakeOptionsMonitor(TOptions opts)
		{
			this.options = opts;
		}
		public TOptions Get(string name)
		{
			return options;
		}

		public IDisposable OnChange(Action<TOptions, string> listener)
		{
			if (listener != null)
			{
				listener(options, "change to FakeOptionsMonitor");
			}

			return null;
		}

		public TOptions CurrentValue => options;
	}
}
