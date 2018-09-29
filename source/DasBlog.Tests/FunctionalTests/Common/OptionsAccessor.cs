using Microsoft.Extensions.Options;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public class OptionsAccessor<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; internal set; }
	}
}
