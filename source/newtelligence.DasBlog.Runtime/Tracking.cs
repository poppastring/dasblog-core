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
    [XmlRoot(Namespace=Data.NamespaceURI)]
    [XmlType(Namespace=Data.NamespaceURI)]
    public class Tracking : IFeedback
    {
        string refererTitle;
        string refererExcerpt;
        string permaLink;
        string refererBlogName;
        string targetEntryId;
        string targetTitle;
        TrackingType trackingType=TrackingType.Unknown;
        
        public string TargetTitle { get { return targetTitle; } set { targetTitle = value; } }
        public string TargetEntryId { get { return targetEntryId; } set { targetEntryId = value; } }
        public string RefererTitle { get { return refererTitle; } set { refererTitle = value; } }
        public string RefererExcerpt { get { return refererExcerpt; } set { refererExcerpt = value; } }
        public string PermaLink { get { return permaLink; } set { permaLink = value; } }
        public string RefererBlogName { get { return refererBlogName; } set { refererBlogName = value; } }
        public TrackingType TrackingType { get { return trackingType; } set { trackingType = value; } }

		string refererIPAddress;
		[XmlIgnore] // This will need to be persisted if we want trackback moderation
		public string RefererIPAddress { get { return refererIPAddress; } set { refererIPAddress = value; } }
		private string _referer;
		[XmlIgnore]
		public string Referer { get { return _referer; } set { _referer = value; } }

		[XmlIgnore]
        public bool TrackingTypeSpecified { get { return trackingType != TrackingType.Unknown; } set { if ( !value ) trackingType = TrackingType.Unknown; }} 

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;
		#region IFeedback Members

		string IFeedback.Author
		{
			get
			{
				return RefererBlogName;
			}
		}

		string IFeedback.AuthorEmail
		{
			get
			{
				return String.Empty;
			}
		}

		string IFeedback.AuthorHomepage
		{
			get
			{
				return PermaLink;
			}
		}
	
		string IFeedback.AuthorIPAddress
		{
			get { return refererIPAddress; }
		}

		string IFeedback.AuthorUserAgent
		{
			get
			{
				return String.Empty;
			}
		}

		string IFeedback.FeedbackType
		{
			get
			{
				return trackingType.ToString();
			}
		}

		string IFeedback.Content
		{
			get
			{
				return RefererExcerpt;
			}
		}

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
