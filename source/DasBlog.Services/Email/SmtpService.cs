using DasBlog.Services.Email.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Email
{
	public class SmtpService : ISmtpService
	{
		public readonly SmtpDataOption smtpDataOpton;

		public SmtpService(IOptions<SmtpDataOption> optionsAccessor)
		{
			smtpDataOpton = optionsAccessor.Value;
		}

		public async Task SendDailyNotificationEmail(CancellationToken cancellationToken)
		{
			// Need to pull in data about the last 24 email...

			await SendingEmail(string.Format("Weblog Daily Activity Report for {0}", DateTime.Now), 
								"Some message for the body", cancellationToken);
		}

		public async Task SendEmail(string subject, string message, CancellationToken cancellationToken)
		{
			await SendingEmail(subject, message, cancellationToken);
		}

		private async Task SendingEmail(string subject, string message, CancellationToken cancellationToken)
		{
			var client = new System.Net.Mail.SmtpClient
			{
				Host = smtpDataOpton.SmtpServer,
				Port = smtpDataOpton.SmtpPort,
				EnableSsl = smtpDataOpton.UseSSLForSMTP,
				Timeout = 1000000,
				UseDefaultCredentials = false,
				Credentials = new System.Net.NetworkCredential(smtpDataOpton.SmtpUserName, smtpDataOpton.SmtpPassword)
			};


			var from = new System.Net.Mail.MailAddress(smtpDataOpton.SmtpUserName, string.Empty, System.Text.Encoding.UTF8);
			var to = new System.Net.Mail.MailAddress(smtpDataOpton.NotificationEMailAddress);
			var mm = new System.Net.Mail.MailMessage(from, to);

			mm.Subject = subject;
			mm.IsBodyHtml = false;
			mm.Body = message;
			mm.Priority = System.Net.Mail.MailPriority.Normal;

			await client.SendMailAsync(mm);
		}
	}
}
