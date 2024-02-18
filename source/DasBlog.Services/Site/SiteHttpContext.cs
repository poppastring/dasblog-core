using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Services.Site
{
	public class SiteHttpContext
	{
		private static IHttpContextAccessor m_httpContextAccessor;

		public static HttpContext Current => m_httpContextAccessor.HttpContext;

		public static string AppBaseUrl => $"{Current.Request.Scheme}://{Current.Request.Host}{Current.Request.PathBase}";

		internal static void Configure(IHttpContextAccessor contextAccessor)
		{
			m_httpContextAccessor = contextAccessor;
		}
	}

	public static class HttpContextExtensions
	{
		public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
		{
			SiteHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
			return app;
		}
	}
}
