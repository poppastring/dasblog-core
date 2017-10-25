
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

using System;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Collections;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using DotNetOpenAuth.OpenId.RelyingParty;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using System.Collections.Generic;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    /// Summary description for CommentView.
    /// </summary>
    public partial class CommentViewBox : System.Web.UI.UserControl
    {
        protected System.Web.UI.HtmlControls.HtmlForm CommentView;
        // approval comments controls
        // gravatar Message


        protected System.Resources.ResourceManager resmgr;

        public CommentViewBox()
        {
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            if (string.IsNullOrWhiteSpace(requestPage.SiteConfig.CheesySpamQ) == false && string.IsNullOrWhiteSpace(requestPage.SiteConfig.CheesySpamA) == false)
            {
                this.lblCheesySpamQ.Text = requestPage.SiteConfig.CheesySpamQ;
            }
            else
            {
                this.trCheesySpam.Visible = false;
            }

            // only allow users to login using openid when we actually allow it
            if (requestPage.SiteConfig.AllowOpenIdComments == true)
            {
                OpenIdRelyingParty openid = new OpenIdRelyingParty();
                IAuthenticationResponse response = openid.GetResponse();
                if (response != null)
                {
                    // Stage 3: OpenID Provider sending assertion response
                    switch (response.Status)
                    {
                        case AuthenticationStatus.Authenticated:
                            ClaimsResponse sregResponse = response.GetExtension<ClaimsResponse>();
                            FetchResponse axResponse = response.GetExtension<FetchResponse>();
                            string nick = response.FriendlyIdentifierForDisplay;
                            string homepage = response.ClaimedIdentifier;
                            string email = "none@none.com";
                            if (sregResponse != null)
                            {
                                nick = string.IsNullOrEmpty(sregResponse.Nickname) ? sregResponse.FullName : sregResponse.Nickname;
                                email = sregResponse.Email;
                            }

                            if (String.IsNullOrEmpty(nick) && axResponse != null && axResponse.Attributes.Contains(WellKnownAttributes.Name.First) && axResponse.Attributes.Contains(WellKnownAttributes.Name.Last))
                            {
                                nick = axResponse.GetAttributeValue(WellKnownAttributes.Name.First) + " " +
                                       axResponse.GetAttributeValue(WellKnownAttributes.Name.Last);
                            }

                            string comment = Session["pendingComment"] as string;
                            string entryId = Session["pendingEntryId"] as string;
                            if (String.IsNullOrEmpty(comment) == false && String.IsNullOrEmpty(entryId) == false)
                            {
                                AddNewComment(nick, email, homepage, comment, entryId, /* openid */ true);
                            }

                            break;
                    }
                }
            }

            // if you are commenting on your own blog, no need for Captha
            if (SiteSecurity.IsValidContributor())
            {
                CaptchaControl1.Enabled = CaptchaControl1.Visible = false;
            }
            else
            {
                CaptchaControl1.Enabled = CaptchaControl1.Visible = requestPage.SiteConfig.EnableCaptcha;
            }

            resmgr = ApplicationResourceTable.Get();

            if (!IsPostBack)
            {
                if (requestPage.WeblogEntryId.Length == 0)
                {
                    requestPage.Redirect(SiteUtilities.GetStartPageUrl(requestPage.SiteConfig));
                }
                ViewState["entryId"] = requestPage.WeblogEntryId;

                if (Request.Cookies["name"] != null)
                {
                    string nameStr = HttpUtility.UrlDecode(Request.Cookies["name"].Value, Encoding.UTF8);
                    //truncate at 32 chars to avoid abuse...
                    name.Text = nameStr.Substring(0, Math.Min(32, nameStr.Length));
                }

                if (Request.Cookies["email"] != null)
                {
                    email.Text = HttpUtility.UrlDecode(Request.Cookies["email"].Value, Encoding.UTF8);
                }

                if (Request.Cookies["homepage"] != null)
                {
                    homepage.Text = HttpUtility.UrlDecode(Request.Cookies["homepage"].Value, Encoding.UTF8);
                }

                if (Request.Cookies["openid"] != null)
                {
                    openid_identifier.Text = HttpUtility.UrlDecode(Request.Cookies["openid"].Value, Encoding.UTF8);
                }

            }

            DataBind();
        }

        override protected void OnInit(EventArgs e)
        {

            this.cvCaptcha.ServerValidate += new System.Web.UI.WebControls.ServerValidateEventHandler(this.cvCaptcha_ServerValidate);
            this.PreRender += new System.EventHandler(this.CommentView_PreRender);

            //HACK: Need to rebuild this page.
            // since we only validate the captcha control server side,
            // all validation should happen server side.
            this.RequiredFieldValidator1.EnableClientScript = false;
            this.validatorREEmail.EnableClientScript = false;
            this.validatorRFComment.EnableClientScript = false;

            base.OnInit(e);
        }

        bool potentialSpamSubmitted = false;

        protected void CommentView_PreRender(object sender, System.EventArgs e)
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            string entryId = (string)ViewState["entryId"];
            bool obfuscateEmail = requestPage.SiteConfig.ObfuscateEmail;

            Entry entry = requestPage.DataService.GetEntry(entryId);
            if (entry != null)
            {
                //Modified 10-3-03 HPierson
                //Render the day template with just the single entry, rather than the item template
                //Modified 12-8-03 HPierson
                //Using entry.CreatedLocalTime causes a bug when dasBlog is not configured to be in 
                //the same time zone as the server. Instead, we use the configured WindowsTimeZone
                //to calculate the dasBlog configured local time for the entry
                requestPage.WeblogEntries.Add(entry);
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    newtelligence.DasBlog.Util.WindowsTimeZone wtz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    requestPage.ProcessDayTemplate(wtz.ToLocalTime(entry.CreatedUtc), commentViewContent);
                }
                else
                {
                    requestPage.ProcessDayTemplate(entry.CreatedUtc, commentViewContent);
                }

                HtmlAnchor commentStart = new HtmlAnchor();
                commentStart.Name = "commentstart";
                commentViewContent.Controls.Add(commentStart);

                // Show all public comments, or all contents if you can approve them
                // This way all non-public comments remain hidden, when you no longer require approval.
                bool allComments = SiteSecurity.IsValidContributor();

                foreach (Comment c in requestPage.DataService.GetCommentsFor(entryId, allComments))
                {
                    SingleCommentView view = (SingleCommentView)LoadControl("SingleCommentView.ascx");
                    view.Comment = c;
                    view.ObfuscateEmail = obfuscateEmail;
                    commentViewContent.Controls.Add(view);
                }

                commentsClosed.Visible = false;
                commentViewTable.Visible = true;
                // only show the openid option when allowed in the config
                openIdTable.Visible = requestPage.SiteConfig.AllowOpenIdComments;

                commentsGravatarEnabled.Visible = requestPage.SiteConfig.CommentsAllowGravatar;

                // show the comments require approval warning when moderating, or suspected spam,
                // maybe users won't post multiple comments when their comment won't show immediately
                commentsModerated.Visible = (requestPage.SiteConfig.CommentsRequireApproval || potentialSpamSubmitted);
                if (potentialSpamSubmitted)
                {
                    labelCommentsModerated.Text = resmgr.GetString("text_comment_potential_spam");
                }
                // display no/some html
                labelCommentHtml.Visible = requestPage.SiteConfig.CommentsAllowHtml && (requestPage.SiteConfig.AllowedTags.AllowedTagsCount > 0);
                labelComment.Visible = !labelCommentHtml.Visible;
                labelCommentHtml.Text = String.Format(resmgr.GetString("text_comment_content_html"), requestPage.SiteConfig.AllowedTags.ToString());

                if (SiteUtilities.AreCommentsAllowed(entry, requestPage.SiteConfig) == false)
                {
                    commentsClosed.Visible = true;
                    commentViewTable.Visible = false;
                    openIdTable.Visible = false;
                    // if comments are not allow, there is no need to show the approval warning
                    commentsModerated.Visible = false;
                }

                if (Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "coCommentScript") == false && requestPage.SiteConfig.EnableCoComment == true)
                {
                    string coCommentScript = String.Format(@"

					<script type=""text/javascript"">
// this ensures coComment gets the correct values
coco =
{{
     tool          : ""dasBlog"",
     siteurl       : ""{0}"",
     sitetitle     : ""{1}"",
     pageurl       : ""{2}"",
     pagetitle     : ""{3}"",
     author        : ""{4}"",
     formID        : ""{5}"",
     textareaID    : ""{6}"",
     buttonID      : ""{7}""
}}
</script>
<script id=""cocomment-fetchlet"" src=""http://www.cocomment.com/js/enabler.js"" type=""text/javascript"">
// this activates coComment
</script>
  ",
                          requestPage.SiteConfig.Root,
                          Server.HtmlEncode(requestPage.SiteConfig.Title),
                          Request.Url.ToString(),
                          Server.HtmlEncode(entry.Title),
                          Server.HtmlEncode(entry.Author),
                          this.Page.Form.ClientID,
                          this.comment.ClientID,
                          this.add.ClientID
                       );
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "coComment", coCommentScript);
                }
            }
        }


        private string FixUrl(string url)
        {
            if (false == String.IsNullOrEmpty(url))
            {

                if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    return url;
                }
                else
                {
                    return "http://" + url;
                }
            }
            else
            {
                return url;
            }
        }

        protected void add_Click(object sender, System.EventArgs e)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            // validate base entries Captcha and Comment
            Page.Validate("Base");
            bool baseValid = Page.IsValid;

            // check the openid controls
            Page.Validate("OpenId");
            bool openIdValid = Page.IsValid;

            // check the classic controls Name and Email
            Page.Validate("Classic");
            bool classicValid = Page.IsValid;

            if (baseValid && openIdValid)
            {
                if (String.IsNullOrEmpty(openid_identifier.Text) == false && openid_identifier.Text != "Click to Sign In")
                {
                    Session["pendingComment"] = comment.Text;
                    Session["pendingEntryId"] = ViewState["entryId"].ToString().ToUpper();
                    OpenIdRelyingParty openid = new OpenIdRelyingParty();
                    try
                    {
                        IAuthenticationRequest req = openid.CreateRequest(openid_identifier.Text);
                        ClaimsRequest sregRequest = new ClaimsRequest();
                        sregRequest.Email = DemandLevel.Require;
                        sregRequest.Nickname = DemandLevel.Require;
                        sregRequest.FullName = DemandLevel.Request;
                        req.AddExtension(sregRequest);

                        // Also add AX request explicitly so we can request first and last name individually.
                        FetchRequest axRequest = new FetchRequest();
                        axRequest.Attributes.AddRequired(WellKnownAttributes.Name.First);
                        axRequest.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                        req.AddExtension(axRequest);

                        SaveCookies();
                        req.RedirectToProvider();
                        return;
                    }
                    catch (UriFormatException ue) //They've entered something that's not a URI!
                    {
                        requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.Error, "ERROR: " + openid_identifier.Text + " is not a valid URL. " + ue.Message, ""));
                    }
                }
            }

            if (baseValid && classicValid)
            {
                //Why isn't Page.Validate("Normal") working? It returns false. Hm.
                if (!String.IsNullOrEmpty(name.Text.Trim()))
                {
                    SaveCookies();
                    AddNewComment(name.Text, email.Text, homepage.Text, comment.Text, ViewState["entryId"].ToString().ToUpper(), /* openid */ false);
                }
            }
        }

        private void SaveCookies()
        {
            if (rememberMe.Checked)
            {
                string path = HttpRuntime.AppDomainAppVirtualPath;

                // We encode the name so High Latin folks like René Drießel don't break
                // the Http Input Validation stuff and get their name remembered correctly.
                // http://sourceforge.net/tracker/index.php?func=detail&aid=1158454&group_id=127624&atid=709018
                HttpCookie cookieName = new HttpCookie("name", HttpUtility.UrlEncode(name.Text, Encoding.UTF8));
                cookieName.Path = path;
                Response.Cookies.Add(cookieName);

                HttpCookie cookieEmail = new HttpCookie("email", HttpUtility.UrlEncode(email.Text, Encoding.UTF8));
                cookieEmail.Path = path;
                Response.Cookies.Add(cookieEmail);

                HttpCookie cookieHomepage = new HttpCookie("homepage", HttpUtility.UrlEncode(homepage.Text, Encoding.UTF8));
                cookieHomepage.Path = path;
                Response.Cookies.Add(cookieHomepage);

                HttpCookie cookieOpenId = new HttpCookie("openid", HttpUtility.UrlEncode(openid_identifier.Text, Encoding.UTF8));
                cookieOpenId.Path = path;
                Response.Cookies.Add(cookieOpenId);

                Response.Cookies["name"].Expires = DateTime.MaxValue;
                Response.Cookies["email"].Expires = DateTime.MaxValue;
                Response.Cookies["homepage"].Expires = DateTime.MaxValue;
                Response.Cookies["openid"].Expires = DateTime.MaxValue;
            }
        }

        public void AddNewComment(string name, string email, string homepage, string comment, string entryId, bool openid)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            // if we allow tags, use the allowed tags, otherwise use an empty array
            ValidTagCollection allowedTags = (requestPage.SiteConfig.CommentsAllowHtml ? requestPage.SiteConfig.AllowedTags : new ValidTagCollection(null));

            Entry entry = requestPage.DataService.GetEntry(entryId);
            if ((entry != null) && SiteUtilities.AreCommentsAllowed(entry, requestPage.SiteConfig))
            {
                Comment c = new Comment();
                c.Initialize();
                c.OpenId = openid;
                c.Author = HttpUtility.HtmlEncode(name);
                c.AuthorEmail = HttpUtility.HtmlEncode(email);
                c.AuthorHomepage = FixUrl(homepage);
                c.AuthorIPAddress = Request.UserHostAddress;
                c.AuthorUserAgent = Request.UserAgent;
                c.Referer = Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : String.Empty;
                // clean the code from html tags


                c.TargetEntryId = entryId;
                c.TargetTitle = entry.Title;

                if (requestPage.SiteConfig.CommentsRequireApproval == true &&
                   (requestPage.SiteConfig.SmtpServer == null || requestPage.SiteConfig.SmtpServer.Length == 0))
                {
                    requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.Error, "ERROR: Comment Moderation is turned on, but you haven't configured an SMTP Server for sending mail!", ""));
                }

                // if comments require moderation, they are not public.
                // except when the commenter is a contributor
                if (SiteSecurity.IsValidContributor())
                {
                    c.IsPublic = true;
                }
                else
                {
                    // bypass spam when the comment is authenticated by openid en openid doesn't require approval
                    if (requestPage.SiteConfig.EnableSpamBlockingService && (requestPage.SiteConfig.BypassSpamOpenIdComment && openid) == false)
                    {

                        // make sure to send the unfiltered comment for analysis by external service
                        c.Content = comment;
                        bool externalServiceSucceeded = false;
                        try
                        {
                            if (requestPage.SiteConfig.SpamBlockingService.IsSpam(c))
                            {
                                potentialSpamSubmitted = true;
                                if (!requestPage.SiteConfig.EnableSpamModeration)
                                {
                                    // abort saving the comment
                                    requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.CommentBlocked, String.Format("Blocking suspected spam from {0} {1} [{2}].", c.Author, c.AuthorEmail, c.AuthorIPAddress), SiteUtilities.GetPermaLinkUrl(entryId)));
                                    clearCommentInput();
                                    return;
                                }
                                c.SpamState = SpamState.Spam;
                                c.IsPublic = false;
                            }
                            else
                            {
                                c.SpamState = SpamState.NotSpam;
                                c.IsPublic = true;
                            }
                            externalServiceSucceeded = true;
                        }
                        catch (Exception ex)
                        {
                            requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.Error, String.Format("The external spam blocking service failed for comment {0}. Original exception: {1}", c.EntryId, ex), SiteUtilities.GetPermaLinkUrl(entryId)));
                        }
                        if (!externalServiceSucceeded)
                        {
                            // If the external service fails, we will hide the comment, but not delete it,
                            // even if moderation is disabled.
                            c.SpamState = SpamState.NotChecked;
                            if (doesFeedbackHaveSpamPotential(c))
                            {
                                potentialSpamSubmitted = true;
                                c.IsPublic = false;
                            }
                            else
                            {
                                c.IsPublic = true;
                            }
                        }
                    }
                    else
                    {
                        c.IsPublic = true;
                    }
                    // If comment moderation enabled, hide all comments regardless of the what the external spam service says
                    if (requestPage.SiteConfig.CommentsRequireApproval)
                    {
                        c.IsPublic = false;
                    }

                    //Our new captcha
                    if (string.IsNullOrWhiteSpace(requestPage.SiteConfig.CheesySpamQ) == false && string.IsNullOrWhiteSpace(requestPage.SiteConfig.CheesySpamA) == false)
                    {
                        if (string.IsNullOrWhiteSpace(txtCheesySpamA.Text) == false &&
                            txtCheesySpamA.Text.IndexOf(requestPage.SiteConfig.CheesySpamA, StringComparison.OrdinalIgnoreCase) > -1)
                        {
                            c.IsPublic = true; //NOT SPAM
                        }
                        else //they did the math wrong
                        {
                            // abort saving the comment
                            requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.CommentBlocked, String.Format("Blocking suspected spam from {0} {1} [{2}].", c.Author, c.AuthorEmail, c.AuthorIPAddress), SiteUtilities.GetPermaLinkUrl(entryId)));
                            clearCommentInput();
                            return;
                        }
                    }
                }

                // FilterHtml html encodes anything we don't like
                string filteredText = SiteUtilities.FilterHtml(comment, allowedTags);
                c.Content = filteredText;


                if (requestPage.SiteConfig.SendCommentsByEmail &&
                    requestPage.SiteConfig.SmtpServer != null &&
                    requestPage.SiteConfig.SmtpServer.Length > 0)
                {
                    SendMailInfo defaultMailInfo = ComposeMail(c);
                    requestPage.DataService.AddComment(c, defaultMailInfo);
                    requestPage.DataService.RunActions(ComposeMailForUsers(entry, c));

                    string commentShort = c.Content.Replace("\n", "");
                    if (commentShort.Length > 50)
                    {
                        commentShort = commentShort.Substring(0, 50) + "...";
                    }
                    requestPage.LoggingService.AddEvent(
                        new EventDataItem(
                        EventCodes.CommentAdded, commentShort, SiteUtilities.GetCommentViewUrl(entryId)));
                }
                else
                {
                    requestPage.DataService.AddComment(c);
                }

                clearCommentInput();

                // break the caching
                requestPage.DataCache.Remove("BlogCoreData");
                Session.Remove("pendingComment");
                Session.Remove("pendingEntryId");

                //Send the user to the comment they JUST posted.
                if (!potentialSpamSubmitted)
                {
                    Response.Redirect(SiteUtilities.GetCommentViewUrl(c.TargetEntryId) + "#" + c.EntryId);
                }
            }
        }

        /// <summary>
        /// Performs simple analysis to make a best guess at the feedback's spam potential
        /// </summary>
        /// <param name="feedback">The feedback to check for spam</param>
        /// <returns></returns>
        private bool doesFeedbackHaveSpamPotential(IFeedback feedback)
        {
            //TODO: Add more rules as we see fit. For now, we will use the simple rule: if it contains hyperlinks, it might be spam.
            return (SiteUtilities.FindHyperLinks(feedback.Content).Count > 0);
        }

        private void clearCommentInput()
        {
            name.Text = "";
            email.Text = "";
            homepage.Text = "";
            comment.Text = "";
        }

        private object[] ComposeMailForUsers(Entry entry, Comment c)
        {
            List<object> actions = new List<object>();

            foreach (User user in SiteSecurity.GetSecurity().Users)
            {
                if (user.EmailAddress == null || user.EmailAddress.Length == 0)
                    continue;

                if (user.NotifyOnAllComment || (user.NotifyOnOwnComment && entry.Author.ToUpper() == user.Name.ToUpper()))
                {
                    SendMailInfo sendMailInfo = ComposeMail(c);
                    sendMailInfo.Message.To.Add(user.EmailAddress);
                    actions.Add(sendMailInfo);
                }
            }

            return actions.ToArray();
        }

        private SendMailInfo ComposeMail(Comment c)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            MailMessage emailMessage = new MailMessage();

            if (requestPage.SiteConfig.NotificationEMailAddress != null &&
               requestPage.SiteConfig.NotificationEMailAddress.Length > 0)
            {
                emailMessage.To.Add(requestPage.SiteConfig.NotificationEMailAddress);
            }
            else
            {
                emailMessage.To.Add(requestPage.SiteConfig.Contact);
            }

            emailMessage.Sender = new MailAddress(requestPage.SiteConfig.Contact);

            emailMessage.Subject = String.Format("Weblog comment by '{0}' from '{1}' on '{2}'", c.Author, c.AuthorHomepage, c.TargetTitle);

            if (requestPage.SiteConfig.CommentsRequireApproval)
            {
                emailMessage.Body = String.Format("{0}\r\nComments page: {1}\r\n\r\nApprove comment: {2}\r\n\r\nDelete Comment: {3}",
                   HttpUtility.HtmlDecode(c.Content),
                   SiteUtilities.GetCommentViewUrl(c.TargetEntryId),
                   SiteUtilities.GetCommentApproveUrl(c.TargetEntryId, c.EntryId),
                   SiteUtilities.GetCommentDeleteUrl(c.TargetEntryId, c.EntryId));
            }
            else
            {
                emailMessage.Body = String.Format("{0}\r\nComments page: {1}\r\n\r\nDelete Comment: {2}",
                   HttpUtility.HtmlDecode(c.Content),
                   SiteUtilities.GetCommentViewUrl(c.TargetEntryId),
                   SiteUtilities.GetCommentDeleteUrl(c.TargetEntryId, c.EntryId));
                if (c.SpamState == SpamState.Spam)
                {
                    emailMessage.Body += "\r\nNot Spam: " + SiteUtilities.GetCommentApproveUrl(c.TargetEntryId, c.EntryId);
                }
            }

            if (requestPage.SiteConfig.EnableSpamBlockingService && (c.SpamState != SpamState.Spam))
            {
                emailMessage.Body += "\r\n\r\nReport as SPAM: "
                   + SiteUtilities.GetCommentReportUrl(requestPage.SiteConfig, c.TargetEntryId, c.EntryId)
                   + "\r\n  (Reporting SPAM will also delete the comment.)";
            }

            emailMessage.Body += "\r\n\r\n" + ApplicationResourceTable.GetSpamStateDescription(c.SpamState);

            emailMessage.IsBodyHtml = false;
            emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
            if (c.AuthorEmail != null && c.AuthorEmail.Length > 0)
            {
                emailMessage.From = new MailAddress(c.AuthorEmail);
            }
            else
            {
                emailMessage.From = new MailAddress(requestPage.SiteConfig.Contact);
            }

            emailMessage.Headers.Add("Sender", requestPage.SiteConfig.Contact);

            // add the X-Originating-IP header
            string hostname = Dns.GetHostName();
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostname);

            if (ipHostEntry.AddressList.Length > 0)
            {
                emailMessage.Headers.Add("X-Originating-IP", ipHostEntry.AddressList[0].ToString());
            }
            SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, requestPage.SiteConfig.SmtpServer,
               requestPage.SiteConfig.EnableSmtpAuthentication, requestPage.SiteConfig.UseSSLForSMTP, requestPage.SiteConfig.SmtpUserName,
                   requestPage.SiteConfig.SmtpPassword, requestPage.SiteConfig.SmtpPort);

            return sendMailInfo;
        }

        private void cvCaptcha_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            if (CaptchaControl1.Enabled && requestPage.SiteConfig.EnableCaptcha)
            {
                args.IsValid = CaptchaControl1.UserValidated;
            }
        }
    }
}
