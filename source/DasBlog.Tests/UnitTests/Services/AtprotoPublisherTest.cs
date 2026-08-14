using DasBlog.Services;
using DasBlog.Services.Atproto;
using DasBlog.Web.Services;
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
		public async Task EnsurePublicationAsync_DisabledByDefault_ReturnsNull()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(false);

			var loggerMock = new Mock<ILogger<AtprotoPublisher>>();

			var publisher = new AtprotoPublisher(resolverMock.Object, loggerMock.Object);

			var result = await publisher.EnsurePublicationAsync();

			Assert.Null(result);
		}

		[Fact]
		public async Task EnsurePublicationAsync_NoHandle_ReturnsNull()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(true);
			resolverMock.Setup(x => x.GetHandle()).Returns(string.Empty);

			var loggerMock = new Mock<ILogger<AtprotoPublisher>>();

			var publisher = new AtprotoPublisher(resolverMock.Object, loggerMock.Object);

			var result = await publisher.EnsurePublicationAsync();

			Assert.Null(result);
		}

		[Fact]
		public async Task EnsurePublicationAsync_ValidConfig_ReturnsAtUri()
		{
			var resolverMock = new Mock<IAtprotoSettingsResolver>();
			resolverMock.Setup(x => x.IsEnabled()).Returns(true);
			resolverMock.Setup(x => x.GetHandle()).Returns("user.bsky.social");
			resolverMock.Setup(x => x.GetPdsUrl()).Returns("https://bsky.social");
			resolverMock.Setup(x => x.GetPublicationRkey()).Returns("site");

			var loggerMock = new Mock<ILogger<AtprotoPublisher>>();

			var publisher = new AtprotoPublisher(resolverMock.Object, loggerMock.Object);

			var result = await publisher.EnsurePublicationAsync();

			Assert.NotNull(result);
			Assert.Contains("at://", result);
			Assert.Contains("user.bsky.social", result);
			Assert.Contains("site.standard.publication", result);
		}
	}
}
