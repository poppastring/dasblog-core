using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services.ActivityLogs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Quartz.Util;

namespace DasBlog.Web.Services
{
	public class LoggingAgent
	{
		private readonly ILogger<LoggingAgent> logger;
		private readonly RequestDelegate _next;

		public LoggingAgent(RequestDelegate next, ILogger<LoggingAgent> logger)
		{
			_next = next;
			this.logger = logger;
		}

		public async Task Invoke(HttpContext context)
		{
			if (!context.Request.Headers["User-Agent"].ToString().IsNullOrWhiteSpace())
			{
				logger.LogDebug(new EventDataItem(EventCodes.HttpUserAgent, null, context.Request.Headers["User-Agent"].ToString()));
			}

			if (!context.Request.Headers["Referrer"].ToString().IsNullOrWhiteSpace())
			{
				logger.LogDebug(new EventDataItem(EventCodes.HttpReferrer, null, context.Request.Headers["Referrer"].ToString()));
			}

			logger.LogDebug(new EventDataItem(EventCodes.HttpReferrer, null, context.Request.HttpContext.Connection.RemoteIpAddress.ToString()));

			await _next.Invoke(context);
		}
	}

	public static class MiddlewareExtensions
	{
		public static IApplicationBuilder UseLoggingAgent(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<LoggingAgent>();
		}
	}


}
