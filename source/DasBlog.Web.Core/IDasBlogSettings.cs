using System;
using DasBlog.Core.Configuration;
using DasBlog.Core.Security;
using NodaTime;

namespace DasBlog.Core
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
		string GetPermaTitle(string title);
		string GetTrackbackUrl(string entryId);
		string GetEntryCommentsRssUrl(string entryId);
		string GetCommentViewUrl(string entryId);
		string GetCategoryViewUrl(string category);
		string GetCategoryViewUrlName(string category);
		string GetRssCategoryUrl(string category);
		User GetUser(string userName);
		void AddUser(User user);
		DateTimeZone GetConfiguredTimeZone();
		DateTime GetContentLookAhead();
	}
}
