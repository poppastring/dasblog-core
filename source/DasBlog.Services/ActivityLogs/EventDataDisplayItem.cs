using System;

namespace DasBlog.Services.ActivityLogs
{
	public class EventDataDisplayItem
	{
		public EventDataDisplayItem(EventCodes eventCode, string htmlMessage, DateTime date)
		{
			this.EventCode = eventCode;
			this.HtmlMessage = htmlMessage;
			this.Date = date;
		}

		public DateTime Date { get;  }

		public string HtmlMessage { get; }

		public EventCodes EventCode { get; }
	}
}
