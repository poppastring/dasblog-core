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
        private Mock<IBlogDataService> dataServiceMock;
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
            dataServiceMock = new Mock<IBlogDataService>();

            var allEntries = new EntryCollection();
            for (int i = 0; i < 23; i++)
            {
                var e = new Entry { Title = "Entry " + i };
                e.CreatedUtc = new DateTime(2003, 7, 31, 0, 0, 0, DateTimeKind.Utc);
                allEntries.Add(e);
            }
            dataServiceMock.Setup(d => d.GetEntries(false)).Returns(allEntries);

            var catEntries = new EntryCollection();
            catEntries.Add(new Entry { Title = "The Mesurability of the Imeasurable" });
            dataServiceMock.Setup(d => d.GetEntriesForCategory("A Random Mathematical Quotation", "")).Returns(catEntries);

            dataServiceMock.Setup(d => d.GetCategoryTitle("a+random+mathematical+quotation")).Returns("A Random Mathematical Quotation");
        }

        private CategoryManager CreateManager()
        {
            return new CategoryManager(siteConfigMock.Object, dataServiceMock.Object);
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
