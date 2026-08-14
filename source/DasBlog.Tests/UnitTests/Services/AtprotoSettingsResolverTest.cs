using DasBlog.Services;
using DasBlog.Services.Atproto;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Services;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class AtprotoSettingsResolverTest
	{
		[Fact]
		public void IsEnabled_DisabledByDefault()
		{
			var metaTags = new MetaTags { AtprotoEnabled = false };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.False(resolver.IsEnabled());
		}

		[Fact]
		public void IsEnabled_TrueWhenSet()
		{
			var metaTags = new MetaTags { AtprotoEnabled = true };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.True(resolver.IsEnabled());
		}

		[Fact]
		public void GetHandle_ReturnsEmptyByDefault()
		{
			var metaTags = new MetaTags { AtprotoHandle = null };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal(string.Empty, resolver.GetHandle());
		}

		[Fact]
		public void GetHandle_ReturnsConfiguredValue()
		{
			var metaTags = new MetaTags { AtprotoHandle = "user.bsky.social" };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal("user.bsky.social", resolver.GetHandle());
		}

		[Fact]
		public void GetPdsUrl_DefaultsToBluesky()
		{
			var metaTags = new MetaTags { AtprotoPdsUrl = null };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal("https://bsky.social", resolver.GetPdsUrl());
		}

		[Fact]
		public void GetPdsUrl_ReturnsConfiguredValue()
		{
			var metaTags = new MetaTags { AtprotoPdsUrl = "https://pds.example.com" };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal("https://pds.example.com", resolver.GetPdsUrl());
		}

		[Fact]
		public void GetPublicationRkey_DefaultsToSite()
		{
			var metaTags = new MetaTags { AtprotoPublicationRkey = null };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal("site", resolver.GetPublicationRkey());
		}

		[Fact]
		public void GetPublicationRkey_ReturnsConfiguredValue()
		{
			var metaTags = new MetaTags { AtprotoPublicationRkey = "myblog" };
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(x => x.MetaTags).Returns(metaTags);

			var resolver = new AtprotoSettingsResolver(settingsMock.Object);

			Assert.Equal("myblog", resolver.GetPublicationRkey());
		}
	}
}
