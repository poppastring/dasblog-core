using System;
using System.Linq;
using System.Text;
using DasBlog.Core.Common;

namespace DasBlog.Services.ActivityLogs
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

		private string url;		// null if there is no valid permalink for this event (e.g. logins, errors)

		private string userMessage;
		public string UserMessage
		{
			get
			{
				System.Diagnostics.Debug.Assert(!userMessage.Contains(Constants.CodeEventFielD));
				System.Diagnostics.Debug.Assert(!userMessage.Contains(Constants.UrlEventFielD));
				System.Diagnostics.Debug.Assert(!userMessage.Contains("\n"));
								// a broken line will mess up the Activity report - just a token protest
				var sb = new StringBuilder();
				sb.Append(Constants.CodeEventFielD);
				sb.Append(Constants.EventFieldSeparator);
				sb.Append(userMessage);
				sb.Append(Constants.EventFieldSeparator);
				sb.Append(Constants.UrlEventFielD);
				return sb.ToString();
			}
		}

		private object[] @params;
		public object[] Params
		{
			get { return (@params ?? new object[0]).Prepend(eventCode).Append(this.url).ToArray(); }
		}

		/// <summary>
		/// Using the EventDataItem structure allows us to ensure that the log line contains
		/// all we need for the Activity report. see the ActivityServiceTest for
		/// an indiction of the format - :: features heavily
		/// </summary>
		/// <param name="eventCode">used by the activity report</param>
		/// <param name="url">s/be null if no valid url is availble otherwise it is typically
		/// a permalink for a blog post.
		/// Sorry this can't be a string - but too many strings allows for errors in building events</param>
		/// <param name="userMessage"> a template with placeholders for other parameters, e.g.
		/// "{email} has logged in"</param>
		/// <param name="params">in the above example there should be one element - the email of the logger in</param>
		public EventDataItem( EventCodes eventCode, Uri url, string userMessage
		  ,params object[] @params)
		{
			this.eventCode = eventCode;
			this.url = url?.ToString();
			this.userMessage = userMessage;
			this.@params = @params;
		}
	}
}
