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

namespace newtelligence.DasBlog.Web.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Xml;
    using System.Xml.Serialization;


    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRoot("opml", Namespace = "")]
    public class Opml
    {
        public OpmlHead head;
        public OpmlBody body;

        public Opml()
        {
            body = new OpmlBody();
        }

        public Opml(string title)
        {
            head = new OpmlHead();
            head.title = title;
            body = new OpmlBody();
        }

        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
        [XmlAnyElement]
        public XmlElement[] anyElements;
    }

    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRoot("head", Namespace = "")]
    public class OpmlHead
    {
        private string _title;
        private string _dateCreated;
        private string _dateModified;
        private string _ownerName;
        private string _ownerEmail;
        private string _expansionState;
        private string _vertScrollState;
        private int _windowTop;
        private int _windowLeft;
        private int _windowBottom;
        private int _windowRight;

        [XmlElement("title")]
        public string title { get { return _title; } set { _title = value; } }
        [XmlElement("dateCreated")]
        public string dateCreated { get { return _dateCreated; } set { _dateCreated = value; } }
        [XmlElement("dateModified")]
        public string dateModified { get { return _dateModified; } set { _dateModified = value; } }
        [XmlElement("ownerName")]
        public string ownerName { get { return _ownerName; } set { _ownerName = value; } }
        [XmlElement("ownerEMail")]
        public string ownerEmail { get { return _ownerEmail; } set { _ownerEmail = value; } }
        [XmlElement("expansionState")]
        public string expansionState { get { return _expansionState; } set { _expansionState = value; } }
        [XmlElement("vertScrollState")]
        public string vertScrollState { get { return _vertScrollState; } set { _vertScrollState = value; } }
        [XmlElement("windowTop")]
        public int windowTop { get { return _windowTop; } set { _windowTop = value; } }
        [XmlElement("windowLeft")]
        public int windowLeft { get { return _windowLeft; } set { _windowLeft = value; } }
        [XmlElement("windowBottom")]
        public int windowBottom { get { return _windowBottom; } set { _windowBottom = value; } }
        [XmlElement("windowRight")]
        public int windowRight { get { return _windowRight; } set { _windowRight = value; } }
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
        [XmlAnyElement]
        public XmlElement[] anyElements;
    }

    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRoot("body", Namespace = "")]
    public class OpmlBody
    {
        OpmlOutlineCollection _outlineCollection = new OpmlOutlineCollection();

        // sort the array when we need to.
        [XmlElement("outline")]
        public OpmlOutline[] outlineArray { 
            get { 
                return _outlineCollection.Count == 0 ? null : _outlineCollection.ToArraySortedByTitle(); 
            } 
            set { 
                _outlineCollection.Clear(); 
                _outlineCollection.AddRange(value); 
            } 
        }
        [XmlIgnore]
        public OpmlOutlineCollection outline { get { return _outlineCollection; } }

        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
        [XmlAnyElement]
        public XmlElement[] anyElements;
    }

    [Serializable]
    [XmlType(Namespace = "")]
    [XmlRoot("outline", Namespace = "")]
    public class OpmlOutline
    {
        string _type;
        string _description;
        string _title;
        string _xmlUrl;
        string _htmlUrl;
        string _language;
        string _isComment;
        string _isBreakpoint;
        OpmlOutlineCollection _outlineCollection = new OpmlOutlineCollection();

        [XmlAttribute]
        public string type { get { return _type; } set { _type = value; } }
        [XmlAttribute]
        public string description { get { return _description; } set { _description = value; } }
        [XmlAttribute]
        public string title { get { return _title; } set { _title = value; } }
        [XmlAttribute]
        public string xmlUrl { get { return _xmlUrl; } set { _xmlUrl = value; } }
        [XmlAttribute]
        public string htmlUrl { get { return _htmlUrl; } set { _htmlUrl = value; } }
        [XmlAttribute]
        public string language { get { return _language; } set { _language = value; } }
        [XmlAttribute]
        public string isComment { get { return _isComment; } set { _isComment = value; } }
        [XmlAttribute]
        public string isBreakpoint { get { return _isBreakpoint; } set { _isBreakpoint = value; } }
        // sort the array when we need to
        [XmlElement("outline")]
        public OpmlOutline[] outlineArray { get { return _outlineCollection.Count == 0 ? null : _outlineCollection.ToArraySortedByTitle(); } set { _outlineCollection.Clear(); _outlineCollection.AddRange(value); } }
        [XmlIgnore]
        public OpmlOutlineCollection outline { get { return _outlineCollection; } }
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
        [XmlAnyElement]
        public XmlElement[] anyElements;
    }

    /// <summary>
    /// A collection of elements of type OpmlOutline
    /// </summary>
    [Serializable]
    public class OpmlOutlineCollection : Collection<OpmlOutline>
    {
        /// <summary>
        /// Initializes a new empty instance of the OpmlOutlineCollection class.
        /// </summary>
        public OpmlOutlineCollection()
            : base()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the OpmlOutlineCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new OpmlOutlineCollection.
        /// </param>
        protected OpmlOutlineCollection(IList<OpmlOutline> items)
            : base()
        {
            if (items == null) {
                throw new ArgumentNullException("items");
            }

            this.AddRange(items);

        }

        /// <summary>
        /// Adds the elements of an array to the end of this OpmlOutlineCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this OpmlOutlineCollection.
        /// </param>
        public virtual void AddRange(IEnumerable<OpmlOutline> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (OpmlOutline item in items)
            {
                this.Items.Add(item);
            }
        }


        public OpmlOutline[] ToArray()
        {
            OpmlOutline[] result = new OpmlOutline[this.Count];
            this.CopyTo(result, 0);

            return result;
        }

        public OpmlOutline[] ToArraySortedByTitle()
        {
            OpmlOutline[] result = ToArray();
            Array.Sort(result, delegate(OpmlOutline x, OpmlOutline y)
            {
                return String.Compare(x.title, y.title, StringComparison.InvariantCultureIgnoreCase);
            });

            return result;
        }

        public OpmlOutline[] ToArraySortedByDescription()
        {
            OpmlOutline[] result = ToArray();
            Array.Sort(result, delegate(OpmlOutline x, OpmlOutline y)
            {
                return String.Compare(x.description, y.description, StringComparison.InvariantCultureIgnoreCase);
            });

            return result;
        }
    }
}
