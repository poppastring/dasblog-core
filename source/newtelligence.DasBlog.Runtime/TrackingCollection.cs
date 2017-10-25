
using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{

    #region Class TrackingCollection

    /// <summary>
    /// Implements a strongly typed collection of <see cref="Tracking"/> elements.
    /// </summary>
    /// <remarks><para>
    /// <b>TrackingCollection</b> provides an <see cref="List{T}"/>
    /// that is strongly typed for <see cref="Tracking"/> elements.
    /// </para></remarks>

    [Serializable]
    public class TrackingCollection :
        List<Tracking>, ICloneable
    {
        #region Public Constructors
        #region TrackingCollection()

        /// <overloads>
        /// Initializes a new instance of the <see cref="TrackingCollection"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingCollection"/> class
        /// that is empty and has the default initial capacity.
        /// </summary>
        /// <remarks>Please refer to <see cref="List{T}()"/> for details.</remarks>

        public TrackingCollection()
            : base()
        {
            //empty
        }

        #endregion
        #region TrackingCollection(Int32)

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingCollection"/> class
        /// that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new
        /// <see cref="TrackingCollection"/> is initially capable of storing.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/> is less than zero.</exception>
        /// <remarks>Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

        public TrackingCollection(int capacity)
            : base(capacity)
        {
            //empty
        }

        #endregion
        #region TrackingCollection(TrackingCollection)

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackingCollection"/> class
        /// that contains elements copied from the specified collection and
        /// that has the same initial capacity as the number of elements copied.
        /// </summary>
        /// <param name="collection">The <see cref="TrackingCollection"/>
        /// whose elements are copied to the new collection.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

        public TrackingCollection(IEnumerable<Tracking> collection)
            : base(collection)
        {
            //empty
        }

        #endregion
        #endregion

        #region Public Properties
        #region IsUnique

        /// <summary>
        /// Gets a value indicating whether the <see cref="TrackingCollection"/> 
        /// ensures that all elements are unique.
        /// </summary>
        /// <value>
        /// <c>true</c> if the <see cref="TrackingCollection"/> ensures that all 
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
        /// Creates a shallow copy of the <see cref="TrackingCollection"/>.
        /// </summary>
        /// <returns>A shallow copy of the <see cref="TrackingCollection"/>.</returns>
        /// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>

        public virtual object Clone()
        {
            TrackingCollection collection = new TrackingCollection(this.ToArray());

            return collection;
        }

        #endregion

        #endregion

        #region Unused classes
        /*
        #region Class ReadOnlyList

        [Serializable]
        private sealed class ReadOnlyList : TrackingCollection
        {
            #region Private Fields

            private TrackingCollection _collection;

            #endregion
            #region Internal Constructors

            internal ReadOnlyList(TrackingCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Tracking[] InnerArray
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

            public override Tracking this[int index]
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

            public override void Add(Tracking value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(TrackingCollection collection)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void AddRange(Tracking[] array)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override int BinarySearch(Tracking value)
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
                return new ReadOnlyList((TrackingCollection)this._collection.Clone());
            }

            public override void CopyTo(Tracking[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(Tracking[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<Tracking> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(Tracking value)
            {
                return this._collection.IndexOf(value);
            }

            public override void Insert(int index, Tracking value)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override bool Remove(Tracking value)
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

            public override void Sort(IComparer<Tracking> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override void Sort(int index, int count, IComparer<Tracking> comparer)
            {
                throw new NotSupportedException(
                    "Read-only collections cannot be modified.");
            }

            public override Tracking[] ToArray()
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
        private sealed class SyncList : TrackingCollection
        {
            #region Private Fields

            private TrackingCollection _collection;
            private object _root;

            #endregion
            #region Internal Constructors

            internal SyncList(TrackingCollection collection) :
                base(Tag.Default)
            {

                this._root = collection.SyncRoot;
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Tracking[] InnerArray
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

            public override Tracking this[int index]
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

            public override void Add(Tracking value)
            {
                lock (this._root) { this._collection.Add(value); }
            }

            public override void AddRange(TrackingCollection collection)
            {
                lock (this._root) this._collection.AddRange(collection);
            }

            public override void AddRange(Tracking[] array)
            {
                lock (this._root) this._collection.AddRange(array);
            }

            public override int BinarySearch(Tracking value)
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
                    return new SyncList((TrackingCollection)this._collection.Clone());
            }

            public override void CopyTo(Tracking[] array)
            {
                lock (this._root) this._collection.CopyTo(array);
            }

            public override void CopyTo(Tracking[] array, int arrayIndex)
            {
                lock (this._root) this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<Tracking> GetEnumerator()
            {
                lock (this._root) return this._collection.GetEnumerator();
            }

            public override int IndexOf(Tracking value)
            {
                lock (this._root) return this._collection.IndexOf(value);
            }

            public override void Insert(int index, Tracking value)
            {
                lock (this._root) this._collection.Insert(index, value);
            }

            public override bool Remove(Tracking value)
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

            public override void Sort(IComparer<Tracking> comparer)
            {
                lock (this._root) this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<Tracking> comparer)
            {
                lock (this._root) this._collection.Sort(index, count, comparer);
            }

            public override Tracking[] ToArray()
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
        private sealed class UniqueList : TrackingCollection
        {
            #region Private Fields

            private TrackingCollection _collection;

            #endregion
            #region Internal Constructors

            internal UniqueList(TrackingCollection collection) :
                base(Tag.Default)
            {
                this._collection = collection;
            }

            #endregion
            #region Protected Properties

            protected override Tracking[] InnerArray
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

            public override Tracking this[int index]
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

            public override void Add(Tracking value)
            {
                CheckUnique(value);
                this._collection.Add(value);
            }

            public override void AddRange(TrackingCollection collection)
            {
                foreach (Tracking value in collection)
                    CheckUnique(value);

                this._collection.AddRange(collection);
            }

            public override void AddRange(Tracking[] array)
            {
                foreach (Tracking value in array)
                    CheckUnique(value);

                this._collection.AddRange(array);
            }

            public override int BinarySearch(Tracking value)
            {
                return this._collection.BinarySearch(value);
            }

            public override void Clear()
            {
                this._collection.Clear();
            }

            public override object Clone()
            {
                return new UniqueList((TrackingCollection)this._collection.Clone());
            }

            public override void CopyTo(Tracking[] array)
            {
                this._collection.CopyTo(array);
            }

            public override void CopyTo(Tracking[] array, int arrayIndex)
            {
                this._collection.CopyTo(array, arrayIndex);
            }

            public override IEnumerator<Tracking> GetEnumerator()
            {
                return this._collection.GetEnumerator();
            }

            public override int IndexOf(Tracking value)
            {
                return this._collection.IndexOf(value);
            }

            public override void Insert(int index, Tracking value)
            {
                CheckUnique(value);
                this._collection.Insert(index, value);
            }

            public override bool Remove(Tracking value)
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

            public override void Sort(IComparer<Tracking> comparer)
            {
                this._collection.Sort(comparer);
            }

            public override void Sort(int index, int count, IComparer<Tracking> comparer)
            {
                this._collection.Sort(index, count, comparer);
            }

            public override Tracking[] ToArray()
            {
                return this._collection.ToArray();
            }

            public override void TrimToSize()
            {
                this._collection.TrimToSize();
            }

            #endregion
            #region Private Methods

            private void CheckUnique(Tracking value)
            {
                if (IndexOf(value) >= 0)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            private void CheckUnique(int index, Tracking value)
            {
                int existing = IndexOf(value);
                if (existing >= 0 && existing != index)
                    throw new NotSupportedException(
                        "Unique collections cannot contain duplicate elements.");
            }

            #endregion
        }

        #endregion */
        #endregion
    }

    #endregion
}
