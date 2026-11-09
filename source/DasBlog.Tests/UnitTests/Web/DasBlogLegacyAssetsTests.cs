using System.Threading.Tasks;
using DasBlog.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web
{
	public class DasBlogLegacyAssetsTests
	{
		[Theory]
		[InlineData("/lib/bootstrap/5.3.3/css/bootstrap.min.css", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.3/css/bootstrap.min.css")]
		[InlineData("/lib/bootstrap/5.3.3/js/bootstrap.bundle.min.js", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.3/js/bootstrap.bundle.min.js")]
		[InlineData("/lib/bootstrap/5.3.3/css/bootstrap.min.css.map", "https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.3/css/bootstrap.min.css.map")]
		[InlineData("/lib/bootswatch/5.3.3/darkly/bootstrap.min.css", "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.3/darkly/bootstrap.min.css")]
		[InlineData("/lib/bootswatch/5.3.3/flatly/bootstrap.min.css", "https://cdnjs.cloudflare.com/ajax/libs/bootswatch/5.3.3/flatly/bootstrap.min.css")]
		[InlineData("/lib/font-awesome/6.7.2/css/all.css", "https://cdn.jsdelivr.net/gh/FortAwesome/Font-Awesome@6.7.2/css/all.css")]
		[InlineData("/lib/font-awesome/6.7.2/js/all.min.js", "https://cdn.jsdelivr.net/gh/FortAwesome/Font-Awesome@6.7.2/js/all.min.js")]
		[InlineData("/lib/font-awesome/6.7.2/webfonts/fa-solid-900.woff2", "https://cdn.jsdelivr.net/gh/FortAwesome/Font-Awesome@6.7.2/webfonts/fa-solid-900.woff2")]
		[InlineData("/lib/font-awesome/6.7.2/svgs/solid/house.svg", "https://cdn.jsdelivr.net/gh/FortAwesome/Font-Awesome@6.7.2/svgs/solid/house.svg")]
		public async Task UseDasBlogLegacyAssets_ShippedAsset_RedirectsToPinnedEquivalent(string path, string destination)
		{
			var context = await Request(path);

			Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
			Assert.Equal(destination, context.Response.Headers.Location.ToString());
		}

		[Theory]
		[InlineData("GET", "")]
		[InlineData("HEAD", "")]
		[InlineData("GET", "/blog")]
		[InlineData("HEAD", "/blog")]
		public async Task UseDasBlogLegacyAssets_QueryAndPathBase_PreservesQueryForGetAndHead(string method, string pathBase)
		{
			var context = await Request("/lib/bootstrap/5.3.3/js/bootstrap.bundle.min.js", method, pathBase, "?v=existing%20theme&cache=1");

			Assert.Equal(StatusCodes.Status302Found, context.Response.StatusCode);
			Assert.Equal("https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.3/js/bootstrap.bundle.min.js?v=existing%20theme&cache=1", context.Response.Headers.Location.ToString());
		}

		[Theory]
		[InlineData("/lib/jquery/3.5.1/jquery.min.js", "GET")]
		[InlineData("/theme/custom/custom.css", "GET")]
		[InlineData("/lib/bootstrap/5.3.2/css/bootstrap.min.css", "GET")]
		[InlineData("/lib/bootstrap/5.3.30/css/bootstrap.min.css", "GET")]
		[InlineData("/lib/font-awesome/6.7.20/css/all.css", "GET")]
		[InlineData("/lib/bootstrap/5.3.3", "GET")]
		[InlineData("/lib/bootstrap/5.3.3/", "GET")]
		[InlineData("/lib/bootstrap/5.3.3/js/bootstrap.bundle.min.js", "POST")]
		[InlineData("/lib/font-awesome/6.7.2/css/all.css", "OPTIONS")]
		public async Task UseDasBlogLegacyAssets_UnrelatedPathOrMethod_ContinuesPipeline(string path, string method)
		{
			var context = await Request(path, method);

			Assert.Equal(StatusCodes.Status418ImATeapot, context.Response.StatusCode);
			Assert.False(context.Response.Headers.ContainsKey("Location"));
		}

		private static async Task<HttpContext> Request(string path, string method = "GET", string pathBase = "", string query = "")
		{
			using var services = new ServiceCollection().BuildServiceProvider();
			var app = new ApplicationBuilder(services);
			app.UseDasBlogLegacyAssets();
			app.Run(context =>
			{
				context.Response.StatusCode = StatusCodes.Status418ImATeapot;
				return Task.CompletedTask;
			});
			var context = new DefaultHttpContext { RequestServices = services };
			context.Request.Method = method;
			context.Request.Path = path;
			context.Request.PathBase = pathBase;
			context.Request.QueryString = new QueryString(query);

			await app.Build()(context);

			return context;
		}
	}
}
