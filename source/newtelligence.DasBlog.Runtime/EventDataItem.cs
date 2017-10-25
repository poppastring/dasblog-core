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
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime
{
    [Serializable]
    [XmlRoot(Namespace=Data.NamespaceURI)]
    [XmlType(Namespace=Data.NamespaceURI)]
    public class EventDataItem
    {
        DateTime _eventTime;
        string _htmlMessage;
        int _eventCode;

        public EventDataItem()
        {
            EventTimeUtc = DateTime.Now.ToUniversalTime();
            HtmlMessage = "";
            EventCode = 0;
        }

		public EventDataItem( EventCodes eventCode, string userMessage, string localUrl, string remoteUrl, string title )
		{
			// http://www.net-security.org/vuln.php?id=3680
			localUrl = LogEncoder.Encode(localUrl);
			remoteUrl = LogEncoder.Encode(remoteUrl);
			userMessage = LogEncoder.EncodeBadTags(userMessage);

			EventTimeUtc = DateTime.Now.ToUniversalTime();
			HtmlMessage = EventMessageTemplates.FormatMessage( eventCode, userMessage, localUrl, remoteUrl, title );
			EventCode = Convert.ToInt32(eventCode);
		}
        
		public EventDataItem( EventCodes eventCode, string userMessage, string localUrl, string remoteUrl )
		{
			// http://www.net-security.org/vuln.php?id=3680
			localUrl = LogEncoder.Encode(localUrl);
			remoteUrl = LogEncoder.Encode(remoteUrl);
			userMessage = LogEncoder.Encode(userMessage);

			EventTimeUtc = DateTime.Now.ToUniversalTime();
			HtmlMessage = EventMessageTemplates.FormatMessage( eventCode, userMessage, localUrl, remoteUrl );
			EventCode = Convert.ToInt32(eventCode);
		}

        public EventDataItem( EventCodes eventCode, string userMessage, string localUrl )
        {
			// http://www.net-security.org/vuln.php?id=3680
			localUrl = LogEncoder.Encode(localUrl);

            EventTimeUtc = DateTime.Now.ToUniversalTime();
            HtmlMessage = EventMessageTemplates.FormatMessage( eventCode, userMessage, localUrl );
            EventCode = Convert.ToInt32(eventCode);
        }

        [XmlIgnore]
        public DateTime EventTimeUtc { get {  return _eventTime; } set { _eventTime= value; } }
        [XmlElement("EventTime")]
        public DateTime EventTimeLocalTime { get { return (EventTimeUtc==DateTime.MinValue||EventTimeUtc==DateTime.MaxValue)?EventTimeUtc:EventTimeUtc.ToLocalTime(); } set { EventTimeUtc = (value==DateTime.MinValue||value==DateTime.MaxValue)?value:value.ToUniversalTime(); } }
        public string HtmlMessage
        {
        	get
        	{
        		return LogEncoder.EncodeBadTags(_htmlMessage);
        	} 
			set
			{
				_htmlMessage = value;
			}
        }
        public int EventCode { get { return _eventCode; } set { _eventCode = value; } }

    }
}
