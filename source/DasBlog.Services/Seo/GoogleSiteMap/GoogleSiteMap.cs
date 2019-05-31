using System;
using System.Xml.Serialization;

namespace DasBlog.Core.Services.GoogleSiteMap
{
	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class urlset
	{

		public urlset() { }

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("url")]
		public urlCollection url;
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public class url
	{
		public url() { }

		public url(string locIn, DateTime lastmodIn, changefreq freqIn, Decimal priorityIn)
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
		public changefreq changefreq;

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
	[System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84")]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.google.com/schemas/sitemap/0.84", IsNullable = false)]
	public enum changefreq
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
	/// A collection of elements of type url
	/// </summary>
	public class urlCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the urlCollection class.
		/// </summary>
		public urlCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the urlCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the new urlCollection.
		/// </param>
		public urlCollection(url[] items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Initializes a new instance of the urlCollection class, containing elements
		/// copied from another instance of urlCollection
		/// </summary>
		/// <param name="items">
		/// The urlCollection whose elements are to be added to the new urlCollection.
		/// </param>
		public urlCollection(urlCollection items)
		{
			this.AddRange(items);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this urlCollection.
		/// </summary>
		/// <param name="items">
		/// The array whose elements are to be added to the end of this urlCollection.
		/// </param>
		public virtual void AddRange(url[] items)
		{
			foreach (url item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds the elements of another urlCollection to the end of this urlCollection.
		/// </summary>
		/// <param name="items">
		/// The urlCollection whose elements are to be added to the end of this urlCollection.
		/// </param>
		public virtual void AddRange(urlCollection items)
		{
			foreach (url item in items)
			{
				this.List.Add(item);
			}
		}

		/// <summary>
		/// Adds an instance of type url to the end of this urlCollection.
		/// </summary>
		/// <param name="value">
		/// The url to be added to the end of this urlCollection.
		/// </param>
		public virtual void Add(url value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic url value is in this urlCollection.
		/// </summary>
		/// <param name="value">
		/// The url value to locate in this urlCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this urlCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(url value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this urlCollection
		/// </summary>
		/// <param name="value">
		/// The url value to locate in the urlCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(url value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the urlCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the url is to be inserted.
		/// </param>
		/// <param name="value">
		/// The url to insert.
		/// </param>
		public virtual void Insert(int index, url value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the url at the given index in this urlCollection.
		/// </summary>
		public virtual url this[int index]
		{
			get
			{
				return (url)this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific url from this urlCollection.
		/// </summary>
		/// <param name="value">
		/// The url value to remove from this urlCollection.
		/// </param>
		public virtual void Remove(url value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by urlCollection.GetEnumerator.
		/// </summary>
		public class Enumerator : System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(urlCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public url Current
			{
				get
				{
					return (url)(this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (url)(this.wrapped.Current);
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
		/// Returns an enumerator that can iterate through the elements of this urlCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>        
		public new virtual urlCollection.Enumerator GetEnumerator()
		{
			return new urlCollection.Enumerator(this);
		}
	}

}
