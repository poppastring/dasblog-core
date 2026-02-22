using System;
using System.IO;
using System.Collections.Generic;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using Microsoft.Extensions.Logging;
using NodaTime;
using DasBlog.Services;
using System.Linq;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class BlogManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<ILogger<BlogManager>> loggerMock;
        private Mock<IBlogDataService> dataServiceMock;

        public BlogManagerTest()
        {
            var rootdir = Directory.GetCurrentDirectory();

            settingsMock = new Mock<IDasBlogSettings>();
            settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
            settingsMock.Setup(s => s.GetContentLookAhead()).Returns(DateTime.Now.AddDays(1));
			
			siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupGet(c => c.FrontPageEntryCount).Returns(5);
            siteConfigMock.SetupGet(c => c.EntriesPerPage).Returns(5);
            siteConfigMock.SetupGet(c => c.EnableTitlePermaLinkUnique).Returns(false);
			siteConfigMock.SetupGet(c => c.ContentLookaheadDays).Returns(1);
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);

            loggerMock = new Mock<ILogger<BlogManager>>();
            dataServiceMock = new Mock<IBlogDataService>();

            var mesurabilityEntry = new Entry
            {
                Title = "The Mesurability of the Imeasurable",
                EntryId = "50d9fa42-a650-459f-85f1-97060ca7e39f",
                Categories = "A Random Mathematical Quotation"
            };
            mesurabilityEntry.CreatedUtc = new DateTime(2004, 3, 27, 7, 26, 24, DateTimeKind.Utc);
            dataServiceMock.Setup(d => d.GetEntry("TheMesurabilityOfTheImeasurable")).Returns(mesurabilityEntry);
            dataServiceMock.Setup(d => d.GetEntryForEdit("50d9fa42-a650-459f-85f1-97060ca7e39f")).Returns(mesurabilityEntry);

            var frontPageEntries = new EntryCollection();
            for (int i = 0; i < 5; i++)
            {
                var e = new Entry { Title = "Entry " + i };
                e.CreatedUtc = new DateTime(2003, 7, 31, i, 0, 0, DateTimeKind.Utc);
                frontPageEntries.Add(e);
            }
            dataServiceMock.Setup(d => d.GetEntriesForDay(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(frontPageEntries);

            var allEntries = new EntryCollection();
            allEntries.Add(mesurabilityEntry);
            for (int i = 0; i < 22; i++)
            {
                var e = new Entry { Title = "Entry " + i };
                e.CreatedUtc = new DateTime(2003, 7, 31, 0, 0, 0, DateTimeKind.Utc);
                allEntries.Add(e);
            }
            dataServiceMock.Setup(d => d.GetEntries(false)).Returns(allEntries);
        }

        private BlogManager CreateManager()
        {
            return new BlogManager(loggerMock.Object, settingsMock.Object, dataServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetBlogPost_ByTitle_ReturnsEntry()
        {
            var manager = CreateManager();
            var result = manager.GetBlogPost("TheMesurabilityOfTheImeasurable", null);
            Assert.NotNull(result);
            Assert.Equal("The Mesurability of the Imeasurable", result.Title);
        }

        [Fact]
        public void GetEntryForEdit_ReturnsEntry()
        {
            var manager = CreateManager();
            var result = manager.GetEntryForEdit("50d9fa42-a650-459f-85f1-97060ca7e39f");
            Assert.NotNull(result);
            Assert.Equal("The Mesurability of the Imeasurable", result.Title);
        }

        [Fact]
        public void GetFrontPagePosts_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetFrontPagePosts("");
            Assert.NotNull(result);
			Assert.Equal(5, result.Count);
        }

        [Fact]
        public void GetAllEntries_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetAllEntries();
			Assert.NotNull(result);

			var oldEntries = result
				.Where(e => e.CreatedUtc < DateTime.UtcNow.AddDays(-10))
				.ToList();

			Assert.NotNull(oldEntries);
			Assert.Equal(23, oldEntries.Count);

			Assert.Equal("The Mesurability of the Imeasurable", oldEntries.First().Title);
        }
    }
}
