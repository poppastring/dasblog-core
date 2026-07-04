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
using DasBlog.Core.Common.Comments;
using DasBlog.Services.FileManagement;
using System.Net.Mail;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Tests.UnitTests.Settings
{
    public class DasBlogSettingsTests
    {
		private readonly DasBlogSettingsMock dasBlogSettingsMock;

		public DasBlogSettingsTests()
        {
			dasBlogSettingsMock = new DasBlogSettingsMock();
		}

        [Fact]
        public void GetBaseUrl_ReturnsRoot_WhenRootIsSet()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetBaseUrl();
            Assert.Equal("https://example.com/", result);
        }

        [Fact]
        public void RelativeToRoot_AppendsRelativePath()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.RelativeToRoot("feed/rss");
            Assert.Equal("https://example.com/feed/rss", result);
        }

        [Theory]
        [InlineData("https://cdn.example.com/images/hero.jpg")]
        [InlineData("http://cdn.example.com/images/hero.jpg")]
        [InlineData("HTTPS://CDN.EXAMPLE.COM/images/hero.jpg")]
        public void RelativeToRoot_ReturnsAbsoluteUrlUnchanged(string absoluteUrl)
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.RelativeToRoot(absoluteUrl);
            Assert.Equal(absoluteUrl, result);
        }

        [Fact]
        public void RelativeToRoot_ReturnsProtocolRelativeUrlUnchanged()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.RelativeToRoot("//cdn.example.com/images/hero.jpg");
            Assert.Equal("//cdn.example.com/images/hero.jpg", result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void RelativeToRoot_ReturnsInputUnchanged_WhenNullOrWhitespace(string input)
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.RelativeToRoot(input);
            Assert.Equal(input, result);
        }

        [Fact]
        public void GetPermaLinkUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetPermaLinkUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/post/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetCommentViewUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetCommentViewUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Contains("https://example.com/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetTrackbackUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetTrackbackUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/feed/trackback/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetEntryCommentsRssUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetEntryCommentsRssUrl("E455F4B8-C51B-4DE9-9481-D770AD5B0BB4");
            Assert.Equal("https://example.com/feed/rss/comments/E455F4B8-C51B-4DE9-9481-D770AD5B0BB4", result);
        }

        [Fact]
        public void GetCategoryViewUrl_ReturnsCorrectUrl()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetCategoryViewUrl("tech");
            Assert.Equal("https://example.com/category/tech", result);
        }

        [Fact]
        public void GetUser_ReturnsUserByName()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var user = dasBlogSettings.GetUser("admin");
            Assert.NotNull(user);
            Assert.Equal("admin@example.com", user.EmailAddress);
        }

        [Fact]
        public void GetUserByEmail_ReturnsUserByEmail()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var user = dasBlogSettings.GetUserByEmail("user1@example.com");
            Assert.NotNull(user);
            Assert.Equal("user1", user.DisplayName);
        }


        [Fact]
        public void AreCommentsPermitted_WithinAllowedDays_ReturnsTrue()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var postDate = DateTime.UtcNow.AddDays(-2);
            Assert.True(dasBlogSettings.AreCommentsPermitted(postDate));
        }

        [Fact]
        public void AreCommentsPermitted_OutsideAllowedDays_ReturnsFalse()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var postDate = DateTime.UtcNow.AddDays(-10);
            Assert.False(dasBlogSettings.AreCommentsPermitted(postDate));
        }

        [Fact]
        public void GetPermaTitle_UsesSpaceReplacement()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetPermaTitle("My+Title");
            Assert.Equal("my-title", result);
        }

        [Fact]
        public void CompressTitle_ReturnsLowercaseCompressed()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.CompressTitle("My Title");
        }

        [Fact]
        public void GeneratePostUrl_UsesCompressedTitle()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var entry = new Entry();
            entry.Title = "My Blog Title Is Awesome!";
            var result = dasBlogSettings.GeneratePostUrl(entry);
            Assert.Contains("my-blog-title-is-awesome", result);
        }

		[Fact]
		public void GetPermaTitle_AppendsAspxExtension_WhenEnabled()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.UseAspxExtension = true;
			var result = dasBlogSettings.GetPermaTitle("My+Title");
			Assert.EndsWith(".aspx", result);

			// .aspx disables lowercasing and replaces "+" with ""
			Assert.Equal("MyTitle.aspx", result); 
		}

		[Fact]
		public void GeneratePostUrl_AppendsAspxExtension_WhenEnabled()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.UseAspxExtension = true;
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
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var mail = new MailMessage("from@example.com", "to@example.com", "subject", "body");
            var info = dasBlogSettings.GetMailInfo(mail);
            Assert.NotNull(info);
            Assert.Equal("smtp.example.com", info.SmtpServer);
        }

		[Fact]
		public void GetDisplayTime_AdjustsTimeZone()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var now = DateTime.UtcNow;
			var display = dasBlogSettings.GetDisplayTime(now);
			Assert.Equal(now.AddHours(2), display);
		}

		[Fact]
		public void GetCreateTime_AdjustsTimeZone()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var now = DateTime.UtcNow;
			var create = dasBlogSettings.GetCreateTime(now);
			Assert.Equal(now.AddHours(-2), create);
		}

		[Fact]
		public void GetCreateTime_IsInverseOfGetDisplayTime()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var utcTime = new DateTime(2024, 7, 15, 14, 30, 0, DateTimeKind.Utc);
			var displayTime = dasBlogSettings.GetDisplayTime(utcTime);
			var roundTripped = dasBlogSettings.GetCreateTime(displayTime);
			Assert.Equal(utcTime, roundTripped);
		}

        [Fact]
        public void GetCategoryViewUrlName_ReturnsEmptyString()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetCategoryViewUrlName("tech");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetRssCategoryUrl_ReturnsEmptyString()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var result = dasBlogSettings.GetRssCategoryUrl("tech");
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetConfiguredTimeZone_ReturnsConfiguredOffset()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var tz = dasBlogSettings.GetConfiguredTimeZone();
            Assert.Equal(NodaTime.Offset.FromHours(2), tz.GetUtcOffset(SystemClock.Instance.GetCurrentInstant()));
        }

        [Fact]
        public void GetContentLookAhead_ReturnsFutureDate()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var lookahead = dasBlogSettings.GetContentLookAhead();
            var tz = dasBlogSettings.GetConfiguredTimeZone();
            var localNow = SystemClock.Instance.GetCurrentInstant().InZone(tz).ToDateTimeUnspecified();
            var expected = localNow.AddDays(dasBlogSettings.SiteConfiguration.ContentLookaheadDays).Date;
            Assert.Equal(expected, lookahead.Date);
        }

        [Fact]
        public void GetContentLookAhead_UsesLocalTimeNotUtc()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var lookahead = dasBlogSettings.GetContentLookAhead();
            var tz = dasBlogSettings.GetConfiguredTimeZone();
            var localNow = SystemClock.Instance.GetCurrentInstant().InZone(tz).ToDateTimeUnspecified();

            // Lookahead should be relative to blog-local time, not UTC
            Assert.True(Math.Abs((lookahead - localNow.AddDays(dasBlogSettings.SiteConfiguration.ContentLookaheadDays)).TotalMinutes) < 1);
        }

		[Fact]
		public void FilterHtml_EncodesIfNoValidTags()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.ValidCommentTags = null; // Simulate no valid tags
			var input = "<b>hello</b> & <script>alert('x')</script>";
			var result = dasBlogSettings.FilterHtml(input);
			Assert.Contains("&lt;b&gt;hello&lt;/b&gt;", result);
		}

		[Fact]
		public void FilterHtml_RespectsConfiguredCommentAllowlist()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.ValidCommentTags = new[]
			{
				new ValidCommentTags
				{
					Tag = new List<Tag>
					{
						new Tag { Name = "p", Allowed = true },
						new Tag { Name = "strong", Allowed = true },
						new Tag { Name = "a", Allowed = true, Attributes = "href,title" }
					}
				}
			};

			var input = "<p>Hello <strong>world</strong> <a href=\"https://example.com\" title=\"Example\">link</a><script>alert('x')</script><img src=\"x\" onerror=\"alert('x')\" /></p>";
			var result = dasBlogSettings.FilterHtml(input);

			Assert.Contains("<p", result, StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<strong", result, StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<a", result, StringComparison.OrdinalIgnoreCase);
			Assert.Contains("https://example.com", result, StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("<script", result, StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("<img", result, StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("onerror", result, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void FilterHtml_DisallowedWrapperTag_PreservesInnerText()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.ValidCommentTags = new[]
			{
				new ValidCommentTags
				{
					Tag = new List<Tag>
					{
						new Tag { Name = "p", Allowed = true }
					}
				}
			};

			var input = "<h2>The title</h2><p>Body</p>";
			var result = dasBlogSettings.FilterHtml(input);

			Assert.DoesNotContain("<h2", result, StringComparison.OrdinalIgnoreCase);
			Assert.Contains("The title", result, StringComparison.Ordinal);
			Assert.Contains("<p>Body</p>", result, StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		public void IsAdmin_ReturnsTrueForAdminHash()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var admin = dasBlogSettings.SecurityConfiguration.Users[0];
            var hash = Core.Common.Utils.GetGravatarHash(admin.EmailAddress);
            Assert.True(dasBlogSettings.IsAdmin(hash));
        }

		[Fact]
		public void IsAdmin_ReturnsFalseForAdminHash()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var admin = dasBlogSettings.SecurityConfiguration.Users[1];
			var hash = Core.Common.Utils.GetGravatarHash(admin.EmailAddress);
			Assert.False(dasBlogSettings.IsAdmin(hash));
		}
	}
}
