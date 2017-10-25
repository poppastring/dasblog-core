using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Util.Html;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.TextEditors;

namespace newtelligence.DasBlog.Web
{
    public partial class EditConfigBox : UserControl
    {
        const string defaultGravatarSize = "80";
        const string passwordPlaceHolder = "########";
        protected const string SPAM_OPTION_DELETE = "DELETE";
        protected const string SPAM_OPTION_SAVE = "SAVE";
        protected ResourceManager resmgr;

        public PingServiceCollection PingServiceCollection
        {
            get
            {
                DataCache cache = CacheFactory.GetCache();

                PingServiceCollection pingServices = (PingServiceCollection)cache["PingServices"];
                if (pingServices == null)
                {
                    pingServices = GetPingServiceInfo();
                }
                return pingServices;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (SiteSecurity.IsInRole("admin") == false)
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            ID = "EditConfigBox";

            SharedBasePage requestPage = Page as SharedBasePage;
            SiteConfig siteConfig = requestPage.SiteConfig;

            if (!IsPostBack)
            {
                textContact.Text = siteConfig.Contact;
                textCopyright.Text = siteConfig.Copyright;
                textPassword.Text = passwordPlaceHolder;
                textConfirmPassword.Text = passwordPlaceHolder;
                textFrontPageCategory.Text = siteConfig.FrontPageCategory;
                textFrontPageDayCount.Text = siteConfig.FrontPageDayCount.ToString();
                textFrontPageEntryCount.Text = siteConfig.FrontPageEntryCount.ToString();
                textEntriesPerPage.Text = siteConfig.EntriesPerPage.ToString();
                textContentLookaheadDays.Text = siteConfig.ContentLookaheadDays.ToString();
                textMainMaxDaysInRss.Text = siteConfig.RssDayCount.ToString();
                textMainMaxEntriesInRss.Text = siteConfig.RssMainEntryCount.ToString();
                textOtherMaxEntriesInRss.Text = siteConfig.RssEntryCount.ToString();
                checkAlwaysIncludeContentInRSS.Checked = siteConfig.AlwaysIncludeContentInRSS;
                checkEnableRSSItemFooter.Checked = siteConfig.EnableRssItemFooters;
                textRSSItemFooter.Text = siteConfig.RssItemFooter;
                txtRSSEndPointRewrite.Text = siteConfig.RSSEndPointRewrite;
                checkPop3Enabled.Checked = siteConfig.EnablePop3;
                textPop3Interval.Text = siteConfig.Pop3Interval.ToString();
                textPop3Server.Text = siteConfig.Pop3Server;
                textPop3SubjectPrefix.Text = siteConfig.Pop3SubjectPrefix;
                textPop3Username.Text = siteConfig.Pop3Username;
                textPop3Password.Text = passwordPlaceHolder;
                textPop3PasswordRepeat.Text = passwordPlaceHolder;
                textRoot.Text = siteConfig.Root;
                textSmtpServer.Text = siteConfig.SmtpServer;
                textSmtpPort.Text = siteConfig.SmtpPort.ToString();
                checkUseSSLForSMTP.Checked = siteConfig.UseSSLForSMTP;
                textNotificationEmailAddress.Text = siteConfig.NotificationEMailAddress;
                textSubtitle.Text = siteConfig.Subtitle;
                textSmtpServer.Text = siteConfig.SmtpServer;
                checkEnableCoComment.Checked = siteConfig.EnableCoComment;
                checkComments.Checked = siteConfig.SendCommentsByEmail;
                checkPingbacks.Checked = siteConfig.SendPingbacksByEmail;
                checkReferrals.Checked = siteConfig.SendReferralsByEmail;
                checkPosts.Checked = siteConfig.SendPostsByEmail;
                checkTrackbacks.Checked = siteConfig.SendTrackbacksByEmail;
                checkShowCommentCounters.Checked = siteConfig.ShowCommentCount;
                checkEnableAutoPingback.Checked = siteConfig.EnableAutoPingback;
                checkEnableBloggerApi.Checked = siteConfig.EnableBloggerApi;
                checkEnableComments.Checked = siteConfig.EnableComments;
                checkEnableCommentApi.Checked = siteConfig.EnableCommentApi;
                checkShowCommentsWhenViewingEntry.Checked = siteConfig.ShowCommentsWhenViewingEntry;
                checkEnableConfigEditService.Checked = siteConfig.EnableConfigEditService;
                checkEnableEditService.Checked = siteConfig.EnableEditService;
                checkEnableAutoSave.Checked = siteConfig.EnableAutoSave;
                checkEnablePingbackService.Checked = siteConfig.EnablePingbackService;
                checkEnableTrackbackService.Checked = siteConfig.EnableTrackbackService;
                checkEnableClickThrough.Checked = siteConfig.EnableClickThrough;
                checkEnableAggregatorBugging.Checked = siteConfig.EnableAggregatorBugging;
                checkXssEnabled.Checked = siteConfig.EnableXSSUpstream;
                textXssEndpoint.Text = siteConfig.XSSUpstreamEndpoint;
                textXssInterval.Text = siteConfig.XSSUpstreamInterval.ToString();
                textXssPassword.Text = passwordPlaceHolder;
                textXssPasswordRepeat.Text = passwordPlaceHolder;
                textXssUsername.Text = siteConfig.XSSUpstreamUsername;
                textXssRssFilename.Text = siteConfig.XSSRSSFilename;
                checkPop3InlineAttachedPictures.Checked = siteConfig.Pop3InlineAttachedPictures;
                textPop3AttachedPicturesPictureThumbnailHeight.Text = siteConfig.Pop3InlinedAttachedPicturesThumbHeight.ToString();
                mailDeletionAll.Checked = siteConfig.Pop3DeleteAllMessages;
                mailDeletionProcessed.Checked = !siteConfig.Pop3DeleteAllMessages;
                logIgnoredEmails.Checked = siteConfig.Pop3LogIgnoredEmails;
                checkShowItemDescriptionInAggregatedViews.Checked = siteConfig.ShowItemDescriptionInAggregatedViews;
                checkEnableStartPageCaching.Checked = siteConfig.EnableStartPageCaching;
                checkEnableBlogrollDescription.Checked = siteConfig.EnableBlogrollDescription;
                checkEntryTitleAsLink.Checked = siteConfig.EntryTitleAsLink;
                checkEnableUrlRewriting.Checked = siteConfig.EnableUrlRewriting;
                checkEnableCrosspost.Checked = siteConfig.EnableCrossposts;
                checkCategoryAllEntries.Checked = siteConfig.CategoryAllEntries;
                checkReferralUrlBlacklist.Checked = siteConfig.EnableReferralUrlBlackList;
                textReferralBlacklist.Text = siteConfig.ReferralUrlBlackList;
                checkCaptchaEnabled.Checked = siteConfig.EnableCaptcha;
                checkReferralBlacklist404s.Checked = siteConfig.EnableReferralUrlBlackList404s;
                textRSSChannelImage.Text = siteConfig.ChannelImageUrl;
                checkEnableTitlePermaLink.Checked = siteConfig.EnableTitlePermaLink;
                checkEnableTitlePermaLinkUnique.Checked = siteConfig.EnableTitlePermaLinkUnique;
                checkEnableTitlePermaLinkSpaces.Checked = siteConfig.EnableTitlePermaLinkSpaces;
                checkEnableEncryptLoginPassword.Checked = siteConfig.EncryptLoginPassword;
                checkEnableSmtpAuthentication.Checked = siteConfig.EnableSmtpAuthentication;
                textSmtpUsername.Text = siteConfig.SmtpUserName;
                textSmtpPassword.Text = passwordPlaceHolder;
                textRssLanguage.Text = siteConfig.RssLanguage;
                checkEnableSearchHighlight.Checked = siteConfig.EnableSearchHighlight;
                checkEnableEntryReferral.Checked = siteConfig.EnableEntryReferrals;
                textFeedBurnerName.Text = siteConfig.FeedBurnerName;
                checkUseFeedScheme.Checked = siteConfig.UseFeedSchemeForSyndication;
                checkLogBlockedReferrals.Checked = siteConfig.LogBlockedReferrals;

                //populate the title space replacement options
                dropDownTitlePermalinkReplacementCharacter.Items.Clear();//in casee someone adds them in the ascx
                foreach (string s in TitleMapperModule.TitlePermalinkSpaceReplacementOptions) dropDownTitlePermalinkReplacementCharacter.Items.Add(s);
                dropDownTitlePermalinkReplacementCharacter.SelectedValue = siteConfig.TitlePermalinkSpaceReplacement;

                checkSpamBlockingEnabled.Checked = siteConfig.EnableSpamBlockingService;
                textSpamBlockingApiKey.Text = siteConfig.SpamBlockingServiceApiKey;
                optionSpamHandling.SelectedValue = siteConfig.EnableSpamModeration ? SPAM_OPTION_SAVE : SPAM_OPTION_DELETE;

                // setup the checkbox list to select which tags to allow
                checkBoxListAllowedTags.DataSource = siteConfig.AllowedTags;
                checkBoxListAllowedTags.DataTextField = "Name";
                checkBoxListAllowedTags.DataValueField = "Name";

                // enable comment moderation 
                checkCommentsRequireApproval.Checked = siteConfig.CommentsRequireApproval;

                // allow html and comments
                checkAllowHtml.Checked = siteConfig.CommentsAllowHtml;

                // populate from config - Gravatar
                GravatarPopulateForm();

                // supress email address display
                checkDisableEmailDisplay.Checked = siteConfig.SupressEmailAddressDisplay;

                checkEnableCommentDays.Checked = siteConfig.EnableCommentDays;

                checkAttemptToHtmlTidyContent.Checked = siteConfig.HtmlTidyContent;
                checkResolveCommenterIP.Checked = siteConfig.ResolveCommenterIP;

                //if ( siteConfig.EnableCommentDays ) 
                //{
                if (siteConfig.DaysCommentsAllowed > 0)
                {
                    textDaysCommentsAllowed.Text = siteConfig.DaysCommentsAllowed.ToString();
                }
                //} 
                //else 
                //{
                //	textDaysCommentsAllowed.Text = null;
                //}

                // supress email address display
                checkDisableEmailDisplay.Checked = siteConfig.SupressEmailAddressDisplay;

                checkEnableCommentDays.Checked = siteConfig.EnableCommentDays;

                //if ( siteConfig.EnableCommentDays ) 
                //{
                if (siteConfig.DaysCommentsAllowed > 0)
                {
                    textDaysCommentsAllowed.Text = siteConfig.DaysCommentsAllowed.ToString();
                }
                //} 
                //else 
                //{
                //	textDaysCommentsAllowed.Text = null;
                //}

                // email daily report
                checkDailyReport.Text = resmgr.GetString("text_daily_activity_report");
                checkDailyReport.Checked = siteConfig.EnableDailyReportEmail;

                WindowsTimeZoneCollection timeZones = WindowsTimeZone.TimeZones;
                foreach (WindowsTimeZone tz in timeZones)
                {
                    listTimeZones.Items.Add(new ListItem(tz.DisplayName, tz.ZoneIndex.ToString()));
                }
                listTimeZones.SelectedValue = siteConfig.DisplayTimeZoneIndex.ToString();
                checkUseUTC.Checked = !siteConfig.AdjustDisplayTimeZone;

                //FIX: hardcoded path
                ThemeDictionary themes = BlogTheme.Load(SiteUtilities.MapPath("themes"));
                foreach (BlogTheme theme in themes.Values)
                {
                    // setting the selected item like this instead of
                    // using 	listThemes.SelectedValue = siteConfig.Theme;
                    // prevents the page from breaking.

                    ListItem item = new ListItem(theme.Title, theme.Name);
                    if (item.Value == siteConfig.Theme)
                    {
                        item.Selected = true;
                    }
                    listThemes.Items.Add(item);
                }

                textTitle.Text = siteConfig.Title;

                checkBoxListPingServices.DataSource = PingServiceCollection;
                checkBoxListPingServices.DataTextField = "Hyperlink";
                checkBoxListPingServices.DataValueField = "Endpoint";

                drpEntryEditControl.Items.Clear();
                foreach (string potentialAssembly in Directory.GetFiles(HttpRuntime.BinDirectory, "*.dll"))
                {
                    try
                    {
                        Assembly a = Assembly.LoadFrom(potentialAssembly);
                        foreach (Type potentialType in a.GetTypes())
                        {
                            if (potentialType.BaseType == typeof(EditControlAdapter))
                            {
                                drpEntryEditControl.Items.Add(new ListItem(potentialType.Name, potentialType.AssemblyQualifiedName));
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //swallow
                    }
                }

                //Reasonable default
                if (string.IsNullOrEmpty(siteConfig.EntryEditControl))
                {
                    siteConfig.EntryEditControl = typeof(TinyMCEAdapter).AssemblyQualifiedName;
                }
                DataBind();

                ListItem li = drpEntryEditControl.Items.FindByText(siteConfig.EntryEditControl);
                if (li != null)
                {
                    li.Selected = true;
                }
                else
                {
                    drpEntryEditControl.SelectedIndex = 0;
                }

                foreach (PingService ps in siteConfig.PingServices)
                {
                    checkBoxListPingServices.Items.FindByValue(ps.Endpoint).Selected = true;
                }

                foreach (ValidTag tag in siteConfig.AllowedTags)
                {
                    checkBoxListAllowedTags.Items.FindByValue(tag.Name).Selected = tag.IsAllowed;
                }

                //check for Smtp permission
                if (SecurityManager.IsGranted(new SmtpPermission(SmtpAccess.ConnectToUnrestrictedPort)))
                {
                    phSmtpTrustWarning.Visible = false;
                }
                else
                {
                    phSmtpTrustWarning.Visible = true;
                }

                //check for Socket permission
                SocketPermission sp;
                if (String.IsNullOrEmpty(textPop3Server.Text))
                {
                    sp = new SocketPermission(PermissionState.Unrestricted);
                }
                else
                {
                    sp = new SocketPermission(NetworkAccess.Connect, TransportType.Tcp, textPop3Server.Text, 110);
                }

                if (SecurityManager.IsGranted(sp))
                {
                    phPop3TrustWarning.Visible = false;
                }
                else
                {
                    phPop3TrustWarning.Visible = true;
                }

                // georss stuff
                checkEnableGeoRss.Checked = siteConfig.EnableGeoRss;
                textGoogleMapsApi.Text = siteConfig.GoogleMapsApiKey;
                textDefaultLatitude.Text = siteConfig.DefaultLatitude.ToString(CultureInfo.InvariantCulture);
                textDefaultLongitude.Text = siteConfig.DefaultLongitude.ToString(CultureInfo.InvariantCulture);
                checkEnableGoogleMaps.Checked = siteConfig.EnableGoogleMaps;
                checkEnableDefaultLatLongForNonGeoCodedPosts.Checked = siteConfig.EnableDefaultLatLongForNonGeoCodedPosts;

                // OpenId
                chkAllowOpenIdAdmin.Checked = siteConfig.AllowOpenIdAdmin;
                chkAllowOpenIdCommenter.Checked = siteConfig.AllowOpenIdComments;
                chkBypassSpamOpenIdCommenter.Checked = siteConfig.BypassSpamOpenIdComment;


                SeoMetaTags smt = new SeoMetaTags().GetMetaTags();

                txtMetaDescription.Text = smt.MetaDescription;
                txtMetaKeywords.Text = smt.MetaKeywords;
                txtTwitterCard.Text = smt.TwitterCard;
                txtTwitterSite.Text = smt.TwitterSite;
                txtTwitterCreator.Text = smt.TwitterCreator;
                txtTwitterImage.Text = smt.TwitterImage;
                txtFaceBookAdmins.Text = smt.FaceBookAdmins;
                txtFaceBookAppID.Text = smt.FaceBookAppID;

                checkAmpEnabled.Checked = siteConfig.AMPPagesEnabled;

            } // end if !postback

            //enable list controls that may have been enabled client-side
            //in 2.0 if they are not enable we won't get there postback data
            checkBoxListAllowedTags.Enabled = true;
            dropGravatarRating.Enabled = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            CleanGravatar();
            CleanAutoExpire();
            CleanHtmlComments();
            CleanGeoRss();

            CreateClientScript();
        }

        static PingServiceCollection GetPingServiceInfo()
        {
            PingServiceCollection pingServiceCollection = new PingServiceCollection();
            string fullPath = SiteConfig.GetConfigPathFromCurrentContext() + "PingServices.xml";

            if (File.Exists(fullPath))
            {
                FileStream fileStream = FileUtils.OpenForRead(fullPath);

                if (fileStream != null)
                {
                    try
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(PingServiceCollection));
                        StreamReader reader = new StreamReader(fileStream);
                        pingServiceCollection = (PingServiceCollection)ser.Deserialize(reader);

                        // add to cache
                        DataCache cache = CacheFactory.GetCache();
                        cache.Insert("PingServices", pingServiceCollection, new CacheDependency(fullPath));
                    }
                    catch (Exception e)
                    {
                        ErrorTrace.Trace(TraceLevel.Error, e);
                    }
                    finally
                    {
                        fileStream.Close();
                    }
                }
            }

            // add some defaults
            if (pingServiceCollection.Count == 0)
            {
                pingServiceCollection = PingService.GetDefaultPingServices();
            }

            return pingServiceCollection;
        }

        protected void buttonSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                // There were validation errors, save client states.
                CleanGravatar();
                CleanAutoExpire();
                CleanHtmlComments();
                CleanGeoRss();
                return;
            }

