
using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{

    #region Class CategoryCacheEntryCollection

    /// <summary>
    /// Implements a strongly typed collection of <see cref="CategoryCacheEntry"/> elements.
    /// </summary>
    /// <remarks><para>
    /// <b>CategoryCacheEntryCollection</b> provides an <see cref="List{T}"/>
    /// that is strongly typed for <see cref="CategoryCacheEntry"/> elements.
    /// </para><para>
    /// The <see cref="CategoryCacheEntry.Name"/> property of the
    /// <see cref="CategoryCacheEntry"/> class can be used as a key
    /// to locate elements in the <b>CategoryCacheEntryCollection</b>.
    /// </para><para>
    /// The collection may contain multiple identical keys. All key access methods 
    /// return the first occurrence of the specified key, if found. Access by key 
    /// is an O(<em>N</em>) operation, where <em>N</em> is the current value of the 
    /// <see cref="SynchronisedList{T}.Count"/> property.
    /// </para></remarks>

    [Serializable]
    public class CategoryCacheEntryCollection : SynchronisedList<CategoryCacheEntry>, ICloneable
    {

        #region Public Constructors
        #region CategoryCacheEntryCollection()

        /// <overloads>
        /// Initializes a new instance of the <see cref="CategoryCacheEntryCollection"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCacheEntryCollection"/> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>Please refer to <see cref="List{T}()"/> for details.</remarks>

        public CategoryCacheEntryCollection()
            : base()
        {
            // empty
        }

        #endregion
        #region CategoryCacheEntryCollection(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCacheEntryCollection"/> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new
        /// <see cref="CategoryCacheEntryCollection"/> is initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>
        public CategoryCacheEntryCollection(int capacity)
            : base(capacity)
        {
            // empty
        }

        #endregion
        #region CategoryCacheEntryCollection(CategoryCacheEntryCollection)

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryCacheEntryCollection"/> class
        /// that contains elements copied from the specified collection and
        /// that has the same initial capacity as the number of elements copied.
        /// </summary>
        /// <param name="collection">The <see cref="CategoryCacheEntryCollection"/>
        /// whose elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public CategoryCacheEntryCollection(IEnumerable<CategoryCacheEntry> collection)
            : base(collection)
        {
            // empty
        }

        #endregion

        #endregion

        #region Public Properties

        #region IsUnique

        /// <summary>
        /// Gets a value indicating whether the <see cref="CategoryCacheEntryCollection"/> 
        /// ensures that all elements are unique.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="CategoryCacheEntryCollection"/> ensures that all 
        /// elements are unique; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public virtual bool IsUnique
        {
            get { return false; }
        }

        #endregion
        #region Item[string]: CategoryCacheEntry

        /// <overloads>
        /// Gets or sets a specific <see cref="CategoryCacheEntry"/> element.
        /// </overloads>
        /// <summary>
        /// Gets the <see cref="CategoryCacheEntry"/> element associated with the first
        /// occurrence of the specified <see cref="CategoryCacheEntry.Name"/> value.
        /// </summary>
        /// <param name="key">
        /// The <see cref="CategoryCacheEntry.Name"/> value whose element to get.</param>
        /// <value>The <see cref="CategoryCacheEntry"/> element associated with the first
        /// occurrence of the specified <paramref name="key"/>, if found; otherwise,
        /// a null reference.
        /// </value>
        /// <remarks>
        /// This indexer has the same effect as the <see cref="GetByKey"/> method.
        /// </remarks>

        public CategoryCacheEntry this[string key]
        {
            get { return GetByKey(key); }
        }

        #endregion
        #endregion
        #region Public Methods
        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="CategoryCacheEntryCollection"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="CategoryCacheEntryCollection"/>.</returns>
        /// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>

        public virtual object Clone()
        {

            // the call to ToArray is synchronised, so we don't need any locking here

            CategoryCacheEntryCollection collection = new CategoryCacheEntryCollection(this.ToArray());

            return collection;
        }

        #endregion

        #region ContainsKey

        /// <summary>
        /// Determines whether the <see cref="CategoryCacheEntryCollection"/> contains
        /// the specified <see cref="CategoryCacheEntry.Name"/> value.
        /// </summary>
        /// <param name="key">The <see cref="CategoryCacheEntry.Name"/>
        /// value to locate in the <see cref="CategoryCacheEntryCollection"/>.</param>
        /// <returns><c>true</c> if <paramref name="key"/> is found in the
        /// <see cref="CategoryCacheEntryCollection"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <b>ContainsKey</b> is similar to <see cref="List{T}.Contains(T)"/> but compares the specified
        /// <paramref name="key"/> to the value of the <see cref="CategoryCacheEntry.Name"/> 
        /// property of each <see cref="CategoryCacheEntry"/> element, rather than to the element itself.
        /// </remarks>

        public bool ContainsKey(string key)
        {
            return (IndexOfKey(key) >= 0);
        }

        #endregion
        #region GetByKey

        /// <summary>
        /// Gets the <see cref="CategoryCacheEntry"/> element associated with the first
        /// occurrence of the specified <see cref="CategoryCacheEntry.Name"/> value.
        /// </summary>
        /// <param name="key">
        /// The <see cref="CategoryCacheEntry.Name"/> value whose element to get.</param>
        /// <returns>The <see cref="CategoryCacheEntry"/> element associated with the first
        /// occurrence of the specified <paramref name="key"/>, if found; otherwise,
        /// a null reference.
        /// </returns>
        /// <remarks>
        /// <b>GetByKey</b> compares the specified <paramref name="key"/> to the value 
        /// of the <see cref="CategoryCacheEntry.Name"/> property of each 
        /// <see cref="CategoryCacheEntry"/> element, 
        /// using the <see cref="StringComparison.InvariantCultureIgnoreCase" />, 
        /// and returns the first matching element.
        /// </remarks>

        public virtual CategoryCacheEntry GetByKey(string key)
        {
            return this.Find(delegate(CategoryCacheEntry entry)
            {
                return String.Compare(entry.Name, key, StringComparison.InvariantCultureIgnoreCase) == 0;
            });
        }

        #endregion
        #region IndexOfKey

        /// <summary>
        /// Returns the zero-based index of the first occurrence of the specified
        /// <see cref="CategoryCacheEntry.Name"/> value in the 
        /// <see cref="CategoryCacheEntryCollection"/>.
        /// </summary>
        /// <param name="key">The <see cref="CategoryCacheEntry.Name"/>
        /// value to locate in the <see cref="CategoryCacheEntryCollection"/>.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of <paramref name="key"/>
        /// in the <see cref="CategoryCacheEntryCollection"/>, if found; otherwise, -1.
        /// </returns>
        /// <remarks>
        /// <b>IndexOfKey</b> is similar to <see cref="List{T}.IndexOf(T)"/> but compares the specified
        /// <paramref name="key"/> to the value of the <see cref="CategoryCacheEntry.Name"/> 
        /// property of each <see cref="CategoryCacheEntry"/> element, rather than to the element itself.
        /// </remarks>

        public virtual int IndexOfKey(string key)
        {

            return this.FindIndex(delegate(CategoryCacheEntry entry)
            {
                return String.Compare(entry.Name, key, StringComparison.InvariantCultureIgnoreCase) == 0;
            });
        }

        #endregion



        public virtual bool HasChildrenInCollection(CategoryCacheEntry entry)
        {

            // finds paths deriving from this path.
            // e.g. for a/b/  this method finds a/b/* but not /a/e/*

            // thread safe access to the enumerator so we should be ok.

            foreach (CategoryCacheEntry seek in this)
            {

                string[] entryPath = entry.CategoryPath;
                string[] seekPath = seek.CategoryPath;
                int entryPathLength = entryPath.Length;
                int seekPathLength = seekPath.Length;

                if (seekPathLength == entryPathLength + 1)
                {
                    bool found = true;

                    for (int j = 0; j < entryPathLength; j++)
                    {
                        if (string.Compare(entryPath[j].Trim(), seekPath[j].Trim(), StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        /* 

        #region Class SyncList

        /// <summary>
        /// Wrapper around a <see cref="CategoryCacheEntryCollection" /> which provides
        /// thread save access.
        /// </summary>
        // WARNING!! This implementation is NOT threadsafe, by using the passed in collection as 
        // the store the user can still manipulate the list without going through the wrapper.
        [Serializable]  
        private sealed class SyncList : CategoryCacheEntryCollection
        {
            #region Private Fields

            private CategoryCacheEntryCollection collection;
            private object root;

            #endregion
            #region Internal Constructors

            internal SyncList(CategoryCacheEntryCollection collection) :
                base()
            {

                this.root = ((ICollection)collection).SyncRoot;
                this.collection = collection;
            }

            #endregion

            public override bool IsSynchronized
            {
                get { return true; }
            }

            public override bool IsUnique
            {
                get { return this.collection.IsUnique; }
            }

            public override object Clone()
            {
                lock (this.root)
                    return new SyncList((CategoryCacheEntryCollection)this.collection.Clone());
            }

            public override CategoryCacheEntry GetByKey(string key)
            {
                lock (this.root) return this.collection.GetByKey(key);
            }


            public override int IndexOfKey(string key)
            {
                lock (this.root) return this.collection.IndexOfKey(key);
            }
        }

        #endregion


        #region unused derived classes
        /*
        #region Class ReadOnlyList

        [Serializable]
        private sealed class ReadOnlyList : CategoryCacheEntryCollection
        {
            #region Private Fields

            private CategoryCacheEntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal ReadOnlyList(CategoryCacheEntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override CategoryCacheEntry[] InnerArray
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

            public override CategoryCacheEntry this[int index]
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

            public override void Add(CategoryCacheEntry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(CategoryCacheEntryCollection collection)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(CategoryCacheEntry[] array)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override int BinarySearch(CategoryCacheEntry value)
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
                return new ReadOnlyList((CategoryCacheEntryCollection)this._collection.Clone());
            }

            public override void CopyTo(CategoryCacheEntry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(CategoryCacheEntry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override CategoryCacheEntry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }

            public override IEnumerator<CategoryCacheEntry> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(CategoryCacheEntry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, CategoryCacheEntry value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override bool Remove(CategoryCacheEntry value)
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

            public override void Sort(IComparer<CategoryCacheEntry> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(int index, int count, IComparer<CategoryCacheEntry> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override CategoryCacheEntry[] ToArray()
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
        private sealed class UniqueList : CategoryCacheEntryCollection
        {
            #region Private Fields

            private CategoryCacheEntryCollection _collection;

            #endregion
            #region Internal Constructors

            internal UniqueList(CategoryCacheEntryCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override CategoryCacheEntry[] InnerArray
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

            public override CategoryCacheEntry this[int index]
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

            public override void Add(CategoryCacheEntry value)
            {
                CheckUnique(value);
                this._collection.Add(value);
            }

            public override void AddRange(CategoryCacheEntryCollection collection)
            {
                foreach (CategoryCacheEntry value in collection)
                    CheckUnique(value);

                this._collection.AddRange(collection);
            }

            public override void AddRange(CategoryCacheEntry[] array)
            {
                foreach (CategoryCacheEntry value in array)
                    CheckUnique(value);

                this._collection.AddRange(array);
            }

            public override int BinarySearch(CategoryCacheEntry value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                this._collection.Clear();
            }

            public override object Clone()
            {
                return new UniqueList((CategoryCacheEntryCollection)this._collection.Clone());
            }

            public override void CopyTo(CategoryCacheEntry[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(CategoryCacheEntry[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override CategoryCacheEntry GetByKey(string key)
            {
                return this._collection.GetByKey(key);
            }

            public override IEnumerator<CategoryCacheEntry> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(CategoryCacheEntry value)
            {
                return this._collection.IndexOf(value);
            }

            public override int IndexOfKey(string key)
            {
                return this._collection.IndexOfKey(key);
            }

            public override void Insert(int index, CategoryCacheEntry value)
            {
                CheckUnique(value);
                this._collection.Insert(index, value);
            }

            public override bool Remove(CategoryCacheEntry value)
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

            public override void Sort(IComparer<CategoryCacheEntry> comparer)
            {
                this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<CategoryCacheEntry> comparer)
            {
                this._collection.Sort(index, count, comparer);
            }

            public override CategoryCacheEntry[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                this._collection.TrimToSize();
            }

            #endregion
            #region Private Methods

            private void CheckUnique(CategoryCacheEntry value)
            {
                if (IndexOf(value) >= 0)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            private void CheckUnique(int index, CategoryCacheEntry value)
            {
                int existing = IndexOf(value);
                if (existing >= 0 && existing != index)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            #endregion
        }

        #endregion 
        #endregion  */

    }

    #endregion
}
