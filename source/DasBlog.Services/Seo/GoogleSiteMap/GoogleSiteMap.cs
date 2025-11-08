using System;
using System.Xml.Serialization;

namespace DasBlog.Core.Services.GoogleSiteMap
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute("urlset", Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute("urlset", Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class UrlSet
	{

		public UrlSet() { }

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("url")]
		public UrlCollection url;
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute("url", Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute("url", Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class Url
	{
		public Url() { }

		public Url(string locIn, DateTime lastmodIn, ChangeFreq freqIn, Decimal priorityIn)
		{
			loc = locIn;
			lastmod = lastmodIn;
			changefreq = freqIn;
			changefreqSpecified = true;
			priority = priorityIn;
			prioritySpecified = true;
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute(DataType = "anyURI")]
		public string loc;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("lastmod")]
		public string lastmodString
		{
			get
			{
				return lastmod.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			}
			set
			{
				lastmod = DateTime.ParseExact(value, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			}
		}

		[XmlIgnore]
		public DateTime lastmod;

		/// <remarks/>
		public ChangeFreq changefreq;

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool changefreqSpecified;

		/// <remarks/>
		public System.Decimal priority;

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool prioritySpecified;
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute("changefreq", Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute("changefreq", Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public enum ChangeFreq
	{

		/// <remarks/>
		always,

		/// <remarks/>
		hourly,

		/// <remarks/>
		daily,

		/// <remarks/>
		weekly,

		/// <remarks/>
		monthly,

		/// <remarks/>
		yearly,

		/// <remarks/>
		never,
	}

	/// <summary>
	/// A collection of elements of type Url
	/// </summary>
	public class UrlCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the UrlCollection class.
		/// </summary>
		public UrlCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the UrlCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new UrlCollection.
		/// </param>
		public UrlCollection(Url[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the UrlCollection class, containing elements
		/// copied from another instance of UrlCollection
		/// </summary>
		/// <param name="items">
		/// The UrlCollection whose elements are to be added to the new UrlCollection.
		/// </param>
		public UrlCollection(UrlCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this UrlCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this UrlCollection.
		/// </param>
		public virtual void AddRange(Url[] items)
		{
			foreach (Url item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another UrlCollection to the end of this UrlCollection.
		/// </summary>
		/// <param name="items">
		/// The UrlCollection whose elements are to be added to the end of this UrlCollection.
		/// </param>
		public virtual void AddRange(UrlCollection items)
		{
			foreach (Url item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type Url to the end of this UrlCollection.
		/// </summary>
		/// <param name="value">
		/// The Url to be added to the end of this UrlCollection.
		/// </param>
		public virtual void Add(Url value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic Url value is in this UrlCollection.
		/// </summary>
		/// <param name="value">
		/// The Url value to locate in this UrlCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this UrlCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(Url value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this UrlCollection
		/// </summary>
		/// <param name="value">
		/// The Url value to locate in the UrlCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(Url value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the UrlCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the Url is to be inserted.
		/// </param>
		/// <param name="value">
		/// The Url to insert.
		/// </param>
		public virtual void Insert(int index, Url value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the Url at the given index in this UrlCollection.
		/// </summary>
		public virtual Url this[int index]
		{
			get
			{
				return (Url)this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific Url from this UrlCollection.
		/// </summary>
		/// <param name="value">
		/// The Url value to remove from this UrlCollection.
		/// </param>
		public virtual void Remove(Url value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by UrlCollection.GetEnumerator.
		/// </summary>
		public class Enumerator : System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(UrlCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public Url Current
			{
				get
				{
					return (Url)(this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (Url)(this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this UrlCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual UrlCollection.Enumerator GetEnumerator()
		{
			return new UrlCollection.Enumerator(this);
		}
	}

}
