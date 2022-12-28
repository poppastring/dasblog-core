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
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;


namespace newtelligence.DasBlog.Runtime
{

	public class CloudEventsTargetHeader
	{
		public string Name { get; set; }
		public string Value { get; set; }
	}

	[Serializable]
    public class CloudEventsTarget
    {
        string profileName;
        string uri;
		CloudEventsTargetHeader[] headers;
		CloudEventsTargetHeader[] queryArgs;

		public CloudEventsTarget()
        {
        }

        public CloudEventsTarget(string profileName, string uri, CloudEventsTargetHeader[] headers, CloudEventsTargetHeader[] queryArgs)
        {
            this.profileName = profileName;
            this.uri = uri;
			this.headers = headers;
			this.queryArgs = queryArgs;
		}

        [XmlElement("ProfileName")]
        public string ProfileName { get { return profileName; } set { profileName = value; } }
        [XmlElement("Uri")]
        public string Uri { get { return uri; } set { uri = value; } }
		[XmlArray("QueryArgs")]
		public CloudEventsTargetHeader[] QueryArgs { get { return queryArgs; } set { queryArgs = value; } }
		[XmlArray("Headers")]
		public CloudEventsTargetHeader[] Headers { get { return headers; } set { headers = value; } }

		[XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

    /// <summary>
    /// A collection of elements of type CloudEventsSite
    /// </summary>
    [Serializable]
    public class CloudEventsTargetCollection: Collection<CloudEventsTarget>
    {
        /// <summary>
        /// Initializes a new empty instance of the CloudEventsSiteCollection class.
        /// </summary>
        public CloudEventsTargetCollection()
            :base()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the CloudEventsSiteCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new CloudEventsSiteCollection.
        /// </param>
        public CloudEventsTargetCollection(IList<CloudEventsTarget> items)
            :base()
        {
            if (items == null) {
                throw new ArgumentNullException("items");
            }

            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this CloudEventsSiteCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this CloudEventsSiteCollection.
        /// </param>
        public virtual void AddRange(IEnumerable<CloudEventsTarget> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (CloudEventsTarget item in items)
            {
                this.Add(item);
            }
        }
    }
}
