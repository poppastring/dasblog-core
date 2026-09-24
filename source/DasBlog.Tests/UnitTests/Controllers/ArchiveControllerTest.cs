using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class ArchiveControllerTest
	{
		[Fact]
		public void Archive_YearOutsideDateTimeRange_ReturnsNotFound()
		{
			var controller = CreateController(Mock.Of<IArchiveManager>(), new MemoryCache(new MemoryCacheOptions()));

			var result = controller.Archive(23323);

			Assert.IsType<NotFoundResult>(result);
		}

		[Theory]
		[InlineData(2025, 2, 29)]
		[InlineData(2025, 13, 1)]
		[InlineData(2025, 1, 32)]
		public void Archive_InvalidCalendarDate_ReturnsNotFound(int year, int month, int day)
		{
			var controller = CreateController(Mock.Of<IArchiveManager>(), new MemoryCache(new MemoryCacheOptions()));

			var result = controller.Archive(year, month, day);

			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public void ArchiveAll_CachedViewModel_DoesNotReloadArchiveEntries()
		{
			var archiveManager = new Mock<IArchiveManager>();
			var memoryCache = new MemoryCache(new MemoryCacheOptions());
			var cachedModel = new ArchiveListViewModel();
			memoryCache.Set("CACHEKEY_ARCHIVE", cachedModel);
			var controller = CreateController(archiveManager.Object, memoryCache);

			var result = controller.ArchiveAll();

			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Same(cachedModel, viewResult.Model);
			Assert.Equal("Complete Archive - My DasBlog!", controller.ViewData["PageTitle"]);
			archiveManager.Verify(manager => manager.GetDaysWithEntries(), Times.Never);
			archiveManager.Verify(manager => manager.GetEntriesForYear(It.IsAny<System.DateTime>(), It.IsAny<string>()), Times.Never);
		}

		private static ArchiveController CreateController(IArchiveManager archiveManager, IMemoryCache memoryCache)
		{
			var httpContext = new DefaultHttpContext();
			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(x => x.SiteConfiguration).Returns(new SiteConfig
			{
				Copyright = "DasBlog",
				EnableBlogFeatures = true,
				Root = "http://localhost/",
				Title = "My DasBlog!"
			});
			settings.SetupGet(x => x.MetaTags).Returns(new MetaTags());

			return new ArchiveController(
				archiveManager,
				Mock.Of<ICommentManager>(),
				new HttpContextAccessor { HttpContext = httpContext },
				Mock.Of<IMapper>(),
				Mock.Of<ILogger<ArchiveController>>(),
				settings.Object,
				memoryCache)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = httpContext
				}
			};
		}
	}
}
