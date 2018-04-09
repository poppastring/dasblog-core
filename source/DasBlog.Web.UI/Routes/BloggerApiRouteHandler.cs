using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DasBlog.Web;
using newtelligence.DasBlog.Runtime;

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
