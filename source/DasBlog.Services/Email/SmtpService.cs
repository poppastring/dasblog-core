using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using DasBlog.Services.Email.Interfaces;

namespace DasBlog.Services.Email
{
	public class SmtpService : ISmtpService
	{
		public readonly SmtpDataOption smtpDataOpton;

		public SmtpService(IOptions<SmtpDataOption> optionsAccessor)
		{
			this.smtpDataOpton = optionsAccessor.Value;
		}

		public async Task SendDailyNotificationEmail(CancellationToken cancellationToken)
		{
			// Need to pull in data about the last 24 email...

			await SendingEmail(smtpDataOpton.ContactEMailAddress, 
								string.Format("Weblog Daily Activity Report for {0}", DateTime.Now), 
								"Some message for the body", cancellationToken);
		}

		public async Task SendEmail(string email, string subject, string message, CancellationToken cancellationToken)
		{
			await SendingEmail(email, subject, message, cancellationToken);
		}

		private async Task SendingEmail(string email, string subject, string message, CancellationToken cancellationToken)
		{
			var mimemessage = new MimeMessage();
			mimemessage.To.Add(new MailboxAddress(smtpDataOpton.NotificationEMailAddress));
			mimemessage.From.Add(new MailboxAddress(this.smtpDataOpton.SmtpUserName));

			mimemessage.Subject = subject;

			mimemessage.Body = new TextPart("plain")
			{
				Text = message
			};

			using (var client = new SmtpClient())
			{
				// Accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				client.Connect(smtpDataOpton.SmtpServer, smtpDataOpton.SmtpPort, smtpDataOpton.UseSSLForSMTP);

				await client.AuthenticateAsync(smtpDataOpton.SmtpUserName, smtpDataOpton.SmtpPassword, cancellationToken);

				await client.SendAsync(mimemessage, cancellationToken);

				client.Disconnect(true);
			}
		}
	}
}
