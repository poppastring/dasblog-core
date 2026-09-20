using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class SiteControllerTest
	{
		[Fact]
		public void RobotsTxt_HttpsConfiguredRoot_UsesConfiguredRootWhenRequestIsHttp()
		{
			var controller = CreateController("https://example.com/blog/");
			controller.Request.Scheme = "http";
			controller.Request.Host = new HostString("example.com");
			controller.Request.PathBase = "/blog";

			var result = controller.RobotsTxt();

			var content = Assert.IsType<ContentResult>(result);
			Assert.Contains("Sitemap: https://example.com/blog/sitemap.xml", content.Content);
			Assert.Contains("Disallow: /blog/account/login", content.Content);
		}

		[Fact]
		public void RobotsTxt_NoConfiguredRoot_UsesRequestSchemeHostAndPathBase()
		{
			var controller = CreateController(null);
			controller.Request.Scheme = "http";
			controller.Request.Host = new HostString("internal.example.com", 8080);
			controller.Request.PathBase = "/blog";

			var result = controller.RobotsTxt();

			var content = Assert.IsType<ContentResult>(result);
			Assert.Contains("Sitemap: http://internal.example.com:8080/blog/sitemap.xml", content.Content);
		}

		private static SiteController CreateController(string siteRoot)
		{
			var siteConfig = new Mock<ISiteConfig>();
			siteConfig.SetupGet(config => config.Root).Returns(siteRoot);
			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(value => value.SiteConfiguration).Returns(siteConfig.Object);

			return new SiteController(
				Mock.Of<ISiteManager>(),
				Mock.Of<IBlogManager>(),
				settings.Object,
				new MemoryCache(new MemoryCacheOptions()),
				Mock.Of<IMapper>(),
				Mock.Of<ILogger<SiteController>>())
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext()
				}
			};
		}
	}
}
