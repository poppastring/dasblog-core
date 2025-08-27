﻿using System;
using System.IO;
using System.Collections.Generic;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using DasBlog.Services;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class ArchiveManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<IBlogDataService> blogDataServiceMock;

        public ArchiveManagerTest()
        {
            var rootdir = Directory.GetCurrentDirectory();

            settingsMock = new Mock<IDasBlogSettings>();
            settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			settingsMock.Setup(s => s.GetConfiguredTimeZone()).Returns(DateTimeZone.Utc);

			siteConfigMock = new Mock<ISiteConfig>();
            siteConfigMock.SetupAllProperties();
            siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
            siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
            siteConfigMock.SetupGet(c => c.FrontPageDayCount).Returns(5);
			siteConfigMock.SetupGet(c => c.DisplayTimeZoneIndex).Returns(0);
			siteConfigMock.SetupGet(c => c.AdjustDisplayTimeZone).Returns(false);
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
        }

        private ArchiveManager CreateManager()
        {
            return new ArchiveManager(settingsMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetEntriesForMonth_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetEntriesForMonth(new DateTime(2003,7,31), "");
            Assert.NotNull(result);
            Assert.Equal(6, result.Count);
            Assert.Equal("Synchronized Management Capability (SMC)", result[0].Title);
        }

        [Fact]
        public void GetEntriesForYear_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetEntriesForYear(new DateTime(2003, 7, 31), "");
			Assert.NotNull(result);
			Assert.True(result.Count > 9);
			Assert.Equal("Synchronized Management Capability (SMC)", result[0].Title);
		}

        [Fact]
        public void GetDaysWithEntries_ReturnsDates()
        {
            var manager = CreateManager();
            var result = manager.GetDaysWithEntries();
            Assert.NotNull(result);
			Assert.Equal(10, result.Count);
			Assert.Contains(new DateTime(2004, 3, 14), result);
		}

		[Fact]
        public void GetEntriesForDay_ReturnsEntries()
        {
            var manager = CreateManager();
            var result = manager.GetEntriesForDay(new DateTime(2003, 7, 31), "");
            Assert.NotNull(result);
			Assert.Equal(5, result.Count);
			Assert.Equal("Synchronized Management Capability (SMC)", result[0].Title);
        }
    }
}
