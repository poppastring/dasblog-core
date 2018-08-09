using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using DasBlog.SmokeTest.Smoking.Interfaces;

namespace DasBlog.SmokeTest.Smoking
{
	public class TestExecutor : ITestExecutor
	{
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
					Console.WriteLine();
					Console.WriteLine("Error Details:");
					if (step.Value == null)
					{
						Console.WriteLine($"a step has been encountered in {testName} with no action or fun");
					}
					else if (step.Value is Action || step.Value is Func<bool>)
					{
						Console.WriteLine($"the step {step.Description} has failed");
					}
					else
					{
						Console.WriteLine("a step of unknown type has been encountered");
					}
					Console.WriteLine();

					throw;
				}
			}
			testResults.Add(testName, true, string.Empty);
		}

	}
}
