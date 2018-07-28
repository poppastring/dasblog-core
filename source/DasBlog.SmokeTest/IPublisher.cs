using System.Collections.Generic;

namespace DasBlog.SmokeTest
{
	public interface IPublisher
	{
		void Publish(IEnumerable<TestResults.Result> results);
	}
}
