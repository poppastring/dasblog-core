using DasBlog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Web.Routing
{
	public class TitlePermaLinkUniqueConstraint : IRouteConstraint
	{
		private readonly bool _whenEnabled;

		public TitlePermaLinkUniqueConstraint(string whenEnabled)
		{
			_whenEnabled = bool.Parse(whenEnabled);
		}

		public bool Match(HttpContext httpContext, IRouter route, string routeKey,
			RouteValueDictionary values, RouteDirection routeDirection)
		{
			var settings = httpContext.RequestServices.GetRequiredService<IDasBlogSettings>();
			return settings.SiteConfiguration.EnableTitlePermaLinkUnique == _whenEnabled;
		}
	}
}
