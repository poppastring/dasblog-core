using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace DasBlog.Services.Email
{
	public class SmtpService
	{
		public readonly SmtpDataOption smtpDataOpton;

		public SmtpService(IOptions<SmtpDataOption> optionsAccessor)
		{
			this.smtpDataOpton = optionsAccessor.Value;
		}

		public async Task SendDailyNotificationEmail(CancellationToken cancellationToken)
		{
			// Need to pull in data about the last 24 email...
			await SendingEmail(smtpDataOpton.ContactEMailAddress, "", "", cancellationToken);
		}

		public async Task SendEmail(string email, string subject, string message, CancellationToken cancellationToken)
		{
			await SendingEmail(email, subject, message, cancellationToken);
		}

		private async Task SendingEmail(string email, string subject, string message, CancellationToken cancellationToken)
		{
			var mimemessage = new MimeMessage();
			mimemessage.From.Add(new MailboxAddress(smtpDataOpton.NotificationEMailAddress));
			mimemessage.To.Add(new MailboxAddress(email));

			mimemessage.Subject = subject;

			mimemessage.Body = new TextPart("plain")
			{
				Text = message
			};

			using (var client = new SmtpClient())
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				client.Connect(smtpDataOpton.SmtpServer, smtpDataOpton.SmtpPort, smtpDataOpton.UseSSLForSMTP);

				// Note: only needed if the SMTP server requires authentication
				await client.AuthenticateAsync(smtpDataOpton.SmtpUserName, smtpDataOpton.SmtpPassword, cancellationToken);

				await client.SendAsync(mimemessage, cancellationToken);

				client.Disconnect(true);
			}
		}
	}
}
