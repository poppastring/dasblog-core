using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Runtime.Proxies;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web
{
    /*
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // STOP! STOP! STOP! STOP!
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // If you want to add entries, please add the properties
    // just above the comment tag at the bottom. SiteConfig
    // is now exposed through a WebService and not putting
    // the properties at the right place will break clients.
    */

    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class SiteConfig
    {
        string title = "(none)";
        string subtitle = "(none)";
        string contact = "(none)";
        string theme = "default";
        string root = "(none)";
        string copyright = "(none)";
        string description = "";
        string frontPageCategory = "";
        int frontPageDayCount = 10;
        int frontPageEntryCount = 50;
        bool categoryAllEntries = true;
        int rssDayCount = 10;
        int rssMainEntryCount = 50;
        int rssEntryCount = 50;
        bool enableRssItemFooters = false;
        string rssItemFooter;
        bool alwaysIncludeContentInRSS = false;
        bool entryTitleAsLink = false;
        bool notifyWebLogsDotCom = false;
        bool notifyBloGs = false;
        bool obfuscateEmail = true;
        string notificationEmailAddress = null;
        bool sendCommentsByEmail = false;
        bool sendPingbacksByEmail = false;
        bool sendTrackbacksByEmail = false;
        bool sendReferralsByEmail = false;
        bool sendPostsByEmail = false;
        bool enableBloggerApi = true;
        bool enableComments = true;
        bool enableCommentApi = true;
        bool enableConfigEditService = false;
        bool showCommentCount = true;
        bool enableAutoPingback = true;
        bool enableEditService = false;
        bool enableTrackbackService = true;
        bool enablePingbackService = true;
        bool applyContentFiltersToWeb = true;
        bool applyContentFiltersToRSS = false;
        bool showItemDescriptionInAggregatedViews = false;
        bool enableStartPageCaching = false;
        bool enableBlogrollDescription = false;
        bool enableUrlRewriting = false;
        bool enableFtb = true;
        bool useUserCulture = true;
        int displayTimeZoneIndex = 90; // this is GMT
        bool adjustDisplayTimeZone = false;
        string editPassword;
        string contentDir;
        string logDir;
        string binariesDir;
        string profilesDir;
        string smtpServer = null;
        bool enablePop3 = false;
        string pop3Server = null;
        string pop3Username = null;
        string pop3Password = null;
        string pop3SubjectPrefix = null;
        bool pop3InlineAttachedPictures = false;
        int pop3InlinedAttachedPicturesThumbHeight = 0;
        int pop3Interval = 240;
        bool enableXSSUpstream = false;
        string xssUpstreamEndpoint = "http://radio.xmlstoragesystem.com/RPC2";
        string xssUpstreamUsername = null;
        string xssUpstreamPassword = null;
        string xssRSSFilename = "rss-dasblog.xml";
        int xssUpstreamInterval = 60 * 60;
        bool enableClickThrough = false;
        bool enableAggregatorBugging = false;
        bool enableCrossposts = true;
        bool enableCrossPostFooter = false;
        string crossPostFooter = null;
        bool extensionlessUrls = false;
        bool enableTitlePermaLink = false;
        bool enableTitlePermaLinkUnique = false;
        bool enableTitlePermaLinkSpaces = false;
        bool enableEntryReferrals = false;
        PingServiceCollection pingServices = new PingServiceCollection();
        int daysCommentsAllowed = 0;
        bool enableCommentDays = false;
        bool logBlockedReferrals = false;
        bool showCommentsWhenViewingEntry = false;
        bool enableAutoSave = false;
        private string titlePermalinkSpaceReplacement = TitleMapperModule.DefaultTitlePermalinkSpaceReplacement;
        bool enableCoComment = true;
        bool enableAMPPages = false;
        string rssEndPointRewrite = string.Empty;
        string cheesySpamA = string.Empty;
        string cheesySpamQ = string.Empty;


        //paulb changed to comments
        bool commentsRequireApproval;
        bool commentsAllowHtml;
        ValidTagCollection allowedTags;
        // default tags
        private const string defaultAllowedTags = "b,i,u,a@href@title,strong,blockquote@cite,em,strike,sup,sub";

        //allow gravatar integration
        bool commentsAllowGravatar = false;
        string commentsGravatarNoImgPath = null;
        string commentsGravatarSize = null;
        string commentsGravatarBorder = null;
        string commentsGravatarRating = null;

        // supress email address display
        bool supressEmailAddressDisplay = false;

        // Allow user to choose default blogging API
        // Can be "Moveable Type", "MetaWeblog" or "Blogger"
        string preferredBloggingAPI = "Moveable Type";

        ContentFilterCollection contentFilters = new ContentFilterCollection();
        CrosspostSiteCollection crosspostSites = new CrosspostSiteCollection();

        public static void Save(SiteConfig siteConfig)
        {
            System.Security.Principal.WindowsImpersonationContext wi = Impersonation.Impersonate();

            XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));

            using (StreamWriter writer = new StreamWriter(SiteConfig.GetConfigFilePathFromCurrentContext()))
            {
                ser.Serialize(writer, siteConfig);
            }

            wi.Undo();
        }

        public static SiteConfig GetSiteConfig()
        {
            DataCache cache = CacheFactory.GetCache();

            SiteConfig config = (SiteConfig)cache["SiteConfig"];
            if (config == null)
            {
                string path = GetConfigFilePathFromCurrentContext();
                config = GetSiteConfig(path);
                cache.Insert("SiteConfig", config, new CacheDependency(path));
            }
            return config;
        }

        public static SiteConfig GetSiteConfig(string configPath)
        {
            SiteConfig config;
            XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));

            using (StreamReader reader = new StreamReader(configPath))
            {
                //SDH: Requires FullTrust
                //XmlNamespaceUpgradeReader xnur = new XmlNamespaceUpgradeReader(reader, "", "urn:newtelligence-com:dasblog:config");
                //config = ser.Deserialize(xnur) as SiteConfig;
                config = ser.Deserialize(reader) as SiteConfig;
            }
            return config;
        }

        public static string GetConfigFilePathFromCurrentContext()
        {
            return SiteUtilities.MapPath("~/SiteConfig/site.config");
        }

        public static string GetSecurityFilePathFromCurrentContext()
        {
            return SiteUtilities.MapPath("~/SiteConfig/siteSecurity.config");
        }

        public static string GetConfigPathFromCurrentContext()
        {
            return SiteUtilities.MapPath("~/SiteConfig/");
        }

        public static string GetContentPathFromCurrentContext()
        {
            return SiteUtilities.MapPath(GetSiteConfig().ContentDir);
        }

        public static string GetLogPathFromCurrentContext()
        {
            return SiteUtilities.MapPath(GetSiteConfig().LogDir);
        }

        public static string GetBinariesPathFromCurrentContext()
        {
            return SiteUtilities.MapPath(GetSiteConfig().BinariesDir);
        }

        public static string GetProfilesPathFromCurrentContext()
        {
            return SiteUtilities.MapPath(GetSiteConfig().ProfilesDir);
        }

        private string CheckTrailingSlashAndRooted(string path)
        {
            if (path == null)
                return path;

            if (path[0] != '~' && path[0] != '/')
                path = "~/" + path;

            if (path[path.Length - 1] != '/')
                return path + '/';
            else
                return path;
        }

        private WindowsTimeZone windowsTimeZone = null;

        public WindowsTimeZone GetConfiguredTimeZone()
        {
            if (windowsTimeZone == null)
            {
                windowsTimeZone = WindowsTimeZone.TimeZones.GetByZoneIndex(displayTimeZoneIndex) as WindowsTimeZone;
            }
            return windowsTimeZone;
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Subtitle
        {
            get { return subtitle; }
            set { subtitle = value; }
        }

        public string Theme
        {
            get { return theme; }
            set { theme = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Contact
        {
            get { return contact; }
            set { contact = value; }
        }

        public string Root
        {
            get
            {
                if (root[root.Length - 1] != '/')
                {
                    root = root + "/";
                }
                return root;
            }
            set
            {
                root = value;
                if (root[root.Length - 1] != '/')
                {
                    root = root + "/";
                }
            }
        }

        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        public int RssDayCount
        {
            get { return rssDayCount; }
            set { rssDayCount = value; }
        }

        public int RssMainEntryCount
        {
            get { return rssMainEntryCount; }
            set { rssMainEntryCount = value; }
        }

        public int RssEntryCount
        {
            get { return rssEntryCount; }
            set { rssEntryCount = value; }
        }

        public bool EnableRssItemFooters
        {
            get { return enableRssItemFooters; }
            set { enableRssItemFooters = value; }
        }

        public string RssItemFooter
        {
            get { return rssItemFooter; }
            set { rssItemFooter = value; }
        }

        public int FrontPageDayCount
        {
            get { return frontPageDayCount; }
            set { frontPageDayCount = value; }
        }

        public int FrontPageEntryCount
        {
            get { return frontPageEntryCount; }
            set { frontPageEntryCount = value; }
        }

        public bool CategoryAllEntries
        {
            get { return categoryAllEntries; }
            set { categoryAllEntries = value; }
        }

        public string FrontPageCategory
        {
            get { return frontPageCategory; }
            set { frontPageCategory = value; }
        }

        public bool AlwaysIncludeContentInRSS
        {
            get { return alwaysIncludeContentInRSS; }
            set { alwaysIncludeContentInRSS = value; }
        }

        public bool EntryTitleAsLink
        {
            get { return entryTitleAsLink; }
            set { entryTitleAsLink = value; }
        }

        [Obsolete]
        public bool NotifyWebLogsDotCom
        {
            get { return notifyWebLogsDotCom; }
            set { notifyWebLogsDotCom = value; }
        }

        [Obsolete]
        public bool NotifyBloGs
        {
            get { return notifyBloGs; }
            set { notifyBloGs = value; }
        }

        public bool ObfuscateEmail
        {
            get { return obfuscateEmail; }
            set { obfuscateEmail = value; }
        }

        public string NotificationEMailAddress
        {
            get { return notificationEmailAddress; }
            set { notificationEmailAddress = value; }
        }

        public bool SendCommentsByEmail
        {
            get { return sendCommentsByEmail; }
            set { sendCommentsByEmail = value; }
        }

        public bool SendReferralsByEmail
        {
            get { return sendReferralsByEmail; }
            set { sendReferralsByEmail = value; }
        }

        public bool SendTrackbacksByEmail
        {
            get { return sendTrackbacksByEmail; }
            set { sendTrackbacksByEmail = value; }
        }

        public bool SendPingbacksByEmail
        {
            get { return sendPingbacksByEmail; }
            set { sendPingbacksByEmail = value; }
        }

        public bool SendPostsByEmail
        {
            get { return sendPostsByEmail; }
            set { sendPostsByEmail = value; }
        }

        public bool EnableBloggerApi
        {
            get { return enableBloggerApi; }
            set { enableBloggerApi = value; }
        }

        public bool EnableComments
        {
            get { return enableComments; }
            set { enableComments = value; }
        }

        public bool EnableCommentApi
        {
            get { return enableCommentApi; }
            set { enableCommentApi = value; }
        }

        public bool EnableConfigEditService
        {
            get { return enableConfigEditService; }
            set { enableConfigEditService = value; }
        }

        public bool EnableEditService
        {
            get { return enableEditService; }
            set { enableEditService = value; }
        }

        public bool EnableAutoPingback
        {
            get { return enableAutoPingback; }
            set { enableAutoPingback = value; }
        }

        public bool ShowCommentCount
        {
            get { return showCommentCount; }
            set { showCommentCount = value; }
        }

        public bool EnableTrackbackService
        {
            get { return enableTrackbackService; }
            set { enableTrackbackService = value; }
        }

        public bool EnablePingbackService
        {
            get { return enablePingbackService; }
            set { enablePingbackService = value; }
        }

        public bool EnableStartPageCaching
        {
            get { return enableStartPageCaching; }
            set { enableStartPageCaching = value; }
        }

        public bool EnableBlogrollDescription
        {
            get { return enableBlogrollDescription; }
            set { enableBlogrollDescription = value; }
        }

        public bool EnableUrlRewriting
        {
            get { return enableUrlRewriting; }
            set { enableUrlRewriting = value; }
        }

        [Obsolete("FreeTextBox is always enabled")]
        public bool EnableFtb
        {
            get { return enableFtb; }
            set { enableFtb = value; }
        }

        public bool EnableCrossposts
        {
            get { return enableCrossposts; }
            set { enableCrossposts = value; }
        }

        public bool UseUserCulture
        {
            get { return useUserCulture; }
            set { useUserCulture = value; }
        }

        public bool ShowItemDescriptionInAggregatedViews
        {
            get { return showItemDescriptionInAggregatedViews; }
            set { showItemDescriptionInAggregatedViews = value; }
        }

        public bool EnableClickThrough
        {
            get { return enableClickThrough; }
            set { enableClickThrough = value; }
        }

        public bool EnableAggregatorBugging
        {
            get { return enableAggregatorBugging; }
            set { enableAggregatorBugging = value; }
        }


        public int DisplayTimeZoneIndex
        {
            get { return displayTimeZoneIndex; }
            set
            {
                displayTimeZoneIndex = value;
                windowsTimeZone = WindowsTimeZone.TimeZones.GetByZoneIndex(displayTimeZoneIndex) as WindowsTimeZone;
            }
        }

        public bool AdjustDisplayTimeZone
        {
            get { return adjustDisplayTimeZone; }
            set { adjustDisplayTimeZone = value; }
        }

        public string EditPassword
        {
            get { return editPassword; }
            set { editPassword = value; }
        }

        public string ContentDir
        {
            get { return CheckTrailingSlashAndRooted(contentDir); }
            set { contentDir = value; }
        }

        public string LogDir
        {
            get { return CheckTrailingSlashAndRooted(logDir); }
            set { logDir = value; }
        }

        public string BinariesDir
        {
            get { return CheckTrailingSlashAndRooted(binariesDir); }
            set { binariesDir = value; }
        }

        public string ProfilesDir
        {
            get { return CheckTrailingSlashAndRooted(profilesDir); }
            set { profilesDir = value; }
        }
        public string BinariesDirRelative
        {
            get
            {
                string retVal = BinariesDir;
                retVal = retVal.TrimStart('~');
                retVal = retVal.TrimStart('/');
                return retVal;
            }
        }


        public string SmtpServer
        {
            get { return smtpServer; }
            set { smtpServer = value; }
        }

        public bool EnablePop3
        {
            get { return enablePop3; }
            set { enablePop3 = value; }
        }

        public string Pop3Server
        {
            get { return pop3Server; }
            set { pop3Server = value; }
        }

        public string Pop3Username
        {
            get { return pop3Username; }
            set { pop3Username = value; }
        }

        public string Pop3Password
        {
            get { return pop3Password; }
            set { pop3Password = value; }
        }

        public string Pop3SubjectPrefix
        {
            get { return pop3SubjectPrefix; }
            set { pop3SubjectPrefix = value; }
        }

        public int Pop3Interval
        {
            get { return pop3Interval; }
            set { pop3Interval = value; }
        }

        public bool Pop3InlineAttachedPictures
        {
            get { return pop3InlineAttachedPictures; }
            set { pop3InlineAttachedPictures = value; }
        }

        public int Pop3InlinedAttachedPicturesThumbHeight
        {
            get { return pop3InlinedAttachedPicturesThumbHeight; }
            set { pop3InlinedAttachedPicturesThumbHeight = value; }
        }

        public bool ApplyContentFiltersToWeb
        {
            get { return applyContentFiltersToWeb; }
            set { applyContentFiltersToWeb = value; }
        }

        public bool ApplyContentFiltersToRSS
        {
            get { return applyContentFiltersToRSS; }
            set { applyContentFiltersToRSS = value; }
        }

        public bool EnableXSSUpstream
        {
            get { return enableXSSUpstream; }
            set { enableXSSUpstream = value; }
        }

        public string XSSUpstreamEndpoint
        {
            get { return xssUpstreamEndpoint; }
            set { xssUpstreamEndpoint = value; }
        }

        public string XSSUpstreamUsername
        {
            get { return xssUpstreamUsername; }
            set { xssUpstreamUsername = value; }
        }

        public string XSSUpstreamPassword
        {
            get { return xssUpstreamPassword; }
            set { xssUpstreamPassword = value; }
        }

        public string XSSRSSFilename
        {
            get { return xssRSSFilename; }
            set { xssRSSFilename = value; }
        }

        public int XSSUpstreamInterval
        {
            get { return xssUpstreamInterval; }
            set { xssUpstreamInterval = value; }
        }

        [XmlIgnore]
        public ContentFilterCollection ContentFilters
        {
            get { return contentFilters; }
        }

        [XmlArray("ContentFilters")]
        public ContentFilter[] ContentFilterArray
        {
            get { return new List<ContentFilter>(contentFilters).ToArray(); }
            set
            {
                if (value == null)
                {
                    contentFilters = new ContentFilterCollection();
                }
                else
                {
                    contentFilters = new ContentFilterCollection(value);
                }
            }
        }

        [XmlIgnore]
        public CrosspostSiteCollection CrosspostSites
        {
            get { return crosspostSites; }
        }

        [XmlArray("CrosspostSites")]
        public CrosspostSite[] CrosspostSiteArray
        {
            get { return new List<CrosspostSite>(CrosspostSites).ToArray(); }
            set
            {
                if (value == null)
                {
                    crosspostSites = new CrosspostSiteCollection();
                }
                else
                {
                    crosspostSites = new CrosspostSiteCollection(value);
                }
            }
        }

        bool pop3DeleteAllMessages = false;

        public bool Pop3DeleteAllMessages
        {
            get { return pop3DeleteAllMessages; }
            set { pop3DeleteAllMessages = value; }
        }

        bool pop3LogIgnoredEmails = true;

        public bool Pop3LogIgnoredEmails
        {
            get { return pop3LogIgnoredEmails; }
            set { pop3LogIgnoredEmails = value; }
        }

        bool enableReferralUrlBlackList = false;
        public bool EnableReferralUrlBlackList
        {
            get { return enableReferralUrlBlackList; }
            set { enableReferralUrlBlackList = value; }
        }
        private string referralUrlBlackList = String.Empty;
        public string ReferralUrlBlackList { get { return referralUrlBlackList; } set { referralUrlBlackList = value; } }
        [XmlIgnore]
        public string[] ReferralUrlBlackListArray { get { return referralUrlBlackList.Split(new char[] { (';') }); } }

        bool enableCaptcha = true;
        public bool EnableCaptcha { get { return enableCaptcha; } set { enableCaptcha = value; } }

        bool enableReferralUrlBlackList404s = true;
        public bool EnableReferralUrlBlackList404s { get { return enableReferralUrlBlackList404s; } set { enableReferralUrlBlackList404s = value; } }

        bool enableMovableTypeBlackList = false;
        public bool EnableMovableTypeBlackList
        {
            get { return enableMovableTypeBlackList; }
            set { enableMovableTypeBlackList = value; }
        }

        private string channelImageUrl;
        public string ChannelImageUrl { get { return channelImageUrl; } set { channelImageUrl = value; } }

        public bool EnableCrossPostFooter
        {
            get { return enableCrossPostFooter; }
            set { enableCrossPostFooter = value; }
        }

        public string CrossPostFooter
        {
            get { return crossPostFooter; }
            set { crossPostFooter = value; }
        }

        public bool ExtensionlessUrls
        {
            get { return extensionlessUrls; }
            set { extensionlessUrls = value; }
        }


        public bool EnableTitlePermaLink
        {
            get { return enableTitlePermaLink; }
            set { enableTitlePermaLink = value; }
        }

        public bool EnableTitlePermaLinkUnique
        {
            get { return enableTitlePermaLinkUnique; }
            set { enableTitlePermaLinkUnique = value; }
        }

        public bool EnableTitlePermaLinkSpaces
        {
            get { return enableTitlePermaLinkSpaces; }
            set { enableTitlePermaLinkSpaces = value; }
        }

        bool encryptLoginPassword = false;

        public bool EncryptLoginPassword
        {
            get { return encryptLoginPassword; }
            set { encryptLoginPassword = value; }
        }

        // RyanG: Added support for SMTP authentication properties
        bool enableSmtpAuthentication = false;

        public bool EnableSmtpAuthentication
        {
            get { return enableSmtpAuthentication; }
            set { enableSmtpAuthentication = value; }
        }

        string smtpUserName = null;

        public string SmtpUserName
        {
            get { return smtpUserName; }
            set { smtpUserName = value; }
        }

        string smtpPassword = null;

        public string SmtpPassword
        {
            get { return smtpPassword; }
            set { smtpPassword = value; }
        }

        string rssLanguage = null;

        public string RssLanguage
        {
            get { return rssLanguage; }
            set { rssLanguage = value; }
        }

        bool enableSearchHighlight = true;

        public bool EnableSearchHighlight
        {
            get { return enableSearchHighlight; }
            set { enableSearchHighlight = value; }
        }

        public bool EnableEntryReferrals
        {
            get { return enableEntryReferrals; }
            set { enableEntryReferrals = value; }
        }

        [XmlArray("PingServices")]
        public PingService[] PingServiceArray
        {
            get { return new List<PingService>(PingServices).ToArray(); }
            set
            {
                if (value == null)
                {
                    pingServices = new PingServiceCollection();
                }
                else
                {
                    pingServices = new PingServiceCollection(value);
                }
            }
        }

        [XmlIgnore]
        public PingServiceCollection PingServices
        {
            get
            {
                // if the user has upgraded we want to maintain their settings
                if (this.pingServices.Count == 0)
                {
                    if (this.notifyBloGs)
                    {
                        this.pingServices.Add(PingService.GetBloGs());
                    }

                    if (this.notifyWebLogsDotCom)
                    {
                        this.pingServices.Add(PingService.GetWebLogsDotCom());
                    }
                }

                return pingServices;
            }
            set { pingServices = value; }
        }

        string feedBurnerName = null;

        public string FeedBurnerName
        {
            get { return feedBurnerName; }
            set { feedBurnerName = value; }
        }

        public int DaysCommentsAllowed
        {
            get { return daysCommentsAllowed; }
            set { daysCommentsAllowed = value; }
        }

        public bool EnableCommentDays
        {
            get { return enableCommentDays; }
            set { enableCommentDays = value; }
        }

        public bool SupressEmailAddressDisplay
        {
            get { return supressEmailAddressDisplay; }
            set { supressEmailAddressDisplay = value; }
        }

        public string EntryEditControl
        {
            get;
            set;
        }

        public bool LogBlockedReferrals
        {
            get { return logBlockedReferrals; }
            set { logBlockedReferrals = value; }
        }

        public bool ShowCommentsWhenViewingEntry
        {
            get { return showCommentsWhenViewingEntry; }
            set { showCommentsWhenViewingEntry = value; }
        }

        bool useFeedSchemeForSyndicationLinks;
        public bool UseFeedSchemeForSyndication
        {
            get { return useFeedSchemeForSyndicationLinks; }
            set { useFeedSchemeForSyndicationLinks = value; }
        }

        private int _contentLookaheadDays = 0;

        public int ContentLookaheadDays
        {
            get { return _contentLookaheadDays; }
            set { _contentLookaheadDays = value; }
        }

        public bool EnableAutoSave
        {
            get { return enableAutoSave; }
            set { enableAutoSave = value; }
        }

        private int _smtpPort = 25;

        public int SmtpPort
        {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        public bool CommentsAllowGravatar
        {
            get { return commentsAllowGravatar; }
            set { commentsAllowGravatar = value; }
        }

        public string CommentsGravatarNoImgPath
        {
            get { return commentsGravatarNoImgPath; }
            set { commentsGravatarNoImgPath = value; }
        }

        public string CommentsGravatarSize
        {
            get { return commentsGravatarSize; }
            set { commentsGravatarSize = value; }
        }

        public string CommentsGravatarBorder
        {
            get { return commentsGravatarBorder; }
            set { commentsGravatarBorder = value; }
        }

        public string CommentsGravatarRating
        {
            get { return commentsGravatarRating; }
            set { commentsGravatarRating = value; }

        }

        /// <summary>
        /// Gets or sets a value indicating whether comments require approval.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if approval is required; otherwise, <see langword="false"/>.
        /// </value>
        public bool CommentsRequireApproval
        {
            get { return this.commentsRequireApproval; }
            set { this.commentsRequireApproval = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether HTML is allowed in comments.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if HTML is allowed in comments; otherwise, <see langword="false"/>.
        /// </value>
        public bool CommentsAllowHtml
        {
            get { return this.commentsAllowHtml; }
            set { this.commentsAllowHtml = value; }
        }

        /// <summary>
        /// Gets the a collection of tags allowed in the comments.
        /// </summary>
        /// <value>The tags allowed in the comments.</value>
        /// <remarks>
        ///		The array is sorted when set during de-serialization.
        ///	</remarks>
        [XmlArray("validCommentTags", IsNullable = true)]
        [XmlArrayItem("tag")]
        public ValidTagCollection XmlAllowedTagsArray
        {
            get
            {
                return this.allowedTags;
            }
            set
            {
                this.allowedTags = value;
            }
        }

        [XmlIgnore]
        public ValidTagCollection AllowedTags
        {
            get
            {
                // if someone deleted all allowed tags, or the tags were never there he get's the default
                if (this.allowedTags == null || this.allowedTags.Count == 0)
                {
                    this.allowedTags = new ValidTagCollection(defaultAllowedTags);
                }
                return this.allowedTags;
            }
        }

        /// <summary>
        /// Gets or sets the tags allowed in the 
        /// comments as a comma separated list..
        /// </summary>
        /// <value>The tags allowed in the comments.</value>
        [XmlElement("AllowedTags")]
        [Obsolete("Please use the AllowedTags property.")]
        public string XmlAllowedTags
        {
            get
            {
                return defaultAllowedTags;
            }
            set
            {
                ;
            }
        }

        public bool EnableCoComment
        {
            get
            {
                return enableCoComment;
            }
            set
            {
                enableCoComment = value;
            }
        }

        private bool enableSpamBlockingService;
        public bool EnableSpamBlockingService
        {
            get { return enableSpamBlockingService; }
            set { enableSpamBlockingService = value; }
        }

        private string spamBlockingServiceApiKey;
        public string SpamBlockingServiceApiKey
        {
            get { return spamBlockingServiceApiKey; }
            set { spamBlockingServiceApiKey = value; }
        }

        [XmlIgnore]
        public ISpamBlockingService SpamBlockingService
        {
            get
            {
                //TODO: this may eventually be configurable, if Akismet alternatives show up
                if (!enableSpamBlockingService || spamBlockingServiceApiKey.Length == 0)
                {
                    return null;
                }
                return new AkismetSpamBlockingService(this.spamBlockingServiceApiKey, this.root);
            }
        }

        private bool enableSpamModeration = true;
        public bool EnableSpamModeration
        {
            get { return enableSpamModeration; }
            set { enableSpamModeration = value; }
        }

        private int _entriesPerPage = 5;
        public int EntriesPerPage
        {
            get { return _entriesPerPage; }
            set { _entriesPerPage = value; }
        }

        private bool enableDailyEmailReport = false;
        public bool EnableDailyReportEmail
        {
            get { return enableDailyEmailReport; }
            set { enableDailyEmailReport = value; }
        }

        bool useSSLForSMTP = false;
        public bool UseSSLForSMTP
        {
            get { return useSSLForSMTP; }
            set { useSSLForSMTP = value; }
        }

        public string PreferredBloggingAPI
        {
            get { return preferredBloggingAPI; }
            set { preferredBloggingAPI = value; }
        }

        bool enableGoogleMaps = false;
        public bool EnableGoogleMaps
        {
            get { return enableGoogleMaps; }
            set { enableGoogleMaps = value; }
        }

        string googleMapsApiKey = string.Empty;
        public string GoogleMapsApiKey
        {
            get { return googleMapsApiKey; }
            set { googleMapsApiKey = value; }
        }

        bool enableGeoRss = false;
        public bool EnableGeoRss
        {
            get { return enableGeoRss; }
            set { enableGeoRss = value; }
        }

        double defaultLatitude = 0;
        public double DefaultLatitude
        {
            get { return defaultLatitude; }
            set { defaultLatitude = value; }
        }

        double defaultLongitude = 0;
        public double DefaultLongitude
        {
            get { return defaultLongitude; }
            set { defaultLongitude = value; }
        }

        bool enableDefaultLatLongForNonGeoCodedPosts = false;
        public bool EnableDefaultLatLongForNonGeoCodedPosts
        {
            get { return enableDefaultLatLongForNonGeoCodedPosts; }
            set { enableDefaultLatLongForNonGeoCodedPosts = value; }
        }

        bool htmlTidyContent = true;
        public bool HtmlTidyContent
        {
            get { return htmlTidyContent; }
            set { htmlTidyContent = value; }
        }

        bool resolveCommenterIP = true;
        public bool ResolveCommenterIP
        {
            get { return resolveCommenterIP; }
            set { resolveCommenterIP = value; }
        }


        bool allowOpenIdComments = false;
        /// <summary>
        /// Indicates whether commenters can login using openid.
        /// </summary>
        public bool AllowOpenIdComments
        {
            get { return this.allowOpenIdComments; }
            set { this.allowOpenIdComments = value; }
        }

        /// <summary>
        /// Indicates whether admins can login using openid.
        /// </summary>
        bool allowOpenIdAdmin = false;
        public bool AllowOpenIdAdmin
        {
            get { return this.allowOpenIdAdmin; }
            set { this.allowOpenIdAdmin = value; }
        }

        /// <summary>
        /// Indicates if commenters authenticated using openid bypass the spam check.
        /// </summary>
        bool bypassSpamOpenIdComment = false;
        public bool BypassSpamOpenIdComment
        {
            get { return this.bypassSpamOpenIdComment; }
            set { this.bypassSpamOpenIdComment = value; }
        }

        /// <summary>
        /// Gets or sets the title permalink space replacement.
        /// </summary>
        /// <value>The title permalink space replacement.</value>
        public string TitlePermalinkSpaceReplacement
        {
            get { return this.titlePermalinkSpaceReplacement; }
            set { this.titlePermalinkSpaceReplacement = value; }
        }

        public bool AMPPagesEnabled
        {
            get { return enableAMPPages; }
            set { this.enableAMPPages = value; }
        }

        public string RSSEndPointRewrite
        {
            get { return rssEndPointRewrite; }
            set { this.rssEndPointRewrite = value; }
        }

        public string CheesySpamQ
        {
            get { return cheesySpamQ; }
            set { cheesySpamQ = value; }
        }
        public string CheesySpamA
        {
            get { return cheesySpamA; }
            set { cheesySpamA = value; }
        }

        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
        /* Add new properties just above this comment*/
        /*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

    [Serializable]
    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class ContentFilter
    {
        string expression = "";
        string mapTo;
        bool isRegex = false;

        public ContentFilter()
        {
        }

        public ContentFilter(string expression, string mapTo)
        {
            this.expression = expression;
            this.mapTo = mapTo;
        }

        [XmlAttribute("find")]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        [XmlAttribute("replace")]
        public string MapTo
        {
            get { return mapTo; }
            set { mapTo = value; }
        }

        [XmlAttribute("isregex")]
        public bool IsRegEx
        {
            get { return isRegex; }
            set { isRegex = value; }
        }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }


    /// <summary>
    /// A collection of elements of type ContentFilter
    /// </summary>
    [Serializable]
    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class ContentFilterCollection : CollectionBase, IEnumerable<ContentFilter>
    {
        /// <summary>
        /// Initializes a new empty instance of the ContentFilterCollection class.
        /// </summary>
        public ContentFilterCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the ContentFilterCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new ContentFilterCollection.
        /// </param>
        public ContentFilterCollection(ContentFilter[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the ContentFilterCollection class, containing elements
        /// copied from another instance of ContentFilterCollection
        /// </summary>
        /// <param name="items">
        /// The ContentFilterCollection whose elements are to be added to the new ContentFilterCollection.
        /// </param>
        public ContentFilterCollection(ContentFilterCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this ContentFilterCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this ContentFilterCollection.
        /// </param>
        public virtual void AddRange(ContentFilter[] items)
        {
            foreach (ContentFilter item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another ContentFilterCollection to the end of this ContentFilterCollection.
        /// </summary>
        /// <param name="items">
        /// The ContentFilterCollection whose elements are to be added to the end of this ContentFilterCollection.
        /// </param>
        public virtual void AddRange(ContentFilterCollection items)
        {
            foreach (ContentFilter item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type ContentFilter to the end of this ContentFilterCollection.
        /// </summary>
        /// <param name="value">
        /// The ContentFilter to be added to the end of this ContentFilterCollection.
        /// </param>
        public virtual void Add(ContentFilter value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic ContentFilter value is in this ContentFilterCollection.
        /// </summary>
        /// <param name="value">
        /// The ContentFilter value to locate in this ContentFilterCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this ContentFilterCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(ContentFilter value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this ContentFilterCollection
        /// </summary>
        /// <param name="value">
        /// The ContentFilter value to locate in the ContentFilterCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(ContentFilter value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the ContentFilterCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the ContentFilter is to be inserted.
        /// </param>
        /// <param name="value">
        /// The ContentFilter to insert.
        /// </param>
        public virtual void Insert(int index, ContentFilter value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the ContentFilter at the given index in this ContentFilterCollection.
        /// </summary>
        public virtual ContentFilter this[int index]
        {
            get { return (ContentFilter)this.List[index]; }
            set { this.List[index] = value; }
        }

        /// <summary>
        /// Removes the first occurrence of a specific ContentFilter from this ContentFilterCollection.
        /// </summary>
        /// <param name="value">
        /// The ContentFilter value to remove from this ContentFilterCollection.
        /// </param>
        public virtual void Remove(ContentFilter value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by ContentFilterCollection.GetEnumerator.
        /// </summary>
        public class Enumerator : IEnumerator<ContentFilter>
        {
            private IEnumerator wrapped;

            public Enumerator(ContentFilterCollection collection)
            {
                this.wrapped = ((CollectionBase)collection).GetEnumerator();
            }

            public ContentFilter Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            object IEnumerator.Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            ContentFilter IEnumerator<ContentFilter>.Current
            {
                get { return (ContentFilter)(this.wrapped.Current); }
            }

            public bool MoveNext()
            {
                return this.wrapped.MoveNext();
            }

            public void Reset()
            {
                this.wrapped.Reset();
            }

            void IDisposable.Dispose()
            {
                this.wrapped.Reset();
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the elements of this ContentFilterCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<ContentFilter> IEnumerable<ContentFilter>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class ServiceDisabledException : Exception
    {
        public ServiceDisabledException()
            : base("Service disabled")
        {
        }
    }
}
