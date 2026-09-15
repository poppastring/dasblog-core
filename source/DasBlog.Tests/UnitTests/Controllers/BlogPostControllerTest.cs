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

		[Fact]
		public void Comment_LegacyRoute_RedirectsToCanonicalPostCommentsAnchor()
		{
			var blogManager = new Mock<IBlogManager>();
			var entry = new Entry
			{
				EntryId = Guid.NewGuid().ToString(),
				Title = "Hello world",
				AllowComments = true,
				IsPublic = true
			};
			blogManager.Setup(x => x.GetBlogPost("hello-world", null)).Returns(entry);

			var controller = CreateController(blogManager.Object);
			var result = controller.Comment("hello-world", null, null, null);

			var redirect = Assert.IsType<RedirectResult>(result);
			Assert.True(redirect.Permanent);
			Assert.Equal("https://example.com/hello-world#comments-start", redirect.Url);
		}

		[Fact]
		public void Index_WhenCommentsEnabledAndLegacySettingIsTrue_ShowsCommentsOnAggregatedPosts()
		{
			var entry = new Entry
			{
				EntryId = Guid.NewGuid().ToString(),
				Title = "Hello world",
				AllowComments = true,
				IsPublic = true,
				CreatedUtc = DateTime.UtcNow,
				ModifiedUtc = DateTime.UtcNow,
				Content = "Test post"
			};

			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(x => x.GetFrontPagePosts(It.IsAny<string>())).Returns(new EntryCollection { entry });

			var commentManager = new Mock<ICommentManager>();
			commentManager.Setup(x => x.GetComments(entry.EntryId, false)).Returns(new CommentCollection
			{
				new Comment { EntryId = entry.EntryId, TargetTitle = entry.Title, Author = "Tester", Content = "Looks good" }
			});

			var siteConfig = new Mock<ISiteConfig>();
			siteConfig.SetupGet(x => x.EnableComments).Returns(true);
			siteConfig.SetupGet(x => x.ShowCommentsWhenViewingEntry).Returns(true);
			siteConfig.SetupGet(x => x.ShowItemSummaryInAggregatedViews).Returns(false);
			siteConfig.SetupGet(x => x.Title).Returns("Example Blog");
			siteConfig.SetupGet(x => x.Copyright).Returns("Example Author");
			siteConfig.SetupGet(x => x.Root).Returns("https://example.com/");
			siteConfig.SetupGet(x => x.PostPinnedToHomePage).Returns(string.Empty);

			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(x => x.SiteConfiguration).Returns(siteConfig.Object);
			settings.SetupGet(x => x.MetaTags).Returns(new Mock<IMetaTags>().Object);
			settings.Setup(x => x.GetBaseUrl()).Returns("https://example.com/");
			settings.Setup(x => x.RelativeToRoot(It.IsAny<string>())).Returns((string path) => "https://example.com/" + path.TrimStart('/'));
			settings.Setup(x => x.GetCommentViewUrl(It.IsAny<string>())).Returns((string path) => "https://example.com/" + path.TrimStart('/').TrimEnd('/') + "#comments-start");

			var mapper = new Mock<IMapper>();
			mapper.Setup(m => m.Map<PostViewModel>(It.IsAny<Entry>())).Returns((Entry e) => new PostViewModel
			{
				EntryId = e.EntryId,
				Title = e.Title,
				AllowComments = e.AllowComments,
				CreatedDateTime = e.CreatedUtc,
				ModifiedDateTime = e.ModifiedUtc,
				PermaLink = e.Title,
				Content = e.Content,
				Comments = new ListCommentsViewModel()
			});
			mapper.Setup(m => m.Map<CommentViewModel>(It.IsAny<Comment>())).Returns((Comment c) => new CommentViewModel
			{
				Name = c.Author,
				Text = c.Content
			});

			var controller = new HomeController(
				blogManager.Object,
				commentManager.Object,
				settings.Object,
				mapper.Object,
				Mock.Of<ILogger<HomeController>>(),
				new MemoryCache(new MemoryCacheOptions()),
				Mock.Of<IExternalEmbeddingHandler>());
			controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

			var result = controller.Index();

			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<ListPostsViewModel>(viewResult.Model);
			Assert.True(model.Posts[0].Comments.ShowComments);
			Assert.Single(model.Posts[0].Comments.Comments);
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
			settings.Setup(value => value.GeneratePostUrl(It.IsAny<Entry>()))
				.Returns((Entry entry) => string.Join("-", entry.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries)).ToLowerInvariant());
			settings.Setup(value => value.GetCommentViewUrl(It.IsAny<string>()))
				.Returns((string url) => "https://example.com/" + url.TrimStart('/').TrimEnd('/') + "#comments-start");

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
