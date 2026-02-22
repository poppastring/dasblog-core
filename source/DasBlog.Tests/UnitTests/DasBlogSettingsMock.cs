using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Xml.Serialization;
using DasBlog.Core.Security;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Moq;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Tests.UnitTests
{
	public class DasBlogSettingsMock
	{
		public readonly Mock<IWebHostEnvironment> envMock;
		public readonly Mock<IOptionsMonitor<SiteConfig>> siteConfigMock;
		public readonly Mock<IOptionsMonitor<MetaTags>> metaTagsMock;
		public readonly Mock<IOptionsMonitor<OEmbedProviders>> oembedMock;
		public readonly Mock<ISiteSecurityConfig> securityConfigMock;
		public readonly Mock<IOptions<ConfigFilePathsDataOption>> configFilePathsMock;
		public readonly SiteConfig siteConfig;
		public readonly List<User> users;

		public DasBlogSettingsMock()
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
				SmtpPort = 25,
				ShowCommentCount = true
			};
			users = new List<User> {
				new User { DisplayName = "admin", EmailAddress = "admin@example.com", Active = true, Role = Role.Admin },
				new User { DisplayName = "user1", EmailAddress = "user1@example.com", Active = true, Role = Role.Contributor },
				new User { DisplayName = "user2", EmailAddress = "user2@example.com", Active = false, Role = Role.Contributor }
			};
			securityConfigMock.SetupGet(s => s.Users).Returns(users);
		}

		public DasBlogSettings CreateSettings()
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
	}
}
