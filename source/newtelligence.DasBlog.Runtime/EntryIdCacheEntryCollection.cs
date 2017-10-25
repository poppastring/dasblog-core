
using System;
using System.Collections;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{

    #region Class EntryIdCacheEntryCollection

    /// <summary>
    /// Implements a strongly typed collection of <see cref="EntryIdCacheEntry"/> elements.
    /// </summary>
    /// <remarks><para>
    /// <b>EntryIdCacheEntryCollection</b> provides an <see cref="List{T}"/>
    /// that is strongly typed for <see cref="EntryIdCacheEntry"/> elements.
    /// </para><para>
    /// The <see cref="EntryIdCacheEntry.EntryId"/> property of the
    /// <see cref="EntryIdCacheEntry"/> class can be used as a key
    /// to locate elements in the <b>EntryIdCacheEntryCollection</b>.
    /// </para><para>
    /// The collection may contain multiple identical keys. All key access methods 
    /// return the first occurrence of the specified key, if found. Access by key 
    /// is an O(<em>N</em>) operation, where <em>N</em> is the current value of the 
    /// <see cref="SynchronisedList{T}.Count"/> property.
    /// </para></remarks>

    [Serializable]
    public class EntryIdCacheEntryCollection :
        SynchronisedList<EntryIdCacheEntry>, ICloneable
    {

        #region Public Constructors
        #region EntryIdCacheEntryCollection()

        /// <overloads>
        /// Initializes a new instance of the <see cref="EntryIdCacheEntryCollection"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="EntryIdCacheEntryCollection"/> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>Please refer to <see cref="SynchronisedList{T}()"/> for details.</remarks>

        public EntryIdCacheEntryCollection()
            : base()
        {
            // empty
        }

        #endregion
        #region EntryIdCacheEntryCollection(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryIdCacheEntryCollection"/> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new
        /// <see cref="EntryIdCacheEntryCollection"/> is initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>Please refer to <see cref="SynchronisedList{T}(Int32)"/> for details.</remarks>

        public EntryIdCacheEntryCollection(int capacity)
            : base(capacity)
        {
            // empty
        }

        #endregion
        #region EntryIdCacheEntryCollection(EntryIdCacheEntryCollection)

        /// <summary>
        /// Initializes a new instance of the <see cref="EntryIdCacheEntryCollection"/> class
        /// that contains elements copied from the specified collection and
        /// that has the same initial capacity as the number of elements copied.
        /// </summary>
        /// <param name="collection">The <see cref="EntryIdCacheEntryCollection"/>
        /// whose elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>Please refer to <see cref="SynchronisedList{T}(IEnumerable{T})"/> for details.</remarks>

        public EntryIdCacheEntryCollection(IEnumerable<EntryIdCacheEntry> collection)
            : base(collection)
        {
            //empty
        }

        #endregion
        #endregion
        #region Public Properties

        #region IsUnique

        /// <summary>
        /// Gets a value indicating whether the <see cref="EntryIdCacheEntryCollection"/> 
        /// ensures that all elements are unique.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="EntryIdCacheEntryCollection"/> ensures that all 
        /// elements are unique; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public virtual bool IsUnique
        {
            get { return false; }
        }

        #endregion
        #region Item[string]: EntryIdCacheEntry

        /// <overloads>
        /// Gets or sets a specific <see cref="EntryIdCacheEntry"/> element.
        /// </overloads>
        /// <summary>
        /// Gets the <see cref="EntryIdCacheEntry"/> element associated with the first
        /// occurrence of the specified <see cref="EntryIdCacheEntry.EntryId"/> value.
        /// </summary>
        /// <param name="key">
        /// The <see cref="EntryIdCacheEntry.EntryId"/> value whose element to get.</param>
        /// <value>The <see cref="EntryIdCacheEntry"/> element associated with the first
        /// occurrence of the specified <paramref name="key"/>, if found; otherwise,
        /// a null reference.
        /// </value>
        /// <remarks>
        /// This indexer has the same effect as the <see cref="GetByKey"/> method.
        /// </remarks>

        public EntryIdCacheEntry this[string key]
        {
            get { return GetByKey(key); }
        }
        #endregion
        #endregion
        #region Public Methods

        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="EntryIdCacheEntryCollection"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="EntryIdCacheEntryCollection"/>.</returns>
        /// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>

        public virtual object Clone()
        {
            EntryIdCacheEntryCollection collection = new EntryIdCacheEntryCollection(this.ToArray());

            return collection;
        }

        #endregion

        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="EntryIdCacheEntryCollection"/> contains
        /// the specified <see cref="EntryIdCacheEntry.EntryId"/> value.
        /// </summary>
        /// <param name="key">The <see cref="EntryIdCacheEntry.EntryId"/>
        /// value to locate in the <see cref="EntryIdCacheEntryCollection"/>.</param>
        /// <returns><c>true</c> if <paramref name="key"/> is found in the
        /// <see cref="EntryIdCacheEntryCollection"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsKey</b> is similar to <see cref="List{T}.Contains(T)"/> but compares the specified
        /// <paramref name="key"/> to the value of the <see cref="EntryIdCacheEntry.EntryId"/> 
        /// property of each <see cref="EntryIdCacheEntry"/> element, rather than to the element itself.
        /// </remarks>

        public bool ContainsKey(string key)
        {
            return (IndexOfKey(key) >= 0);
        }

        #endregion

        #region GetByKey

        /// <summary>
        /// Gets the <see cref="EntryIdCacheEntry"/> element associated with the first
        /// occurrence of the specified <see cref="EntryIdCacheEntry.EntryId"/> value.
        /// </summary>
        /// <param name="key">
        /// The <see cref="EntryIdCacheEntry.EntryId"/> value whose element to get.</param>
        /// <returns>The <see cref="EntryIdCacheEntry"/> element associated with the first
        /// occurrence of the specified <paramref name="key"/>, if found; otherwise,
        /// a null reference.
        /// </returns>
        /// <remarks>
        /// <b>GetByKey</b> compares the specified <paramref name="key"/> to the value 
        /// of the <see cref="EntryIdCacheEntry.EntryId"/> property of each 
        /// <see cref="EntryIdCacheEntry"/> element, and returns the first matching element.
        /// </remarks>

        public virtual EntryIdCacheEntry GetByKey(string key)
        {
            return this.Find(delegate(EntryIdCacheEntry entry)
            {
                return string.Compare(entry.EntryId, key, StringComparison.InvariantCultureIgnoreCase) == 0;
            });
        }

        #endregion

        #region IndexOfKey

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified
        /// <see cref="EntryIdCacheEntry.EntryId"/> value in the 
        /// <see cref="EntryIdCacheEntryCollection"/>.
        /// </summary>
        /// <param name="key">The <see cref="EntryIdCacheEntry.EntryId"/>
        /// value to locate in the <see cref="EntryIdCacheEntryCollection"/>.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="key"/>
        /// in the <see cref="EntryIdCacheEntryCollection"/>, if found; otherwise, -1.
        /// </returns>
        /// <remarks>
        /// <b>IndexOfKey</b> is similar to <see cref="List{T}.IndexOf(T)"/> but compares the specified
        /// <paramref name="key"/> to the value of the <see cref="EntryIdCacheEntry.EntryId"/> 
        /// property of each <see cref="EntryIdCacheEntry"/> element, rather than to the element itself.
        /// </remarks>

        public virtual int IndexOfKey(string key)
        {

            return this.FindIndex(delegate(EntryIdCacheEntry entry)
            {
                return string.Compare(entry.EntryId, key, StringComparison.InvariantCultureIgnoreCase) == 0;
            });

        }

        #endregion


        #endregion
/*
        #region Class SyncList

        [Serializable]
        // TODO: make sure this class is thread safe
        private sealed class SyncList : EntryIdCacheEntryCollection
        {
            #region Private Fields

            private EntryIdCacheEntryCollection _collection;
            private object _root;

            #endregion
            #region Internal Constructors

            internal SyncList(EntryIdCacheEntryCollection collection)
                : base()
            {

                this._root = ((ICollection)collection).SyncRoot;
                this._collection = collection;
            }

            #endregion
            #region Public Properties

            public bool IsSynchronized
            {
                get { return true; }
            }


            #endregion

            #region Public methods
            public override object Clone()
            {
                lock (this._root)
                    return new SyncList((EntryIdCacheEntryCollection)this._collection.Clone());
            }

            public override EntryIdCacheEntry GetByKey(string key)
            {
                lock (this._root) return this._collection.GetByKey(key);
            }

            public override int IndexOfKey(string key)
            {
                lock (this._root) return this._collection.IndexOfKey(key);
            }

            #endregion
        }

        #endregion

        #region Unused classes
        /*
        #region Class ReadOnlyList

        [Serializable]
        private sealed class ReadOnlyList : EntryIdCacheEntryCollection
        {
            #region Private Fields

            private EntryIdCacheEntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal ReadOnlyList(EntryIdCacheEntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override EntryIdCacheEntry[] InnerArray
            {
                get { return this._collection.InnerArray; }
            }

            #endregion
            #region Public Properties

            public override int Capacity
            {
                get { return this._collection.Capacity; }
                set
                {
                    throw new NotSupportedException(
                          "Read-only collections cannot be modified.");
                }
            }

            public override int Count
            {
                get { return this._collection.Count; }
            }

            public override bool IsFixedSize
            {
                get { return true; }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override bool IsSynchronized
            {
                get { return this._collection.IsSynchronized; }
            }

            public override bool IsUnique
            {
                get { return this._collection.IsUnique; }
            }

            public override EntryIdCacheEntry this[int index]
            {
                get { return this._collection[index]; }
                set
                {
                    throw new NotSupportedException(
                          "Read-only collections cannot be modified.");
                }
            }

            public override object SyncRoot
            {
                get { return this._collection.SyncRoot; }
            }

            #endregion
            #region Public Methods

            public override void Add(EntryIdCacheEntry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(EntryIdCacheEntryCollection collection)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(EntryIdCacheEntry[] array)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override int BinarySearch(EntryIdCacheEntry value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override object Clone()
            {
                return new ReadOnlyList((EntryIdCacheEntryCollection)this._collection.Clone());
            }

            public override void CopyTo(EntryIdCacheEntry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(EntryIdCacheEntry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override EntryIdCacheEntry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }

            public override IEnumerator<EntryIdCacheEntry> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(EntryIdCacheEntry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, EntryIdCacheEntry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override bool Remove(EntryIdCacheEntry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void RemoveAt(int index)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void RemoveRange(int index, int count)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Reverse()
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Reverse(int index, int count)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort()
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(IComparer<EntryIdCacheEntry> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(int index, int count, IComparer<EntryIdCacheEntry> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override EntryIdCacheEntry[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            #endregion
        }

        #endregion
        #region Class UniqueList

        [Serializable]
        private sealed class UniqueList : EntryIdCacheEntryCollection
        {
            #region Private Fields

            private EntryIdCacheEntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal UniqueList(EntryIdCacheEntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override EntryIdCacheEntry[] InnerArray
            {
                get { return this._collection.InnerArray; }
            }

            #endregion
            #region Public Properties

            public override int Capacity
            {
                get { return this._collection.Capacity; }
                set { this._collection.Capacity = value; }
            }

            public override int Count
            {
                get { return this._collection.Count; }
            }

            public override bool IsFixedSize
            {
                get { return this._collection.IsFixedSize; }
            }

            public override bool IsReadOnly
            {
                get { return this._collection.IsReadOnly; }
            }

            public override bool IsSynchronized
            {
                get { return this._collection.IsSynchronized; }
            }

            public override bool IsUnique
            {
                get { return true; }
            }

            public override EntryIdCacheEntry this[int index]
            {
                get { return this._collection[index]; }
                set
                {
                    CheckUnique(index, value);
                    this._collection[index] = value;
                }
            }

            public override object SyncRoot
            {
                get { return this._collection.SyncRoot; }
            }

            #endregion
            #region Public Methods

            public override void Add(EntryIdCacheEntry value)
            {
                CheckUnique(value);
                this._collection.Add(value);
            }

            public override void AddRange(EntryIdCacheEntryCollection collection)
            {
                foreach (EntryIdCacheEntry value in collection)
                    CheckUnique(value);

                this._collection.AddRange(collection);
            }

            public override void AddRange(EntryIdCacheEntry[] array)
            {
                foreach (EntryIdCacheEntry value in array)
                    CheckUnique(value);

                this._collection.AddRange(array);
            }

            public override int BinarySearch(EntryIdCacheEntry value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                this._collection.Clear();
            }

            public override object Clone()
            {
                return new UniqueList((EntryIdCacheEntryCollection)this._collection.Clone());
            }

            public override void CopyTo(EntryIdCacheEntry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(EntryIdCacheEntry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override EntryIdCacheEntry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }

            public override IEnumerator<EntryIdCacheEntry> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(EntryIdCacheEntry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, EntryIdCacheEntry value)
            {
                CheckUnique(value);
                this._collection.Insert(index, value);
            }

            public override bool Remove(EntryIdCacheEntry value)
            {
                return this._collection.Remove(value);
            }

            public override void RemoveAt(int index)
            {
                this._collection.RemoveAt(index);
            }

            public override void RemoveRange(int index, int count)
            {
                this._collection.RemoveRange(index, count);
            }

            public override void Reverse()
            {
                this._collection.Reverse();
            }

            public override void Reverse(int index, int count)
            {
                this._collection.Reverse(index, count);
            }

            public override void Sort()
            {
                this._collection.Sort();
            }

            public override void Sort(IComparer<EntryIdCacheEntry> comparer)
            {
                this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<EntryIdCacheEntry> comparer)
            {
                this._collection.Sort(index, count, comparer);
            }

            public override EntryIdCacheEntry[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                this._collection.TrimToSize();
            }

            #endregion
            #region Private Methods

            private void CheckUnique(EntryIdCacheEntry value)
            {
                if (IndexOf(value) >= 0)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            private void CheckUnique(int index, EntryIdCacheEntry value)
            {
                int existing = IndexOf(value);
                if (existing >= 0 && existing != index)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            #endregion
        }
           
        #endregion   */
    }

        #endregion
}
