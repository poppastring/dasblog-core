using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
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
			Assert.Equal("article", controller.ViewData["OgType"]);
		}

		[Fact]
		public void StaticPage_UsesPageSpecificSeoMetadata()
		{
			var settings = new Mock<IDasBlogSettings>();
			var siteConfig = new Mock<ISiteConfig>();
			var metaTags = new Mock<IMetaTags>();
			siteConfig.SetupGet(config => config.Root).Returns("https://example.com/blog/");
			siteConfig.SetupGet(config => config.Title).Returns("Example Blog");
			siteConfig.SetupGet(config => config.Copyright).Returns("Example Author");
			siteConfig.SetupGet(config => config.EnableBlogFeatures).Returns(true);
			metaTags.SetupGet(tags => tags.MetaDescription).Returns("Site description");
			settings.SetupGet(value => value.SiteConfiguration).Returns(siteConfig.Object);
			settings.SetupGet(value => value.MetaTags).Returns(metaTags.Object);
			settings.Setup(value => value.RelativeToRoot("about"))
				.Returns("https://example.com/blog/about");
			var controller = new TestController(settings.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext()
				}
			};
			controller.HttpContext.Request.Path = "/about";

			controller.SetStaticPage(new StaticPageViewModel
			{
				Name = "about",
				Content = "<p>Learn more about this site.</p>"
			});

			Assert.Equal("About - Example Blog", controller.ViewData["PageTitle"]);
			Assert.Equal("about", controller.ViewData["Description"]);
			Assert.Equal("https://example.com/blog/about", controller.ViewData["Canonical"]);
			Assert.Equal("WebPage", controller.ViewData["SchemaType"]);
			Assert.Equal("article", controller.ViewData["OgType"]);
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

			public void SetStaticPage(StaticPageViewModel page)
			{
				StaticPage(page);
			}
		}
	}
}