            SharedBasePage requestPage = Page as SharedBasePage;
            SiteConfig siteConfig = requestPage.SiteConfig;
            siteConfig.Contact = textContact.Text;
            siteConfig.Copyright = textCopyright.Text;
            siteConfig.FrontPageCategory = textFrontPageCategory.Text;
            siteConfig.FrontPageDayCount = int.Parse(textFrontPageDayCount.Text);
            siteConfig.FrontPageEntryCount = int.Parse(textFrontPageEntryCount.Text);
            siteConfig.EntriesPerPage = int.Parse(textEntriesPerPage.Text);
            try
            {
                siteConfig.ContentLookaheadDays = int.Parse(textContentLookaheadDays.Text);
            }
            catch (FormatException)
            {
                siteConfig.ContentLookaheadDays = 0;
            }
            siteConfig.RssDayCount = int.Parse(textMainMaxDaysInRss.Text);
            siteConfig.RssMainEntryCount = int.Parse(textMainMaxEntriesInRss.Text);
            siteConfig.RssEntryCount = int.Parse(textOtherMaxEntriesInRss.Text);
            siteConfig.AlwaysIncludeContentInRSS = checkAlwaysIncludeContentInRSS.Checked;
            siteConfig.EnableRssItemFooters = checkEnableRSSItemFooter.Checked;
            siteConfig.RssItemFooter = textRSSItemFooter.Text;
            siteConfig.RSSEndPointRewrite = txtRSSEndPointRewrite.Text;
            siteConfig.EnablePop3 = checkPop3Enabled.Checked;
            siteConfig.Pop3Interval = int.Parse(textPop3Interval.Text);
            siteConfig.Pop3Server = textPop3Server.Text;
            siteConfig.Pop3SubjectPrefix = textPop3SubjectPrefix.Text;
            siteConfig.Pop3Username = textPop3Username.Text;
            if (textPop3Password.Text.Length > 0 &&
                textPop3Password.Text != passwordPlaceHolder)
            {
                siteConfig.Pop3Password = textPop3Password.Text;
            }
            siteConfig.Pop3DeleteAllMessages = mailDeletionAll.Checked;
            siteConfig.Pop3LogIgnoredEmails = logIgnoredEmails.Checked;
            siteConfig.EnableXSSUpstream = checkXssEnabled.Checked;
            siteConfig.XSSUpstreamEndpoint = textXssEndpoint.Text;
            siteConfig.XSSUpstreamInterval = int.Parse(textXssInterval.Text);
            if (textXssPassword.Text.Length > 0 &&
                textXssPassword.Text != passwordPlaceHolder)
            {
                siteConfig.XSSUpstreamPassword = textXssPassword.Text;
            }
            siteConfig.XSSUpstreamUsername = textXssUsername.Text;
            siteConfig.XSSRSSFilename = textXssRssFilename.Text;

