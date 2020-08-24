using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DasBlog.Services.ActivityLogs;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using Quartz;

namespace DasBlog.Services.Scheduler
{
	public class SiteEmailReport : IJob
	{
		private readonly ILogger<SiteEmailReport> logger;
		private readonly IActivityService activityService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly DateTime midnight;

		private const string MAIN_HEADER = "<table><tbody><tr><td class='x_mainheader' width='100%'>{0}</td></tr></tbody></table>";
		private const string TABLE = "<table width='100%'><tbody>{0}</tbody></table>";
		private const string TABLE_HEADER_ROW = "<tr><td class='x_data' width='90%'><b>{0}</b></td><td class='x_data' width='10%'><b>{1}</b></td></tr>";
		private const string TABLE_BODY_ROW = "<tr><td class='x_data'>{0}</td><td class='x_data'>{1}</td></tr>";
		private const string HTML = "<html>{0}</html>";
		private readonly string EMAIL_TITLE = string.Empty;

		public SiteEmailReport(ILogger<SiteEmailReport> logger, IActivityService activityService, IDasBlogSettings dasBlogSettings)
		{
			this.logger = logger;
			this.activityService = activityService;
			this.dasBlogSettings = dasBlogSettings;
			midnight = DateTime.Now.Date;
			EMAIL_TITLE = string.Format("Weblog Daily Activity Report for {0}, {1}", midnight.DayOfWeek, midnight.ToString("MMMM dd, yyyy"));
		}

		public async Task Execute(IJobExecutionContext context)
		{
			logger.LogInformation(context.JobDetail.Key + " job executing, triggered by " + context.Trigger.Key);

			var emailbody = FormatEmail();
			
			var emailinfo = ComposeMail(emailbody);

			try
			{
				emailinfo.SendMyMessage();
			}
			catch (Exception ex)
			{
				logger.LogError(new ActivityLogs.EventDataItem(ActivityLogs.EventCodes.SmtpError,
											new Uri(dasBlogSettings.SiteConfiguration.Root),
											string.Format("Weblog Daily Activity Report Failed: {0}", ex.Message)));
			}

			await Task.Delay(TimeSpan.FromMilliseconds(1));
		}

		private string FormatEmail()
		{
			var body = new StringBuilder();
			var events = activityService.GetEventsForDay(midnight);

			//header
			body.Append(string.Format(MAIN_HEADER, EMAIL_TITLE));

			//
			// body.Append(string.Format(TABLE, string.Format(TABLE_HEADER_ROW, "Summary", "Hits")));
			
			//summary header


			events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpReferrer);
			events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpUserAgent);
			events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpUserDomain);


			//details


			foreach (var evt in events)
			{
				// sub header
				if (evt.EventCode == ActivityLogs.EventCodes.HttpReferrer)
				{
					body.Append(string.Format("{0} <br />", evt.HtmlMessage));
				}
			}

			return string.Empty;
		}

		private SendMailInfo ComposeMail(string body)
		{
			var emailMessage = new MailMessage();

			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.NotificationEMailAddress))
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.NotificationEMailAddress);
			}
			else
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);
			}

			emailMessage.Subject = string.Format("Weblog Daily Activity Report for {0}, {1}", midnight.DayOfWeek, midnight.ToString("MMMM dd, yyyy"));
			
			emailMessage.Body = body;

			emailMessage.IsBodyHtml = true;
			emailMessage.BodyEncoding = System.Text.Encoding.UTF8;

			emailMessage.From = new MailAddress(dasBlogSettings.SiteConfiguration.SmtpUserName);

			return dasBlogSettings.GetMailInfo(emailMessage);
		}


	}
}
