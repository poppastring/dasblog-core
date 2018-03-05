using System;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.Core.Security;

namespace DasBlog.Web.Core
{
	public interface IDasBlogSettings
	{
		ISiteConfig SiteConfiguration { get; }
		IMetaTags MetaTags { get; }
		ISiteSecurityConfig SecurityConfiguration { get; }

		string WebRootDirectory { get; }

		string RssUrl { get; }
		string CategoryUrl { get; }
		string ArchiveUrl { get; }
		string MicroSummaryUrl { get; }
		string RsdUrl { get; }

		string ShortCutIconUrl { get; }

		string RelativeToRoot(string relative);
		string GetBaseUrl();
		string GetPermaLinkUrl(string entryId);
		string GetTrackbackUrl(string entryId);
		string GetEntryCommentsRssUrl(string entryId);
		string GetCommentViewUrl(string entryId);
		string GetCategoryViewUrl(string category);
		User GetUser(string userName);
		void AddUser(User user);
		TimeZone GetConfiguredTimeZone();
	}
}
