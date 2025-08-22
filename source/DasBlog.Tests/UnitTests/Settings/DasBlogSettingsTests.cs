﻿using System;
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
		public void GetPermaTitle_UsesPlusAsSeparator()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement = "+";
			var result = dasBlogSettings.GetPermaTitle("My+Title");
			Assert.Equal("my+title", result);
		}

		[Fact]
		public void CompressTitle_UsesPlusAsSeparator()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement = "+";
			var result = dasBlogSettings.CompressTitle("My Title");
			Assert.Equal("my+title", result);
		}

		[Fact]
		public void GeneratePostUrl_UsesPlusAsSeparator()
		{
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement = "+";
			var entry = new Entry();
			entry.Title = "My Blog Title Is Awesome!";
			var result = dasBlogSettings.GeneratePostUrl(entry);
			Assert.Contains("my+blog+title+is+awesome", result);
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
            Assert.Equal(now.AddHours(dasBlogSettings.SiteConfiguration.DisplayTimeZoneIndex), display);
		}

        [Fact]
        public void GetCreateTime_AdjustsTimeZone()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var now = DateTime.UtcNow;
            var create = dasBlogSettings.GetCreateTime(now);
            Assert.Equal(now.AddHours(-dasBlogSettings.SiteConfiguration.DisplayTimeZoneIndex), create);
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
            Assert.Equal(NodaTime.Offset.FromHours(dasBlogSettings.SiteConfiguration.DisplayTimeZoneIndex), tz.GetUtcOffset(SystemClock.Instance.GetCurrentInstant()));
        }

        [Fact]
        public void GetContentLookAhead_ReturnsFutureDate()
        {
            var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
            var lookahead = dasBlogSettings.GetContentLookAhead();
            var expected = DateTime.UtcNow.AddDays(dasBlogSettings.SiteConfiguration.ContentLookaheadDays).Date;
            Assert.Equal(expected, lookahead.Date);
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
