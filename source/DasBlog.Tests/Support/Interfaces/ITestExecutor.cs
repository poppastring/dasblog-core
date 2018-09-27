using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DasBlog.Tests.Support.Interfaces
{
	public interface ITestExecutor
	{
		void Execute(IList<TestStep> steps, TestResults testResults, [CallerMemberName] string testName = null);
	}
}
