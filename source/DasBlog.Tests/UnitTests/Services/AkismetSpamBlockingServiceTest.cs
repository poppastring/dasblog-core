using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class AkismetSpamBlockingServiceTest
	{
		private readonly Mock<ISiteConfig> siteConfigMock = new();

		private AkismetSpamBlockingService CreateService()
		{
			siteConfigMock.Object.Root ??= "https://example.com/";
			return new AkismetSpamBlockingService(siteConfigMock.Object, NullLogger<AkismetSpamBlockingService>.Instance);
		}

		private void Configure(bool moderationEnabled, string apiKey)
		{
			siteConfigMock.SetupAllProperties();
			siteConfigMock.Object.EnableSpamModeration = moderationEnabled;
			siteConfigMock.Object.AkismetAPIKey = apiKey;
			siteConfigMock.Object.Root = "https://example.com/";
		}

		[Fact]
		public void IsEnabled_NoApiKey_ReturnsFalse()
		{
			Configure(moderationEnabled: true, apiKey: null);
			Assert.False(CreateService().IsEnabled);
		}

		[Fact]
		public void IsEnabled_EmptyApiKey_ReturnsFalse()
		{
			Configure(moderationEnabled: true, apiKey: "   ");
			Assert.False(CreateService().IsEnabled);
		}

		[Fact]
		public void IsEnabled_ModerationDisabled_ReturnsFalse()
		{
			Configure(moderationEnabled: false, apiKey: "valid-key");
			Assert.False(CreateService().IsEnabled);
		}

		[Fact]
		public void IsEnabled_BothConfigured_ReturnsTrue()
		{
			Configure(moderationEnabled: true, apiKey: "valid-key");
			Assert.True(CreateService().IsEnabled);
		}

		[Fact]
		public async Task IsSpamAsync_Disabled_ReturnsFalseWithoutNetworkCall()
		{
			// When disabled, IsSpamAsync must short-circuit before any AkismetClient is created.
			// Reaching the network would either throw or hang in a unit-test environment.
			Configure(moderationEnabled: false, apiKey: null);
			var feedback = new Mock<IFeedback>().Object;

			Assert.False(await CreateService().IsSpamAsync(feedback));
		}

		[Fact]
		public void ReportSpam_Disabled_DoesNothing()
		{
			Configure(moderationEnabled: false, apiKey: null);
			var feedback = new Mock<IFeedback>().Object;

			// Should not throw.
			CreateService().ReportSpam(feedback);
		}

		[Fact]
		public void ReportNotSpam_Disabled_DoesNothing()
		{
			Configure(moderationEnabled: false, apiKey: null);
			var feedback = new Mock<IFeedback>().Object;

			// Should not throw.
			CreateService().ReportNotSpam(feedback);
		}
	}
}
