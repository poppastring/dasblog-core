using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web.Services.InstantArticle
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlType(Namespace="", IncludeInSchema=false)]
    [XmlRoot("rss", Namespace= "http://purl.org/rss/1.0/modules/content/")]
    public class IARoot
    {
        [XmlNamespaceDeclarations] 
        public XmlSerializerNamespaces Namespaces;
        private string version;
        private IAChannelCollection channels;

        public IARoot()
        {
            Namespaces = new XmlSerializerNamespaces();
            channels = new IAChannelCollection();
            version = "2.0";
        }

        [XmlAttribute("version")]
        public string Version { get { return version; } set { version = value; } }
        [XmlElement("channel")]
        public IAChannelCollection Channels { get { return channels; } set { channels = value; } }
    }

    [XmlRoot("channel")]
    public class IAChannel
    {
        IAItemCollection items = new IAItemCollection();

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("description", IsNullable=false)]
        public string Description { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("lastBuildDate")]
        public string LastBuildDate { get; set; }

        [XmlElement("language")]
		public string Language { get; set; }

        [XmlElement("generator")]
        public string Generator { get; set; }

        [XmlElement("docs")]
        public string Docs { get; set; }

        [XmlElement("item")]
        public IAItemCollection Items { get { return items; } set { items = value; } }

		public IAChannel()
		{
            Generator = "newtelligence dasBlog "+GetType().Assembly.GetName().Version; 
		}
    }

    [XmlRoot("item")]
    public class IAItem
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("pubDate")]
        public string PubDate { get; set; }

        [XmlElement("link")]
        public string Link { get; set; }

        [XmlElement("guid")]
        public string Guid { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlAnyElement]
        public XmlElement[] anyElements;
    }

	/// <summary>
    /// A collection of elements of type RssItem
    /// </summary>
    public class IAItemCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RssItemCollection class.
        /// </summary>
        public IAItemCollection()
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
        public IAItemCollection(IAItem[] items)
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
        public IAItemCollection(IAItemCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RssItemCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RssItemCollection.
        /// </param>
        public virtual void AddRange(IAItem[] items)
        {
            foreach (IAItem item in items)
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
        public virtual void AddRange(IAItemCollection items)
        {
            foreach (IAItem item in items)
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
        public virtual void Add(IAItem value)
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
        public virtual bool Contains(IAItem value)
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
        public virtual int IndexOf(IAItem value)
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
        public virtual void Insert(int index, IAItem value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RssItem at the given index in this RssItemCollection.
        /// </summary>
        public virtual IAItem this[int index]
        {
            get
            {
                return (IAItem) this.List[index];
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
        public virtual void Remove(IAItem value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RssItemCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(IAItemCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public IAItem Current
            {
                get
                {
                    return (IAItem) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (IAItem) (this.wrapped.Current);
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
        public new virtual IAItemCollection.Enumerator GetEnumerator()
        {
            return new IAItemCollection.Enumerator(this);
        }
    }
    /// <summary>
    /// A collection of elements of type RssChannel
    /// </summary>
    public class IAChannelCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the RssChannelCollection class.
        /// </summary>
        public IAChannelCollection()
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
        public IAChannelCollection(IAChannel[] items)
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
        public IAChannelCollection(IAChannelCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this RssChannelCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this RssChannelCollection.
        /// </param>
        public virtual void AddRange(IAChannel[] items)
        {
            foreach (IAChannel item in items)
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
        public virtual void AddRange(IAChannelCollection items)
        {
            foreach (IAChannel item in items)
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
        public virtual void Add(IAChannel value)
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
        public virtual bool Contains(IAChannel value)
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
        public virtual int IndexOf(IAChannel value)
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
        public virtual void Insert(int index, IAChannel value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the RssChannel at the given index in this RssChannelCollection.
        /// </summary>
        public virtual IAChannel this[int index]
        {
            get
            {
                return (IAChannel) this.List[index];
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
        public virtual void Remove(IAChannel value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by RssChannelCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(IAChannelCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public IAChannel Current
            {
                get
                {
                    return (IAChannel) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (IAChannel) (this.wrapped.Current);
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
        public new virtual IAChannelCollection.Enumerator GetEnumerator()
        {
            return new IAChannelCollection.Enumerator(this);
        }
    }

}
