using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Services.Email
{
	public class SmtpDataOption
	{
		public string SmtpServer { get; set; }
		public int SmtpPort { get; set; }
		public string SmtpUserName { get; set; }
		public string SmtpPassword { get; set; }
		public bool UseSSLForSMTP { get; set; }
		public string NotificationEMailAddress { get; set; }
		public string ContactEMailAddress { get; set; }
	}
}
