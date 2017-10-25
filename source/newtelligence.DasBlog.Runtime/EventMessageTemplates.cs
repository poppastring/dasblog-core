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
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
    public static class EventMessageTemplates
    {
        static Dictionary<EventCodes, string> messages;

        static EventMessageTemplates()
		{
			string errorTemplate = "<span>{0}:<br/>{1}<br/> while processing {2}.</span>";
            messages = new Dictionary<EventCodes, string>();
			messages.Add(EventCodes.Error,errorTemplate);
			messages.Add(EventCodes.StorageError,errorTemplate);
			messages.Add(EventCodes.SmtpError,errorTemplate);
			messages.Add(EventCodes.EntryAdded,"<span>Info {0}:<br/>Entry '{1}' added. <a href=\"{2}\">Permalink</a></span>");
			messages.Add(EventCodes.EntryChanged,"<span>Info {0}:<br/>Entry '{1}' changed. <a href=\"{2}\">Permalink</a></span>");
			messages.Add(EventCodes.EntryDeleted,"<span>Info {0}:<br/>Entry '{1}' deleted. <a href=\"{2}\">Invalid permalink</a></span>");
			messages.Add(EventCodes.CommentAdded,"<span>Info {0}:<br/>Comment '{1}' added. <a href=\"{2}\">Comment view</a></span>");
			messages.Add(EventCodes.CommentChanged,"<span>Info {0}:<br/>Comment '{1}' changed. <a href=\"{2}\">Comment view</a></span>");
			messages.Add(EventCodes.CommentDeleted,"<span>Info {0}:<br/>Comment '{1}' deleted.");
			messages.Add( EventCodes.CommentApproved, "<span>Info {0}:<br/>Comment '{1}' approved.");
			messages.Add(EventCodes.CommentBlocked, "<span>Info {0}:<br/>{1} on {2}.");
			messages.Add(EventCodes.TrackbackSent,"<span>Info {0}:<br/>Trackback sent for <a href=\"{2}\">{1}</a> to <a href=\"{3}\">target</a></span>");
			messages.Add(EventCodes.TrackbackReceived,"<span>Info {0}:<br/>Trackback received for <a href=\"{2}\">{1}</a> from target <a href=\"{3}\">{4}</a></span>");
			messages.Add(EventCodes.TrackbackBlocked,"<span>Info {0}:<br/>Trackback blocked for <a href=\"{2}\">{5}</a> from <a href=\"{3}\">{4}</a> originating at IP Address {1}</span>");
			messages.Add(EventCodes.TrackbackServerError,errorTemplate);
			messages.Add(EventCodes.PingbackSent,"<span>Info {0}:<br/>Pingback sent for <a href=\"{2}\">{1}</a> to <a href=\"{3}\">target</a></span>");
			messages.Add(EventCodes.PingbackReceived,"<span>Info {0}:<br/>Pingback received for <a href=\"{2}\">{1}</a> from target <a href=\"{3}\">{4}</a></span>");
			messages.Add(EventCodes.PingbackBlocked,"<span>Info {0}:<br/>{1} For entry {2}</span>");
			messages.Add(EventCodes.PingbackServerError,errorTemplate);
			messages.Add(EventCodes.ReferralReceived,"<span>Info {0}:<br/>Referral received from target <a href=\"{3}\">{4}</a> originating at IP Address {1} </span>");
			messages.Add(EventCodes.ItemReferralReceived,"<span>Info {0}:<br/>Item referral received for <a href=\"{2}\">{5}</a> from target <a href=\"{3}\">{4}</a> originating at IP Address {1} </span>");
			messages.Add(EventCodes.ItemReferralDeleted,"<span>Info {0}:<br/>Referral '{1}' deleted.");
			messages.Add(EventCodes.Pop3EntryReceived,"<span>Info {0}:<br/>Pop3 message received from <a href=\"mailto:{3}\">{3}</a> with subject '{1}'</span>");
			messages.Add(EventCodes.Pop3EntryIgnored,"<span>Info {0}:<br/>Pop3 message ignored from <a href=\"mailto:{3}\">{3}</a> with subject '{1}'</span>");
			messages.Add(EventCodes.Pop3EntryDiscarded,"<span>Info {0}:<br/>Pop3 message discarded from <a href=\"mailto:{3}\">{3}</a> with subject '{1}'</span>");
			messages.Add(EventCodes.Pop3ServiceStart,"<span>Mail-To-Weblog service started</span>");
			messages.Add(EventCodes.Pop3ServiceShutdown,"<span>Mail-To-Weblog service shut down</span>");
			messages.Add(EventCodes.Pop3ServerError,errorTemplate);
			messages.Add(EventCodes.XSSUpstreamSuccess,"<span>Info {0}:<br/>Upstreamed '{2}' to {3}</span>");
			messages.Add(EventCodes.XSSUpstreamError,errorTemplate);
			messages.Add(EventCodes.XSSServiceStart,"<span>XmlStorageSystem Upstream service started</span>");
			messages.Add(EventCodes.XSSServiceShutdown,"<span>XmlStorageSystem Upstream service shut down</span>");
			messages.Add(EventCodes.ApplicationStartup,"<span>Application started</span>");
			messages.Add(EventCodes.PingWeblogsError,"<span>Pinging {5} failed: {1} at {3}.");
			messages.Add(EventCodes.Search,"<span>Search for: {1} from: {2}");
			messages.Add(EventCodes.CrosspostAdded,"<span>Info {0}:<br/>Crosspost Added on {1}");
			messages.Add(EventCodes.CrosspostChanged,"<span>Info {0}:<br/>Crosspost Changed on {1}");
			messages.Add(EventCodes.CrosspostDeleted,"<span>Info {0}:<br/>Crosspost Deleted for Entry {1} on {2}");
			messages.Add(EventCodes.ReferralBlocked,"<span>Info {0}:<br/>Referral blocked from target <a href=\"{3}\">{4}</a> originating at IP Address {1}</span>");
			messages.Add(EventCodes.ItemReferralBlocked,"<span>Info {0}:<br/>Item Referral blocked for <a href=\"{2}\">{5}</a> from <a href=\"{3}\">{4}</a> originating at IP Address {1}</span>");
			messages.Add(EventCodes.SecuritySuccess,"<span>Audit: {0} Login with username {1} from {2}</span>");
			messages.Add(EventCodes.SecurityFailure,"<span>Audit: {0} Login with username {1} from {2}</span>");
			messages.Add(EventCodes.CommentEmail,"<span>Comment Email From : {2}<br/> {1}</span>");
			messages.Add(EventCodes.ReportMailerServiceError,errorTemplate);
			messages.Add(EventCodes.ReportMailerServiceStart,"<span>Report-Mailer Service Started</span>");
			messages.Add(EventCodes.ReportMailerServiceShutdown,"<span>Report-Mailer Service Shutdown</span>");
			messages.Add(EventCodes.ReportMailerReportSent,"<span>Report-Mailer Service sent report</span>");
		}

        private static string GetMessageTemplate(EventCodes code)
        {
            string template = messages[code] as string;
            if ( template == null )
            {
                return "Message template not found";
            }
            else
            {
                return template;
            }
        }

		public static string FormatMessage( EventCodes eventCode, string userMessage, string localUrl, string remoteUrl, string title )
		{
			string clippedUrl = ClipString(remoteUrl);
			return String.Format( GetMessageTemplate( eventCode ), eventCode, userMessage, localUrl, remoteUrl, clippedUrl, title );
		}

        public static string FormatMessage( EventCodes eventCode, string userMessage, string localUrl, string remoteUrl )
        {
			string clippedUrl = ClipString(remoteUrl);
            return String.Format( GetMessageTemplate( eventCode ), eventCode, userMessage, localUrl, remoteUrl, clippedUrl );
        }

        public static string FormatMessage( EventCodes eventCode, string userMessage, string localUrl )
        {
            return String.Format( GetMessageTemplate( eventCode ), eventCode, userMessage, localUrl, null, null );
        }

		private static string ClipString( string text )
		{
			int length = 60;

			if (text != null && text.Length > 0)
			{
				if (text.Length > length)
				{
					text = text.Substring(0, length) + "...";
				}
			}
			else
			{
				text = "";
			}
			return text;
		}

    }
}
