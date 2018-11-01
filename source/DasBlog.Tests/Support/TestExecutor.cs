using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Support
{
	public class TestExecutor : ITestExecutor
	{
		private ILogger<TestExecutor> logger;

		public TestExecutor(ILogger<TestExecutor> logger)
		{
			this.logger = logger;
		}
		public void Execute(IList<TestStep> testSteps, TestResults testResults, [CallerMemberName]string testName = null)
		{
			foreach (var step in testSteps)
			{
				try
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
				catch (Exception e)
				{
					_  = e;
					logger.LogError(string.Empty);
					logger.LogError("Error Details:");
					if (step.Value == null)
					{
						logger.LogError($"a step has been encountered in {testName} with no action or fun");
					}
					else if (step.Value is Action || step.Value is Func<bool>)
					{
						logger.LogError($"the step {step.Description} has failed");
					}
					else
					{
						logger.LogError("a step of unknown type has been encountered");
					}
					logger.LogError(string.Empty);

					throw;
				}
			}
			testResults.Add(testName, true, string.Empty);
		}

	}
}
