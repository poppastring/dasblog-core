using System;
using System.IO;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using DasBlog.Services;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SubscriptionManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
		private Mock<IMetaTags> metaTagsMock;
		private Mock<IBlogDataService> blogDataServiceMock;

        public SubscriptionManagerTest()
        {
			var rootdir = Directory.GetCurrentDirectory();

			settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupAllProperties();
			siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupGet(c => c.RssMainEntryCount).Returns(10);
			siteConfigMock.SetupGet(c => c.RssDayCount).Returns(10);
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
			siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.Title).Returns("Test Blog");
			siteConfigMock.SetupGet(c => c.TitlePermalinkSpaceReplacement).Returns("-");
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);

			metaTagsMock = new Mock<IMetaTags>();
			metaTagsMock.SetupAllProperties();
			settingsMock.Setup(s => s.MetaTags).Returns(metaTagsMock.Object);
		}

        private SubscriptionManager CreateManager()
        {
            return new SubscriptionManager(settingsMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetRss_ReturnsRssRoot()
        {
            var manager = CreateManager();
            var result = manager.GetRss();
			Assert.NotNull(result);
			Assert.Equal("2.0", result.Version);
        }

        [Fact]
        public void GetRssCategory_ReturnsRssRoot()
        {
            var manager = CreateManager();
            var result = manager.GetRssCategory("A Random Mathematical Quotation");
			Assert.NotNull(result);
			Assert.Equal(1, result.Channels.Count);
			Assert.Equal("Test Blog - A Random Mathematical Quotation", result.Channels[0].Title);
		}

        [Fact]
        public void GetAtom_ThrowsNotImplementedException()
        {
            var manager = CreateManager();
            Assert.Throws<NotImplementedException>(() => manager.GetAtom());
        }

        [Fact]
        public void GetAtomCategory_ThrowsNotImplementedException()
        {
            var manager = CreateManager();
            Assert.Throws<NotImplementedException>(() => manager.GetAtomCategory("TestCategory"));
        }

        [Fact]
        public void GetRsd_ReturnsRsdRoot()
        {
            var manager = CreateManager();
            var result = manager.GetRsd();
            Assert.NotNull(result);
			Assert.Equal("https://github.com/poppastring/dasblog-core", result.Services[0].EngineLink);
		}
    }
}
