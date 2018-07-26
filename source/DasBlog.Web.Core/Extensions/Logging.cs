using System;
using Microsoft.Extensions.Logging;

namespace DasBlog.Core.Extensions
{
	public static class Logging
	{
		public static void LogTrace(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogTrace(100, ex, edi.UserMessage, edi.Params);
				
			}
			else
			{
				logger.LogTrace(100, edi.UserMessage, edi.Params);
				
			}
		}
		public static void LogDebug(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogDebug(200, ex, edi.UserMessage, edi.Params);
				
			}
			else
			{
				logger.LogDebug(200, edi.UserMessage, edi.Params);
				
			}
		}
		public static void LogInformation(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogInformation(300, ex, edi.UserMessage, edi.Params);
				
			}
			else
			{
				logger.LogInformation(300, edi.UserMessage, edi.Params);
				
			}
		}

		public static void LogWarning(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogWarning(400, ex, edi.UserMessage, edi.Params);
			}
			else
			{
				logger.LogWarning(400, edi.UserMessage, edi.Params);

			}
		}

		public static void LogError(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogError(500, ex, edi.UserMessage, edi.Params);
			}
			else
			{
				logger.LogError(500, edi.UserMessage, edi.Params);

			}
		}

		public static void LogCritical(this ILogger logger, EventDataItem edi, Exception ex = null)
		{
			if (ex != null)
			{
				logger.LogCritical(600, ex, edi.UserMessage, edi.Params);
			}
			else
			{
				logger.LogCritical(600, edi.UserMessage, edi.Params);
				
			}
		}
	}
}
