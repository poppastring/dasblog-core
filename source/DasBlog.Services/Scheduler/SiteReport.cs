using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Coravel.Invocable;
using DasBlog.Services.ActivityLogs;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Services.Scheduler
{
	public class SiteEmailReport : IInvocable
	{
		private readonly ILogger<SiteEmailReport> logger;
		private readonly IActivityService activityService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly DateTime midnight;

		private const string MAIN_HEADER_TABLE = "<html><table><tbody><tr><td class='x_mainheader' width='100%'>{0}</td></tr></tbody></table>";
		private const string TABLE = "<table width='100%'><tbody>{0}</tbody></table>";
		private const string TABLE_HEADER_ROW = "<tr><td class='x_data' width='90%'><b>{0}</b></td><td class='x_data' width='10%'><b>{1}</b></td></tr>";
		private const string TABLE_BODY_ROW = "<tr><td class='x_data'>{0}</td><td class='x_data'>{1}</td></tr>";
		private const string HTML_CLOSE_TAG = "</html>";
		private readonly string EMAIL_TITLE = string.Empty;

		public SiteEmailReport(ILogger<SiteEmailReport> logger, IActivityService activityService, IDasBlogSettings dasBlogSettings)
		{
			this.logger = logger;
			this.activityService = activityService;
			this.dasBlogSettings = dasBlogSettings;
			midnight = DateTime.Now.Date;
			EMAIL_TITLE = string.Format("Weblog Daily Activity Report for {0}, {1}", midnight.DayOfWeek, midnight.ToString("MMMM dd, yyyy"));
		}

		public async Task Invoke()
		{
			if(dasBlogSettings.SiteConfiguration.EnableDailyReportEmail)
			{ 
				logger.LogInformation("Daily email report triggered");

				var emailbody = FormatEmail();
			
				var emailinfo = ComposeMail(emailbody);

				try
				{
					emailinfo?.SendMyMessage();
				}
				catch (Exception ex)
				{
					logger.LogError(new ActivityLogs.EventDataItem(ActivityLogs.EventCodes.SmtpError,
												new Uri(dasBlogSettings.SiteConfiguration.Root),
												string.Format("Weblog Daily Activity Report Failed: {0}", ex.Message)));
				}
			}

			await Task.Delay(TimeSpan.FromMilliseconds(1));
		}

		private string FormatEmail()
		{
			var body = new StringBuilder();
			var table = new StringBuilder();
			var events = activityService.GetEventsForDay(midnight);

			//header
			body.Append(string.Format(MAIN_HEADER_TABLE, EMAIL_TITLE));

			//summary header
			table.Append(string.Format(TABLE_HEADER_ROW, "Summary", "Hits"));
			table.Append(string.Format(TABLE_BODY_ROW, "Referrer", events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpReferrer)));
			table.Append(string.Format(TABLE_BODY_ROW, "User Agents", events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpUserAgent)));
			table.Append(string.Format(TABLE_BODY_ROW, "Domain", events.Count(e => e.EventCode == ActivityLogs.EventCodes.HttpUserDomain)));

			body.Append(string.Format(TABLE, table.ToString()));

			//Referrer
			table.Clear();
			table.Append(string.Format(TABLE_HEADER_ROW, "Referrer", "Count"));

			var referrer = events.Where(x => x.EventCode == ActivityLogs.EventCodes.HttpReferrer)
								.GroupBy(info => info.HtmlMessage)
								.Select(group => new { Referrer = group.Key, Count = group.Count() })
								.OrderBy(y => y.Count);

			foreach (var row in referrer)
			{
				table.Append(string.Format(TABLE_BODY_ROW, row.Referrer, row.Count));
			}
	
			body.Append(string.Format(TABLE, table.ToString()));

			//User Agents
			table.Clear();
			table.Append(string.Format(TABLE_HEADER_ROW, "User Agents", "Count"));

			var useragent = events.Where(x => x.EventCode == ActivityLogs.EventCodes.HttpUserAgent)
								.GroupBy(info => info.HtmlMessage)
								.Select(group => new { Referrer = group.Key, Count = group.Count() })
								.OrderBy(y => y.Count);

			foreach (var row in useragent)
			{
				table.Append(string.Format(TABLE_BODY_ROW, row.Referrer, row.Count));
			}

			body.Append(string.Format(TABLE, table.ToString()));

			//Domain
			table.Clear();
			table.Append(string.Format(TABLE_HEADER_ROW, "Domain", "Count"));

			var domain = events.Where(x => x.EventCode == ActivityLogs.EventCodes.HttpUserDomain)
								.GroupBy(info => info.HtmlMessage)
								.Select(group => new { Referrer = group.Key, Count = group.Count() })
								.OrderBy(y => y.Count);

			foreach (var row in domain)
			{
				table.Append(string.Format(TABLE_BODY_ROW, row.Referrer, row.Count));
			}

			body.Append(string.Format(TABLE, table.ToString()));

			body.Append(HTML_CLOSE_TAG);

			return body.ToString();
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
				if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.Contact))
				{
					emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);
				}
				else
				{
					return null;
				}
			}

			emailMessage.Subject = string.Format("Weblog Daily Activity Report for {0}, {1}", midnight.DayOfWeek, midnight.ToString("MMMM dd, yyyy"));
			
			emailMessage.Body = body;

			emailMessage.IsBodyHtml = true;
			emailMessage.BodyEncoding = Encoding.UTF8;

			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.SmtpUserName))
			{
				emailMessage.From = new MailAddress(dasBlogSettings.SiteConfiguration.SmtpUserName);
			}
			else
			{
				return null;
			}
		
			return dasBlogSettings.GetMailInfo(emailMessage);
		}


	}
}
