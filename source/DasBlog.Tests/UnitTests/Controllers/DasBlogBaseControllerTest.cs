using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class DasBlogBaseControllerTest
	{
		[Fact]
		public void DefaultPage_NamedPage_UsesCurrentPathAsCanonicalUrl()
		{
			var settings = new Mock<IDasBlogSettings>();
			var siteConfig = new Mock<ISiteConfig>();
			var metaTags = new Mock<IMetaTags>();
			siteConfig.SetupGet(config => config.Root).Returns("https://example.com/blog/");
			siteConfig.SetupGet(config => config.Title).Returns("Example Blog");
			settings.SetupGet(value => value.SiteConfiguration).Returns(siteConfig.Object);
			settings.SetupGet(value => value.MetaTags).Returns(metaTags.Object);
			settings.Setup(value => value.RelativeToRoot("archive/2026/9"))
				.Returns("https://example.com/blog/archive/2026/9");
			var controller = new TestController(settings.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext()
				}
			};
			controller.HttpContext.Request.Path = "/archive/2026/9";

			controller.SetDefaultPage("Archive");

			Assert.Equal("https://example.com/blog/archive/2026/9", controller.ViewData["Canonical"]);
		}

		private sealed class TestController : DasBlogBaseController
		{
			public TestController(IDasBlogSettings settings) : base(settings)
			{
			}

			public void SetDefaultPage(string pageTitle)
			{
				DefaultPage(pageTitle);
			}
		}
	}
}
