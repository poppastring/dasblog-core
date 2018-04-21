using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using DasBlog.Core;

namespace DasBlog.Web.Routes
{
    public class BloggerApiRouteHandler : IRouteHandler
    {
        private IDasBlogSettings _dasBlogSettings;

        public BloggerApiRouteHandler(IDasBlogSettings settings)
        {
            _dasBlogSettings = settings;
        }

        public RequestDelegate GetRequestHandler(HttpContext httpContext, RouteData routeData)
        {
            return null;
            // return new BloggerAPI(_dasBlogSettings);
        }
    }
}
