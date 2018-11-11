using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Core.Configuration
{
    public interface ISiteConfig
    {
        string Title { get; set; }

        string Subtitle { get; set; }

        string Theme { get; set; }

        string Description { get; set; }
    
        string Contact { get; set; }

        string Root { get; set; }

        string Copyright { get; set; }

        int RssDayCount { get; set; }

        int RssMainEntryCount { get; set; }

        int RssEntryCount { get; set; }

        bool EnableRssItemFooters { get; set; }

        string RssItemFooter { get; set; }

        int FrontPageDayCount { get; set; }

        int FrontPageEntryCount { get; set; }

        bool CategoryAllEntries { get; set; }

        string FrontPageCategory { get; set; }

        bool AlwaysIncludeContentInRSS { get; set; }

        bool EntryTitleAsLink { get; set; }

        bool ObfuscateEmail { get; set; }

        string NotificationEMailAddress { get; set; }

        bool SendCommentsByEmail { get; set; }

        bool SendReferralsByEmail { get; set; }

        bool SendTrackbacksByEmail { get; set; }

        bool SendPingbacksByEmail { get; set; }

        bool SendPostsByEmail { get; set; }

        bool EnableBloggerApi { get; set; }

        bool EnableComments { get; set; }

        bool EnableCommentApi { get; set; }

        bool EnableConfigEditService { get; set; }

        bool EnableEditService { get; set; }

        bool EnableAutoPingback { get; set; }

        bool ShowCommentCount { get; set; }

        bool EnableTrackbackService { get; set; }

        bool EnablePingbackService { get; set; }

        bool EnableStartPageCaching { get; set; }

        bool EnableBlogrollDescription { get; set; }

        bool EnableUrlRewriting { get; set; }

        bool EnableCrossposts { get; set; }

        bool UseUserCulture { get; set; }

        bool ShowItemDescriptionInAggregatedViews { get; set; }

		bool ShowItemSummaryInAggregatedViews { get; set; }

		bool EnableClickThrough { get; set; }

        bool EnableAggregatorBugging { get; set; }

        int DisplayTimeZoneIndex { get; set; }

        bool AdjustDisplayTimeZone { get; set; }

        string EditPassword { get; set; }

        string ContentDir { get; set; }

        string LogDir { get; set; }

        string BinariesDir { get; set; }

        string ProfilesDir { get; set; }

        string BinariesDirRelative { get; set; }

        string SmtpServer { get; set; }

        bool EnablePop3 { get; set; }

        string Pop3Server { get; set; }

        string Pop3Username { get; set; }

        string Pop3Password { get; set; }

        string Pop3SubjectPrefix { get; set; }

        int Pop3Interval { get; set; }

        bool Pop3InlineAttachedPictures { get; set; }

        int Pop3InlinedAttachedPicturesThumbHeight { get; set; }

        bool ApplyContentFiltersToWeb { get; set; }

        bool ApplyContentFiltersToRSS { get; set; }

        bool EnableXSSUpstream { get; set; }

        string XSSUpstreamEndpoint { get; set; }

        string XSSUpstreamUsername { get; set; }

        string XSSUpstreamPassword { get; set; }

        string XSSRSSFilename { get; set; }

        int XSSUpstreamInterval { get; set; }

        [XmlIgnore]
        ContentFilterCollection ContentFilters { get; set; }

        ContentFilter[] ContentFilterArray { get; set; }

        [XmlIgnore]
        CrosspostSiteCollection CrosspostSites { get; set; }

        [XmlArray("CrosspostSites")]
        CrosspostSite[] CrosspostSiteArray { get; set; }

        bool Pop3DeleteAllMessages { get; set; }

        bool Pop3LogIgnoredEmails { get; set; }

        bool EnableReferralUrlBlackList { get; set; }

        string ReferralUrlBlackList { get; set; }

        string[] ReferralUrlBlackListArray { get; set; }

        bool EnableCaptcha { get; set; }

        bool EnableReferralUrlBlackList404s { get; set; }

        bool EnableMovableTypeBlackList { get; set; }

        string ChannelImageUrl { get; set; }

        bool EnableCrossPostFooter { get; set; }

        string CrossPostFooter { get; set; }

        bool ExtensionlessUrls { get; set; }

        bool EnableTitlePermaLink { get; set; }

        bool EnableTitlePermaLinkUnique { get; set; }

        bool EnableTitlePermaLinkSpaces { get; set; }

        bool EncryptLoginPassword { get; set; }

        bool EnableSmtpAuthentication { get; set; }

        string SmtpUserName { get; set; }

        string SmtpPassword { get; set; }

        string RssLanguage { get; set; }

        bool EnableSearchHighlight { get; set; }

        bool EnableEntryReferrals { get; set; }

        [XmlArray("PingServices", IsNullable = true)]
        PingService[] PingServiceArray { get; set; }

        [XmlIgnore]
        PingServiceCollection PingServices { get; set; }

        string FeedBurnerName { get; set; }

        int DaysCommentsAllowed { get; set; }

        bool EnableCommentDays { get; set; }

        bool SupressEmailAddressDisplay { get; set; }

        string EntryEditControl { get; set; }

        bool LogBlockedReferrals { get; set; }

        bool ShowCommentsWhenViewingEntry { get; set; }

        bool UseFeedSchemeForSyndication { get; set; }

        int ContentLookaheadDays { get; set; }

        bool EnableAutoSave { get; set; }

        int SmtpPort { get; set; }

        bool CommentsAllowGravatar { get; set; }

        string CommentsGravatarNoImgPath { get; set; }

        string CommentsGravatarSize { get; set; }

        string CommentsGravatarBorder { get; set; }

        string CommentsGravatarRating { get; set; }

        bool CommentsRequireApproval { get; set; }

        bool CommentsAllowHtml { get; set; }

        [XmlArray("validCommentTags", IsNullable = true)]
        [XmlArrayItem("tag")]
        ValidTagCollection XmlAllowedTagsArray { get; set; }

        [XmlIgnore]
        ValidTagCollection AllowedTags { get; set; }

        [XmlElement("AllowedTags")]
        [Obsolete("Please use the AllowedTags property.")]
        string XmlAllowedTags { get; set; }

        bool EnableCoComment { get; set; }

        bool EnableSpamBlockingService { get; set; }

        string SpamBlockingServiceApiKey { get; set; }

        [XmlIgnore]
        ISpamBlockingService SpamBlockingService { get; set; }

        bool EnableSpamModeration { get; set; }

        int EntriesPerPage { get; set; }

        bool EnableDailyReportEmail { get; set; }

        bool UseSSLForSMTP { get; set; }

        string PreferredBloggingAPI { get; set; }

        bool EnableGoogleMaps { get; set; }

        string GoogleMapsApiKey { get; set; }

        bool EnableGeoRss { get; set; }

        double DefaultLatitude { get; set; }

        double DefaultLongitude { get; set; }

        bool EnableDefaultLatLongForNonGeoCodedPosts { get; set; }

        bool HtmlTidyContent { get; set; }

        bool ResolveCommenterIP { get; set; }

        bool AllowOpenIdComments { get; set; }

        bool AllowOpenIdAdmin { get; set; }

        bool BypassSpamOpenIdComment { get; set; }

        string TitlePermalinkSpaceReplacement { get; set; }

        bool AMPPagesEnabled { get; set; }

        string RSSEndPointRewrite { get; set; }

        string CheesySpamQ { get; set; }

        string CheesySpamA { get; set; }

        [XmlAnyElement]
        XmlElement[] anyElements { get; set; }

        [XmlAnyAttribute]
        XmlAttribute[] anyAttributes { get; set; }
    }
}
