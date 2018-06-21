using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
	/// <summary>
	/// Implements a strongly typed collection of <see cref="Entry"/> elements.
	/// </summary>
	/// <remarks><para>
	/// <b>EntryCollection</b> provides an <see cref="List{T}"/>
	/// that is strongly typed for <see cref="Entry"/> elements.
	/// </para><para>
	/// The <see cref="EntryBase.EntryId"/> property of the
	/// <see cref="Entry"/> class can be used as a key
	/// to locate elements in the <b>EntryCollection</b>.
	/// </para><para>
	/// The collection may contain multiple identical keys. All key access methods
	/// return the first occurrence of the specified key, if found. Access by key
	/// is an O(<em>N</em>) operation, where <em>N</em> is the current value of the
	/// <see cref="List{T}.Count"/> property.
	/// </para></remarks>
	[Serializable]
	public class EntryCollection : List<Entry>, ICloneable
	{
		/// <overloads>
		/// Initializes a new instance of the <see cref="EntryCollection"/> class.
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the <see cref="EntryCollection"/> class
		/// that is empty and has the default initial capacity.
		/// </summary>
		/// <remarks>Please refer to <see cref="List{T}()"/> for details.</remarks>
		public EntryCollection()
			: base()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryCollection"/> class
		/// that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new
		/// <see cref="EntryCollection"/> is initially capable of storing.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="capacity"/> is less than zero.</exception>
		/// <remarks>Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>
		public EntryCollection(int capacity)
			: base(capacity)
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntryCollection"/> class
		/// that contains elements copied from the specified collection and
		/// that has the same initial capacity as the number of elements copied.
		/// </summary>
		/// <param name="collection">The <see cref="EntryCollection"/>
		/// whose elements are copied to the new collection.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is a null reference.</exception>
		/// <remarks>Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>
		public EntryCollection(IEnumerable<Entry> collection)
			: base(collection)
		{
			// empty
		}

		/// <overloads>
		/// Gets or sets a specific <see cref="Entry"/> element.
		/// </overloads>
		/// <summary>
		/// Gets the <see cref="Entry"/> element associated with the first
		/// occurrence of the specified <see cref="EntryBase.EntryId"/> value.
		/// </summary>
		/// <param name="key">
		/// The <see cref="EntryBase.EntryId"/> value whose element to get.</param>
		/// <value>The <see cref="Entry"/> element associated with the first
		/// occurrence of the specified <paramref name="key"/>, if found; otherwise,
		/// a null reference.
		/// </value>
		/// <remarks>
		/// This indexer has the same effect as the <see cref="GetByKey"/> method.
		/// </remarks>
		public Entry this[string key]
		{
			get { return GetByKey(key); }
		}

		/// <summary>
		/// Creates a shallow copy of the <see cref="EntryCollection"/>.
		/// </summary>
		/// <returns>A shallow copy of the <see cref="EntryCollection"/>.</returns>
		/// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>
		public virtual object Clone()
		{
			EntryCollection collection = new EntryCollection(this.ToArray());

			return collection;
		}

		/// <summary>
		/// Determines whether the <see cref="EntryCollection"/> contains
		/// the specified <see cref="EntryBase.EntryId"/> value.
		/// </summary>
		/// <param name="key">The <see cref="EntryBase.EntryId"/>
		/// value to locate in the <see cref="EntryCollection"/>.</param>
		/// <returns><c>true</c> if <paramref name="key"/> is found in the
		/// <see cref="EntryCollection"/>; otherwise, <c>false</c>.</returns>
		/// <remarks>
		/// <b>ContainsKey</b> is similar to <see cref="List{T}.Contains(T)"/> but compares the specified
		/// <paramref name="key"/> to the value of the <see cref="EntryBase.EntryId"/>
		/// property of each <see cref="Entry"/> element, rather than to the element itself.
		/// </remarks>
		public bool ContainsKey(string key)
		{
			return (IndexOfKey(key) >= 0);
		}

		/// <summary>
		/// Gets the <see cref="Entry"/> element associated with the first
		/// occurrence of the specified <see cref="EntryBase.EntryId"/> value.
		/// </summary>
		/// <param name="key">
		/// The <see cref="EntryBase.EntryId"/> value whose element to get.</param>
		/// <returns>The <see cref="Entry"/> element associated with the first
		/// occurrence of the specified <paramref name="key"/>, if found; otherwise,
		/// a null reference.
		/// </returns>
		/// <remarks>
		/// <b>GetByKey</b> compares the specified <paramref name="key"/> to the value
		/// of the <see cref="EntryBase.EntryId"/> property of each
		/// <see cref="Entry"/> element, and returns the first matching element.
		/// </remarks>
		public virtual Entry GetByKey(string key)
		{
			return this.Find(EntryCollectionFilter.DefaultFilters.HasEntryId(key));
		}

		/// <summary>
		/// Returns the zero-based index of the first occurrence of the specified
		/// <see cref="EntryBase.EntryId"/> value in the
		/// <see cref="EntryCollection"/>.
		/// </summary>
		/// <param name="key">The <see cref="EntryBase.EntryId"/>
		/// value to locate in the <see cref="EntryCollection"/>.</param>
		/// <returns>
		/// The zero-based index of the first occurrence of <paramref name="key"/>
		/// in the <see cref="EntryCollection"/>, if found; otherwise, -1.
		/// </returns>
		/// <remarks>
		/// <b>IndexOfKey</b> is similar to <see cref="List{T}.IndexOf(T)"/> but compares the specified
		/// <paramref name="key"/> to the value of the <see cref="EntryBase.EntryId"/>
		/// property of each <see cref="Entry"/> element, rather than to the element itself.
		/// </remarks>
		public virtual int IndexOfKey(string key)
		{
			return this.FindIndex(EntryCollectionFilter.DefaultFilters.HasEntryId(key));
		}

		public IEnumerable<Entry> GetPublicEntries()
		{
			foreach (Entry entry in this)
			{
				if (entry.IsPublic)
				{
					yield return entry;
				}
			}
		}
	}
}
