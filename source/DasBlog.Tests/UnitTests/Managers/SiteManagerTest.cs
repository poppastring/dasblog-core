using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Moq;
using Xunit;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using DasBlog.Core.Services.GoogleSiteMap;
using DasBlog.Services;

namespace DasBlog.Tests.UnitTests.Managers
{
    public class SiteManagerTest
    {
        private Mock<IDasBlogSettings> settingsMock;
        private Mock<ISiteConfig> siteConfigMock;
        private Mock<IBlogDataService> dataServiceMock;
        private Mock<ILoggingDataService> loggingServiceMock;
        public SiteManagerTest()
        {
			var rootdir = Directory.GetCurrentDirectory();

			settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupAllProperties();
			siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
			siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.Title).Returns("Test Blog");
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
			settingsMock.Setup(s => s.GetBaseUrl()).Returns("http://localhost/");
			settingsMock.Setup(s => s.RelativeToRoot(It.IsAny<string>()))
				.Returns((string path) => $"http://localhost/{path.TrimStart('/')}");
			settingsMock.Setup(s => s.GeneratePostUrl(It.IsAny<Entry>()))
				.Returns("seo-post");
			settingsMock.Setup(s => s.GetCategoryViewUrl(It.IsAny<string>()))
				.Returns((string category) => $"http://localhost/category/{category}");
			dataServiceMock = new Mock<IBlogDataService>();
			loggingServiceMock = new Mock<ILoggingDataService>();

			dataServiceMock.Setup(d => d.GetEntries(false)).Returns(new EntryCollection());
			dataServiceMock.Setup(d => d.GetCategories()).Returns(new CategoryCacheEntryCollection());
		}

        private SiteManager CreateManager()
        {
            return new SiteManager(settingsMock.Object, dataServiceMock.Object, loggingServiceMock.Object);
        }

        [Fact]
        public void Constructor_WithValidDependencies_ConstructsInstance()
        {
            var manager = CreateManager();
            Assert.NotNull(manager);
        }

        [Fact]
        public void GetGoogleSiteMap_ReturnsUrlSet()
        {
            var manager = CreateManager();
            var result = manager.GetGoogleSiteMap();
            Assert.NotNull(result);
			Assert.Equal("daily", result.url[0].changefreq.ToString());
        }

		[Fact]
		public void GetGoogleSiteMap_ModifiedEntry_UsesModifiedDateForRelatedUrls()
		{
			var entry = new Entry
			{
				EntryId = "entry-1",
				Title = "SEO Post",
				Categories = "SEO",
				IsPublic = true,
				CreatedUtc = new DateTime(2026, 8, 1, 12, 0, 0, DateTimeKind.Utc),
				ModifiedUtc = new DateTime(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc)
			};
			var entries = new EntryCollection { entry };
			var categories = new CategoryCacheEntryCollection
			{
				new CategoryCacheEntry { Name = "SEO", IsPublic = true }
			};
			dataServiceMock.Setup(d => d.GetEntries(false)).Returns(entries);
			dataServiceMock.Setup(d => d.GetCategories()).Returns(categories);

			var result = CreateManager().GetGoogleSiteMap();
			var urls = result.url.Cast<Url>().ToList();

			Assert.Equal("2026-09-10", urls.Single(url => url.loc == "http://localhost/").lastmodString);
			Assert.Equal("2026-09-10", urls.Single(url => url.loc == "http://localhost/archive").lastmodString);
			Assert.Equal("2026-09-10", urls.Single(url => url.loc == "http://localhost/category").lastmodString);
			Assert.Equal("2026-09-10", urls.Single(url => url.loc == "http://localhost/seo-post").lastmodString);
			Assert.Equal("2026-09-10", urls.Single(url => url.loc == "http://localhost/category/seo").lastmodString);
		}

		[Fact]
		public void UrlSet_Serialized_UsesStandardSitemapNamespace()
		{
			var serializer = new XmlSerializer(typeof(UrlSet));
			using var writer = new StringWriter();

			serializer.Serialize(writer, CreateManager().GetGoogleSiteMap());

			Assert.Contains("xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"", writer.ToString());
		}
    }
}
