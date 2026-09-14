using System;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Site;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class BlogPostControllerTest
	{
		[Fact]
		public void Post_MissingPublicRoute_RendersBuiltInNotFoundPage_With404Status()
		{
			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(x => x.GetBlogPost("missing-route", null)).Returns((Entry)null);
			blogManager.Setup(x => x.GetStaticPage("missing-route")).Returns((StaticPage)null);
			blogManager.Setup(x => x.GetStaticPage("404")).Returns((StaticPage)null);

			var controller = CreateController(blogManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			};

			var result = controller.Post("missing-route", null, null, null);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("NotFound", viewResult.ViewName);
			Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
		}

		[Fact]
		public void Post_MissingPublicRoute_RendersCustom404Page_With404Status()
		{
			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(x => x.GetBlogPost("missing-route", null)).Returns((Entry)null);
			blogManager.Setup(x => x.GetStaticPage("missing-route")).Returns((StaticPage)null);
			blogManager.Setup(x => x.GetStaticPage("404")).Returns(new StaticPage
			{
				Name = "404",
				Content = "<p>Page not found.</p>"
			});

			var controller = CreateController(blogManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			};

			var result = controller.Post("missing-route", null, null, null);

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("LoadStaticPage", viewResult.ViewName);
			var model = Assert.IsType<StaticPageViewModel>(viewResult.Model);
			Assert.Equal("404", model.Name);
			Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
		}

		[Fact]
		public void NotFoundPage_MissingRoute_RendersBuiltInNotFoundPage_With404Status()
		{
			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(x => x.GetBlogPost(It.IsAny<string>(), It.IsAny<DateTime?>())).Returns((Entry)null);
			blogManager.Setup(x => x.GetStaticPage("missing-route")).Returns((StaticPage)null);
			blogManager.Setup(x => x.GetStaticPage("404")).Returns((StaticPage)null);

			var controller = CreateController(blogManager.Object);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext()
			};

			var result = controller.NotFoundPage();

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Equal("NotFound", viewResult.ViewName);
			Assert.Equal(StatusCodes.Status404NotFound, controller.Response.StatusCode);
		}

		private static BlogPostController CreateController(IBlogManager blogManager)
		{
			var settings = new Mock<IDasBlogSettings>();
			var siteConfig = new Mock<ISiteConfig>();
			var metaTags = new Mock<IMetaTags>();
			siteConfig.SetupGet(config => config.Title).Returns("Example Blog");
			siteConfig.SetupGet(config => config.Copyright).Returns("Example Author");
			siteConfig.SetupGet(config => config.Root).Returns("https://example.com/");
			metaTags.SetupGet(tags => tags.MetaDescription).Returns("Example description");
			metaTags.SetupGet(tags => tags.MetaKeywords).Returns("example, blog");
			metaTags.SetupGet(tags => tags.TwitterCreator).Returns("@example");
			metaTags.SetupGet(tags => tags.TwitterImage).Returns("https://example.com/image.png");
			metaTags.SetupGet(tags => tags.TwitterSite).Returns("@example");
			metaTags.SetupGet(tags => tags.TwitterCard).Returns("summary");
			settings.SetupGet(value => value.SiteConfiguration).Returns(siteConfig.Object);
			settings.SetupGet(value => value.MetaTags).Returns(metaTags.Object);
			settings.Setup(value => value.RelativeToRoot(It.IsAny<string>()))
				.Returns((string path) => "https://example.com/" + path.TrimStart('/'));

			var mapper = new Mock<IMapper>();
			mapper.Setup(m => m.Map<StaticPageViewModel>(It.IsAny<StaticPage>()))
				.Returns((StaticPage page) => new StaticPageViewModel
				{
					Name = page.Name,
					Content = page.Content
				});

			return new BlogPostController(
				blogManager,
				Mock.Of<ICommentManager>(),
				Mock.Of<ISearchManager>(),
				new HttpContextAccessor { HttpContext = new DefaultHttpContext() },
				settings.Object,
				mapper.Object,
				Mock.Of<ICategoryManager>(),
				Mock.Of<IFileSystemBinaryManager>(),
				Mock.Of<ILogger<BlogPostController>>(),
				Mock.Of<IBlogPostViewModelCreator>(),
				new MemoryCache(new MemoryCacheOptions()),
				Mock.Of<IExternalEmbeddingHandler>(),
				Mock.Of<IDasBlogPathResolver>());
		}
	}
}
