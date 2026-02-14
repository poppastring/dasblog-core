using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Services.ConfigFile.Interfaces;
using Moq;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class ActivityPubManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<IBlogDataService> dataServiceMock;
        public ActivityPubManagerTest()
        {
            var rootdir = Directory.GetCurrentDirectory();

            settingsMock = new Mock<IDasBlogSettings>();
            settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			settingsMock.Setup(s => s.GetConfiguredTimeZone()).Returns(DateTimeZone.Utc);
			siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
            siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
            siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.DisplayTimeZoneIndex).Returns(0);
			siteConfigMock.SetupGet(c => c.AdjustDisplayTimeZone).Returns(false);
			siteConfigMock.SetupGet(c => c.MastodonAccount).Returns("testuser");
            siteConfigMock.SetupGet(c => c.MastodonServerUrl).Returns("https://mastodon.example.com");
            settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
            dataServiceMock = new Mock<IBlogDataService>();

            dataServiceMock.Setup(d => d.GetEntriesForMonth(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>())).Returns(new EntryCollection());
            var entryForPage = new EntryCollection();
            entryForPage.Add(new Entry { Title = "Test Entry", EntryId = "test-id" });
            dataServiceMock.Setup(d => d.GetEntriesForMonth(It.Is<DateTime>(dt => dt.Month == 7), It.IsAny<DateTimeZone>(), It.IsAny<string>())).Returns(entryForPage);
        }

        private ActivityPubManager CreateManager()
        {
            return new ActivityPubManager(settingsMock.Object, dataServiceMock.Object);
        }

		private ArchiveManager CreateArchiveManager()
		{
			return new ArchiveManager(settingsMock.Object, dataServiceMock.Object);
		}

		[Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetUser_ReturnsUser()
        {
            var manager = CreateManager();
            var result = manager.GetUser();
            Assert.NotNull(result);
            Assert.Equal("https://www.w3.org/ns/activitystreams", result.Context);
        }

        [Fact]
        public void GetUserPage_ReturnsUserPage()
        {
            var manager = CreateManager();
			var archivemanager = CreateArchiveManager();
			var entries = archivemanager.GetEntriesForYear(new DateTime(2003, 7, 31), "");
			Assert.NotNull(entries);

			var result = manager.GetUserPage(entries);
            Assert.NotNull(result);
            Assert.Equal("OrderedCollectionPage", result.Type);
            Assert.NotEmpty(result.OrderItems);
        }

        [Fact]
        public void WebFinger_ReturnsWebFinger()
        {
            var manager = CreateManager();
            var result = manager.WebFinger("testuser");
            Assert.NotNull(result);
            Assert.Contains("acct:testuser@mastodon.example.com", result.Subject);
        }

        [Fact]
        public void WebFinger_ReturnsNull_WhenMissingConfig()
        {
            var manager = CreateManager();
            var result = manager.WebFinger("testuser");
            Assert.NotNull(result);
			Assert.Equal("acct:testuser@mastodon.example.com", result.Subject);
		}
    }
}