            siteConfig.Root = textRoot.Text;
            siteConfig.SmtpServer = textSmtpServer.Text;
            siteConfig.SmtpPort = int.Parse(textSmtpPort.Text);
            siteConfig.UseSSLForSMTP = checkUseSSLForSMTP.Checked;
            siteConfig.NotificationEMailAddress = textNotificationEmailAddress.Text;
            siteConfig.SendCommentsByEmail = checkComments.Checked;
            siteConfig.EnableCoComment = checkEnableCoComment.Checked;
            siteConfig.SendPingbacksByEmail = checkPingbacks.Checked;
            siteConfig.SendReferralsByEmail = checkReferrals.Checked;
            siteConfig.SendTrackbacksByEmail = checkTrackbacks.Checked;
            siteConfig.SendPostsByEmail = checkPosts.Checked;
            siteConfig.EnableAutoPingback = checkEnableAutoPingback.Checked;
            siteConfig.EnableBloggerApi = checkEnableBloggerApi.Checked;
            siteConfig.EnableComments = checkEnableComments.Checked;
            siteConfig.EnableCommentApi = checkEnableCommentApi.Checked;
            siteConfig.ShowCommentsWhenViewingEntry = checkShowCommentsWhenViewingEntry.Checked;
            siteConfig.EnableConfigEditService = checkEnableConfigEditService.Checked;
            siteConfig.EnableEditService = checkEnableEditService.Checked;
            siteConfig.EnableAutoSave = checkEnableAutoSave.Checked;
            siteConfig.EnableTrackbackService = checkEnableTrackbackService.Checked;
            siteConfig.EnablePingbackService = checkEnablePingbackService.Checked;
            siteConfig.EnableClickThrough = checkEnableClickThrough.Checked;
            siteConfig.EnableAggregatorBugging = checkEnableAggregatorBugging.Checked;
            siteConfig.Subtitle = textSubtitle.Text;
            siteConfig.Title = textTitle.Text;
            siteConfig.ShowCommentCount = checkShowCommentCounters.Checked;
            siteConfig.Pop3InlineAttachedPictures = checkPop3InlineAttachedPictures.Checked;
            siteConfig.Pop3InlinedAttachedPicturesThumbHeight = int.Parse(textPop3AttachedPicturesPictureThumbnailHeight.Text);
            siteConfig.ShowItemDescriptionInAggregatedViews = checkShowItemDescriptionInAggregatedViews.Checked;
            siteConfig.EnableStartPageCaching = checkEnableStartPageCaching.Checked;
            siteConfig.EnableBlogrollDescription = checkEnableBlogrollDescription.Checked;
            siteConfig.EnableUrlRewriting = checkEnableUrlRewriting.Checked;
            siteConfig.DisplayTimeZoneIndex = Convert.ToInt32(listTimeZones.SelectedValue);
            siteConfig.AdjustDisplayTimeZone = !checkUseUTC.Checked;
            siteConfig.EntryTitleAsLink = checkEntryTitleAsLink.Checked;
            siteConfig.EnableCrossposts = checkEnableCrosspost.Checked;
            if (textPassword.Text.Length > 0 &&
                textPassword.Text != passwordPlaceHolder)
            {
                SiteSecurity.SetPassword(requestPage.User.Identity.Name, textPassword.Text);
            }
            siteConfig.CategoryAllEntries = checkCategoryAllEntries.Checked;
            requestPage.UserTheme = siteConfig.Theme = listThemes.SelectedValue;

