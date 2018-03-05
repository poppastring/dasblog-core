using System;
using System.IO;
using System.Xml.Serialization;
using DasBlog.Web.Core;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.Core.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Util;

namespace DasBlog.Web.UI.Settings
{
	public class DasBlogSettings : IDasBlogSettings
	{
		private readonly IFileProvider fileProvider;

		public DasBlogSettings(IHostingEnvironment env, IOptions<SiteConfig> siteConfig, IOptions<MetaTags> metaTagsConfig, IOptions<SiteSecurityConfig> siteSecurityConfig, IFileProvider fileProvider)
		{
			this.fileProvider = fileProvider;

			WebRootDirectory = env.WebRootPath;
			SiteConfiguration = siteConfig.Value;
			SecurityConfiguration = siteSecurityConfig.Value;
			MetaTags = metaTagsConfig.Value;

			RssUrl = RelativeToRoot("feed/rss");
			CategoryUrl = RelativeToRoot("category");
			ArchiveUrl = RelativeToRoot("archive");
			MicroSummaryUrl = RelativeToRoot("microsummary");
			RsdUrl = RelativeToRoot("rsd");
			ShortCutIconUrl = RelativeToRoot("icon.jpg");
		}

		public string WebRootDirectory { get; }

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
			//TODO: Old links vs new links
			return RelativeToRoot("post/" + entryId);
		}

		public string GetCommentViewUrl(string entryId)
		{
			//TODO: Old links vs new links
			return RelativeToRoot("comment/" + entryId);
		}

		public string GetTrackbackUrl(string entryId)
		{
			//TODO: Old links vs new links
			return RelativeToRoot("trackback/" + entryId);
		}

		public string GetEntryCommentsRssUrl(string entryId)
		{
			//TODO: Old links vs new links
			return RelativeToRoot("feed/rss/comments/" + entryId);
		}

		public string GetCategoryViewUrl(string category)
		{
			return RelativeToRoot("category/" + category);
		}

		public User GetUser(string userName)
		{
			if (false == string.IsNullOrEmpty(userName))
			{
				return SecurityConfiguration.Users.Find(user => string.Compare(user.Name, userName, StringComparison.InvariantCultureIgnoreCase) == 0);
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

		public TimeZone GetConfiguredTimeZone()
		{
			// Need to figure out how to handle time...
			return new UTCTimeZone();

			//if (SiteConfiguration.AdjustDisplayTimeZone)
			//{
			//    return TimeZone.CurrentTimeZone as WindowsTimeZone;
			//}
		}
	}
}
