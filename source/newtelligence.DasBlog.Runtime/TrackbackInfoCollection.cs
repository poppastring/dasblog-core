
using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{

    #region Class TrackbackInfoCollection

    /// <summary>
    /// Implements a strongly typed collection of <see cref="TrackbackInfo"/> elements.
    /// </summary>
    /// <remarks><para>
    /// <b>TrackbackInfoCollection</b> provides an <see cref="List{T}"/>
    /// that is strongly typed for <see cref="TrackbackInfo"/> elements.
    /// </para></remarks>

    [Serializable]
    public class TrackbackInfoCollection :
       List<TrackbackInfo>, ICloneable
    {

        #region Public Constructors
        #region TrackbackInfoCollection()

        /// <overloads>
        /// Initializes a new instance of the <see cref="TrackbackInfoCollection"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackbackInfoCollection"/> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>Please refer to <see cref="List{T}()"/> for details.</remarks>

        public TrackbackInfoCollection()
            : base()
        {
            //empty
        }

        #endregion
        #region TrackbackInfoCollection(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackbackInfoCollection"/> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new
        /// <see cref="TrackbackInfoCollection"/> is initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public TrackbackInfoCollection(int capacity)
            : base(capacity)
        {
            //empty
        }

        #endregion
        #region TrackbackInfoCollection(TrackbackInfoCollection)

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackbackInfoCollection"/> class
        /// that contains elements copied from the specified collection and
        /// that has the same initial capacity as the number of elements copied.
        /// </summary>
        /// <param name="collection">The <see cref="TrackbackInfoCollection"/>
        /// whose elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public TrackbackInfoCollection(IEnumerable<TrackbackInfo> collection)
            : base(collection)
        {
            //empty
        }

        #endregion
        #endregion
        #region Public Properties

        #region IsUnique

        /// <summary>
        /// Gets a value indicating whether the <see cref="TrackbackInfoCollection"/> 
        /// ensures that all elements are unique.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="TrackbackInfoCollection"/> ensures that all 
        /// elements are unique; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        public virtual bool IsUnique
        {
            get { return false; }
        }

        #endregion

        #endregion
        #region Public Methods

        #region Clone

        /// <summary>
        /// Creates a shallow copy of the <see cref="TrackbackInfoCollection"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="TrackbackInfoCollection"/>.</returns>
        /// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>

        public virtual object Clone()
        {
            TrackbackInfoCollection collection = new TrackbackInfoCollection(this.ToArray());

            return collection;
        }

        #endregion

        #endregion

        #region Unused classes
        /*
        #region Class ReadOnlyList

        [Serializable]
        private sealed class ReadOnlyList : TrackbackInfoCollection
        {
            #region Private Fields

            private TrackbackInfoCollection _collection;

            #endregion
            #region Internal Constructors

            internal ReadOnlyList(TrackbackInfoCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override TrackbackInfo[] InnerArray
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

            public override TrackbackInfo this[int index]
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

            public override void Add(TrackbackInfo value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(TrackbackInfoCollection collection)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(TrackbackInfo[] array)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override int BinarySearch(TrackbackInfo value)
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
                return new ReadOnlyList((TrackbackInfoCollection)this._collection.Clone());
            }

            public override void CopyTo(TrackbackInfo[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(TrackbackInfo[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<TrackbackInfo> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(TrackbackInfo value)
            {
                return this._collection.IndexOf(value);
            }

            public override void Insert(int index, TrackbackInfo value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override bool Remove(TrackbackInfo value)
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

            public override void Sort(IComparer<TrackbackInfo> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(int index, int count, IComparer<TrackbackInfo> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override TrackbackInfo[] ToArray()
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
        private sealed class SyncList : TrackbackInfoCollection
        {
            #region Private Fields

            private TrackbackInfoCollection _collection;
            private object _root;

            #endregion
            #region Internal Constructors

            internal SyncList(TrackbackInfoCollection collection) :
                base(Tag.Default)
            {

                this._root = collection.SyncRoot;
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override TrackbackInfo[] InnerArray
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

            public override TrackbackInfo this[int index]
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

            public override void Add(TrackbackInfo value)
            {
                lock (this._root) { this._collection.Add(value); }
            }

            public override void AddRange(TrackbackInfoCollection collection)
            {
                lock (this._root) this._collection.AddRange(collection);
            }

            public override void AddRange(TrackbackInfo[] array)
            {
                lock (this._root) this._collection.AddRange(array);
            }

            public override int BinarySearch(TrackbackInfo value)
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
                    return new SyncList((TrackbackInfoCollection)this._collection.Clone());
            }

            public override void CopyTo(TrackbackInfo[] array)
            {
                lock (this._root) this._collection.CopyTo(array);
            }

            public override void CopyTo(TrackbackInfo[] array, int arrayIndex)
            {
                lock (this._root) this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<TrackbackInfo> GetEnumerator()
            {
                lock (this._root) return this._collection.GetEnumerator();
            }

            public override int IndexOf(TrackbackInfo value)
            {
                lock (this._root) return this._collection.IndexOf(value);
            }

            public override void Insert(int index, TrackbackInfo value)
            {
                lock (this._root) this._collection.Insert(index, value);
            }

            public override bool Remove(TrackbackInfo value)
            {
                lock (this._root) { return this._collection.Remove(value); }
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

            public override void Sort(IComparer<TrackbackInfo> comparer)
            {
                lock (this._root) this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<TrackbackInfo> comparer)
            {
                lock (this._root) this._collection.Sort(index, count, comparer);
            }

            public override TrackbackInfo[] ToArray()
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
        private sealed class UniqueList : TrackbackInfoCollection
        {
            #region Private Fields

            private TrackbackInfoCollection _collection;

            #endregion
            #region Internal Constructors

            internal UniqueList(TrackbackInfoCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override TrackbackInfo[] InnerArray
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

            public override TrackbackInfo this[int index]
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

            public override void Add(TrackbackInfo value)
            {
                CheckUnique(value);
                this._collection.Add(value);
            }

            public override void AddRange(TrackbackInfoCollection collection)
            {
                foreach (TrackbackInfo value in collection)
                    CheckUnique(value);

                this._collection.AddRange(collection);
            }

            public override void AddRange(TrackbackInfo[] array)
            {
                foreach (TrackbackInfo value in array)
                    CheckUnique(value);

                this._collection.AddRange(array);
            }

            public override int BinarySearch(TrackbackInfo value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                this._collection.Clear();
            }

            public override object Clone()
            {
                return new UniqueList((TrackbackInfoCollection)this._collection.Clone());
            }

            public override void CopyTo(TrackbackInfo[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(TrackbackInfo[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<TrackbackInfo> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(TrackbackInfo value)
            {
                return this._collection.IndexOf(value);
            }

            public override void Insert(int index, TrackbackInfo value)
            {
                CheckUnique(value);
                this._collection.Insert(index, value);
            }

            public override bool Remove(TrackbackInfo value)
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

            public override void Sort(IComparer<TrackbackInfo> comparer)
            {
                this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<TrackbackInfo> comparer)
            {
                this._collection.Sort(index, count, comparer);
            }

            public override TrackbackInfo[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                this._collection.TrimToSize();
            }

            #endregion
            #region Private Methods

            private void CheckUnique(TrackbackInfo value)
            {
                if (IndexOf(value) >= 0)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            private void CheckUnique(int index, TrackbackInfo value)
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
    }

    #endregion
}
