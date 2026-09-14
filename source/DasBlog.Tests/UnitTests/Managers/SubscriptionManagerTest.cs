using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DasBlog.Core.Security;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Rss.Rsd;
using newtelligence.DasBlog.Runtime;
using DasBlog.Services;
using NodaTime;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SubscriptionManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
		private Mock<IMetaTags> metaTagsMock;
        private Mock<IBlogDataService> dataServiceMock;
        private Mock<ILoggingDataService> loggingServiceMock;
        private Mock<ISiteSecurityConfig> securityConfigMock;

        public SubscriptionManagerTest()
        {
			var rootdir = Directory.GetCurrentDirectory();

			settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupAllProperties();
			siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupGet(c => c.RssMainEntryCount).Returns(10);
			siteConfigMock.SetupGet(c => c.RssDayCount).Returns(10);
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
			siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.Title).Returns("Test Blog");
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
			securityConfigMock = new Mock<ISiteSecurityConfig>();
			securityConfigMock.SetupGet(s => s.Users).Returns(new List<User>());
			settingsMock.Setup(s => s.SecurityConfiguration).Returns(securityConfigMock.Object);
			settingsMock.Setup(s => s.GetUserByEmail(It.IsAny<string>())).Returns((User)null);

			metaTagsMock = new Mock<IMetaTags>();
			metaTagsMock.SetupAllProperties();
			settingsMock.Setup(s => s.MetaTags).Returns(metaTagsMock.Object);
			dataServiceMock = new Mock<IBlogDataService>();
			loggingServiceMock = new Mock<ILoggingDataService>();

			dataServiceMock.Setup(d => d.GetEntriesForDay(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new EntryCollection());
			dataServiceMock.Setup(d => d.GetCategories()).Returns(new CategoryCacheEntryCollection());
		}

        private SubscriptionManager CreateManager()
        {
            return new SubscriptionManager(settingsMock.Object, dataServiceMock.Object, loggingServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetRss_ReturnsRssRoot()
        {
            var manager = CreateManager();
            var result = manager.GetRss();
			Assert.NotNull(result);
			Assert.Equal("2.0", result.Version);
        }

		[Fact]
		public void GetRssCategory_ReturnsRssRoot()
		{
			var manager = CreateManager();
			var result = manager.GetRssCategory("A Random Mathematical Quotation");
			Assert.NotNull(result);
			Assert.Single(result.Channels);
			Assert.Equal("Test Blog - A Random Mathematical Quotation", result.Channels[0].Title);
		}

		[Fact]
		public void GetRss_WhenAuthorMatchesConfiguredUser_PopulatesDcCreator()
		{
			var configuredUser = new User { DisplayName = "Jane Doe", EmailAddress = "jane@example.com", Active = true };
			securityConfigMock.SetupGet(s => s.Users).Returns(new List<User> { configuredUser });
			settingsMock.Setup(s => s.GetUserByEmail("jane@example.com")).Returns(configuredUser);
			dataServiceMock.Setup(d => d.GetEntriesForDay(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(new EntryCollection
				{
					new Entry
					{
						EntryId = "entry-1",
						Title = "Post by Jane",
						Author = "jane@example.com",
						Content = "Hello",
						CreatedUtc = new DateTime(2024, 01, 01, 0, 0, 0, DateTimeKind.Utc),
						IsPublic = true,
						Syndicated = true,
					}
				});

			var manager = CreateManager();
			var result = manager.GetRss();

			var item = Assert.IsType<DasBlog.Services.Rss.Rss20.RssItem>(result.Channels[0].Items[0]);
			var creator = item.anyElements.OfType<System.Xml.XmlElement>().Single(e => e.LocalName == "creator" && e.NamespaceURI == "http://purl.org/dc/elements/1.1/");
			Assert.Equal("Jane Doe", creator.InnerText);
			Assert.Equal("jane@example.com", item.Author);
		}

		[Fact]
		public void GetRss_WhenRawDisplayNameIsPresent_UsesItBeforeEmailLookup()
		{
			var configuredUser = new User { DisplayName = "Jane Doe", EmailAddress = "jane@example.com", Active = true };
			securityConfigMock.SetupGet(s => s.Users).Returns(new List<User> { configuredUser });
			settingsMock.Setup(s => s.GetUser("Jane Doe")).Returns(configuredUser);
			settingsMock.Setup(s => s.GetUserByEmail("jane@example.com")).Returns(configuredUser);
			dataServiceMock.Setup(d => d.GetEntriesForDay(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(new EntryCollection
				{
					new Entry
					{
						EntryId = "entry-2",
						Title = "Post by display name",
						Author = "Jane Doe",
						Content = "Hello",
						CreatedUtc = new DateTime(2024, 01, 02, 0, 0, 0, DateTimeKind.Utc),
						IsPublic = true,
						Syndicated = true,
					}
				});

			var manager = CreateManager();
			var result = manager.GetRss();

			var item = Assert.IsType<DasBlog.Services.Rss.Rss20.RssItem>(result.Channels[0].Items[0]);
			var creator = item.anyElements.OfType<System.Xml.XmlElement>().Single(e => e.LocalName == "creator" && e.NamespaceURI == "http://purl.org/dc/elements/1.1/");
			Assert.Equal("Jane Doe", creator.InnerText);
			Assert.Equal("Jane Doe", item.Author);
		}

		[Fact]
		public void GetRss_WhenAuthorIsUnknown_UsesFallbackAuthorAndEscapesXml()
		{
			dataServiceMock.Setup(d => d.GetEntriesForDay(It.IsAny<DateTime>(), It.IsAny<DateTimeZone>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
				.Returns(new EntryCollection
				{
					new Entry
					{
						EntryId = "entry-3",
						Title = "Post with special author",
						Author = "special & <author>@example.com",
						Content = "Hello",
						CreatedUtc = new DateTime(2024, 01, 03, 0, 0, 0, DateTimeKind.Utc),
						IsPublic = true,
						Syndicated = true,
					}
				});

			var manager = CreateManager();
			var result = manager.GetRss();

			var item = Assert.IsType<DasBlog.Services.Rss.Rss20.RssItem>(result.Channels[0].Items[0]);
			var creator = item.anyElements.OfType<System.Xml.XmlElement>().Single(e => e.LocalName == "creator" && e.NamespaceURI == "http://purl.org/dc/elements/1.1/");
			Assert.Equal("special & <author>@example.com", creator.InnerText);
			Assert.Equal("special & <author>@example.com", item.Author);

			var document = new System.Xml.XmlDocument();
			var encodedCreator = document.CreateElement("dc", "creator", "http://purl.org/dc/elements/1.1/");
			encodedCreator.InnerText = creator.InnerText;
			Assert.Contains("special &amp; &lt;author&gt;@example.com", encodedCreator.OuterXml);
		}

		[Fact]
		public void GetAtom_ReturnsAtomRoot()
        {
            var manager = CreateManager();
            var result = manager.GetAtom();
            Assert.NotNull(result);
            Assert.Equal("Test Blog", result.Title.Text);
        }

        [Fact]
        public void GetAtomCategory_ReturnsAtomRoot()
        {
            var manager = CreateManager();
            var result = manager.GetAtomCategory("A Random Mathematical Quotation");
            Assert.NotNull(result);
            Assert.Equal("Test Blog - A Random Mathematical Quotation", result.Title.Text);
        }

		[Fact]
		public void GetRsd_ReturnsRsdRoot()
		{
			var manager = CreateManager();
			var result = manager.GetRsd();
			Assert.NotNull(result);
			Assert.Equal("https://github.com/poppastring/dasblog-core", result.Services[0].EngineLink);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void GetRsd_WhenBloggerApiEnabled_AdvertisesPublishingApis()
		{
			siteConfigMock.Object.EnableBloggerApi = true;
			var manager = CreateManager();

			var result = manager.GetRsd();

			Assert.NotNull(result);
			var apis = result.Services[0].RsdApiCollection.OfType<RsdApi>().ToList();
			Assert.Equal(3, apis.Count);
			Assert.Contains(apis, a => a.Name == "MetaWeblog");
			Assert.Contains(apis, a => a.Name == "Blogger");
			Assert.Contains(apis, a => a.Name == "Moveable Type");
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void GetRsd_WhenBloggerApiDisabled_OmitsPublishingApis()
		{
			siteConfigMock.Object.EnableBloggerApi = false;
			var manager = CreateManager();

			var result = manager.GetRsd();

			Assert.NotNull(result);
			Assert.Single(result.Services);
			Assert.Empty(result.Services[0].RsdApiCollection.OfType<RsdApi>());
		}
	}
}
