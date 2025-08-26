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

namespace DasBlog.Tests.UnitTests.Managers
{
    public class BlogManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<ILogger<BlogManager>> loggerMock;

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
            siteConfigMock.SetupGet(c => c.TitlePermalinkSpaceReplacement).Returns("-");
			siteConfigMock.SetupGet(c => c.FrontPageEntryCount).Returns(5);
            siteConfigMock.SetupGet(c => c.EntriesPerPage).Returns(5);
            siteConfigMock.SetupGet(c => c.EnableTitlePermaLinkUnique).Returns(false);
			siteConfigMock.SetupGet(c => c.ContentLookaheadDays).Returns(1);
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);

            loggerMock = new Mock<ILogger<BlogManager>>();
        }

        private BlogManager CreateManager()
        {
            return new BlogManager(loggerMock.Object, settingsMock.Object);
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
            Assert.Equal(23, result.Count);
            Assert.Equal("The Mesurability of the Imeasurable", result[0].Title);
        }
    }
}
