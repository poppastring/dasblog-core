using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using DasBlog.Web.Mappers;
using DasBlog.Web.Models.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

		[Theory]
		[InlineData("smtp.sendgrid.net")]
		[InlineData("smtp-relay.brevo.com")]
		[InlineData("smtp.example.com")]
		[Trait("Category", "UnitTest")]
		public void Settings_LegacyCustomSmtp_PreservesConfigurationWhenSaving(string server)
		{
			var existing = new SiteConfig
			{
				SmtpServer = server,
				SmtpPort = 2525,
				SmtpUserName = "legacy-user",
				SmtpPassword = "existing-password",
				EnableSmtpAuthentication = false,
				UseSSLForSMTP = false
			};
			var controller = CreateController(existing, out var fileSystemBinaryManager);
			var view = Assert.IsType<ViewResult>(controller.Settings());
			var model = Assert.IsType<DasBlogSettingsViewModel>(view.Model);
			Assert.Equal(MailProviderType.Custom, model.SiteConfig.MailProvider);
			Assert.Empty(model.SiteConfig.Validate(new ValidationContext(model.SiteConfig)));

			model.SiteConfig.SmtpPassword = null;
			controller.Settings(model);

			fileSystemBinaryManager.Verify(manager => manager.SaveSiteConfig(It.Is<SiteConfig>(saved =>
				saved.MailProvider == MailProviderType.Custom && saved.SmtpServer == server
				&& saved.SmtpPort == 2525 && saved.SmtpUserName == "legacy-user"
				&& saved.SmtpPassword == "existing-password" && !saved.EnableSmtpAuthentication
				&& !saved.UseSSLForSMTP)), Times.Once);
		}

		[Theory]
		[InlineData(MailProviderType.SendGrid, null, false)]
		[InlineData(MailProviderType.Brevo, null, false)]
		[InlineData(MailProviderType.SendGrid, "existing-password", true)]
		[InlineData(MailProviderType.Brevo, "existing-password", true)]
		[Trait("Category", "UnitTest")]
		public void Settings_PostSelectedProvider_ValidatesEffectivePassword(MailProviderType provider, string existingPassword, bool shouldSave)
		{
			var controller = CreateController(new SiteConfig { SmtpPassword = existingPassword }, out var fileSystemBinaryManager);
			var model = new DasBlogSettingsViewModel
			{
				MetaConfig = new MetaViewModel(),
				SiteConfig = new SiteViewModel { MailProvider = provider }
			};

			controller.Settings(model);

			if (shouldSave)
			{
				fileSystemBinaryManager.Verify(manager => manager.SaveSiteConfig(It.Is<SiteConfig>(saved => saved.SmtpPassword == existingPassword)), Times.Once);
			}
			else
			{
				Assert.True(controller.ModelState.ContainsKey("SiteConfig.SmtpPassword"));
				fileSystemBinaryManager.Verify(manager => manager.SaveSiteConfig(It.IsAny<SiteConfig>()), Times.Never);
			}
		}

		private static SiteConfig SaveSettings(bool enableBlogFeatures, bool enableComments)
		{
			var controller = CreateController(new SiteConfig(), out var fileSystemBinaryManager);
			SiteConfig savedSiteConfig = null;
			fileSystemBinaryManager.Setup(manager => manager.SaveSiteConfig(It.IsAny<SiteConfig>()))
				.Callback<SiteConfig>(config => savedSiteConfig = config).Returns(true);

			controller.Settings(new DasBlogSettingsViewModel
			{
				MetaConfig = new MetaViewModel(),
				SiteConfig = new SiteViewModel
				{
					EnableBlogFeatures = enableBlogFeatures,
					EnableComments = enableComments
				}
			});

			return Assert.IsType<SiteConfig>(savedSiteConfig);
		}

		private static AdminController CreateController(SiteConfig existing, out Mock<IFileSystemBinaryManager> fileSystemBinaryManager)
		{
			var dasBlogSettings = new Mock<IDasBlogSettings>();
			dasBlogSettings.SetupGet(settings => settings.SiteConfiguration).Returns(existing);
			dasBlogSettings.SetupGet(settings => settings.MetaTags).Returns(new MetaTags());

			fileSystemBinaryManager = new Mock<IFileSystemBinaryManager>();
			fileSystemBinaryManager.Setup(manager => manager.SaveSiteConfig(It.IsAny<SiteConfig>())).Returns(true);
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
			return controller;
		}
	}
}
