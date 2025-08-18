using System;
using Xunit;
using Moq;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Core.Security;
using System.Collections.Generic;
using DasBlog.Services.FileManagement;

namespace DasBlog.Tests.UnitTests.Settings
{
    public class DasBlogSettingsTests
    {
        private readonly Mock<IWebHostEnvironment> envMock;
        private readonly Mock<IOptionsMonitor<SiteConfig>> siteConfigMock;
        private readonly Mock<IOptionsMonitor<MetaTags>> metaTagsMock;
        private readonly Mock<IOptionsMonitor<OEmbedProviders>> oembedMock;
        private readonly Mock<ISiteSecurityConfig> securityConfigMock;
        private readonly Mock<IOptions<ConfigFilePathsDataOption>> configFilePathsMock;

        public DasBlogSettingsTests()
        {
            envMock = new Mock<IWebHostEnvironment>();
            siteConfigMock = new Mock<IOptionsMonitor<SiteConfig>>();
            metaTagsMock = new Mock<IOptionsMonitor<MetaTags>>();
            oembedMock = new Mock<IOptionsMonitor<OEmbedProviders>>();
            securityConfigMock = new Mock<ISiteSecurityConfig>();
            configFilePathsMock = new Mock<IOptions<ConfigFilePathsDataOption>>();
        }

        [Fact]
        public void GetBaseUrl_ReturnsRoot_WhenRootIsSet()
        {
            // Arrange
            var siteConfig = new SiteConfig { Root = "https://example.com/", Theme = "default" };
            envMock.Setup(e => e.ContentRootPath).Returns("/app");
            siteConfigMock.Setup(s => s.CurrentValue).Returns(siteConfig);
            metaTagsMock.Setup(m => m.CurrentValue).Returns(new MetaTags());
            oembedMock.Setup(o => o.CurrentValue).Returns(new OEmbedProviders());
            configFilePathsMock.Setup(c => c.Value).Returns(new ConfigFilePathsDataOption { SecurityConfigFilePath = "security.config" });

            var dasBlogSettings = new DasBlogSettings(
                envMock.Object,
                siteConfigMock.Object,
                metaTagsMock.Object,
                oembedMock.Object,
                securityConfigMock.Object,
                configFilePathsMock.Object
            );

            // Act
            var result = dasBlogSettings.GetBaseUrl();

            // Assert
            Assert.Equal("https://example.com/", result);
        }
    }
}
