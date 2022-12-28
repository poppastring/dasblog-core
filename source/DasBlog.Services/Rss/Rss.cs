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

namespace DasBlog.Services.Rss.Rss20
{
    [XmlType(Namespace="", IncludeInSchema=false)]
    [XmlRoot("rss", Namespace="")]
    public class RssRoot
    {
        [XmlNamespaceDeclarations] 
        public XmlSerializerNamespaces Namespaces;
        private string version;
        private RssChannelCollection channels;

        public RssRoot()
        {
            Namespaces = new XmlSerializerNamespaces();
            channels = new RssChannelCollection();
            version = "2.0";
        }

        [XmlAttribute("version")]
        public string Version { get { return version; } set { version = value; } }
        [XmlElement("channel")]
        public RssChannelCollection Channels { get { return channels; } set { channels = value; } }
        
        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

    [XmlRoot("channel")]
    public class RssChannel
    {
		public RssChannel()
		{
			generator = "dasBlog Core " + GetType().Assembly.GetName().Version;
		}

		private readonly string generator;

		[XmlElement("title")]
		public string Title { get; set; }

		[XmlElement("link")]
		public string Link { get; set; }

		[XmlElement("description", IsNullable = false)]
		public string Description { get; set; }

		[XmlElement("image")]
		public ChannelImage Image { get; set; }

		[XmlElement("language")]
		public string Language { get; set; }

		[XmlElement("copyright")]
		public string Copyright { get; set; }

		[XmlElement("lastBuildDate")]
		public string LastBuildDate { get; set; }

		[XmlElement("generator")]
		public string Generator { get { return generator; }  }

		[XmlElement("managingEditor")]
		public string ManagingEditor { get; set; }

		[XmlElement("webMaster")]
		public string WebMaster { get; set; }

		[XmlElement("item")]
		public RssItemCollection Items { get; set; }

		[XmlAnyElement]
		public XmlElement[] anyElements;
		[XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;
	}

    [XmlRoot("item")]
    public class RssItem
    {	
		[XmlAttribute("xml:lang")]
        public string Language;

		[XmlAttribute("id")]
		public string Id;

		[XmlElement("author")]
		public string Author { get; set; }

		[XmlElement("title")]
        public string Title { get; set; }

		[XmlElement("guid")]
        public Guid Guid { get; set; }

		[XmlElement("link")]
        public string Link { get; set; }

		[XmlElement("pubDate")]
        public string PubDate { get; set; }

		[XmlElement("description")]
        public string Description { get; set; }

		[XmlElement("comments")]
        public string Comments { get; set; }

		[XmlElement("category")]
		public RssCategoryCollection Categories { get; set; }

		[XmlElement("enclosure")]
		public Enclosure Enclosure { get; set; }

		[XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }

	/// <summary>
	/// Link
	/// </summary>
	[Serializable]
	[XmlRoot("guid")]
	public class Guid
	{
		bool isPermalink;
		string text;

		[XmlAttribute("isPermaLink")] public bool IsPermaLink { get {return this.isPermalink;} set {this.isPermalink = value;} }

		[XmlText]
		public string Text{ get {return this.text;} set {this.text = value;} }

	}

	[XmlRoot("category")]
	public class RssCategory
	{
		string text;

		public RssCategory() {}
		[XmlText]
		public string Text{ get {return this.text;} set {this.text = value;} }
	}

	/// <summary>
	/// RSS Enclosure
	/// </summary>
	[Serializable]
	[XmlRoot("enclosure")]
	public class Enclosure
	{
		[XmlAttribute("url")]
		public string Url { get; set; }

		[XmlAttribute("type")]
		public string Type { get; set; }

		[XmlAttribute("length")]
		public string Length { get; set; }
	}

