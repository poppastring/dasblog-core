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
    public class CategoryManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;

        public CategoryManagerTest()
        {
            var rootdir = Directory.GetCurrentDirectory();

            settingsMock = new Mock<IDasBlogSettings>();
            settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
            siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
            siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
            siteConfigMock.SetupGet(c => c.TitlePermalinkSpaceReplacement).Returns("-");
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
        }

        private CategoryManager CreateManager()
        {
            return new CategoryManager(settingsMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetEntries_NoArgs_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetEntries();
            Assert.NotNull(result);
			Assert.True(result.Count > 22);
        }

        [Fact]
        public void GetEntries_WithCategoryAndLang_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetEntries("A Random Mathematical Quotation", "");
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("The Mesurability of the Imeasurable", result[0].Title);

		}

        [Fact]
        public void GetCategoryTitle_ReturnsTitle()
        {
            var manager = CreateManager();
            var result = manager.GetCategoryTitle("a-random-mathematical-quotation");
            Assert.Equal("A Random Mathematical Quotation", result);
        }
    }
}
