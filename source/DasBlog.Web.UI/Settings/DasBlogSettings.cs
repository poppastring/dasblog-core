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

		public DasBlogSettings(IWebHostEnvironment env, IOptionsMonitor<SiteConfig> siteConfig, IOptionsMonitor<MetaTags> metaTagsConfig, 
									ISiteSecurityConfig siteSecurityConfig, IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			WebRootDirectory = env.ContentRootPath;
			SiteConfiguration = siteConfig.CurrentValue;

			SecurityConfiguration = siteSecurityConfig;
			MetaTags = metaTagsConfig.CurrentValue;
			filePathDataOptions = optionsAccessor.Value;

			RssUrl = RelativeToRoot("feed/rss");
			PingBackUrl = RelativeToRoot("feed/pingback");
			CategoryUrl = RelativeToRoot("category");
			ArchiveUrl = RelativeToRoot("archive");
			MicroSummaryUrl = RelativeToRoot("site/microsummary");
			RsdUrl = RelativeToRoot("feed/rsd");
			ShortCutIconUrl = RelativeToRoot(string.Format("theme/{0}/favicon.ico", SiteConfiguration.Theme));
			ThemeCssUrl = RelativeToRoot(string.Format("theme/{0}/custom.css", SiteConfiguration.Theme));

			siteSecurityConfigFilePath = filePathDataOptions.SecurityConfigFilePath;
		}

		public string WebRootDirectory { get; }

		public string PingBackUrl { get; }

		public string RssUrl { get; }

		public string CategoryUrl { get; }

		public string ArchiveUrl { get; }

		public string MicroSummaryUrl { get; }

		public string RsdUrl { get; }

		public string ShortCutIconUrl { get; }

		public string ThemeCssUrl { get; }
		
		public IMetaTags MetaTags { get; set; }

		public ISiteConfig SiteConfiguration { get; set;  }

		public ISiteSecurityConfig SecurityConfiguration { get; }

		private static Regex htmlFilterRegex = new Regex("<(?<end>/)?(?<name>\\w+)((\\s+(?<attNameValue>(?<attName>\\w+)(\\s*=\\s*(?:\"(?<attVal>[^\"]*)\"|'(?<attVal>[^']*)'|(?<attVal>[^'\">\\s]+)))?))+\\s*|\\s*)(?<self>/)?>",
			RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

		public string GetBaseUrl()
		{
			return new Uri(SiteConfiguration.Root).AbsoluteUri;
		}

		public string RelativeToRoot(string relative)
		{
			return new Uri(new Uri(SiteConfiguration.Root), relative).AbsoluteUri;
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
            return RelativeToRoot(RssUrl + $"/{entryId}/comments/");
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
			return RelativeToRoot($"feed/tags/{category}/rss");
		}

		public string GetRssEntryUrl(string entryId)
		{
			return RelativeToRoot($"feed/rss/{entryId}");
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
