using DasBlog.Web.UI.Core.Configuration;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.UI.Core
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
        TimeZone GetConfiguredTimeZone();
    }
}
