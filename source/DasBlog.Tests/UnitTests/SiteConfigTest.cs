using DasBlog.Core.Common.Comments;
using DasBlog.Core.Configuration;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using System;
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
		public string CdnFrom{ get => ""; set => throw new NotImplementedException(); }
		public string CdnTo{ get => ""; set => throw new NotImplementedException(); }
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
		public bool EnableBloggerApi { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TinyMCEApiKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableComments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableStartPageCaching { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public decimal DisplayTimeZoneIndex { get => 4; set => throw new NotImplementedException(); }
		public bool AdjustDisplayTimeZone { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ContentDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string LogDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string BinariesDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ProfilesDir { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string BinariesDirRelative { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpServer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCaptcha { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RecaptchaSiteKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RecaptchaSecretKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double RecaptchaMinimumScore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string ChannelImageUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLink { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLinkUnique { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableTitlePermaLinkSpaces { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSmtpAuthentication { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpUserName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpPassword { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string SmtpFromEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string RssLanguage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSearchHighlight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int DaysCommentsAllowed { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableCommentDays { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string EntryEditControl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool LogBlockedReferrals { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool ShowCommentsWhenViewingEntry { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int ContentLookaheadDays { get => 2; set => throw new NotImplementedException(); }
		public int SmtpPort { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsAllowGravatar { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarNoImgPath { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarBorder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string CommentsGravatarRating { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsRequireApproval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool CommentsAllowHtml { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public ISpamBlockingService SpamBlockingService { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableSpamModeration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public int EntriesPerPage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableDailyReportEmail { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool UseSSLForSMTP { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string PreferredBloggingAPI { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableGeoRss { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double DefaultLatitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public double DefaultLongitude { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableDefaultLatLongForNonGeoCodedPosts { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool HtmlTidyContent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string TitlePermalinkSpaceReplacement { get => "-"; set => throw new NotImplementedException(); }
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
		public bool EnableRewritingHashtagsToCategoryLinks { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableRewritingBareLinksToEmbeddings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public bool EnableRewritingBareLinksToIcons { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
		public string PostPinnedToHomePage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
	}
}
