using DasBlog.Core.Common;
using DasBlog.Core.Common.Comments;
using DasBlog.Core.Security;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Site;
using Ganss.Xss;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Xml.Serialization;

namespace DasBlog.Web.Settings
{
	public class DasBlogSettings : IDasBlogSettings
	{
		private readonly string siteSecurityConfigFilePath;
		private readonly ConfigFilePathsDataOption filePathDataOptions;
		private readonly IOptionsMonitor<SiteConfig> siteConfigMonitor;
		private readonly IOptionsMonitor<MetaTags> metaTagsMonitor;
		private readonly IOptionsMonitor<OEmbedProviders> embedProvidersMonitor;
		private readonly ITimeZoneProvider timeZoneProvider;

		public DasBlogSettings(IWebHostEnvironment env, IOptionsMonitor<SiteConfig> siteConfig, IOptionsMonitor<MetaTags> metaTagsConfig, 
									IOptionsMonitor<OEmbedProviders> embedProvidersConfig, 
									ISiteSecurityConfig siteSecurityConfig, IOptions<ConfigFilePathsDataOption> optionsAccessor,
									ITimeZoneProvider timeZoneProvider)
		{
			WebRootDirectory = env.ContentRootPath;
			siteConfigMonitor = siteConfig;
			metaTagsMonitor = metaTagsConfig;
			embedProvidersMonitor = embedProvidersConfig;
			SecurityConfiguration = siteSecurityConfig;
			filePathDataOptions = optionsAccessor.Value;
			this.timeZoneProvider = timeZoneProvider;

			siteSecurityConfigFilePath = filePathDataOptions.SecurityConfigFilePath;
		}

		public string WebRootDirectory { get; }

		public string PingBackUrl => RelativeToRoot("feed/pingback");

		public string RssUrl => RelativeToRoot("feed/rss");

		public string CategoryUrl => RelativeToRoot("category");

		public string ArchiveUrl => RelativeToRoot("archive");

		public string MicroSummaryUrl => RelativeToRoot("site/microsummary");

		public string RsdUrl => RelativeToRoot("feed/rsd");

		public string ShortCutIconUrl => RelativeToRoot(string.Format("theme/{0}/favicon.ico", SiteConfiguration.Theme));

		public string ThemeCssUrl => RelativeToRoot(string.Format("theme/{0}/custom.css", SiteConfiguration.Theme));

		public IMetaTags MetaTags => metaTagsMonitor.CurrentValue;

		public ISiteConfig SiteConfiguration => siteConfigMonitor.CurrentValue;

		public ISiteSecurityConfig SecurityConfiguration { get; }
		public IOEmbedProviders OEmbedProviders => embedProvidersMonitor.CurrentValue;

		public string GetBaseUrl()
		{
			if (!string.IsNullOrWhiteSpace(SiteConfiguration.Root))
			{
				return new Uri(SiteConfiguration.Root).AbsoluteUri;
			}
			else
			{
				return "/";
			}
		}

		public string RelativeToRoot(string relative)
		{
			if (string.IsNullOrWhiteSpace(relative))
			{
				return relative;
			}

			// Already absolute (http(s)://...) or protocol-relative (//...) — leave it alone.
			// This lets callers safely pass values that may originate as either a
			// site-relative path or a fully-qualified remote URL (e.g. an explicit
			// hero image hosted on a CDN).
			if (relative.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
				|| relative.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
				|| relative.StartsWith("//", StringComparison.Ordinal))
			{
				return relative;
			}

			if (!string.IsNullOrWhiteSpace(SiteConfiguration.Root))
			{
				return new Uri(new Uri(GetBaseUrl()), relative).AbsoluteUri;
			}
			else
			{
				return relative;
			}
		}

        public string GetPermaLinkUrl(string entryId)
        {
            return RelativeToRoot("post/" + entryId);
        }

		public string GetCommentViewUrl(string entryId)
        {
            return RelativeToRoot(entryId) + $"/comments#{Constants.CommentsStartId}";
        }

        public string GetTrackbackUrl(string entryId)
        {
            return RelativeToRoot("feed/trackback/" + entryId);
        }

        public string GetEntryCommentsRssUrl(string entryId)
        {
            return RelativeToRoot(RssUrl + "/comments/" + entryId);
        }

		public string GetCategoryViewUrl(string category)
		{
			return RelativeToRoot("category/" + category);
		}

		public string GetCategoryViewUrlName(string category)
		{
			return string.Empty;
		}

		public string GetRssCategoryUrl(string category)
		{
			return string.Empty;
		}

		public User GetUser(string userName)
		{
			if (!string.IsNullOrEmpty(userName))
			{
				return SecurityConfiguration.Users.Find(delegate (User x)
				{
					return string.Compare(x.DisplayName, userName, StringComparison.InvariantCultureIgnoreCase) == 0;
				});
			}
			return null;
		}

		public User GetUserByEmail(string userEmail)
		{
			if (!string.IsNullOrEmpty(userEmail))
			{
				return SecurityConfiguration.Users.Find(delegate (User x)
				{
					return string.Compare(x.EmailAddress, userEmail, StringComparison.InvariantCultureIgnoreCase) == 0;
				});
			}
			return null;
		}

