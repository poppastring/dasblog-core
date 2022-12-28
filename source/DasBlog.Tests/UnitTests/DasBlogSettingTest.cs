using System;
using System.Collections.Generic;
using System.IO;
using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.FileProviders;
using NodaTime;
using System.Xml.Serialization;
using DasBlog.Services;
using newtelligence.DasBlog.Runtime;
using System.Net.Mail;

namespace DasBlog.Tests.UnitTests
{
	public class DasBlogSettingTest : IDasBlogSettings
	{
		public const string SITESECURITYCONFIG = @"Config\siteSecurity.config";
		public const string SITECONFIG = @"Config\site.config";
		public const string METACONFIG = @"Config\metaConfig.config";

		private readonly IFileProvider fileProvider;

		public DasBlogSettingTest()
		{
			fileProvider = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);

			WebRootDirectory = "";
			SiteConfiguration = GetSiteConfig(fileProvider.GetFileInfo(SITECONFIG).PhysicalPath);

			// SecurityConfiguration = GetSecurity(fileProvider.GetFileInfo(SITESECURITYCONFIG).PhysicalPath);
			// MetaTags = GetMetaConfig(fileProvider.GetFileInfo(SITESECURITYCONFIG).PhysicalPath);

			RssUrl = RelativeToRoot("feed/rss");
			CategoryUrl = RelativeToRoot("category");
			ArchiveUrl = RelativeToRoot("archive");
			MicroSummaryUrl = RelativeToRoot("site/microsummary");
			RsdUrl = RelativeToRoot("rsd");
			ShortCutIconUrl = RelativeToRoot("icon.jpg");
			ThemeCssUrl = RelativeToRoot(string.Format("{0}.css", SiteConfiguration.Theme));
		}

		public string WebRootDirectory { get; }
		
		public string RssUrl { get; }

		public string PingBackUrl { get; }

		public string CategoryUrl { get; }

		public string ArchiveUrl { get; }

		public string MicroSummaryUrl { get; }

		public string RsdUrl { get; }

		public string ShortCutIconUrl { get; }

		public string ThemeCssUrl { get; }

		public IMetaTags MetaTags { get; set; }

		public ISiteConfig SiteConfiguration { get; set; }

		public ISiteSecurityConfig SecurityConfiguration { get; }

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

		public string GetPermaTitle(string title)
		{
			string titlePermalink = title.Trim().ToLower();

			titlePermalink = titlePermalink.Replace("+", SiteConfiguration.TitlePermalinkSpaceReplacement);

			return titlePermalink;
		}

		public string GetCommentViewUrl(string entryId)
		{
			return GetPermaLinkUrl(entryId) + "/comments";
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
			if (false == string.IsNullOrEmpty(userName))
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
			var fileInfo = fileProvider.GetFileInfo(SITESECURITYCONFIG);
			using (var writer = new StreamWriter(fileInfo.PhysicalPath))
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

		public SiteSecurityConfig GetSecurity(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			var ser = new XmlSerializer(typeof(SiteSecurityConfig));

			using (var reader = new StreamReader(path))
			{
				return (SiteSecurityConfig)ser.Deserialize(reader);
			}
		}

		public IMetaTags GetMetaConfig(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}

			var ser = new XmlSerializer(typeof(SiteSecurityConfig));

			using (var reader = new StreamReader(path))
			{
				return (IMetaTags)ser.Deserialize(reader);
			}
		}

		public ISiteConfig GetSiteConfig(string path)
		{
			// Need to point this to the config file
			return new SiteConfigTest();

			//if (string.IsNullOrEmpty(path))
			//{
			//	throw new ArgumentNullException("path");
			//}

			//var ser = new XmlSerializer(typeof(ISiteConfig));

			//using (var reader = new StreamReader(path))
			//{
			//	return (ISiteConfig)ser.Deserialize(reader);
			//}
		}

		public string FilterHtml(string input)
		{
			return input;
		}

		public bool AreCommentsPermitted(DateTime blogpostdate)
		{
			return true;
		}

		public string CompressTitle(string title)
		{
			string titlePermalink = title.Trim().ToLower();

			titlePermalink = titlePermalink.Replace("+", SiteConfiguration.TitlePermalinkSpaceReplacement);

			return titlePermalink;
		}

		public bool IsAdmin(string gravatarhash)
		{
			throw new NotImplementedException();
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

		public SendMailInfo GetMailInfo(MailMessage emailmessage)
		{
			throw new NotImplementedException();
		}

		public DateTime GetDisplayTime(DateTime datetime)
		{
			throw new NotImplementedException();
		}

		public DateTime GetCreateTime(DateTime datetime)
		{
			throw new NotImplementedException();
		}

		public string GetRssEntryUrl(string entryId)
		{
			throw new NotImplementedException();
		}
	}
}
