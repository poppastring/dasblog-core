using System;
using System.IO;
using System.Collections.Generic;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using DasBlog.Core.Services.GoogleSiteMap;
using DasBlog.Services;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SiteManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;

        public SiteManagerTest()
        {
			var rootdir = Directory.GetCurrentDirectory();

			settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupAllProperties();
			siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
			siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.Title).Returns("Test Blog");
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
		}

        private SiteManager CreateManager()
        {
            return new SiteManager(settingsMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetGoogleSiteMap_ReturnsUrlSet()
        {
            var manager = CreateManager();
            var result = manager.GetGoogleSiteMap();
            Assert.NotNull(result);
			Assert.Equal("daily", result.url[0].changefreq.ToString());
        }
    }
}
