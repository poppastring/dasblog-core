using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;
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
			try
			{
				if (!context.Request.Headers["User-Agent"].ToString().IsNullOrWhiteSpace())
				{
					logger.LogInformation(new EventDataItem(EventCodes.HttpUserAgent, null, context.Request.Headers["User-Agent"].ToString()));
				}

				if (!context.Request.Headers["Referrer"].ToString().IsNullOrWhiteSpace())
				{
					logger.LogInformation(new EventDataItem(EventCodes.HttpReferrer, null, context.Request.Headers["Referrer"].ToString()));
				}

				if (!context.Request.HttpContext.Connection.RemoteIpAddress.ToString().IsNullOrWhiteSpace())
				{
					logger.LogInformation(new EventDataItem(EventCodes.HttpReferrer, null, context.Request.HttpContext.Connection.RemoteIpAddress.ToString()));
				}
			}
			catch (Exception ex)
			{
				logger.LogError(new EventDataItem(EventCodes.Error, null, string.Format("Logging Agent Exception:{0}", ex.Message)));
			}

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