            siteConfig.ReferralUrlBlackList = textReferralBlacklist.Text.TrimEnd(';');
            siteConfig.EnableReferralUrlBlackList = checkReferralUrlBlacklist.Checked;
            siteConfig.EnableCaptcha = checkCaptchaEnabled.Checked;
            siteConfig.EnableReferralUrlBlackList404s = checkReferralBlacklist404s.Checked;
            siteConfig.ChannelImageUrl = textRSSChannelImage.Text;
            siteConfig.EnableTitlePermaLink = checkEnableTitlePermaLink.Checked;
            siteConfig.EnableTitlePermaLinkUnique = checkEnableTitlePermaLinkUnique.Checked;
            siteConfig.EnableTitlePermaLinkSpaces = checkEnableTitlePermaLinkSpaces.Checked;
            siteConfig.EncryptLoginPassword = checkEnableEncryptLoginPassword.Checked;
            siteConfig.EnableSmtpAuthentication = checkEnableSmtpAuthentication.Checked;
            siteConfig.SmtpUserName = textSmtpUsername.Text;
            if (textSmtpPassword.Text.Length > 0 &&
                textSmtpPassword.Text != passwordPlaceHolder)
            {
                siteConfig.SmtpPassword = textSmtpPassword.Text;
            }
            siteConfig.RssLanguage = textRssLanguage.Text;
            siteConfig.EnableSearchHighlight = checkEnableSearchHighlight.Checked;
            siteConfig.EnableEntryReferrals = checkEnableEntryReferral.Checked;

