using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Email.Interfaces
{
	public interface ISmtpService
	{
		Task SendDailyNotificationEmail(CancellationToken cancellationToken);

		Task SendEmail(string email, string subject, string message, CancellationToken cancellationToken);
	}
}