	/// <summary>
	/// A collection of elements of type RssCategory
	/// </summary>
	public class RssCategoryCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the RssCategoryCollection class.
		/// </summary>
		public RssCategoryCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the RssCategoryCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the new RssCategoryCollection.
		/// </param>
		public RssCategoryCollection(RssCategory[] entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Initializes a new instance of the RssCategoryCollection class, containing elements
		/// copied from another instance of RssCategoryCollection
		/// </summary>
		/// <param name="entries">
		/// The RssCategoryCollection whose elements are to be added to the new RssCategoryCollection.
		/// </param>
		public RssCategoryCollection(RssCategoryCollection entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this RssCategoryCollection.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the end of this RssCategoryCollection.
		/// </param>
		public virtual void AddRange(RssCategory[] entries)
		{
			foreach (RssCategory entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds the elements of another RssCategoryCollection to the end of this RssCategoryCollection.
		/// </summary>
		/// <param name="entries">
		/// The RssCategoryCollection whose elements are to be added to the end of this RssCategoryCollection.
		/// </param>
		public virtual void AddRange(RssCategoryCollection entries)
		{
			foreach (RssCategory entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds an instance of type RssCategory to the end of this RssCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The RssCategory to be added to the end of this RssCategoryCollection.
		/// </param>
		public virtual void Add(RssCategory value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic RssCategory value is in this RssCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The RssCategory value to locate in this RssCategoryCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this RssCategoryCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(RssCategory value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this RssCategoryCollection
		/// </summary>
		/// <param name="value">
		/// The RssCategory value to locate in the RssCategoryCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(RssCategory value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the RssCategoryCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the RssCategory is to be inserted.
		/// </param>
		/// <param name="value">
		/// The RssCategory to insert.
		/// </param>
		public virtual void Insert(int index, RssCategory value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the RssCategory at the given index in this RssCategoryCollection.
		/// </summary>
		public virtual RssCategory this[int index]
		{
			get
			{
				return (RssCategory) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific RssCategory from this RssCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The RssCategory value to remove from this RssCategoryCollection.
		/// </param>
		public virtual void Remove(RssCategory value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by RssCategoryCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(RssCategoryCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public RssCategory Current
			{
				get
				{
					return (RssCategory) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (RssCategory) (this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this RssCategoryCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual RssCategoryCollection.Enumerator GetEnumerator()
		{
			return new RssCategoryCollection.Enumerator(this);
		}
	}

	/// <summary>
    /// A collection of elements of type RssItem
    /// </summary>
    public class RssItemCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RssItemCollection class.
        /// </summary>
        public RssItemCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the RssItemCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new RssItemCollection.
        /// </param>
        public RssItemCollection(RssItem[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the RssItemCollection class, containing elements
        /// copied from another instance of RssItemCollection
        /// </summary>
        /// <param name="items">
        /// The RssItemCollection whose elements are to be added to the new RssItemCollection.
        /// </param>
        public RssItemCollection(RssItemCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RssItemCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RssItemCollection.
        /// </param>
        public virtual void AddRange(RssItem[] items)
        {
            foreach (RssItem item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another RssItemCollection to the end of this RssItemCollection.
        /// </summary>
        /// <param name="items">
        /// The RssItemCollection whose elements are to be added to the end of this RssItemCollection.
        /// </param>
        public virtual void AddRange(RssItemCollection items)
        {
            foreach (RssItem item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type RssItem to the end of this RssItemCollection.
        /// </summary>
        /// <param name="value">
        /// The RssItem to be added to the end of this RssItemCollection.
        /// </param>
        public virtual void Add(RssItem value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic RssItem value is in this RssItemCollection.
        /// </summary>
        /// <param name="value">
        /// The RssItem value to locate in this RssItemCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this RssItemCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(RssItem value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this RssItemCollection
        /// </summary>
        /// <param name="value">
        /// The RssItem value to locate in the RssItemCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(RssItem value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the RssItemCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the RssItem is to be inserted.
        /// </param>
        /// <param name="value">
        /// The RssItem to insert.
        /// </param>
        public virtual void Insert(int index, RssItem value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RssItem at the given index in this RssItemCollection.
        /// </summary>
        public virtual RssItem this[int index]
        {
            get
            {
                return (RssItem) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific RssItem from this RssItemCollection.
        /// </summary>
        /// <param name="value">
        /// The RssItem value to remove from this RssItemCollection.
        /// </param>
        public virtual void Remove(RssItem value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RssItemCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(RssItemCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public RssItem Current
            {
                get
                {
                    return (RssItem) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (RssItem) (this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this RssItemCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual RssItemCollection.Enumerator GetEnumerator()
        {
            return new RssItemCollection.Enumerator(this);
        }
    }
    /// <summary>
    /// A collection of elements of type RssChannel
    /// </summary>
    public class RssChannelCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RssChannelCollection class.
        /// </summary>
        public RssChannelCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the RssChannelCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new RssChannelCollection.
        /// </param>
        public RssChannelCollection(RssChannel[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the RssChannelCollection class, containing elements
        /// copied from another instance of RssChannelCollection
        /// </summary>
        /// <param name="items">
        /// The RssChannelCollection whose elements are to be added to the new RssChannelCollection.
        /// </param>
        public RssChannelCollection(RssChannelCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RssChannelCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RssChannelCollection.
        /// </param>
        public virtual void AddRange(RssChannel[] items)
        {
            foreach (RssChannel item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another RssChannelCollection to the end of this RssChannelCollection.
        /// </summary>
        /// <param name="items">
        /// The RssChannelCollection whose elements are to be added to the end of this RssChannelCollection.
        /// </param>
        public virtual void AddRange(RssChannelCollection items)
        {
            foreach (RssChannel item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type RssChannel to the end of this RssChannelCollection.
        /// </summary>
        /// <param name="value">
        /// The RssChannel to be added to the end of this RssChannelCollection.
        /// </param>
        public virtual void Add(RssChannel value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic RssChannel value is in this RssChannelCollection.
        /// </summary>
        /// <param name="value">
        /// The RssChannel value to locate in this RssChannelCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this RssChannelCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(RssChannel value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this RssChannelCollection
        /// </summary>
        /// <param name="value">
        /// The RssChannel value to locate in the RssChannelCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(RssChannel value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the RssChannelCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the RssChannel is to be inserted.
        /// </param>
        /// <param name="value">
        /// The RssChannel to insert.
        /// </param>
        public virtual void Insert(int index, RssChannel value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RssChannel at the given index in this RssChannelCollection.
        /// </summary>
        public virtual RssChannel this[int index]
        {
            get
            {
                return (RssChannel) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific RssChannel from this RssChannelCollection.
        /// </summary>
        /// <param name="value">
        /// The RssChannel value to remove from this RssChannelCollection.
        /// </param>
        public virtual void Remove(RssChannel value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RssChannelCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(RssChannelCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public RssChannel Current
            {
                get
                {
                    return (RssChannel) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (RssChannel) (this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this RssChannelCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual RssChannelCollection.Enumerator GetEnumerator()
        {
            return new RssChannelCollection.Enumerator(this);
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
