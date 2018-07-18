using System;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Core
{
	public class EventDataItem
	{
		public int EventCode { get; }
		public string UserMessage { get; }
		public string LoalUrl { get; }

		public EventDataItem( EventCodes eventCode, string userMessage, string localUrl )
		{
			EventCode = (int)eventCode;
			UserMessage = userMessage;
			LoalUrl = localUrl;
		}
		public void Deconstruct(out int eventId, out string format, out object[] @params)
		{
			eventId = (int)EventCode;
			format = UserMessage;
			@params = new object[] {LoalUrl};
		}

	}
}
