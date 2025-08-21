using System;
using Xunit;
using Moq;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Core.Security;
using System.Collections.Generic;
using DasBlog.Services.FileManagement;
using System.Net.Mail;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Tests.UnitTests.Settings
{
    public class DasBlogSettingsTests
    {
        private readonly Mock<IWebHostEnvironment> envMock;
        private readonly Mock<IOptionsMonitor<SiteConfig>> siteConfigMock;
        private readonly Mock<IOptionsMonitor<MetaTags>> metaTagsMock;
        private readonly Mock<IOptionsMonitor<OEmbedProviders>> oembedMock;
        private readonly Mock<ISiteSecurityConfig> securityConfigMock;
        private readonly Mock<IOptions<ConfigFilePathsDataOption>> configFilePathsMock;
        private readonly SiteConfig siteConfig;
        private readonly List<User> users;

        public DasBlogSettingsTests()
        {
            envMock = new Mock<IWebHostEnvironment>();
            siteConfigMock = new Mock<IOptionsMonitor<SiteConfig>>();
            metaTagsMock = new Mock<IOptionsMonitor<MetaTags>>();
            oembedMock = new Mock<IOptionsMonitor<OEmbedProviders>>();
            securityConfigMock = new Mock<ISiteSecurityConfig>();
            configFilePathsMock = new Mock<IOptions<ConfigFilePathsDataOption>>();

            siteConfig = new SiteConfig
            {
                Root = "https://example.com/",
                Theme = "default",
                TitlePermalinkSpaceReplacement = "-",
                UseAspxExtension = false,
                EnableTitlePermaLinkUnique = false,
                EnableComments = true,
                EnableCommentDays = true,
                DaysCommentsAllowed = 7,
                AdjustDisplayTimeZone = true,
                DisplayTimeZoneIndex = 2,
                ContentLookaheadDays = 3,
                SmtpServer = "smtp.example.com",
                EnableSmtpAuthentication = true,
                UseSSLForSMTP = true,
                SmtpUserName = "user",
                SmtpPassword = "pass",
                SmtpPort = 25
            };
            users = new List<User> {
                new User { DisplayName = "admin", EmailAddress = "admin@example.com", Active = true, Role = Role.Admin },
				new User { DisplayName = "user1", EmailAddress = "user1@example.com", Active = true, Role = Role.Contributor },
				new User { DisplayName = "user2", EmailAddress = "user2@example.com", Active = false, Role = Role.Contributor }
			};
            securityConfigMock.SetupGet(s => s.Users).Returns(users);
        }

        private DasBlogSettings CreateSettings()
        {
            envMock.Setup(e => e.ContentRootPath).Returns("/app");
            siteConfigMock.Setup(s => s.CurrentValue).Returns(siteConfig);
            metaTagsMock.Setup(m => m.CurrentValue).Returns(new MetaTags());
            oembedMock.Setup(o => o.CurrentValue).Returns(new OEmbedProviders());
            configFilePathsMock.Setup(c => c.Value).Returns(new ConfigFilePathsDataOption { SecurityConfigFilePath = "security.config" });
            return new DasBlogSettings(
                envMock.Object,
                siteConfigMock.Object,
                metaTagsMock.Object,
                oembedMock.Object,
                securityConfigMock.Object,
                configFilePathsMock.Object
            );
        }

        [Fact]
        public void GetBaseUrl_ReturnsRoot_WhenRootIsSet()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetBaseUrl();
            Assert.Equal("https://example.com/", result);
        }

        [Fact]
        public void RelativeToRoot_AppendsRelativePath()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.RelativeToRoot("feed/rss");
            Assert.Equal("https://example.com/feed/rss", result);
        }

        [Fact]
        public void GetPermaLinkUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetPermaLinkUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/post/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetCommentViewUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetCommentViewUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Contains("https://example.com/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetTrackbackUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetTrackbackUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/feed/trackback/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetEntryCommentsRssUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetEntryCommentsRssUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/feed/rss/comments/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetCategoryViewUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetCategoryViewUrl("tech");
            Assert.Equal("https://example.com/category/tech", result);
        }

        [Fact]
        public void GetUser_ReturnsUserByName()
        {
            var dasBlogSettings = CreateSettings();
            var user = dasBlogSettings.GetUser("admin");
            Assert.NotNull(user);
            Assert.Equal("admin@example.com", user.EmailAddress);
        }

        [Fact]
        public void GetUserByEmail_ReturnsUserByEmail()
        {
            var dasBlogSettings = CreateSettings();
            var user = dasBlogSettings.GetUserByEmail("user1@example.com");
            Assert.NotNull(user);
            Assert.Equal("user1", user.DisplayName);
        }


        [Fact]
        public void AreCommentsPermitted_WithinAllowedDays_ReturnsTrue()
        {
            var dasBlogSettings = CreateSettings();
            var postDate = DateTime.UtcNow.AddDays(-2);
            Assert.True(dasBlogSettings.AreCommentsPermitted(postDate));
        }

        [Fact]
        public void AreCommentsPermitted_OutsideAllowedDays_ReturnsFalse()
        {
            var dasBlogSettings = CreateSettings();
            var postDate = DateTime.UtcNow.AddDays(-10);
            Assert.False(dasBlogSettings.AreCommentsPermitted(postDate));
        }

        [Fact]
        public void GetPermaTitle_UsesSpaceReplacement()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetPermaTitle("My+Title");
            Assert.Equal("my-title", result);
        }

        [Fact]
        public void CompressTitle_ReturnsLowercaseCompressed()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.CompressTitle("My Title");
        }

        [Fact]
        public void GeneratePostUrl_UsesCompressedTitle()
        {
            var dasBlogSettings = CreateSettings();
            var entry = new Entry();
            entry.Title = "My Blog Title Is Awesome!";
            var result = dasBlogSettings.GeneratePostUrl(entry);
            Assert.Contains("my-blog-title-is-awesome", result);
        }

		[Fact]
		public void GetPermaTitle_UsesPlusAsSeparator()
		{
			siteConfig.TitlePermalinkSpaceReplacement = "+";
			var dasBlogSettings = CreateSettings();
			var result = dasBlogSettings.GetPermaTitle("My+Title");
			Assert.Equal("my+title", result);
		}

		[Fact]
		public void CompressTitle_UsesPlusAsSeparator()
		{
			siteConfig.TitlePermalinkSpaceReplacement = "+";
			var dasBlogSettings = CreateSettings();
			var result = dasBlogSettings.CompressTitle("My Title");
			Assert.Equal("my+title", result);
		}

		[Fact]
		public void GeneratePostUrl_UsesPlusAsSeparator()
		{
			siteConfig.TitlePermalinkSpaceReplacement = "+";
			var dasBlogSettings = CreateSettings();
			var entry = new Entry();
			entry.Title = "My Blog Title Is Awesome!";
			var result = dasBlogSettings.GeneratePostUrl(entry);
			Assert.Contains("my+blog+title+is+awesome", result);
		}

		[Fact]
		public void GetPermaTitle_AppendsAspxExtension_WhenEnabled()
		{
			siteConfig.UseAspxExtension = true;
			var dasBlogSettings = CreateSettings();
			var result = dasBlogSettings.GetPermaTitle("My+Title");
			Assert.EndsWith(".aspx", result);

			// .aspx disables lowercasing and replaces "+" with ""
			Assert.Equal("MyTitle.aspx", result); 
		}

		[Fact]
		public void GeneratePostUrl_AppendsAspxExtension_WhenEnabled()
		{
			siteConfig.UseAspxExtension = true;
			var dasBlogSettings = CreateSettings();
			var entry = new Entry();
			entry.Title = "My Blog Title Is Awesome!";
			var result = dasBlogSettings.GeneratePostUrl(entry);
			Assert.EndsWith(".aspx", result);

			// CompressTitle does not add .aspx, but should still return lowercase
			Assert.Equal("MyBlogTitleIsAwesome.aspx", result);
		}

		[Fact]
        public void GetMailInfo_ReturnsSendMailInfo()
        {
            var dasBlogSettings = CreateSettings();
            var mail = new MailMessage("from@example.com", "to@example.com", "subject", "body");
            var info = dasBlogSettings.GetMailInfo(mail);
            Assert.NotNull(info);
            Assert.Equal("smtp.example.com", info.SmtpServer);
        }

        [Fact]
        public void GetDisplayTime_AdjustsTimeZone()
        {
            var dasBlogSettings = CreateSettings();
            var now = DateTime.UtcNow;
            var display = dasBlogSettings.GetDisplayTime(now);
            Assert.Equal(now.AddHours(siteConfig.DisplayTimeZoneIndex), display);
        }

        [Fact]
        public void GetCreateTime_AdjustsTimeZone()
        {
            var dasBlogSettings = CreateSettings();
            var now = DateTime.UtcNow;
            var create = dasBlogSettings.GetCreateTime(now);
            Assert.Equal(now.AddHours(-siteConfig.DisplayTimeZoneIndex), create);
        }

        [Fact]
        public void GetCategoryViewUrlName_ReturnsEmptyString()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetCategoryViewUrlName("tech");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetRssCategoryUrl_ReturnsEmptyString()
        {
            var dasBlogSettings = CreateSettings();
            var result = dasBlogSettings.GetRssCategoryUrl("tech");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetConfiguredTimeZone_ReturnsConfiguredOffset()
        {
            var dasBlogSettings = CreateSettings();
            var tz = dasBlogSettings.GetConfiguredTimeZone();
            Assert.Equal(NodaTime.Offset.FromHours(siteConfig.DisplayTimeZoneIndex), tz.GetUtcOffset(SystemClock.Instance.GetCurrentInstant()));
        }

        [Fact]
        public void GetContentLookAhead_ReturnsFutureDate()
        {
            var dasBlogSettings = CreateSettings();
            var lookahead = dasBlogSettings.GetContentLookAhead();
            var expected = DateTime.UtcNow.AddDays(siteConfig.ContentLookaheadDays).Date;
            Assert.Equal(expected, lookahead.Date);
        }

        [Fact]
        public void FilterHtml_EncodesIfNoValidTags()
        {
            var dasBlogSettings = CreateSettings();
            siteConfig.ValidCommentTags = null; // Simulate no valid tags
            var input = "<b>hello</b> & <script>alert('x')</script>";
            var result = dasBlogSettings.FilterHtml(input);
            Assert.Contains("&lt;b&gt;hello&lt;/b&gt;", result);
        }

        [Fact]
        public void IsAdmin_ReturnsTrueForAdminHash()
        {
            var dasBlogSettings = CreateSettings();
            var admin = users[0];
            var hash = Core.Common.Utils.GetGravatarHash(admin.EmailAddress);
            Assert.True(dasBlogSettings.IsAdmin(hash));
        }

		[Fact]
		public void IsAdmin_ReturnsFalseForAdminHash()
		{
			var dasBlogSettings = CreateSettings();
			var admin = users[1];
			var hash = Core.Common.Utils.GetGravatarHash(admin.EmailAddress);
			Assert.False(dasBlogSettings.IsAdmin(hash));
		}
	}
}
