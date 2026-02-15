using System;
using System.IO;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class FileSystemBinaryManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<IConfigFileService<MetaTags>> metaTagFileServiceMock;
        private Mock<IConfigFileService<OEmbedProviders>> oembedProvidersServiceMock;
        private Mock<IConfigFileService<SiteConfig>> siteConfigFileServiceMock;
        private Mock<IOptions<ConfigFilePathsDataOption>> optionsAccessorMock;
        private Mock<ILoggingDataService> loggingDataServiceMock;

        public FileSystemBinaryManagerTest()
        {
            settingsMock = new Mock<IDasBlogSettings>();
            siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
            siteConfigMock.SetupGet(c => c.CdnFrom).Returns(string.Empty);
            siteConfigMock.SetupGet(c => c.CdnTo).Returns(string.Empty);
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
            settingsMock.Setup(s => s.RelativeToRoot(It.IsAny<string>()))
                .Returns("http://localhost/content/binary/");

            metaTagFileServiceMock = new Mock<IConfigFileService<MetaTags>>();
            oembedProvidersServiceMock = new Mock<IConfigFileService<OEmbedProviders>>();
            siteConfigFileServiceMock = new Mock<IConfigFileService<SiteConfig>>();
            loggingDataServiceMock = new Mock<ILoggingDataService>();

            var configOptions = new ConfigFilePathsDataOption
            {
                BinaryFolder = Path.GetTempPath(),
                BinaryUrlRelative = "content/binary/"
            };
            optionsAccessorMock = new Mock<IOptions<ConfigFilePathsDataOption>>();
            optionsAccessorMock.Setup(o => o.Value).Returns(configOptions);
        }

        private FileSystemBinaryManager CreateManager()
        {
            return new FileSystemBinaryManager(
                settingsMock.Object,
                metaTagFileServiceMock.Object,
                oembedProvidersServiceMock.Object,
                siteConfigFileServiceMock.Object,
                optionsAccessorMock.Object,
                loggingDataServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }
    }
}
