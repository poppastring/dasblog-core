using DasBlog.Core;
using DasBlog.Core.Common;
using DasBlog.Core.Common.Comments;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
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

namespace DasBlog.Web.Settings
{
	public class DasBlogSettings : IDasBlogSettings
	{
		private readonly IFileProvider fileProvider;
		private readonly string siteSecurityConfigFilePath;
		private readonly ConfigFilePathsDataOption filePathDataOptions;

		public DasBlogSettings(IWebHostEnvironment env, IOptions<SiteConfig> siteConfig, IOptions<MetaTags> metaTagsConfig, ISiteSecurityConfig siteSecurityConfig, 
								IFileProvider fileProvider, IOptions<ConfigFilePathsDataOption> optionsAccessor)
		{
			this.fileProvider = fileProvider;

			WebRootDirectory = env.ContentRootPath;
			SiteConfiguration = siteConfig.Value;
			SecurityConfiguration = siteSecurityConfig;
			MetaTags = metaTagsConfig.Value;
			filePathDataOptions = optionsAccessor.Value;

			RssUrl = RelativeToRoot("feed/rss");
			PingBackUrl = RelativeToRoot("feed/pingback");
			CategoryUrl = RelativeToRoot("category");
			ArchiveUrl = RelativeToRoot("archive");
			MicroSummaryUrl = RelativeToRoot("site/microsummary");
			RsdUrl = RelativeToRoot("feed/rsd");
			ShortCutIconUrl = RelativeToRoot(string.Format("theme/{0}/icon.ico", SiteConfiguration.Theme));
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

		public static string GetBaseUrl(string root)
		{
			return new Uri(root).AbsoluteUri;
		}

		public string RelativeToRoot(string relative)
		{
			return new Uri(new Uri(SiteConfiguration.Root), relative).AbsoluteUri;
		}

		public static string RelativeToRoot(string relative, string root)
		{
			return new Uri(new Uri(root), relative).AbsoluteUri;
		}
		/// <summary>
		/// sticks root on the front of the entry id
		/// </summary>
		/// <param name="entryId">typically a guid</param>
		/// <param name="root">e.g. http://localhost:50432/</param>
		/// <returns></returns>
        public string GetPermaLinkUrl(string entryId)
        {
            return RelativeToRoot("post/" + entryId);
        }

		public static string GetPermaLinkUrl(string entryId, string root)
		{
			return RelativeToRoot("post/" + entryId, root);
		}

		public string GetPermaTitle(string title)
		{
			string titlePermalink = title.Trim().ToLower();

			titlePermalink = titlePermalink.Replace("+", SiteConfiguration.TitlePermalinkSpaceReplacement);
			
			return titlePermalink;
		}

		public static string GetPermaTitle(string title, string permaLinkSpaceReplacement)
		{
			string titlePermalink = title.Trim().ToLower();

			titlePermalink = titlePermalink.Replace("+", permaLinkSpaceReplacement);
			
			return titlePermalink;
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

		public static DateTimeZone GetConfiguredTimeZone(bool adjustDisplayTimeZone, decimal displayTimeZoneIndex)
		{
			if (adjustDisplayTimeZone)
			{
				return DateTimeZone.ForOffset(Offset.FromSeconds((int)displayTimeZoneIndex * 3600));
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

		public static DateTime GetContentLookAhead(int contentLookAheadDays)
		{
			return DateTime.UtcNow.AddDays(contentLookAheadDays);
		}

		public string FilterHtml(string input)
		{
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

			return (DateTime.UtcNow.AddDays(-1 * SiteConfiguration.DaysCommentsAllowed) < blogpostdate);
		}

		/// <summary>
		/// sticks root on the front of the feeds url
		/// </summary>
		/// <param name="root">e.g. http://localhost:50432/</param>
		/// <returns>e.g. http://localhost:50432;feed/rsd</returns>
		public static string GetRsdUrl(string root)
		{
			return RelativeToRoot("feed/rsd", root);
		}

		public string CompressTitle(string title)
		{
			return Entry.InternalCompressTitle(title, SiteConfiguration.TitlePermalinkSpaceReplacement).ToLower();
		}
	}
}
