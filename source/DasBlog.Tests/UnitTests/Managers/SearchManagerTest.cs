using System;
using System.Linq;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SearchManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<IBlogDataService> dataServiceMock;

        public SearchManagerTest()
        {
            settingsMock = new Mock<IDasBlogSettings>();
            siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
            settingsMock.Setup(s => s.GetConfiguredTimeZone()).Returns(DateTimeZone.Utc);

            dataServiceMock = new Mock<IBlogDataService>();
        }

        private SearchManager CreateManager()
        {
            return new SearchManager(settingsMock.Object, dataServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void SearchEntries_EmptyString_ReturnsAllEntries()
        {
            var entries = new EntryCollection();
            entries.Add(new Entry { Title = "Entry1" });
            entries.Add(new Entry { Title = "Entry2" });
            dataServiceMock.Setup(d => d.GetEntriesForDay(
                It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()
            )).Returns(entries);

            var manager = CreateManager();
            var result = manager.SearchEntries("", null);
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void SearchEntries_MatchingTerm_ReturnsMatches()
        {
            var entries = new EntryCollection();
            entries.Add(new Entry { Title = "Hello World" });
            entries.Add(new Entry { Title = "Goodbye Moon" });
            dataServiceMock.Setup(d => d.GetEntriesForDay(
                It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(),
                It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()
            )).Returns(entries);

            var manager = CreateManager();
            var result = manager.SearchEntries("Hello", null);
            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.Contains(result.Cast<Entry>(), e => e.Title == "Hello World");
        }
    }
}
