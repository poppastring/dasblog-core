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
using newtelligence.DasBlog.Runtime;

using System;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Services.ConfigFile.Interfaces
{
    public interface ISiteConfig
    {
        string Title { get; set; }

        string Subtitle { get; set; }

        string Theme { get; set; }

        string Description { get; set; }
    
        string Contact { get; set; }

        string Root { get; set; }

        string CdnFrom { get; set; }
        string CdnTo { get; set; }

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

        string TinyMCEApiKey { get; set; }
        
        bool EnableBloggerApi { get; set; }

        bool EnableComments { get; set; }

		bool AllowMarkdownInComments {get; set;}

		bool ShowCommentCount { get; set; }

		bool EnableStartPageCaching { get; set; }

		bool ShowItemSummaryInAggregatedViews { get; set; }

        decimal DisplayTimeZoneIndex { get; set; }

        bool AdjustDisplayTimeZone { get; set; }

        string ContentDir { get; set; }

        string LogDir { get; set; }

        string BinariesDir { get; set; }

        string ProfilesDir { get; set; }

        string BinariesDirRelative { get; set; }

        string SmtpServer { get; set; }

        bool EnableCaptcha { get; set; }

        string RecaptchaSiteKey { get; set; }

        string RecaptchaSecretKey { get; set; } 

        double RecaptchaMinimumScore { get; set; }

        string ChannelImageUrl { get; set; }

        bool EnableTitlePermaLink { get; set; }

        bool EnableTitlePermaLinkUnique { get; set; }

        bool EnableTitlePermaLinkSpaces { get; set; }

        bool EnableSmtpAuthentication { get; set; }

        string SmtpUserName { get; set; }
        string SmtpFromEmail { get; set; }

        string SmtpPassword { get; set; }

        string RssLanguage { get; set; }

        bool EnableSearchHighlight { get; set; }

        int DaysCommentsAllowed { get; set; }

        bool EnableCommentDays { get; set; }

        string EntryEditControl { get; set; }

        bool LogBlockedReferrals { get; set; }

        bool ShowCommentsWhenViewingEntry { get; set; }

        int ContentLookaheadDays { get; set; }

        int SmtpPort { get; set; }

        bool CommentsAllowGravatar { get; set; }

        string CommentsGravatarNoImgPath { get; set; }

        string CommentsGravatarSize { get; set; }

        string CommentsGravatarBorder { get; set; }

        string CommentsGravatarRating { get; set; }

        bool CommentsRequireApproval { get; set; }

		bool CommentsAllowHtml { get; set; }

		ValidCommentTags [] ValidCommentTags { get; set; }

		[XmlIgnore]
		ISpamBlockingService SpamBlockingService { get; set; }

		bool EnableSpamModeration { get; set; }

        int EntriesPerPage { get; set; }

        bool EnableDailyReportEmail { get; set; }

        bool UseSSLForSMTP { get; set; }

        string PreferredBloggingAPI { get; set; }

        bool EnableGeoRss { get; set; }

        double DefaultLatitude { get; set; }

        double DefaultLongitude { get; set; }

        bool EnableDefaultLatLongForNonGeoCodedPosts { get; set; }

        bool HtmlTidyContent { get; set; }

        string TitlePermalinkSpaceReplacement { get; set; }

        string CheesySpamQ { get; set; }

        string CheesySpamA { get; set; }

		bool UseAspxExtension { get; set; }

		string SecurityScriptSources { get; set; }

		string SecurityStyleSources { get; set; }

		string DefaultSources { get; set; }

		bool CookieConsentEnabled { get; set; }

		string MastodonServerUrl { get; set; }

		string MastodonAccount { get; set; }

		[XmlAnyElement]
        XmlElement[] anyElements { get; set; }

        [XmlAnyAttribute]
        XmlAttribute[] anyAttributes { get; set; }
		bool EnableRewritingHashtagsToCategoryLinks { get; set; }
		bool EnableRewritingBareLinksToEmbeddings { get; set; }
		bool EnableRewritingBareLinksToIcons { get; set; }

		string PostPinnedToHomePage { get; set; }
	}
}