		public void AddUser(User user)
		{
			SecurityConfiguration.Users.Add(user);
			var ser = new XmlSerializer(typeof(SiteSecurityConfig));

			using (var writer = new StreamWriter(siteSecurityConfigFilePath))
			{
				ser.Serialize(writer, SecurityConfiguration);
			}
		}

		public DateTimeZone GetConfiguredTimeZone()
		{
			return timeZoneProvider.GetConfiguredTimeZone();
		}

		public DateTime GetContentLookAhead()
		{
			var tz = timeZoneProvider.GetConfiguredTimeZone();
			var localNow = SystemClock.Instance.GetCurrentInstant().InZone(tz).ToDateTimeUnspecified();
			return localNow.AddDays(SiteConfiguration.ContentLookaheadDays);
		}

		public string FilterHtml(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}

			var sanitizer = CreateCommentSanitizer();
			if (sanitizer == null)
			{
				return System.Net.WebUtility.HtmlEncode(input);
			}

			return sanitizer.Sanitize(input);
		}

		private HtmlSanitizer CreateCommentSanitizer()
		{
			if (SiteConfiguration.ValidCommentTags == null || SiteConfiguration.ValidCommentTags.Length == 0)
			{
				return null;
			}

			var configuredTags = SiteConfiguration.ValidCommentTags[0]?.Tag
				?.Where(tag => tag?.Allowed == true && !string.IsNullOrWhiteSpace(tag.Name))
				.ToList();

			if (configuredTags == null || configuredTags.Count == 0)
			{
				return null;
			}

			var sanitizer = new HtmlSanitizer();
			sanitizer.KeepChildNodes = true;
			sanitizer.AllowedTags.Clear();
			sanitizer.AllowedAttributes.Clear();
			sanitizer.AllowedSchemes.Clear();
			sanitizer.AllowedSchemes.Add("http");
			sanitizer.AllowedSchemes.Add("https");
			sanitizer.AllowedSchemes.Add("mailto");
			sanitizer.UriAttributes.Add("cite");

			foreach (var tag in configuredTags)
			{
				sanitizer.AllowedTags.Add(tag.Name.Trim().ToLowerInvariant());

				foreach (var attribute in SplitAttributes(tag.Attributes))
				{
					sanitizer.AllowedAttributes.Add(attribute);
				}
			}

			return sanitizer;
		}

		private static IEnumerable<string> SplitAttributes(string attributes)
		{
			if (string.IsNullOrWhiteSpace(attributes))
			{
				yield break;
			}

			foreach (var attribute in attributes.Split(',', StringSplitOptions.RemoveEmptyEntries))
			{
				var trimmed = attribute.Trim().ToLowerInvariant();
				if (!string.IsNullOrWhiteSpace(trimmed))
				{
					yield return trimmed;
				}
			}
		}

		public bool AreCommentsPermitted(DateTime blogpostdate)
		{
			if (!SiteConfiguration.EnableComments)
			{
				return false;
			}
			else if(SiteConfiguration.EnableComments && !SiteConfiguration.EnableCommentDays)
			{
				return true;
			}

			return (DateTime.UtcNow.AddDays(-1 * SiteConfiguration.DaysCommentsAllowed) < blogpostdate);
		}

		public string GetPermaTitle(string titleurl)
		{
			var titlePermalink = titleurl.Trim();

			if (!SiteConfiguration.UseAspxExtension)
			{
				titlePermalink = titlePermalink.ToLower();

				titlePermalink = titlePermalink.Replace("+", "-");
			}
			else
			{
				titlePermalink = string.Format("{0}.aspx", titlePermalink.Replace("+", string.Empty));
			}

			return titlePermalink;
		}

		public string CompressTitle(string title)
		{
			return Entry.InternalCompressTitle(title, "-").ToLower();
		}

		public string GeneratePostUrl(Entry entry)
		{
			string link;

			if (SiteConfiguration.EnableTitlePermaLinkUnique)
			{
				link = GetPermaTitle(entry.CompressedTitleUnique);
			}
			else
			{
				link = GetPermaTitle(entry.CompressedTitle);
			}

			return link;
		}

		public bool IsAdmin(string gravatarhashid)
		{
			return (Utils.GetGravatarHash(SecurityConfiguration.Users.First().EmailAddress) == gravatarhashid);
		}

		public SendMailInfo GetMailInfo(MailMessage emailmessage)
		{
			return new SendMailInfo(emailmessage, SiteConfiguration.SmtpServer,
						   SiteConfiguration.EnableSmtpAuthentication, SiteConfiguration.UseSSLForSMTP,
						   SiteConfiguration.SmtpUserName, SiteConfiguration.SmtpPassword, SiteConfiguration.SmtpPort);
		}

		public DateTime GetDisplayTime(DateTime datetime)
		{
			var tz = timeZoneProvider.GetConfiguredTimeZone();
			var utc = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
			var instant = Instant.FromDateTimeUtc(utc);
			return instant.InZone(tz).ToDateTimeUnspecified();
		}

		public DateTime GetCreateTime(DateTime datetime)
		{
			var tz = timeZoneProvider.GetConfiguredTimeZone();
			var local = LocalDateTime.FromDateTime(datetime);
			return local.InZoneLeniently(tz).ToDateTimeUtc();
		}
	}
}
