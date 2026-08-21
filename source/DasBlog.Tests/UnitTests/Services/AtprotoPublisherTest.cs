using DasBlog.Services;
using DasBlog.Services.Atproto;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class AtprotoPublisherTest
	{
		[Fact]
		public async Task PublishPublicationAsync_Disabled_ReturnsNullWithoutCallingAtproto()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(false);
			var repositoryClientMock = new Mock<IAtprotoRepositoryClient>();

			var result = await CreatePublisher(resolverMock.Object, repositoryClientMock.Object, new Mock<IDasBlogSettings>().Object).PublishPublicationAsync();

			Assert.Null(result);
			repositoryClientMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task PublishPublicationAsync_MissingHandle_ReturnsNullWithoutCallingAtproto()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(true);
			resolverMock.Setup(x => x.GetHandle()).Returns(string.Empty);
			var repositoryClientMock = new Mock<IAtprotoRepositoryClient>();

			var result = await CreatePublisher(resolverMock.Object, repositoryClientMock.Object, new Mock<IDasBlogSettings>().Object).PublishPublicationAsync();

			Assert.Null(result);
			repositoryClientMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task PublishPublicationAsync_MissingAppPassword_ReturnsNullWithoutCallingAtproto()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(true);
			resolverMock.Setup(x => x.GetHandle()).Returns("user.bsky.social");
			resolverMock.Setup(x => x.GetAppPassword()).Returns(string.Empty);
			var repositoryClientMock = new Mock<IAtprotoRepositoryClient>();

			var result = await CreatePublisher(resolverMock.Object, repositoryClientMock.Object, new Mock<IDasBlogSettings>().Object).PublishPublicationAsync();

			Assert.Null(result);
			repositoryClientMock.VerifyNoOtherCalls();
		}

		[Fact]
		public async Task PublishPublicationAsync_ValidConfig_PublishesToAuthenticatedDid()
		{
			var resolverMock = CreateConfiguredResolver();
			var repositoryClientMock = new Mock<IAtprotoRepositoryClient>();
			repositoryClientMock
				.Setup(x => x.CreateSessionAsync("https://bsky.social", "user.bsky.social", "app-password", default))
				.ReturnsAsync(new AtprotoSession { Did = "did:plc:example", AccessJwt = "access-token", PdsUrl = "https://bsky.social" });
			var siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupGet(x => x.Title).Returns("Example Blog");
			siteConfigMock.SetupGet(x => x.Description).Returns("Example description");
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.GetBaseUrl()).Returns("https://example.com/");
			settingsMock.SetupGet(x => x.SiteConfiguration).Returns(siteConfigMock.Object);

			var result = await CreatePublisher(resolverMock.Object, repositoryClientMock.Object, settingsMock.Object).PublishPublicationAsync();

			Assert.Equal("at://did:plc:example/site.standard.publication/site", result);
			repositoryClientMock.Verify(x => x.PutPublicationAsync(
				It.Is<AtprotoSession>(session => session.Did == "did:plc:example"),
				"site",
				It.Is<AtprotoPublication>(publication => publication.Url == "https://example.com" && publication.Name == "Example Blog" && publication.Description == "Example description"),
				default), Times.Once);
		}

		[Fact]
		public async Task DeletePublicationAsync_ValidConfig_DeletesConfiguredRecord()
		{
			var resolverMock = CreateConfiguredResolver();
			var repositoryClientMock = new Mock<IAtprotoRepositoryClient>();
			repositoryClientMock
				.Setup(x => x.CreateSessionAsync("https://bsky.social", "user.bsky.social", "app-password", default))
				.ReturnsAsync(new AtprotoSession { Did = "did:plc:example", AccessJwt = "access-token", PdsUrl = "https://bsky.social" });

			await CreatePublisher(resolverMock.Object, repositoryClientMock.Object, new Mock<IDasBlogSettings>().Object).DeletePublicationAsync();

			repositoryClientMock.Verify(x => x.DeletePublicationAsync(
				It.Is<AtprotoSession>(session => session.Did == "did:plc:example"), "site", default), Times.Once);
		}

		private static Mock<IAtprotoSettingsResolver> CreateConfiguredResolver()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(true);
			resolverMock.Setup(x => x.GetHandle()).Returns("user.bsky.social");
			resolverMock.Setup(x => x.GetPdsUrl()).Returns("https://bsky.social");
			resolverMock.Setup(x => x.GetAppPassword()).Returns("app-password");
			resolverMock.Setup(x => x.GetPublicationRkey()).Returns("site");
			return resolverMock;
		}

		private static AtprotoPublisher CreatePublisher(IAtprotoSettingsResolver resolver, IAtprotoRepositoryClient repositoryClient, IDasBlogSettings settings)
		{
			return new AtprotoPublisher(resolver, settings, repositoryClient, new Mock<ILogger<AtprotoPublisher>>().Object);
		}
	}
}
