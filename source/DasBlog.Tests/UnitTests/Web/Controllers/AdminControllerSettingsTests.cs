using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using DasBlog.Web.Mappers;
using DasBlog.Web.Models.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Controllers
{
	public class AdminControllerSettingsTests
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void Settings_Post_BlogFeaturesEnabled_PersistsCommentsDisabled()
		{
			var savedSiteConfig = SaveSettings(enableBlogFeatures: true, enableComments: false);

			Assert.True(savedSiteConfig.EnableBlogFeatures);
			Assert.False(savedSiteConfig.EnableComments);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Settings_Post_BlogFeaturesDisabled_PersistsCommentsDisabled()
		{
			var savedSiteConfig = SaveSettings(enableBlogFeatures: false, enableComments: true);

			Assert.False(savedSiteConfig.EnableBlogFeatures);
			Assert.False(savedSiteConfig.EnableComments);
		}

		private static SiteConfig SaveSettings(bool enableBlogFeatures, bool enableComments)
		{
			var dasBlogSettings = new Mock<IDasBlogSettings>();
			dasBlogSettings.SetupGet(settings => settings.SiteConfiguration).Returns(new SiteConfig());
			dasBlogSettings.SetupGet(settings => settings.MetaTags).Returns(new MetaTags());

			var savedSiteConfig = (SiteConfig)null;
			var fileSystemBinaryManager = new Mock<IFileSystemBinaryManager>();
			fileSystemBinaryManager
				.Setup(manager => manager.SaveSiteConfig(It.IsAny<SiteConfig>()))
				.Callback<SiteConfig>(config => savedSiteConfig = config)
				.Returns(true);
			fileSystemBinaryManager.Setup(manager => manager.SaveMetaConfig(It.IsAny<MetaTags>())).Returns(true);

			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(manager => manager.GetAllEntries()).Returns(new EntryCollection());
			blogManager.Setup(manager => manager.GetCategories()).Returns(new CategoryCacheEntryCollection());

			var mapperConfiguration = new MapperConfiguration(
				configuration => configuration.AddProfile<ProfileSettings>(),
				NullLoggerFactory.Instance);
			var mastodonSettingsResolver = new Mock<IMastodonSettingsResolver>();
			var controller = new AdminController(
				dasBlogSettings.Object,
				mastodonSettingsResolver.Object,
				fileSystemBinaryManager.Object,
				mapperConfiguration.CreateMapper(),
				blogManager.Object,
				Mock.Of<ICommentManager>(),
				Mock.Of<IHostApplicationLifetime>(),
				Mock.Of<ILogger<AdminController>>(),
				Mock.Of<IMemoryCache>(),
				Mock.Of<ISiteSecurityManager>());
			controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

			controller.Settings(new DasBlogSettingsViewModel
			{
				MetaConfig = new MetaViewModel(),
				SiteConfig = new SiteViewModel
				{
					EnableBlogFeatures = enableBlogFeatures,
					EnableComments = enableComments
				}
			});

			Assert.NotNull(savedSiteConfig);
			return savedSiteConfig;
		}
	}
}
