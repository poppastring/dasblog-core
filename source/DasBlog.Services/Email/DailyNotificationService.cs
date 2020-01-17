using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Email
{
	public class DailyNotificationService : HostedService
	{
		private readonly SmtpService smtpService;

		public DailyNotificationService(SmtpService smtpService)
		{
			this.smtpService = smtpService;;
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				await smtpService.SendDailyNotificationEmail(cancellationToken);

				// We need to wait until it is time to send the email, say around midnight.
				await Task.Delay(TimeSpan.FromDays(1), cancellationToken);
			}
		}
	}
}
