using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Core.Security;
using DasBlog.Core.Exceptions;
using Moq;
using Xunit;
using newtelligence.DasBlog.Runtime;
using DasBlog.Services.XmlRpc.MoveableType;
using System.Linq;

namespace DasBlog.Tests.UnitTests.Managers
{
	public class XmlRpcManagerTest
	{
		private Mock<IDasBlogSettings> settingsMock;
		private Mock<ISiteSecurityManager> securityMock;
		private Mock<IFileSystemBinaryManager> binaryManagerMock;
		private Mock<ISiteConfig> siteConfigMock;

		public XmlRpcManagerTest()
		{
			var rootdir = Directory.GetCurrentDirectory();

			settingsMock = new Mock<IDasBlogSettings>();
			securityMock = new Mock<ISiteSecurityManager>();
			binaryManagerMock = new Mock<IFileSystemBinaryManager>();
			settingsMock.Setup(s => s.WebRootDirectory).Returns(rootdir);
			siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupAllProperties();
			siteConfigMock.SetupGet(c => c.LogDir).Returns(Path.Combine(rootdir, "logs"));
			siteConfigMock.SetupGet(c => c.ContentDir).Returns(Path.Combine(rootdir, "TestContent"));
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
			siteConfigMock.SetupGet(c => c.Root).Returns("http://localhost/");
			siteConfigMock.SetupGet(c => c.Title).Returns("Test Blog");
			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
		}

		private XmlRpcManager CreateManagerWithDataService()
		{
			return new XmlRpcManager(settingsMock.Object, securityMock.Object, binaryManagerMock.Object);
		}

		[Fact]
		public void Constructor_WithValidDependencies_ConstructsInstance()
		{
			var manager = CreateManagerWithDataService();
			Assert.NotNull(manager);
		}

		[Fact]
		public void mt_publishPost_ValidAccess_ReturnsTrue()
		{
			securityMock.Setup(s => s.GetUser(It.IsAny<string>())).Returns(new User { Password = "pw", DisplayName = "user", EmailAddress = "a@b.com" });
			securityMock.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var manager = CreateManagerWithDataService();
			var result = manager.mt_publishPost("postid", "user", "pw");
			Assert.True(result);
		}

		[Fact]
		public void mt_supportedMethods_BloggerApiEnabled_ReturnsMethods()
		{
			var manager = CreateManagerWithDataService();
			var methods = manager.mt_supportedMethods();
			Assert.Contains("mt.getCategoryList", methods);
		}

		[Fact]
		public void mt_supportedMethods_BloggerApiDisabled_ThrowsServiceDisabledException()
		{
			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, false);

			settingsMock.Setup(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
			var manager = CreateManagerWithDataService();
			Assert.Throws<ServiceDisabledException>(() => manager.mt_supportedMethods());

			siteConfigMock.SetupProperty(c => c.EnableBloggerApi, true);
		}

		[Fact]
		public void mt_getCategoryList_ReturnsCategories()
		{
			securityMock.Setup(s => s.GetUser(It.IsAny<string>())).Returns(new User { Password = "pw" });
			securityMock.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var manager = CreateManagerWithDataService();
			var result = manager.mt_getCategoryList("", "user", "pw");
			Assert.NotEmpty(result);
			Assert.Equal("A Random Mathematical Quotation", result[0].categoryId);
		}

		[Fact]
		public void mt_getPostCategories_ReturnsCategoriesForPost()
		{
			securityMock.Setup(s => s.GetUser(It.IsAny<string>())).Returns(new User { Password = "pw" });
			securityMock.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var manager = CreateManagerWithDataService();
			var result = manager.mt_getPostCategories("50d9fa42-a650-459f-85f1-97060ca7e39f", "user", "pw");
			Assert.NotNull(result);
			Assert.Equal("A Random Mathematical Quotation", result[0].categoryId);
		}

		[Fact]
		public void mt_getRecentPostTitles_ReturnsRecentPosts()
		{
			securityMock.Setup(s => s.GetUser(It.IsAny<string>())).Returns(new User { Password = "pw" });
			securityMock.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var manager = CreateManagerWithDataService();
			var result = manager.mt_getRecentPostTitles("blogid", "user", "pw", 1);
			Assert.NotEmpty(result);

			var titles = result.Select(pt => pt.title).ToList();
			Assert.Contains("The Mesurability of the Imeasurable", titles);
		}

		[Fact]
		public void mt_getTrackbackPings_ReturnsTrackbacks()
		{
			var manager = CreateManagerWithDataService();
			var result = manager.mt_getTrackbackPings("50d9fa42-a650-459f-85f1-97060ca7e39f");
			Assert.Empty(result);
		}

		[Fact]
		public void mt_setPostCategories_Valid_ReturnsTrue()
		{
			securityMock.Setup(s => s.GetUser(It.IsAny<string>())).Returns(new User { Password = "pw" });
			securityMock.Setup(s => s.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			var manager = CreateManagerWithDataService();
			var categories = new[] { new Category { categoryId = "A Random Mathematical Quotation", categoryName = "A Random Mathematical Quotation" } };
			var result = manager.mt_setPostCategories("50d9fa42-a650-459f-85f1-97060ca7e39f", "user", "pw", categories);
			Assert.True(result);
		}

		[Fact]
		public void mt_supportedTextFilters_ReturnsTextFilters()
		{
			var manager = CreateManagerWithDataService();
			var result = manager.mt_supportedTextFilters();
			Assert.NotEmpty(result);
			Assert.Equal("default", result[0].key);
		}
	}
}
