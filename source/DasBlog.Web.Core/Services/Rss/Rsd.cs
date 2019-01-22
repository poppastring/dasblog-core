using System;
using System.Xml;
using System.Xml.Serialization;

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

namespace DasBlog.Core.Services.Rsd
{
    [XmlType(Namespace="", IncludeInSchema=false)]
    [XmlRoot("rsd", Namespace="http://archipelago.phrasewise.com/rsd")]
    public class RsdRoot
    {
        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces;
        private string version;
        private RssServiceCollection services;

        public RsdRoot()
        {
            Namespaces = new XmlSerializerNamespaces();
            services = new RssServiceCollection();
            version = "1.0";
        }

        [XmlAttribute("version")]
        public string Version { get { return version; } set { version = value; } }
        [XmlElement("service")]
        public RssServiceCollection Services { get { return services; } set { services = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

	[XmlRoot("service", Namespace="")]
    public class RsdService
    {
        string engineName;
        string engineLink;
        RsdApiCollection rsdApis = new RsdApiCollection();

        [XmlElement("engineName")]
        public string EngineName { get { return engineName; } set { engineName = value; } }

		[XmlElement("engineLink")]
        public string EngineLink { get { return engineLink; } set { engineLink = value; } }

        [XmlElement("homePageLink", IsNullable=false)]
		public string HomePageLink { get; set; }

		[XmlArray("apis")]
		[XmlArrayItem("api")]
		public RsdApiCollection RsdApiCollection { get { return rsdApis; } set { rsdApis = value; } }

		[XmlAnyElement]
        public XmlElement[] anyElements;

		[XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;

		public RsdService()
		{
			engineName = "dasBlog Core " + GetType().Assembly.GetName().Version;
			engineLink = "https://github.com/poppastring/dasblog-core";
		}
    }

	[Serializable]
	[XmlRoot(ElementName = "api")]
    public class RsdApi
    {
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("preferred")]
        public bool Preferred { get; set; }

		[XmlAttribute("apiLink")]
        public string ApiLink { get; set; }

		[XmlAttribute("blogID")]
        public string BlogID { get; set; }

		[XmlAnyElement]
        public XmlElement[] anyElements;

		[XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

    /// <summary>
    /// A collection of elements of type RsdApi
    /// </summary>
    public class RsdApiCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RsdApiCollection class.
        /// </summary>
        public RsdApiCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the RsdApiCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new RsdApiCollection.
        /// </param>
        public RsdApiCollection(RsdApi[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the RsdApiCollection class, containing elements
        /// copied from another instance of RsdApiCollection
        /// </summary>
        /// <param name="items">
        /// The RsdApiCollection whose elements are to be added to the new RsdApiCollection.
        /// </param>
        public RsdApiCollection(RsdApiCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RsdApiCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RsdApiCollection.
        /// </param>
        public virtual void AddRange(RsdApi[] items)
        {
            foreach (RsdApi item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another RsdApiCollection to the end of this RsdApiCollection.
        /// </summary>
        /// <param name="items">
        /// The RsdApiCollection whose elements are to be added to the end of this RsdApiCollection.
        /// </param>
        public virtual void AddRange(RsdApiCollection items)
        {
            foreach (RsdApi item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type RsdApi to the end of this RsdApiCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdApi to be added to the end of this RsdApiCollection.
        /// </param>
        public virtual void Add(RsdApi value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic RsdApi value is in this RsdApiCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdApi value to locate in this RsdApiCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this RsdApiCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(RsdApi value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this RsdApiCollection
        /// </summary>
        /// <param name="value">
        /// The RsdApi value to locate in the RsdApiCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(RsdApi value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the RsdApiCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the RsdApi is to be inserted.
        /// </param>
        /// <param name="value">
        /// The RsdApi to insert.
        /// </param>
        public virtual void Insert(int index, RsdApi value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RsdApi at the given index in this RsdApiCollection.
        /// </summary>
        public virtual RsdApi this[int index]
        {
            get
            {
                return (RsdApi) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific RsdApi from this RsdApiCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdApi value to remove from this RsdApiCollection.
        /// </param>
        public virtual void Remove(RsdApi value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RsdApiCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(RsdApiCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public RsdApi Current
            {
                get
                {
                    return (RsdApi) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (RsdApi) (this.wrapped.Current);
                }
            }

            public bool MoveNext()
            {
                return this.wrapped.MoveNext();
            }

            public void Reset()
            {
                this.wrapped.Reset();
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the elements of this RsdApiCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>
        public new virtual RsdApiCollection.Enumerator GetEnumerator()
        {
            return new RsdApiCollection.Enumerator(this);
        }
    }
    /// <summary>
    /// A collection of elements of type RsdService
    /// </summary>
    public class RssServiceCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RssServiceCollection class.
        /// </summary>
        public RssServiceCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the RssServiceCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new RssServiceCollection.
        /// </param>
        public RssServiceCollection(RsdService[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the RssServiceCollection class, containing elements
        /// copied from another instance of RssServiceCollection
        /// </summary>
        /// <param name="items">
        /// The RssServiceCollection whose elements are to be added to the new RssServiceCollection.
        /// </param>
        public RssServiceCollection(RssServiceCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RssServiceCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RssServiceCollection.
        /// </param>
        public virtual void AddRange(RsdService[] items)
        {
            foreach (RsdService item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another RssServiceCollection to the end of this RssServiceCollection.
        /// </summary>
        /// <param name="items">
        /// The RssServiceCollection whose elements are to be added to the end of this RssServiceCollection.
        /// </param>
        public virtual void AddRange(RssServiceCollection items)
        {
            foreach (RsdService item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type RsdService to the end of this RssServiceCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdService to be added to the end of this RssServiceCollection.
        /// </param>
        public virtual void Add(RsdService value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic RsdService value is in this RssServiceCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdService value to locate in this RssServiceCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this RssServiceCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(RsdService value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this RssServiceCollection
        /// </summary>
        /// <param name="value">
        /// The RsdService value to locate in the RssServiceCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(RsdService value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the RssServiceCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the RsdService is to be inserted.
        /// </param>
        /// <param name="value">
        /// The RsdService to insert.
        /// </param>
        public virtual void Insert(int index, RsdService value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RsdService at the given index in this RssServiceCollection.
        /// </summary>
        public virtual RsdService this[int index]
        {
            get
            {
                return (RsdService) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific RsdService from this RssServiceCollection.
        /// </summary>
        /// <param name="value">
        /// The RsdService value to remove from this RssServiceCollection.
        /// </param>
        public virtual void Remove(RsdService value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RssServiceCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(RssServiceCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public RsdService Current
            {
                get
                {
                    return (RsdService) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (RsdService) (this.wrapped.Current);
                }
            }

            public bool MoveNext()
            {
                return this.wrapped.MoveNext();
            }

            public void Reset()
            {
                this.wrapped.Reset();
            }
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the elements of this RssServiceCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>
        public new virtual RssServiceCollection.Enumerator GetEnumerator()
        {
            return new RssServiceCollection.Enumerator(this);
        }
    }

	[XmlRoot("image")]
	public class ChannelImage
	{
		public ChannelImage(){}

		[XmlElement("url")]
		public string Url { get; set; }

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("link")]
		public string Link { get; set; }
	}

}
