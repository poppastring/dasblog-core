
using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{

    #region Class EntryCollection

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
        #region Public Constructors
        #region EntryCollection()

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

        #endregion
        #region EntryCollection(Int32)

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

        #endregion
        #region EntryCollection(EntryCollection)

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

        #region Item[string]: Entry

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

        #endregion

        #endregion
        #region Public Methods

        #region Clone

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

        #endregion
        #region ContainsKey

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

        #endregion
        #region GetByKey

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

        #endregion

        #region IndexOfKey

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

        #endregion


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


        #endregion
        #endregion

        #region unused derived classes

        // The following classes are not used, remove? 

        /*

        #region Class ReadOnlyList

        [Serializable]
        private sealed class ReadOnlyList : EntryCollection
        {
            #region Private Fields

            private EntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal ReadOnlyList(EntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Entry[] InnerArray
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

            public override Entry this[int index]
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

            public override void Add(Entry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(EntryCollection collection)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(Entry[] array)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override int BinarySearch(Entry value)
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
                return new ReadOnlyList((EntryCollection)this._collection.Clone());
            }

            public override void CopyTo(Entry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(Entry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override Entry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }
                                                                                     
            public override IEnumerator<Entry> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }
                                                                                     
            public override int IndexOf(Entry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, Entry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override bool Remove(Entry value)
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

            public override void Sort(IComparer<Entry> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override Entry[] ToArray()
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
        #region Class SyncList

        [Serializable]
        private sealed class SyncList : EntryCollection
        {
            #region Private Fields

            private EntryCollection _collection;
            private object _root;

            #endregion
            #region Internal Constructors

            internal SyncList(EntryCollection collection) :
                base(Tag.Default)
            {

                this._root = collection.SyncRoot;
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Entry[] InnerArray
            {
                get { lock (this._root) return this._collection.InnerArray; }
            }

            #endregion
            #region Public Properties

            public override int Capacity
            {
                get { lock (this._root) return this._collection.Capacity; }
                set { lock (this._root) this._collection.Capacity = value; }
            }

            public override int Count
            {
                get { lock (this._root) return this._collection.Count; }
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
                get { return true; }
            }

            public override bool IsUnique
            {
                get { return this._collection.IsUnique; }
            }

            public override Entry this[int index]
            {
                get { lock (this._root) return this._collection[index]; }
                set { lock (this._root) this._collection[index] = value; }
            }

            public override object SyncRoot
            {
                get { return this._root; }
            }

            #endregion
            #region Public Methods

            public override void Add(Entry value)
            {
                lock (this._root) { this._collection.Add(value); }
            }

            public override void AddRange(EntryCollection collection)
            {
                lock (this._root) this._collection.AddRange(collection);
            }

            public override void AddRange(Entry[] array)
            {
                lock (this._root) this._collection.AddRange(array);
            }

            public override int BinarySearch(Entry value)
            {
                lock (this._root) return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                lock (this._root) this._collection.Clear();
            }

            public override object Clone()
            {
                lock (this._root)
                    return new SyncList((EntryCollection)this._collection.Clone());
            }

            public override void CopyTo(Entry[] array)
            {
                lock (this._root) this._collection.CopyTo(array);
            }

            public override void CopyTo(Entry[] array, int arrayIndex)
            {
                lock (this._root) this._collection.CopyTo(array, arrayIndex);
            }

            public override Entry GetByKey(string key)
            {
                lock (this._root) return this._collection.GetByKey(key);
            }
                                                                                       
            //public override IEnumerator<Entry> GetEnumerator()
            //{
            //    lock (this._root) return this._collection.GetEnumerator();
            //}
                                                                                       
            public override int IndexOf(Entry value)
            {
                lock (this._root) return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                lock (this._root) return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, Entry value)
            {
                lock (this._root) this._collection.Insert(index, value);
            }

            public override bool Remove(Entry value)
            {
                lock (this._root) return this._collection.Remove(value);
            }

            public override void RemoveAt(int index)
            {
                lock (this._root) this._collection.RemoveAt(index);
            }

            public override void RemoveRange(int index, int count)
            {
                lock (this._root) this._collection.RemoveRange(index, count);
            }

            public override void Reverse()
            {
                lock (this._root) this._collection.Reverse();
            }

            public override void Reverse(int index, int count)
            {
                lock (this._root) this._collection.Reverse(index, count);
            }

            public override void Sort()
            {
                lock (this._root) this._collection.Sort();
            }

            public override void Sort(IComparer<Entry> comparer)
            {
                lock (this._root) this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                lock (this._root) this._collection.Sort(index, count, comparer);
            }

            public override Entry[] ToArray()
            {
                lock (this._root) return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                lock (this._root) this._collection.TrimToSize();
            }

            #endregion
        }

        #endregion
        #region Class UniqueList

        [Serializable]

        private sealed class UniqueList : EntryCollection
        {
            #region Private Fields

            private EntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal UniqueList(EntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Entry[] InnerArray
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

            public override Entry this[int index]
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

            public override void Add(Entry value)
            {
                CheckUnique(value);
                 this._collection.Add(value);
            }

            public override void AddRange(EntryCollection collection)
            {
                foreach (Entry value in collection)
                    CheckUnique(value);

                this._collection.AddRange(collection);
            }

            public override void AddRange(Entry[] array)
            {
                foreach (Entry value in array)
                    CheckUnique(value);

                this._collection.AddRange(array);
            }

            public override int BinarySearch(Entry value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                this._collection.Clear();
            }

            public override object Clone()
            {
                return new UniqueList((EntryCollection)this._collection.Clone());
            }

            public override void CopyTo(Entry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(Entry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override Entry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }
                                                                                       
            //public override IEnumerator<Entry> GetEnumerator()
            //{
            //    return this._collection.GetEnumerator();
            //}
                                                                                       
            public override int IndexOf(Entry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, Entry value)
            {
                CheckUnique(value);
                this._collection.Insert(index, value);
            }

            public override bool Remove(Entry value)
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

            public override void Sort(IComparer<Entry> comparer)
            {
                this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer comparer)
            {
                this._collection.Sort(index, count, comparer);
            }

            public override Entry[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                this._collection.TrimToSize();
            }

            #endregion
            #region Private Methods

            private void CheckUnique(Entry value)
            {
                if (IndexOf(value) >= 0)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            private void CheckUnique(int index, Entry value)
            {
                int existing = IndexOf(value);
                if (existing >= 0 && existing != index)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            #endregion
        }

      
#endregion     */

        #endregion
    #endregion
    }
}
