using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Services
{
    /// <summary>
    /// This is the handler class for the Daily Report email functionality.
    /// </summary>
    public class ReportMailer
    {
        string configPath;
        string contentPath;
        string logPath;

        public ReportMailer(string configPath, string contentPath, string logPath)
        {
            this.configPath = configPath;
            this.contentPath = contentPath;
            this.logPath = logPath;
        }


        /// <summary>
        /// Report-Mailer runs in background thread and this is the thread function.
        /// </summary>
        public void Run()
        {
            IBlogDataService dataService = null;
            ILoggingDataService loggingService = null;
            DateTime lastReportDateUTC = DateTime.Now.ToUniversalTime();


            SiteConfig siteConfig = SiteConfig.GetSiteConfig(configPath);
            loggingService = LoggingDataServiceFactory.GetService(logPath);
            dataService = BlogDataServiceFactory.GetService(contentPath, loggingService);

            ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info, "ReportMailer thread spinning up");
            loggingService.AddEvent(new EventDataItem(EventCodes.ReportMailerServiceStart, "", ""));

            do
            {
                try
                {
                    // reload on every cycle to get the current settings
                    siteConfig = SiteConfig.GetSiteConfig(configPath);
                    loggingService = LoggingDataServiceFactory.GetService(logPath);
                    dataService = BlogDataServiceFactory.GetService(contentPath, loggingService);

                    if (siteConfig.EnableDailyReportEmail)
                    {
                        if (siteConfig.SmtpServer != null && siteConfig.SmtpServer.Length > 0 &&
                            lastReportDateUTC.Day != DateTime.Now.ToUniversalTime().Day)
                        {
                            // It's a new day so send the report
                            SendEmailReport(lastReportDateUTC, siteConfig, dataService, loggingService);
                            // and update the cached date to today
                            lastReportDateUTC = DateTime.Now.ToUniversalTime();
                        }
                    }

                    // tick again in an hour
                    Thread.Sleep(TimeSpan.FromSeconds(3600));

                }

                catch (ThreadAbortException abortException)
                {
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info, abortException);
                    loggingService.AddEvent(new EventDataItem(EventCodes.ReportMailerServiceShutdown, "", ""));
                    break;
                }
                catch (Exception e)
                {
                    // if the siteConfig can't be read, stay running regardless 
                    // default wait time is 60 minutes in that case
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, e);
                    loggingService.AddEvent(new EventDataItem(EventCodes.ReportMailerServiceError, e.ToString().Replace("\n", "<br />"), null, null));
                    Thread.Sleep(TimeSpan.FromSeconds(3600));
                }

            }
            while (true);

            ErrorTrace.Trace(System.Diagnostics.TraceLevel.Info, "ReportMailer thread terminating");
            loggingService.AddEvent(new EventDataItem(EventCodes.ReportMailerServiceShutdown, "", ""));
        }


        public void SendEmailReport(DateTime reportDate, SiteConfig siteConfig, IBlogDataService dataService, ILoggingDataService loggingService)
        {
            MailMessage emailMessage = new MailMessage();
            if (siteConfig.NotificationEMailAddress != null && siteConfig.NotificationEMailAddress.Length > 0)
            {
                emailMessage.To.Add(siteConfig.NotificationEMailAddress);
            }
            else
            {
                emailMessage.To.Add(siteConfig.Contact);
            }

            emailMessage.Subject = String.Format("Weblog Daily Activity Report for '{0}'", reportDate.ToLongDateString());
            emailMessage.Body = GenerateReportEmailBody(reportDate);
            emailMessage.IsBodyHtml = true;
            emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            emailMessage.From = new MailAddress(siteConfig.Contact);

            SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, siteConfig.SmtpServer,
                siteConfig.EnableSmtpAuthentication, siteConfig.UseSSLForSMTP, siteConfig.SmtpUserName, siteConfig.SmtpPassword, siteConfig.SmtpPort);

            dataService.AddTracking(null, sendMailInfo); // use this with null tracking object, just to get the email sent
            loggingService.AddEvent(new EventDataItem(EventCodes.ReportMailerReportSent, "", ""));
        }


        private string GenerateReportEmailBody(DateTime reportDate)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(GetStyleSheet());
            sb.Append("<p><table><tr><td class=\"mainheader\" width=\"100%\">Weblog Daily Email Report (" + reportDate.ToLongDateString() + " UTC)</td></tr></table></p>");

            try
            {
                Dictionary<string, int> referrerUrls = new Dictionary<string, int>();
                Dictionary<string, int> userAgents = new Dictionary<string, int>();
                Dictionary<string, int> searchUrls = new Dictionary<string, int>();
                Dictionary<string, int> userDomains = new Dictionary<string, int>();

                SiteConfig siteConfig = SiteConfig.GetSiteConfig(configPath);
                ILoggingDataService logService = LoggingDataServiceFactory.GetService(logPath);
                string siteRoot = siteConfig.Root.ToUpper();

                LogDataItemCollection logItems = new LogDataItemCollection();
                logItems.AddRange(logService.GetReferralsForDay(reportDate));

                foreach (LogDataItem log in logItems)
                {
                    bool exclude = false;
                    if (log.UrlReferrer != null)
                    {
                        exclude = log.UrlReferrer.ToUpper().StartsWith(siteRoot);

                        // Let Utils.ParseSearchString decide whether it's a search engine referrer.
                        HyperLink link = SiteUtilities.ParseSearchString(log.UrlReferrer);
                        if (link != null)
                        {
                            string linktext = "<a href=\"" + link.NavigateUrl + "\">" + link.Text + "</a>";
                            exclude = true;
                            if (!searchUrls.ContainsKey(linktext))
                            {
                                searchUrls[linktext] = 0;
                            }
                            searchUrls[linktext] = searchUrls[linktext] + 1;
                        }
                    }

                    if (!exclude)
                    {
                        string linktext = log.UrlReferrer;
                        if (linktext.Length > 0)
                            linktext = "<a href=\"" + log.UrlReferrer + "\">" + log.UrlReferrer + "</a>";

                        if (!referrerUrls.ContainsKey(linktext))
                        {
                            referrerUrls[linktext] = 0;
                        }

                        referrerUrls[linktext] = referrerUrls[linktext] + 1;

                        log.UserAgent = HttpUtility.HtmlEncode(log.UserAgent);
                        if (!userAgents.ContainsKey(log.UserAgent))
                        {
                            userAgents[log.UserAgent] = 0;
                        }

                        userAgents[log.UserAgent] = userAgents[log.UserAgent] + 1;

                        log.UserDomain = HttpUtility.HtmlEncode(log.UserDomain);
                        if (!userDomains.ContainsKey(log.UserDomain))
                        {
                            userDomains[log.UserDomain] = 0;
                        }

                        userDomains[log.UserDomain] = userDomains[log.UserDomain] + 1;
                    }
                }

                sb.Append("<p>");
                sb.Append("<table width=\"100%\">");
                sb.Append(MakeTableHeader("Summary", "Hits"));
                sb.Append(MakeTableRow("Internet Searches", GetTotal(searchUrls)));
                sb.Append(MakeTableRow("Referrers", GetTotal(referrerUrls)));
                sb.Append("</table>");
                sb.Append("</p>");

                sb.Append("<p>");
                sb.Append("<table width=\"100%\">");
                sb.Append(MakeTableHeader("Internet Searches", "Count"));
                sb.Append(MakeTableRowsFromArray(searchUrls));
                sb.Append("</table>");
                sb.Append("</p>");

                sb.Append("<p>");
                sb.Append("<table width=\"100%\">");
                sb.Append(MakeTableHeader("Referrers", "Count"));
                sb.Append(MakeTableRowsFromArray(referrerUrls));
                sb.Append("</table>");
                sb.Append("</p>");

                sb.Append("<p>");
                sb.Append("<table width=\"100%\">");
                sb.Append(MakeTableHeader("User Agents", "Count"));
                sb.Append(MakeTableRowsFromArray(userAgents));
                sb.Append("</table>");
                sb.Append("</p>");

                sb.Append("<p>");
                sb.Append("<table width=\"100%\">");
                sb.Append(MakeTableHeader("User Domains", "Count"));
                sb.Append(MakeTableRowsFromArray(userDomains));
                sb.Append("</table>");
                sb.Append("</p>");
                sb.Append("<br/><br/>");

            }
            catch (Exception e)
            {
                sb.Append("<p>Error : " + e.ToString() + "</p>");
            }

            return sb.ToString();

        }


        private string GetStyleSheet()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<style type=\"text/css\">");
            sb.Append("table {border: 0; border-spacing: 0;	border-collapse: collapse; padding: 0;	width: 100%; font-family: Tahoma,Arial,Helvetica; font-size: 8pt; }");
            sb.Append("td.heading { text-align: left;	font-weight: bold; color: white; background-color: #919eae;	border: 0;	padding: 10px 5px 10px 5px;}");
            sb.Append("td.data {font-weight: normal;	border: 0; border-color: #fff #fff #abc #fff; border-style: solid; border-width: 1px;	padding: 0 5px 0 5px; }");
            sb.Append("td.mainheader {text-align: center; font-weight: bold; color: white; background-color: #919eae; border: 0; padding: 10px 5px 10px 5px;	font-size: 16pt;}");
            sb.Append("</style>");
            return sb.ToString();
        }

        private string GetTotal(Dictionary<string, int> hash)
        {
            return hash.Values.Sum().ToString();
        }

        private string MakeTableHeader(string col1, string col2)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<tr>");
            sb.Append("<td class=\"heading\" width=\"90%\"><b>" + HttpUtility.HtmlEncode(col1) + "</b></td>");
            sb.Append("<td class=\"heading\" width=\"10%\"><b>" + HttpUtility.HtmlEncode(col2) + "</b></td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        private string MakeTableRow(string title, string count)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("<tr>");

            sb.Append("<td class=\"data\">"); // needed to split this as the doted IP is not liked 
            sb.Append(title);                 // when adding strings together ??
            sb.Append("</td>");               // depending on email client / SMTP server ??

            sb.Append("<td class=\"data\">" + count + "</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }


        private string MakeTableRowsFromArray(IDictionary<string, int> hash)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            List<ActivityItem> arrayList = GenerateSortedItemList(hash);
            foreach (ActivityItem ai in arrayList)
            {
                sb.Append(MakeTableRow(ai.Key, ai.Val.ToString()));
            }

            return sb.ToString();

        }


        protected List<ActivityItem> GenerateSortedItemList(IDictionary<string, int> dict)
        {
            List<ActivityItem> listItems = new List<ActivityItem>(dict.Count);
            foreach (KeyValuePair<string, int> de in dict)
            {
                listItems.Add(new ActivityItem(de.Key, (int)de.Value));
            }
            listItems.Sort(new ActivityItem.Comparer());
            return listItems;
        }


    }

    public sealed class ActivityItem
    {
        public string Key { get; private set; }
        public int Val { get; private set; }

        public ActivityItem(string key, int val)
        {
            Key = key;
            Val = val;
        }

        internal class Comparer : IComparer<ActivityItem>
        {
            public int Compare(ActivityItem left, ActivityItem right)
            {
                int leftValue = left.Val;
                int rightValue = right.Val;

                return rightValue - leftValue;
            }
        }
    }
}
