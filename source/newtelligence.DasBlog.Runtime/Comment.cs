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
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime
{
   [Serializable]
   [XmlRoot(Namespace = Data.NamespaceURI)]
   [XmlType(Namespace = Data.NamespaceURI)]
   public class Comment : EntryBase, IFeedback
   {
      private string _author;
      private string _authorEmail;
      private string _authorHomepage;
      private string _authorIpAddress;
      private string _targetEntryId;
      private string _targetTitle;
      // used for comment moderation
      // default true for backwards compatibility
      private bool _isPublic = true;
      private SpamState _spamState = SpamState.NotChecked;
      private string _userAgent;
      private bool _openid = false;

      public string TargetTitle { get { return _targetTitle; } set { _targetTitle = value; } }
      public string TargetEntryId { get { return _targetEntryId; } set { _targetEntryId = value; } }
      public string Author { get { return _author; } set { _author = value; } }
      public string AuthorEmail { get { return _authorEmail; } set { _authorEmail = value; } }
      public string AuthorHomepage { get { return _authorHomepage; } set { _authorHomepage = value; } }
      public string AuthorIPAddress { get { return _authorIpAddress; } set { _authorIpAddress = value; } }

      private string _referer;
      [XmlIgnore]
      public string Referer { get { return _referer; } set { _referer = value; } }

      /// <summary>
      /// Gets or sets a value indicating whether this instance is public.
      /// </summary>
      /// <value>
      /// 	<see langword="true"/> if this instance is public; otherwise, <see langword="false"/>.
      /// </value>
      public bool IsPublic
      {
         get { return _isPublic; }
         set { _isPublic = value; }
      }

      public bool OpenId
      {
         get { return _openid; }
         set { _openid = value; }
      }


      [XmlAnyElement]
      public XmlElement[] anyElements;
      [XmlAnyAttribute]
      public XmlAttribute[] anyAttributes;

      public SpamState SpamState { get { return _spamState; } set { _spamState = value; } }
      public string AuthorUserAgent { get { return _userAgent; } set { _userAgent = value; } }

      #region IFeedback Members

      [XmlIgnore]
      string IFeedback.FeedbackType
      {
         get
         {
            return "comment";
         }
      }

      [XmlIgnore]
      string IFeedback.Referer
      {
         get
         {
            return _referer;
         }
      }

      #endregion
   }
}
