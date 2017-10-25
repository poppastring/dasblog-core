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
    [XmlRoot(Namespace = Data.NamespaceURI)]
    [XmlType(Namespace = Data.NamespaceURI)]
    public class EntryBase
    {
        string _content;
        DateTime _created;
        DateTime _modified;
        string _entryId;
        string _lang;


        [XmlAttribute("xml:lang")]
        public string Language
        {
            get
            {
                return _lang;
            }
            set
            {
                _lang = value;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }



        [XmlIgnore]
        public DateTime CreatedUtc { get { return _created; } set { _created = value; } }
        [XmlElement("Created")]
        public DateTime CreatedLocalTime { get { return (CreatedUtc == DateTime.MinValue || CreatedUtc == DateTime.MaxValue) ? CreatedUtc : CreatedUtc.ToLocalTime(); } set { CreatedUtc = (value == DateTime.MinValue || value == DateTime.MaxValue) ? value : value.ToUniversalTime(); } }

        [XmlIgnore]
        public DateTime ModifiedUtc { get { return _modified; } set { _modified = value; } }
        [XmlElement("Modified")]
        public DateTime ModifiedLocalTime { get { return (ModifiedUtc == DateTime.MinValue || ModifiedUtc == DateTime.MaxValue) ? ModifiedUtc : ModifiedUtc.ToLocalTime(); } set { ModifiedUtc = (value == DateTime.MinValue || value == DateTime.MaxValue) ? value : value.ToUniversalTime(); } }

        public string EntryId { get { return _entryId; } set { _entryId = value; } }

        public void Initialize()
        {
            CreatedUtc = DateTime.Now.ToUniversalTime();
            ModifiedUtc = DateTime.Now.ToUniversalTime();
            EntryId = Guid.NewGuid().ToString();
        }

        public void Modify()
        {
            ModifiedUtc = DateTime.Now.ToUniversalTime();
        }
    }
}