            siteConfig.FeedBurnerName = textFeedBurnerName.Text.Trim();
            siteConfig.UseFeedSchemeForSyndication = checkUseFeedScheme.Checked;
            siteConfig.LogBlockedReferrals = checkLogBlockedReferrals.Checked;
            siteConfig.EnableSpamBlockingService = checkSpamBlockingEnabled.Checked;
            siteConfig.SpamBlockingServiceApiKey = textSpamBlockingApiKey.Text;
            siteConfig.EnableSpamModeration = (optionSpamHandling.SelectedValue != SPAM_OPTION_DELETE);
            siteConfig.EnableCommentDays = checkEnableCommentDays.Checked;
            siteConfig.HtmlTidyContent = checkAttemptToHtmlTidyContent.Checked;
            siteConfig.ResolveCommenterIP = checkResolveCommenterIP.Checked;

            siteConfig.TitlePermalinkSpaceReplacement = dropDownTitlePermalinkReplacementCharacter.SelectedValue;

            if (checkEnableCommentDays.Checked)
            {
                try
                {
                    int days = Convert.ToInt32(textDaysCommentsAllowed.Text);
                    if (days > 0)
                    {
                        siteConfig.DaysCommentsAllowed = days;
                    }
                }
                catch (FormatException)
                {
                    siteConfig.DaysCommentsAllowed = 60;
                }
            }
            else
            {
                siteConfig.DaysCommentsAllowed = 60;
            }

