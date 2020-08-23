using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace DasBlog.Services.Scheduler
{
	public class SiteEmailReport : IJob
	{
		private readonly ILogger<SiteEmailReport> logger;

		public SiteEmailReport(ILogger<SiteEmailReport> logger)
		{
			this.logger = logger;
		}

		public async Task Execute(IJobExecutionContext context)
		{


			logger.LogInformation(context.JobDetail.Key + " job executing, triggered by " + context.Trigger.Key);
			await Task.Delay(TimeSpan.FromSeconds(1));
		}
	}
}
