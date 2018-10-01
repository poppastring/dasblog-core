using System;
using System.Collections.Generic;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Support
{
	public class Publisher : IPublisher
	{
		private ILogger<Publisher> logger;

		public Publisher(ILogger<Publisher> logger)
		{
			this.logger = logger;
		}
		public void Publish(IEnumerable<TestResults.Result> results)
		{
			logger.LogInformation("Test Results");
			logger.LogInformation(string.Format("{0, 60} {1,-6} {2}", "Test", "Passed", "Failed Step"));
			logger.LogInformation(string.Format("{0, 60} {1,-6} {2}", "".PadRight(30, '='), "".PadRight(6, '='), "".PadRight(52, '=')));
			foreach (var result in results)
			{
				logger.LogInformation(string.Format("{0, 60} {1,-6} {2}", result.TestId, result.Success, result.ErrorMessge));
			}
			logger.LogInformation("");
		}
		
	}
}