            // comments approval
            siteConfig.CommentsRequireApproval = checkCommentsRequireApproval.Checked;

            // removed the ability to edit the html tags, which are allowed for now
            foreach (ListItem li in checkBoxListAllowedTags.Items)
            {
                ValidTag tag = siteConfig.AllowedTags[li.Value];
                if (tag != null)
                {
                    tag.IsAllowed = li.Selected;
                }
            }

            // comments allow html
            siteConfig.CommentsAllowHtml = checkAllowHtml.Checked;

            // comments allow Gravatar and alt path
            GravatarPopulateConfig();

            // supress email address display
            siteConfig.SupressEmailAddressDisplay = checkDisableEmailDisplay.Checked;

            // enable daily report email
            siteConfig.EnableDailyReportEmail = checkDailyReport.Checked;

            PingServiceCollection savePingServices = new PingServiceCollection();

            foreach (PingService pingService in PingServiceCollection)
            {
                if (checkBoxListPingServices.Items.FindByValue(pingService.Endpoint).Selected)
                {
                    savePingServices.Add(pingService);
                }
            }

            siteConfig.PingServices = savePingServices;
            siteConfig.EntryEditControl = drpEntryEditControl.SelectedValue;

            // GeoRSS stuff.
            siteConfig.EnableGeoRss = checkEnableGeoRss.Checked;
            if (checkEnableGeoRss.Checked)
            {
                siteConfig.EnableDefaultLatLongForNonGeoCodedPosts = checkEnableDefaultLatLongForNonGeoCodedPosts.Checked;
                if (checkEnableDefaultLatLongForNonGeoCodedPosts.Checked)
                {
                    double latitude;
                    if (double.TryParse(textDefaultLatitude.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
                    {
                        siteConfig.DefaultLatitude = latitude;
                    }

                    double longitude;
                    if (double.TryParse(textDefaultLongitude.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                    {
                        siteConfig.DefaultLongitude = longitude;
                    }
                }

                siteConfig.EnableGoogleMaps = checkEnableGoogleMaps.Checked;
                if (checkEnableGoogleMaps.Checked)
                {
                    siteConfig.GoogleMapsApiKey = textGoogleMapsApi.Text;
                }
            }

            // open id stuff
            siteConfig.AllowOpenIdAdmin = chkAllowOpenIdAdmin.Checked;
            siteConfig.AllowOpenIdComments = chkAllowOpenIdCommenter.Checked;
            siteConfig.BypassSpamOpenIdComment = chkBypassSpamOpenIdCommenter.Checked;

            siteConfig.AMPPagesEnabled = checkAmpEnabled.Checked;

            SiteConfig.Save(siteConfig);

            SeoMetaTags smt = new SeoMetaTags().GetMetaTags();
            smt.MetaDescription = txtMetaDescription.Text;
            smt.MetaKeywords = txtMetaKeywords.Text ;
            smt.TwitterCard = txtTwitterCard.Text;
            smt.TwitterSite = txtTwitterSite.Text;
            smt.TwitterCreator = txtTwitterCreator.Text;
            smt.TwitterImage = txtTwitterImage.Text;
            smt.FaceBookAdmins = txtFaceBookAdmins.Text;
            smt.FaceBookAppID = txtFaceBookAppID.Text;

            SeoMetaTags.Save(smt);

            if (siteConfig.EnableReferralUrlBlackList && siteConfig.ReferralUrlBlackList.Length != 0)
            {
                ReferralBlackListFactory.AddBlacklist(new ReferralUrlBlacklist(), siteConfig.ReferralUrlBlackList);
            }
            else
            {
                ReferralBlackListFactory.RemoveBlacklist(typeof(ReferralUrlBlacklist));
            }

            requestPage.Redirect(Page.Request.Url.AbsoluteUri);
        }

        protected void EditConfigBox_Init(object sender, EventArgs e)
        {
            resmgr = ApplicationResourceTable.Get();
        }

        protected void buttonTestSMTP_Click(object sender, EventArgs e)
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            SiteConfig siteConfig = requestPage.SiteConfig;

            if (textSmtpServer.Text != "" & textNotificationEmailAddress.Text != "")
            {
                MailMessage emailMessage = new MailMessage();

                emailMessage.To.Add(textNotificationEmailAddress.Text);
                emailMessage.Subject = String.Format("dasBlog test message");
                emailMessage.Body =
                    String.Format("This is a test message from dasBlog. If you are reading this then everything is working properly.");
                emailMessage.IsBodyHtml = false;
                emailMessage.BodyEncoding = Encoding.UTF8;
                emailMessage.From = new MailAddress(siteConfig.Contact);
                SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, textSmtpServer.Text,
                                                             checkEnableSmtpAuthentication.Checked, checkUseSSLForSMTP.Checked,
                                                             textSmtpUsername.Text, textSmtpPassword.Text,
                                                             int.Parse(textSmtpPort.Text));

                try
                {
                    sendMailInfo.SendMyMessage();
                }
                catch (Exception ex)
                {
                    //RyanG: Decode the real reason the error occured by looking at the inner exceptions
                    StringBuilder exceptionMessage = new StringBuilder();
                    Exception lastException = ex;
                    while (lastException != null)
                    {
                        if (exceptionMessage.Length > 0)
                        {
                            exceptionMessage.Append("; ");
                        }
                        exceptionMessage.Append(lastException.Message);
                        lastException = lastException.InnerException;
                    }

                    ILoggingDataService logService = requestPage.LoggingService;
                    logService.AddEvent(
                        new EventDataItem(EventCodes.SmtpError, "", exceptionMessage.ToString()));

                    Response.Redirect("FormatPage.aspx?path=SiteConfig/pageerror.format.html", true);
                }
            }
        }

        void CreateClientScript()
        {
            checkAllowHtml.Attributes.Add("onclick",
                                          WebUtil.CreateEnableDisableScript(checkAllowHtml, checkBoxListAllowedTags));

            checkEnableGeoRss.Attributes.Add("onclick",
                                             WebUtil.CreateEnableDisableScript(checkEnableGeoRss,
                                                                               checkEnableDefaultLatLongForNonGeoCodedPosts,
                                                                               textDefaultLatitude,
                                                                               textDefaultLongitude,
                                                                               checkEnableGoogleMaps,
                                                                               textGoogleMapsApi));

            checkEnableGravatar.Attributes.Add("onclick", WebUtil.CreateEnableDisableScript(checkEnableGravatar,
                                                                                            textGravatarSize,
                                                                                            textNoGravatarPath,
                                                                                            textGravatarBorderColor,
                                                                                            dropGravatarRating));

            checkEnableCommentDays.Attributes.Add("onclick",
                                                  String.Format(
                                                    "{0}if(document.getElementById('{1}').value == ''){{document.getElementById('{1}').value = '60';}}",
                                                    WebUtil.CreateEnableDisableScript(checkEnableCommentDays,
                                                                                      textDaysCommentsAllowed),
                                                    textDaysCommentsAllowed.ClientID));
        }

        #region Cleanup Routines
        void CleanGeoRss()
        {
            checkEnableDefaultLatLongForNonGeoCodedPosts.Enabled = checkEnableGeoRss.Checked;
            textDefaultLatitude.Enabled = checkEnableGeoRss.Checked;
            textDefaultLongitude.Enabled = checkEnableGeoRss.Checked;
            checkEnableGoogleMaps.Enabled = checkEnableGeoRss.Checked;
            textGoogleMapsApi.Enabled = checkEnableGeoRss.Checked;
        }

        void CleanAutoExpire()
        {
            textDaysCommentsAllowed.Enabled = checkEnableCommentDays.Checked;
            textDefaultLatitude.Enabled = checkEnableCommentDays.Checked;
            textDefaultLongitude.Enabled = checkEnableCommentDays.Checked;
            checkEnableGoogleMaps.Enabled = checkEnableCommentDays.Checked;
            textGoogleMapsApi.Enabled = checkEnableCommentDays.Checked;
        }

        void CleanHtmlComments()
        {
            checkBoxListAllowedTags.Enabled = checkAllowHtml.Checked;
        }

        void CleanGravatar()
        {
            textGravatarSize.Enabled = checkEnableGravatar.Checked;
            textNoGravatarPath.Enabled = checkEnableGravatar.Checked;
            textGravatarBorderColor.Enabled = checkEnableGravatar.Checked;
            dropGravatarRating.Enabled = checkEnableGravatar.Checked;
        }
        #endregion

        #region Gravatar Support
        void GravatarPopulateForm()
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            SiteConfig siteConfig = requestPage.SiteConfig;

            checkEnableGravatar.Checked = siteConfig.CommentsAllowGravatar;

            if (!String.IsNullOrEmpty(siteConfig.CommentsGravatarSize))
            {
                int gpix = Convert.ToInt32(siteConfig.CommentsGravatarSize);
                if (gpix > 0)
                {
                    textGravatarSize.Text = gpix.ToString();
                }
                else
                {
                    textGravatarSize.Text = defaultGravatarSize;
                }
            }

            if (siteConfig.CommentsGravatarNoImgPath != null)
            {
                textNoGravatarPath.Text = siteConfig.CommentsGravatarNoImgPath;
            }

            if (siteConfig.CommentsGravatarBorder != null)
            {
                textGravatarBorderColor.Text = siteConfig.CommentsGravatarBorder;
            }

            if (siteConfig.CommentsGravatarRating != null)
            {
                dropGravatarRating.SelectedValue = siteConfig.CommentsGravatarRating;
            }
        }

