using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class SiteViewModel
	{
		[DisplayName("Title")]
		[Description("Main title of the blog")]
		public string Title { get; set; }

		[DisplayName("Subtitle")]
		[Description("Subtitle of the blog")]
		public string Subtitle { get; set; }

		[DisplayName("Theme")]
		[Description("Allows you to select one of several themes in the 'themes' folder. You can also create your own theme folder and update this element accordingly")]
		public string Theme { get; set; }

		[DisplayName("Description")]
		[Description("A more detailed description of the blog")]
		public string Description { get; set; }

		[DisplayName("Email contact")]
		[Description("The email that you want to be publicly available, it also shows up in the RSS feed")]
		[EmailAddress]
		public string Contact { get; set; }


		[DisplayName("Notification email")]
		[Description("Private email where notifications will be sent")]
		[EmailAddress]
		public string NotificationEMailAddress { get; set; }

		[DisplayName("Root URL")]
		[Description("This is the most important element for you to change. This element contains the external root URL of you Weblog. All relative links are built using this value, instead of relying on the URL that was used for the incoming request, because that URL may not be what we want to have, especially when the URLs come through a complex redirect")]
		public string Root { get; set; }

		[DisplayName("Copyright")]
		[Description("Name of the sites copyright owner")]
		public string Copyright { get; set; }



		[DisplayName("Front Page Day Count")]
		[Description("The maximum number of days to appear on your home page")]
		public int FrontPageDayCount { get; set; }

		[DisplayName("Front Page Entry Count")]
		[Description("Number of blog posts on the home page of your blog")]
		public int FrontPageEntryCount { get; set; }

		[DisplayName("Front Page Category")]
		[Description("Default category of the front page")]
		public string FrontPageCategory { get; set; }

		[DisplayName("Entries Per Page")]
		[Description(@"Number of blog posts on the pages of your blog e.g \page\1, page\2, etc")]
		public int EntriesPerPage { get; set; }

		[DisplayName("Enable Start Page Caching")]
		[Description("Uses standard web based caching on the server side (will increase memory usage).")]
		public bool EnableStartPageCaching { get; set; }

		[DisplayName("Content Look ahead Days")]
		[Description("Looks for future posts for this number of days into the future")]
		public int ContentLookaheadDays { get; set; }

		[DisplayName("Show 'Item Summary' in Aggregated Views")]
		[Description("This allows you to design a summary view for each blog post on the home page")]
		public bool ShowItemSummaryInAggregatedViews { get; set; }



		[DisplayName("RSS Day Count")]
		[Description("Maximum number of days to appear in your RSS feed")]
		public int RssDayCount { get; set; }

		[DisplayName("RSS Main Entry Count")]
		[Description("The number of entries permitted in the main RSS feed")]
		public int RssMainEntryCount { get; set; }

		[DisplayName("RSS Entry Category Count")]
		[Description("The number of entries permitted per RSS category feed")]
		public int RssEntryCount { get; set; }

		[DisplayName("Enable RSS Item Footers")]
		[Description("Include the message defined in RssItemFooter in the RSS Feed")]
		public bool EnableRssItemFooters { get; set; }

		[DisplayName("RSS Item Footer")]
		[Description("The message to include in the RSS footer")]
		public string RssItemFooter { get; set; }

		[DisplayName("Always Include Content In RSS Feed")]
		[Description("Allows you to include blog content in RSS feeds")]
		public bool AlwaysIncludeContentInRSS { get; set; }




		[DisplayName("Enable Comments")]
		[Description("Allow comments on your blog posts")]
		public bool EnableComments { get; set; }

		[DisplayName("Enable Comment Days")]
		[Description("Once enabled comments are allowed as defined by 'Days Comments Allowed'")]
		public bool EnableCommentDays { get; set; }

		[DisplayName("Days Comments Allowed")]
		[Description("The number of days a post can receive comments after publishing when 'Enable Comment Days' is set to true")]
		public int DaysCommentsAllowed { get; set; }

		[DisplayName("Show Comments When Viewing an Entry")]
		[Description("Shows the comments associated with a blog post by default.")]
		public bool ShowCommentsWhenViewingEntry { get; set; }

		[DisplayName("Spam Prevention Question")]
		[Description("Cheesy Spam Question - Defines a question that end users need to answer before a comment is submitted (only enabled when CheesySpamA has a value).")]
		public string CheesySpamQ { get; set; }

		[DisplayName("Spam Prevention Answer")]
		[Description("Cheesy Spam Answer - Defines an answer that the commenters need to respond with in order to submit a comment (only enabled when CheesySpamQ has a val")]
		public string CheesySpamA { get; set; }



		[DisplayName("")]
		[Description(@"Enable Title PermaLink Unique - Ensures all urls are unique by adding a date to the URL '\somepost' becomes '20191112\some - post'")]
		public bool EnableTitlePermaLinkUnique { get; set; }

		[DisplayName("Title Permalink Space Replacement")]
		[Description(@"Defaults to '-', however, '+' is the other valid option")]
		public string TitlePermalinkSpaceReplacement { get; set; }



		[DisplayName("Enable Blogger API")]
		[Description("")]
		public bool EnableBloggerApi { get; set; }
		
		[DisplayName("Preferred Blogging API")]
		[Description("")]
		public string PreferredBloggingAPI { get; set; }

		[DisplayName("RSS Channel Image Url")]
		[Description("")]
		public string ChannelImageUrl { get; set; }


		[DisplayName("Entry Edit Control")]
		[Description("")]
		public string EntryEditControl { get; set; }



		[DisplayName("Content Directory")]
		[Description("")]
		public string ContentDir { get; set; }

		[DisplayName("Logging Dir")]
		[Description("")]
		public string LogDir { get; set; }

		[DisplayName("Adjust Display TimeZone")]
		[Description("")]
		public bool AdjustDisplayTimeZone { get; set; }

		[DisplayName("Display TimeZone Index")]
		[Description("")]
		public int DisplayTimeZoneIndex { get; set; }
		

		[DisplayName("")]
		[Description("")]
		public string BinariesDir { get; set; }
		public bool EntryTitleAsLink { get; set; }
		public bool ObfuscateEmail { get; set; }
		public bool SendCommentsByEmail { get; set; }
		public bool SendReferralsByEmail { get; set; }
		public bool SendTrackbacksByEmail { get; set; }
		public bool SendPingbacksByEmail { get; set; }
		public bool SendPostsByEmail { get; set; }
		public bool EnableCommentApi { get; set; }
		public bool EnableConfigEditService { get; set; }
		public bool EnableEditService { get; set; }
		public bool EnableAutoPingback { get; set; }
		public bool ShowCommentCount { get; set; }
		public bool EnableTrackbackService { get; set; }
		public bool EnablePingbackService { get; set; }
		public bool EnableBlogrollDescription { get; set; }
		public bool EnableUrlRewriting { get; set; }
		public bool EnableCrossposts { get; set; }
		public bool UseUserCulture { get; set; }
		public bool EnableClickThrough { get; set; }
		public bool EnableAggregatorBugging { get; set; }
		public string EditPassword { get; set; }
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
		public bool Pop3DeleteAllMessages { get; set; }
		public bool Pop3LogIgnoredEmails { get; set; }
		public bool EnableReferralUrlBlackList { get; set; }
		public string ReferralUrlBlackList { get; set; }
		public string[] ReferralUrlBlackListArray { get; set; }
		public bool EnableCaptcha { get; set; }
		public bool EnableReferralUrlBlackList404s { get; set; }
		public bool EnableMovableTypeBlackList { get; set; }
		public bool EnableCrossPostFooter { get; set; }
		public string CrossPostFooter { get; set; }
		public bool ExtensionlessUrls { get; set; }
		public bool EnableTitlePermaLink { get; set; }
		public bool EnableTitlePermaLinkSpaces { get; set; }
		public bool EncryptLoginPassword { get; set; }
		public bool EnableSmtpAuthentication { get; set; }
		public string SmtpUserName { get; set; }
		public string SmtpPassword { get; set; }
		public string RssLanguage { get; set; }
		public bool EnableSearchHighlight { get; set; }
		public bool EnableEntryReferrals { get; set; }
		public string FeedBurnerName { get; set; }
		public bool SupressEmailAddressDisplay { get; set; }
		public bool LogBlockedReferrals { get; set; }
		public bool UseFeedSchemeForSyndication { get; set; }
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
		//public ISpamBlockingService SpamBlockingService { get; set; }
		public bool EnableSpamModeration { get; set; }
		public bool EnableDailyReportEmail { get; set; }
		public bool UseSSLForSMTP { get; set; }
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
		public bool AMPPagesEnabled { get; set; }
		public string RSSEndPointRewrite { get; set; }
		public bool CategoryAllEntries { get; set; }
		public string AllowedHosts { get; set; }
	}
}
