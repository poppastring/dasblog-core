using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;
using DasBlog.Core.Common;

namespace DasBlog.Core
{
	/// <summary>
	/// takes parameters like
	/// (EventCodes.SecuritySuccess, "example.com", "user [email] logged in", "mike@mikesite.com")
	/// and makes the info available to the logger in the form
	/// UserMessage = "{eventurl_6473} :: user {email} logged in :: {eventurl_6473}"
	/// Params = [EventCodes.SecuritySuccess, "mike@mikesite.com", "example.com"]
	/// </summary>
	public class EventDataItem
	{
		private EventCodes eventCode;

		private string url;

		private string userMessage;
		public string UserMessage
		{
			get
			{
				System.Diagnostics.Debug.Assert(!userMessage.Contains(Constants.CODE_EVENT_FIElD));
				System.Diagnostics.Debug.Assert(!userMessage.Contains(Constants.URL_EVENT_FIElD));
				var sb = new StringBuilder();
				sb.Append(Constants.CODE_EVENT_FIElD);
				sb.Append(Constants.EVENT_FIELD_SEPARATOR);
				sb.Append(userMessage);
				sb.Append(Constants.EVENT_FIELD_SEPARATOR);
				sb.Append(Constants.URL_EVENT_FIElD);
				return sb.ToString();
			}
		}

		private object[] @params;
		public object[] Params
		{
			get { return (@params ?? new object[0]).Prepend(eventCode).Append(this.url).ToArray(); }
		}

		public EventDataItem( EventCodes eventCode, string url, string userMessage
		  ,params object[] @params)
		{
			this.eventCode = eventCode;
			this.url = url;
			this.userMessage = userMessage;
			this.@params = @params;
		}
	}
}
