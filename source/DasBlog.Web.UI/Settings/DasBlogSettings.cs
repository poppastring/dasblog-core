﻿using System;
using System.IO;
using System.Xml.Serialization;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using DasBlog.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Util;
using NodaTime;

namespace DasBlog.Web.Settings
{
	public class DasBlogSettings : IDasBlogSettings
	{
		private readonly IFileProvider fileProvider;

		public DasBlogSettings(IHostingEnvironment env, IOptions<SiteConfig> siteConfig, IOptions<MetaTags> metaTagsConfig, ISiteSecurityConfig siteSecurityConfig, IFileProvider fileProvider)
		{
			this.fileProvider = fileProvider;

			WebRootDirectory = Startup.GetDataRoot(env);
			SiteConfiguration = siteConfig.Value;
			SecurityConfiguration = siteSecurityConfig;
			MetaTags = metaTagsConfig.Value;

			RssUrl = RelativeToRoot("feed/rss");
			PingBackUrl = RelativeToRoot("feed/pingback");
			CategoryUrl = RelativeToRoot("category");
			ArchiveUrl = RelativeToRoot("archive");
			MicroSummaryUrl = RelativeToRoot("microsummary");
			RsdUrl = RelativeToRoot("rsd");
			ShortCutIconUrl = RelativeToRoot("icon.jpg");
		}

		public string WebRootDirectory { get; }

		public string PingBackUrl { get; }

		public string RssUrl { get; }

		public string CategoryUrl { get; }

		public string ArchiveUrl { get; }

		public string MicroSummaryUrl { get; }

		public string RsdUrl { get; }

		public string ShortCutIconUrl { get; }

		public IMetaTags MetaTags { get; }

		public ISiteConfig SiteConfiguration { get; }

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

		public void AddUser(User user)
		{
			SecurityConfiguration.Users.Add(user);
			var ser = new XmlSerializer(typeof(SiteSecurityConfig));
			var fileInfo = fileProvider.GetFileInfo(Startup.SITESECURITYCONFIG);
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
	}
}
