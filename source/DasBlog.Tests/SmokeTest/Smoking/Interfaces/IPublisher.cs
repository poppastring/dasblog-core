using System.Collections.Generic;

namespace DasBlog.Tests.SmokeTest.Smoking.Interfaces
{
	public interface IPublisher
	{
		void Publish(IEnumerable<TestResults.Result> results);
	}
}
