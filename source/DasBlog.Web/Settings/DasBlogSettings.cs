using DasBlog.Core.Common;
using DasBlog.Core.Common.Comments;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using NodaTime;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.ConfigFile;
using DasBlog.Services;
using System.Linq;
using newtelligence.DasBlog.Runtime;
using DasBlog.Services.FileManagement;
using System.Net.Mail;

namespace DasBlog.Web.Settings
{
	public class DasBlogSettings : IDasBlogSettings
	{
		private readonly string siteSecurityConfigFilePath;
		private readonly ConfigFilePathsDataOption filePathDataOptions;
		private readonly IOptionsMonitor<SiteConfig> siteConfigMonitor;
		private readonly IOptionsMonitor<MetaTags> metaTagsMonitor;
		private readonly IOptionsMonitor<OEmbedProviders> embedProvidersMonitor;

		public DasBlogSettings(IWebHostEnvironment env, IOptionsMonitor<SiteConfig> siteConfig, IOptionsMonitor<MetaTags> metaTagsConfig, 
									IOptionsMonitor<OEmbedProviders> embedProvidersConfig, 
									ISiteSecurityConfig siteSecurityConfig, IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			WebRootDirectory = env.ContentRootPath;
			siteConfigMonitor = siteConfig;
			metaTagsMonitor = metaTagsConfig;
			embedProvidersMonitor = embedProvidersConfig;
			SecurityConfiguration = siteSecurityConfig;
			filePathDataOptions = optionsAccessor.Value;

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

		private static Regex htmlFilterRegex = new Regex("<(?<end>/)?(?<name>\\w+)((\\s+(?<attNameValue>(?<attName>\\w+)(\\s*=\\s*(?:\"(?<attVal>[^\"]*)\"|'(?<attVal>[^']*)'|(?<attVal>[^'\">\\s]+)))?))+\\s*|\\s*)(?<self>/)?>",
			RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

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
			if (SiteConfiguration.AdjustDisplayTimeZone)
			{
				return DateTimeZone.ForOffset(Offset.FromHours(SiteConfiguration.DisplayTimeZoneIndex));
			}
			else
			{
				return DateTimeZone.Utc;
			}
		}

		public DateTime GetContentLookAhead()
		{
			return DateTime.UtcNow.AddDays(SiteConfiguration.ContentLookaheadDays);
		}

		public string FilterHtml(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return string.Empty;
			}

			if (SiteConfiguration.ValidCommentTags == null || SiteConfiguration.ValidCommentTags[0].Tag.Count(s => s.Allowed == true) == 0)
			{
				return WebUtility.HtmlEncode(input);
			}

			// check for matches
			var matches = htmlFilterRegex.Matches(input);

			// no matches, normal encoding
			if (matches.Count == 0)
			{
				return WebUtility.HtmlEncode(input);
			}

			var sb = new StringBuilder();


			var collection = new MatchedTagCollection(SiteConfiguration.ValidCommentTags);
			collection.Init(matches);

			int inputIndex = 0;

			foreach (MatchedTag tag in collection)
			{
				// add the normal text between the current index and the index of the current tag
				if (inputIndex < tag.Index)
				{
					sb.Append(WebUtility.HtmlEncode(input.Substring(inputIndex, tag.Index - inputIndex)));
				}

				// add the filtered value
				sb.Append(tag.FilteredValue);

				// move the current index past the tag
				inputIndex = tag.Index + tag.Length;
			}

			// add remainder
			if (inputIndex < input.Length)
			{
				sb.Append(WebUtility.HtmlEncode(input.Substring(inputIndex)));
			}

			return sb.ToString();
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

				titlePermalink = titlePermalink.Replace("+", SiteConfiguration.TitlePermalinkSpaceReplacement);
			}
			else
			{
				titlePermalink = string.Format("{0}.aspx", titlePermalink.Replace("+", string.Empty));
			}

			return titlePermalink;
		}

		public string CompressTitle(string title)
		{
			return Entry.InternalCompressTitle(title, SiteConfiguration.TitlePermalinkSpaceReplacement).ToLower();
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
			if (SiteConfiguration.AdjustDisplayTimeZone)
			{
				return datetime.AddHours(SiteConfiguration.DisplayTimeZoneIndex);
			}
			return datetime;
		}

		public DateTime GetCreateTime(DateTime datetime)
		{
			if (SiteConfiguration.AdjustDisplayTimeZone)
			{
				datetime = datetime.AddHours(-1 * SiteConfiguration.DisplayTimeZoneIndex);
			}

			return datetime;
		}
	}
}
