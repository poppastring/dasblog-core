using System.Collections.Generic;

namespace DasBlog.Tests.Support.Interfaces
{
	public interface IPublisher
	{
		void Publish(IEnumerable<TestResults.Result> results);
	}
}
