#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using DasBlog.Core.Common.Comments;
using DasBlog.Core.Configuration;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using System;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Services.ConfigFile
{
	[Serializable]
	[XmlType("SiteConfig")]
	public class SiteConfig : ISiteConfig
    {
        private string _root;

        public SiteConfig() { }

        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Theme { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
        public string Root {
            get
            {
                return _root;
            }
            set 
            {
                if ( !string.IsNullOrEmpty(value) )
                {
                    _root = value + (value.EndsWith("/")?"":"/");
                }
                else
                {
                    _root = value;
                }
            }
        }
		public string AllowedHosts { get; set; }
		public string Copyright { get; set; }
        public int RssDayCount { get; set; }
        public int RssMainEntryCount { get; set; }
        public int RssEntryCount { get; set; }
        public bool EnableRssItemFooters { get; set; }
        public string RssItemFooter { get; set; }
        public int FrontPageDayCount { get; set; }
        public int FrontPageEntryCount { get; set; }
        public bool CategoryAllEntries { get; set; }
        public string FrontPageCategory { get; set; }
        public bool AlwaysIncludeContentInRSS { get; set; }
        public bool EntryTitleAsLink { get; set; }
        public bool ObfuscateEmail { get; set; }
        public string NotificationEMailAddress { get; set; }
        public bool SendCommentsByEmail { get; set; }
        public bool SendReferralsByEmail { get; set; }
        public bool SendTrackbacksByEmail { get; set; }
        public bool SendPingbacksByEmail { get; set; }
        public bool SendPostsByEmail { get; set; }
        public bool EnableAboutView { get; set; }
        public string TinyMCEApiKey { get; set; }
        public bool EnableBloggerApi { get; set; }
        public bool EnableComments { get; set; }
        public bool AllowMarkdownInComments {get; set;}
        public bool EnableCommentApi { get; set; }
        public bool EnableConfigEditService { get; set; }
        public bool EnableEditService { get; set; }
        public bool EnableAutoPingback { get; set; }
        public bool ShowCommentCount { get; set; }
        public bool EnableTrackbackService { get; set; }
        public bool EnablePingbackService { get; set; }
        public bool EnableStartPageCaching { get; set; }
        public bool EnableBlogrollDescription { get; set; }
        public bool EnableUrlRewriting { get; set; }
        public bool EnableCrossposts { get; set; }
        public bool UseUserCulture { get; set; }
        public bool EnableClickThrough { get; set; }
        public bool EnableAggregatorBugging { get; set; }
        public int DisplayTimeZoneIndex { get; set; }
        public bool AdjustDisplayTimeZone { get; set; }
        public string EditPassword { get; set; }
        public string ContentDir { get; set; }
        public string LogDir { get; set; }
        public string BinariesDir { get; set; }
        public string ProfilesDir { get; set; }
        public string BinariesDirRelative { get; set; }
        public string SmtpServer { get; set; }
        public bool EnablePop3 { get; set; }
        public string Pop3Server { get; set; }
        public string Pop3Username { get; set; }
        public string Pop3Password { get; set; }
        public string Pop3SubjectPrefix { get; set; }
        public int Pop3Interval { get; set; }
        public bool Pop3InlineAttachedPictures { get; set; }
        public int Pop3InlinedAttachedPicturesThumbHeight { get; set; }
        public bool ApplyContentFiltersToWeb { get; set; }
        public bool ApplyContentFiltersToRSS { get; set; }
        public bool EnableXSSUpstream { get; set; }
        public string XSSUpstreamEndpoint { get; set; }
        public string XSSUpstreamUsername { get; set; }
        public string XSSUpstreamPassword { get; set; }
        public string XSSRSSFilename { get; set; }
        public int XSSUpstreamInterval { get; set; }
        public ContentFilterCollection ContentFilters { get; set; }
        public ContentFilter[] ContentFilterArray { get; set; }
        public CrosspostSiteCollection CrosspostSites { get; set; }
		public CloudEventsTargetCollection CloudEventsTargets { get; set; }
		public CrosspostSite[] CrosspostSiteArray { get; set; }
		public CloudEventsTarget[] CloudEventsTargetArray { get; set; }
		public bool Pop3DeleteAllMessages { get; set; }
        public bool Pop3LogIgnoredEmails { get; set; }
        public bool EnableReferralUrlBlackList { get; set; }
        public string ReferralUrlBlackList { get; set; }
        public string[] ReferralUrlBlackListArray { get; set; }
        public bool EnableCaptcha { get; set; }
        public string RecaptchaSiteKey { get; set; }
        public string RecaptchaSecretKey { get; set; } 
        public double RecaptchaMinimumScore {get; set; }
        public bool EnableReferralUrlBlackList404s { get; set; }
        public bool EnableMovableTypeBlackList { get; set; }
        public string ChannelImageUrl { get; set; }
        public bool EnableCrossPostFooter { get; set; }
        public string CrossPostFooter { get; set; }
        public bool ExtensionlessUrls { get; set; }
        public bool EnableTitlePermaLink { get; set; }
        public bool EnableTitlePermaLinkUnique { get; set; }
        public bool EnableTitlePermaLinkSpaces { get; set; }
        public bool EncryptLoginPassword { get; set; }
        public bool EnableSmtpAuthentication { get; set; }
        public string SmtpUserName { get; set; }
        public string SmtpFromEmail { get; set; }
        public string SmtpPassword { get; set; }
        public string RssLanguage { get; set; }
        public bool EnableSearchHighlight { get; set; }
        public bool EnableEntryReferrals { get; set; }
        public PingService[] PingServiceArray { get; set; }
        public PingServiceCollection PingServices { get; set; }
        public string FeedBurnerName { get; set; }
        public int DaysCommentsAllowed { get; set; }
        public bool EnableCommentDays { get; set; }
        public bool SupressEmailAddressDisplay { get; set; }
        public string EntryEditControl { get; set; }
        public bool LogBlockedReferrals { get; set; }
        public bool ShowCommentsWhenViewingEntry { get; set; }
        public bool UseFeedSchemeForSyndication { get; set; }
        public int ContentLookaheadDays { get; set; }
        public bool EnableAutoSave { get; set; }
        public int SmtpPort { get; set; }
        public bool CommentsAllowGravatar { get; set; }
        public string CommentsGravatarNoImgPath { get; set; }
        public string CommentsGravatarSize { get; set; }
        public string CommentsGravatarBorder { get; set; }
        public string CommentsGravatarRating { get; set; }
        public bool CommentsRequireApproval { get; set; }
        public bool CommentsAllowHtml { get; set; }

        public bool EnableCoComment { get; set; }
        public bool EnableSpamBlockingService { get; set; }
        public string SpamBlockingServiceApiKey { get; set; }

		[XmlIgnore]
        public ISpamBlockingService SpamBlockingService { get; set; }
        public bool EnableSpamModeration { get; set; }
        public int EntriesPerPage { get; set; }
        public bool EnableDailyReportEmail { get; set; }
        public bool UseSSLForSMTP { get; set; }
        public string PreferredBloggingAPI { get; set; }
        public bool EnableGoogleMaps { get; set; }
        public string GoogleMapsApiKey { get; set; }
        public bool EnableGeoRss { get; set; }
        public double DefaultLatitude { get; set; }
        public double DefaultLongitude { get; set; }
        public bool EnableDefaultLatLongForNonGeoCodedPosts { get; set; }
        public bool HtmlTidyContent { get; set; }
        public bool ResolveCommenterIP { get; set; }
        public bool AllowOpenIdComments { get; set; }
        public bool AllowOpenIdAdmin { get; set; }
        public bool BypassSpamOpenIdComment { get; set; }
        public string TitlePermalinkSpaceReplacement { get; set; }
        public bool AMPPagesEnabled { get; set; }
        public string RSSEndPointRewrite { get; set; }
        public string CheesySpamQ { get; set; }
        public string CheesySpamA { get; set; }
		
		[XmlIgnore]
        public XmlElement[] anyElements { get; set; }

		[XmlIgnore]
		public XmlAttribute[] anyAttributes { get; set; }
		public bool ShowItemSummaryInAggregatedViews { get; set; }

		[XmlElement]
		public ValidCommentTags [] ValidCommentTags { get; set; }

		public bool UseAspxExtension { get; set; }
		public bool CookieConsentEnabled { get; set; }

		public string SecurityScriptSources { get; set; }

		public string SecurityStyleSources { get; set; }

		public string DefaultSources { get; set; }

		public string MastodonServerUrl { get; set; }

		public string MastodonAccount { get; set; }
		public bool EnableCloudEvents { get; set; }
	}
}