        void GravatarPopulateConfig()
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            SiteConfig siteConfig = requestPage.SiteConfig;

            siteConfig.CommentsAllowGravatar = checkEnableGravatar.Checked;
            siteConfig.CommentsGravatarNoImgPath = textNoGravatarPath.Text.Trim();
            siteConfig.CommentsGravatarBorder = textGravatarBorderColor.Text.Trim();
            siteConfig.CommentsGravatarRating = dropGravatarRating.SelectedValue;

            try
            {
                int gpix = Convert.ToInt32(textGravatarSize.Text.Trim());
                if (gpix > 0 && gpix <= 80)
                {
                    siteConfig.CommentsGravatarSize = gpix.ToString();
                }
                else
                {
                    siteConfig.CommentsGravatarSize = defaultGravatarSize;
                }
            }
            catch (FormatException)
            {
                siteConfig.CommentsGravatarSize = String.Empty;
            }
        }
        #endregion

        #region GeoRSS Validation
        protected void cvDefaultLatitude_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            CheckBox chkEnableGeoRss = FindControl(checkEnableGeoRss.ID) as CheckBox;
            CheckBox chkEnableDefaultLatLong = FindControl(checkEnableDefaultLatLongForNonGeoCodedPosts.ID) as CheckBox;

            if (chkEnableGeoRss != null && chkEnableGeoRss.Checked &&
                chkEnableDefaultLatLong != null && chkEnableDefaultLatLong.Checked)
            {
                double value;
                if (double.TryParse(args.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    if (value >= -90d && value <= 90d)
                    {
                        args.IsValid = true;
                    }
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void cvDefaultLongitude_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            CheckBox chkEnableGeoRss = FindControl(checkEnableGeoRss.ID) as CheckBox;
            CheckBox chkEnableDefaultLatLong = FindControl(checkEnableDefaultLatLongForNonGeoCodedPosts.ID) as CheckBox;

            if (chkEnableGeoRss != null && chkEnableGeoRss.Checked &&
                chkEnableDefaultLatLong != null && chkEnableDefaultLatLong.Checked)
            {
                double value;
                if (double.TryParse(args.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    if (value >= -180d && value <= 180d)
                    {
                        args.IsValid = true;
                    }
                }
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void cvGoogleMapsApi_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            CheckBox chkEnableGeoRss = FindControl(checkEnableGeoRss.ID) as CheckBox;
            CheckBox chkGoogleMapsApi = FindControl(checkEnableGoogleMaps.ID) as CheckBox;

            if (chkEnableGeoRss != null && chkEnableGeoRss.Checked &&
                chkGoogleMapsApi != null && chkGoogleMapsApi.Checked)
            {
                args.IsValid = chkGoogleMapsApi.Checked && !String.IsNullOrEmpty(args.Value) ||
                               !chkGoogleMapsApi.Checked;
            }
            else
            {
                args.IsValid = true;
            }
        }
        #endregion

        #region Web Form Designer generated code
        protected override void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            optionSpamHandling.Items.AddRange(new ListItem[]
                                                {
                                                    new ListItem(resmgr.GetString("text_save_spam"), SPAM_OPTION_SAVE),
                                                    new ListItem(resmgr.GetString("text_delete_spam"), SPAM_OPTION_DELETE)
                                                });
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.EditConfigBox_Init);
        }
        #endregion
    }
}
