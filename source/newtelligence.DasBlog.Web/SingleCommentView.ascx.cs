using System.Resources;

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


namespace newtelligence.DasBlog.Web
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
   
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Core;

   /// <summary>
   ///		Summary description for EntryView.
   /// </summary>
   public partial class SingleCommentView : System.Web.UI.UserControl
   {

      Comment comment;
      bool obfuscateEmail = true;

      public bool ObfuscateEmail { get { return obfuscateEmail; } set { obfuscateEmail = value; } }

      public Comment Comment
      {
         get
         {
            return comment;
         }
         set
         {
            comment = value;
         }
      }


      private string FixUrl(string url)
      {
          if (string.IsNullOrEmpty(url))
          {
              return url;
          }

          if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
          {
              return url;
          }
          else
          {
              return "http://" + url;
          }
      }

      protected void Page_PreRender(object sender, System.EventArgs e)
      {
         SharedBasePage requestPage = Page as SharedBasePage;

         Control root = this;
         HtmlGenericControl entry = new HtmlGenericControl("div");
         if (SiteSecurity.GetUserByEmail(comment.AuthorEmail) == null)
         {
            entry.Attributes["class"] = "commentBoxStyle";
         }
         else
         {
            entry.Attributes["class"] = "commentBoxStyle commentBoxAuthorStyle";
         }
         root.Controls.Add(entry);

         HtmlGenericControl entryTitle = new HtmlGenericControl("div");
         entryTitle.Attributes["class"] = "commentDateStyle";

         //Add the unique anchor for each comment
         HtmlAnchor anchor = new HtmlAnchor();
         anchor.Name = comment.EntryId;
         entryTitle.Controls.Add(anchor);

         if (requestPage.SiteConfig.AdjustDisplayTimeZone)
         {
            entryTitle.Controls.Add(new LiteralControl(requestPage.SiteConfig.GetConfiguredTimeZone().FormatAdjustedUniversalTime(comment.CreatedUtc)));
         }
         else
         {
            entryTitle.Controls.Add(new LiteralControl(comment.CreatedUtc.ToString("U") + " UTC"));
         }
         entry.Controls.Add(entryTitle);


         HtmlGenericControl entryBody = new HtmlGenericControl("div");
         if (SiteSecurity.GetUserByEmail(comment.AuthorEmail) == null)
         {
            entryBody.Attributes["class"] = "commentBodyStyle";
         }
         else
         {
            entryBody.Attributes["class"] = "commentBodyStyle commentBodyAuthorStyle";
         }

         if (comment.Content != null)
         {
            entryBody.Controls.Add(new LiteralControl(Regex.Replace(comment.Content, "\n", "<br />")));
         }
         if (!requestPage.HideAdminTools && SiteSecurity.IsInRole("admin"))
         {
            HtmlGenericControl spamStatus = new HtmlGenericControl("div");
            spamStatus.Attributes["class"] = "commentSpamStateStyle";
            spamStatus.InnerText = ApplicationResourceTable.GetSpamStateDescription(comment.SpamState);
            entryBody.Controls.Add(spamStatus);
         }


         entry.Controls.Add(entryBody);

         HtmlGenericControl footer = new HtmlGenericControl("div");
         footer.Attributes["class"] = "commentBoxFooterStyle";
         entry.Controls.Add(footer);


         if (requestPage.SiteConfig.CommentsAllowGravatar && String.IsNullOrEmpty(comment.AuthorEmail) == false)
         {
            string hash = "";
            byte[] data, enc;

            data = Encoding.Default.GetBytes(comment.AuthorEmail.ToLowerInvariant());

            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                enc = md5.TransformFinalBlock(data, 0, data.Length);
                foreach (byte b in md5.Hash)
                {
                    hash += Convert.ToString(b, 16).ToLower().PadLeft(2, '0');
                }
                md5.Clear();
            }

            string nogravpath = "";
            if (requestPage.SiteConfig.CommentsGravatarNoImgPath != null)
            {
               if (requestPage.SiteConfig.CommentsGravatarNoImgPath != "")
               {
                  if (requestPage.SiteConfig.CommentsGravatarNoImgPath.Substring(0, 4) == "http")
                  {
                     nogravpath = "&default=" + Server.UrlEncode(requestPage.SiteConfig.CommentsGravatarNoImgPath);
                  }
                  else
                  {
                     nogravpath = "&default=" + Server.UrlEncode(requestPage.SiteConfig.Root + requestPage.SiteConfig.CommentsGravatarNoImgPath);
                  }
               }
            }

            if (String.IsNullOrEmpty(requestPage.SiteConfig.CommentsGravatarNoImgPath) == false)
            {
                if (requestPage.SiteConfig.CommentsGravatarNoImgPath == "identicon" ||
                requestPage.SiteConfig.CommentsGravatarNoImgPath == "wavatar" ||
                requestPage.SiteConfig.CommentsGravatarNoImgPath == "monsterid" ||
                requestPage.SiteConfig.CommentsGravatarNoImgPath.Substring(0, 4) == "http")
                {
                    nogravpath = "&default=" + Server.UrlEncode(requestPage.SiteConfig.CommentsGravatarNoImgPath);
                }
                else
                {
                    nogravpath = "&default=" + Server.UrlEncode(requestPage.SiteConfig.Root + requestPage.SiteConfig.CommentsGravatarNoImgPath);
                }
            }

            string gravborder = "";
            if (requestPage.SiteConfig.CommentsGravatarBorder != null)
            {
               if (requestPage.SiteConfig.CommentsGravatarBorder != "")
               {
                  gravborder = "&border=" + requestPage.SiteConfig.CommentsGravatarBorder;
               }
            }

            string gravsize = "";
            if (requestPage.SiteConfig.CommentsGravatarSize != null)
            {
               if (requestPage.SiteConfig.CommentsGravatarSize != "")
               {
                  gravsize = "&size=" + requestPage.SiteConfig.CommentsGravatarSize;
               }
            }

            string gravrating = "";
            if (requestPage.SiteConfig.CommentsGravatarRating != null)
            {
               if (requestPage.SiteConfig.CommentsGravatarRating != "")
               {
                  gravrating = "&rating=" + requestPage.SiteConfig.CommentsGravatarRating;
               }
            }

            HtmlGenericControl entryGRAVATAR = new HtmlGenericControl("span");
            entryGRAVATAR.Attributes["class"] = "commentGravatarBlock";
            entryGRAVATAR.InnerHtml = "<img class=\"commentGravatar\" src=\"//www.gravatar.com/avatar.php?gravatar_id=" + hash + gravrating + gravsize + nogravpath + gravborder + "\"/>";
            footer.Controls.Add(entryGRAVATAR);
         }

         string authorLink = null;
         if (comment.AuthorHomepage != null && comment.AuthorHomepage.Length > 0)
         {
            authorLink = FixUrl(comment.AuthorHomepage);
         }
         else if (comment.AuthorEmail != null && comment.AuthorEmail.Length > 0)
         {
            if (!requestPage.SiteConfig.SupressEmailAddressDisplay)
            {
               authorLink = "mailto:" + SiteUtilities.SpamBlocker(comment.AuthorEmail);
            }
         }

         if (authorLink != null)
         {
            HyperLink link = new HyperLink();
            link.Attributes["class"] = "commentPermalinkStyle";
            link.NavigateUrl = authorLink;
            link.Text = comment.Author;
            link.Attributes.Add("rel", "nofollow");
            footer.Controls.Add(link);

            if (comment.OpenId)
            {
               System.Web.UI.WebControls.Image i = new System.Web.UI.WebControls.Image();
               i.ImageUrl = "~/images/openid-icon-small.gif";
               i.CssClass = "commentOpenId";
               link.Controls.Add(i);
               Literal l = new Literal();
               l.Text = comment.Author;
               link.Controls.Add(l);
            }
         }
         else
         {
            Label l = new Label();
            l.Attributes["class"] = "commentPermalinkStyle";
            l.Text = comment.Author;
            footer.Controls.Add(l);
         }


         if (!requestPage.SiteConfig.SupressEmailAddressDisplay)
         {
            if (comment.AuthorEmail != null && comment.AuthorEmail.Length > 0)
            {
               footer.Controls.Add(new LiteralControl(" | "));

               HtmlGenericControl mailto = new HtmlGenericControl("span");
               footer.Controls.Add(mailto);

               HyperLink link = new HyperLink();
               link.CssClass = "commentMailToStyle";
               link.NavigateUrl = "mailto:" + SiteUtilities.SpamBlocker(comment.AuthorEmail);
               link.Text = SiteUtilities.SpamBlocker(comment.AuthorEmail);
               mailto.Controls.Add(link);
            }
         }

         if (!requestPage.HideAdminTools && SiteSecurity.IsInRole("admin"))
         {
            if (!string.IsNullOrEmpty(comment.AuthorIPAddress))
            {
               try
               {
                  if (requestPage.SiteConfig.ResolveCommenterIP == true)
                  {
                     System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostEntry(comment.AuthorIPAddress);
                     footer.Controls.Add(
                        new LiteralControl(" (" + comment.AuthorIPAddress + " " + hostInfo.HostName + ") "));
                  }
                  else
                  {
                     footer.Controls.Add(new LiteralControl(" (" + comment.AuthorIPAddress + ") "));
                  }
               }
               catch
               {
                  footer.Controls.Add(new LiteralControl(" (" + comment.AuthorIPAddress + ") "));
               }
            }

            footer.Controls.Add(new LiteralControl(" "));

            // create delete hyperlink
            HyperLink deleteHl = new HyperLink();
            deleteHl.CssClass = "deleteLinkStyle";
            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
            img.CssClass = "deleteLinkImageStyle";
            img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("deletebutton")).ToString();
            img.BorderWidth = 0;
            deleteHl.Controls.Add(img);
             deleteHl.NavigateUrl = String.Format("javascript:deleteComment(\"{0}\", \"{1}\", \"{2}\")", Comment.TargetEntryId, Comment.EntryId, Comment.Author == null ? String.Empty : Comment.Author.Replace("\"", "\\\""));

            ResourceManager resmgr = resmgr = ApplicationResourceTable.Get();

            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "deleteCommentScript"))
            {
               // add the javascript to allow deletion of the comment
               string scriptString = "<script type=\"text/javascript\" language=\"JavaScript\">\n";
               scriptString += "function deleteComment(entryId, commentId, commentFrom)\n";
               scriptString += "{\n";
               scriptString += String.Format("	if(confirm(\"{0} \\n\\n\" + commentFrom))\n", resmgr.GetString("text_delete_confirm"));
               scriptString += "	{\n";
               scriptString += "		location.href=\"deleteItem.ashx?entryid=\" +  entryId + \"&commentId=\" + commentId\n";
               scriptString += "	}\n";
               scriptString += "}\n";
               scriptString += "</script>";

               Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "deleteCommentScript", scriptString);
            }


            footer.Controls.Add(deleteHl);

            // create approve hyperlink, when a comment is not public or if its marked as spam
            if ((!Comment.IsPublic) || (Comment.SpamState == SpamState.Spam))
            {

               HyperLink approveHl = new HyperLink();
               approveHl.CssClass = "approveLinkStyle";
               System.Web.UI.WebControls.Image okImg = new System.Web.UI.WebControls.Image();
               okImg.CssClass = "approveImageStyle";
               okImg.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("okbutton-list")).ToString();
               okImg.BorderWidth = 0;
               approveHl.Controls.Add(okImg);
               approveHl.NavigateUrl = String.Format("javascript:approveComment(\"{0}\", \"{1}\")", Comment.TargetEntryId, Comment.EntryId);

               if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "approveCommentScript"))
               {

                  string approveScript = "<script type=\"text/javascript\" language=\"JavaScript\">\n";
                  approveScript += "function approveComment(entryId, commentId)\n";
                  approveScript += "{\n";
                  approveScript += "	location.href=\"approveItem.ashx?entryid=\" +  entryId + \"&commentId=\" + commentId\n";
                  approveScript += "}\n";
                  approveScript += "</script>";

                  Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "approveCommentScript", approveScript);
               }

               footer.Controls.Add(approveHl);
            }
            ISpamBlockingService spamBlockingService = requestPage.SiteConfig.SpamBlockingService;
            if ((spamBlockingService != null) && (comment.SpamState != SpamState.Spam))
            {
               HyperLink reportSpamLink = new HyperLink();
               reportSpamLink.CssClass = "approveLinkStyle";
               System.Web.UI.WebControls.Image spamImg = new System.Web.UI.WebControls.Image();
               spamImg.CssClass = "approveImageStyle";
               spamImg.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("reportspambutton")).ToString();
               spamImg.BorderWidth = 0;
               reportSpamLink.Controls.Add(spamImg);
               reportSpamLink.NavigateUrl = String.Format("javascript:reportComment(\"{0}\", \"{1}\", \"{2}\")", Comment.TargetEntryId, Comment.EntryId, Comment.Author == null ? String.Empty : Comment.Author.Replace("\"", "\\\""));

               string reportScript = "<script type=\"text/javascript\" language=\"JavaScript\">\n";
               reportScript += "function reportComment(entryId, commentId, commentFrom)\n";
               reportScript += "{\n";
               reportScript += String.Format("	if(confirm(\"{0} \\n\\n\" + commentFrom))\n", resmgr.GetString("text_reportspam_confirm"));
               reportScript += "	{\n";
               reportScript += "		location.href=\"deleteItem.ashx?report=true&entryid=\" +  entryId + \"&commentId=\" + commentId\n";
               reportScript += "	}\n";
               reportScript += "}\n";
               reportScript += "</script>";

               if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "reportCommentScript"))
               {
                  Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "reportCommentScript", reportScript);
               }

               footer.Controls.Add(reportSpamLink);
            }

         }
      }
   }
}
