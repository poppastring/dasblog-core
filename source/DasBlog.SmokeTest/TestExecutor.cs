using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest
{
	public class TestExecutor : ITestExecutor
	{
		public void Execute(IList<TestStep> testSteps, TestResults testResults, [CallerMemberName]string testName = null)
		{
			foreach (var step in testSteps)
			{
				switch (step.Value)
				{
					case Action action:
						action();
						break;
					case Func<bool> func:
						bool result = func();
						if (!result)
						{
							testResults.Add(testName, false, (step.Description as Expression<Func<bool>>).ToString());
							return;
						}
						break;
					default:
						throw new Exception("Unknow test step encountered");
				}
			}
			testResults.Add(testName, false, string.Empty);
		}

	}
}
