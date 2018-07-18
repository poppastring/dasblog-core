using Microsoft.Extensions.Logging;

namespace DasBlog.Core.Extensions
{
	public static class Logging
	{
		public static void LogInformation(this ILogger logger, EventDataItem edi)
		{
			logger.LogInformation(edi.EventCode, edi.UserMessage, edi.LoalUrl);
		}
	}
}
