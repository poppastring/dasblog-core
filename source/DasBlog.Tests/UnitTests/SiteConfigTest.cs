using DasBlog.Core.Common.Comments;
using DasBlog.Core.Configuration;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DasBlog.Tests.UnitTests
{
	public class SiteConfigTest : ISiteConfig
	{
		public string Title { get => "Title"; set => throw new NotImplementedException(); }
		public string Subtitle { get => "Subtitle"; set => throw new NotImplementedException(); }
		public string Theme { get => "dasblog"; set => throw new NotImplementedException(); }
		public string Description { get => "Description"; set => throw new NotImplementedException(); }
		public string Contact { get => "Contact"; set => throw new NotImplementedException(); }
		public string Root { get => "http://www.poppastring.com/"; set => throw new NotImplementedException(); }
		public string Copyright { get => "CopyRight"; set => throw new NotImplementedException(); }
		public int RssDayCount { get => 100; set => throw new NotImplementedException(); }
		public bool ShowCommentCount { get => true; set => throw new NotImplementedException(); }

		public int RssMainEntryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int RssEntryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableRssItemFooters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RssItemFooter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int FrontPageDayCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int FrontPageEntryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CategoryAllEntries { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string FrontPageCategory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool AlwaysIncludeContentInRSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EntryTitleAsLink { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ObfuscateEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string NotificationEMailAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SendCommentsByEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SendReferralsByEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SendTrackbacksByEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SendPingbacksByEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SendPostsByEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableAboutView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableBloggerApi { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TinyMCEApiKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableComments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCommentApi { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableConfigEditService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableEditService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableAutoPingback { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTrackbackService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnablePingbackService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableStartPageCaching { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableBlogrollDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableUrlRewriting { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCrossposts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseUserCulture { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ShowItemDescriptionInAggregatedViews { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableClickThrough { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableAggregatorBugging { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int DisplayTimeZoneIndex { get => 4; set => throw new NotImplementedException(); }
		public bool AdjustDisplayTimeZone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string EditPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ContentDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string LogDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string BinariesDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ProfilesDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string BinariesDirRelative { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnablePop3 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Pop3Server { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Pop3Username { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Pop3Password { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string Pop3SubjectPrefix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Pop3Interval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool Pop3InlineAttachedPictures { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int Pop3InlinedAttachedPicturesThumbHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ApplyContentFiltersToWeb { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ApplyContentFiltersToRSS { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableXSSUpstream { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string XSSUpstreamEndpoint { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string XSSUpstreamUsername { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string XSSUpstreamPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string XSSRSSFilename { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int XSSUpstreamInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ContentFilterCollection ContentFilters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ContentFilter[] ContentFilterArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public CrosspostSiteCollection CrosspostSites { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public CrosspostSite[] CrosspostSiteArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool Pop3DeleteAllMessages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool Pop3LogIgnoredEmails { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableReferralUrlBlackList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ReferralUrlBlackList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string[] ReferralUrlBlackListArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCaptcha { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string RecaptchaSiteKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string RecaptchaSecretKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double RecaptchaMinimumScore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableReferralUrlBlackList404s { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableMovableTypeBlackList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ChannelImageUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCrossPostFooter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CrossPostFooter { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ExtensionlessUrls { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLink { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLinkUnique { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLinkSpaces { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EncryptLoginPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSmtpAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpUserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpFromEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RssLanguage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSearchHighlight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableEntryReferrals { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public PingService[] PingServiceArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public PingServiceCollection PingServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string FeedBurnerName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int DaysCommentsAllowed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCommentDays { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool SupressEmailAddressDisplay { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string EntryEditControl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool LogBlockedReferrals { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ShowCommentsWhenViewingEntry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseFeedSchemeForSyndication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int ContentLookaheadDays { get => 2; set => throw new NotImplementedException(); }
		public bool EnableAutoSave { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int SmtpPort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsAllowGravatar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarNoImgPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarBorder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsRequireApproval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsAllowHtml { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string XmlAllowedTags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCoComment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSpamBlockingService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SpamBlockingServiceApiKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ISpamBlockingService SpamBlockingService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSpamModeration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int EntriesPerPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableDailyReportEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseSSLForSMTP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string PreferredBloggingAPI { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableGoogleMaps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string GoogleMapsApiKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableGeoRss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double DefaultLatitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double DefaultLongitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableDefaultLatLongForNonGeoCodedPosts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool HtmlTidyContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ResolveCommenterIP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool AllowOpenIdComments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool AllowOpenIdAdmin { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool BypassSpamOpenIdComment { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TitlePermalinkSpaceReplacement { get => "-"; set => throw new NotImplementedException(); }
		public bool AMPPagesEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RSSEndPointRewrite { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CheesySpamQ { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CheesySpamA { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public XmlElement[] anyElements { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public XmlAttribute[] anyAttributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ShowItemSummaryInAggregatedViews { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseAspxExtension { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CookieConsentEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		ValidCommentTags[] ISiteConfig.ValidCommentTags { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

		public string SecurityScriptSources { get; set; }

		public string SecurityStyleSources { get; set; }
		public string DefaultSources { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string MastodonServerUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string MastodonAccount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AllowMarkdownInComments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCloudEvents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public CloudEventsTargetCollection CloudEventsTargets { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public CloudEventsTarget[] CloudEventsTargetArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
