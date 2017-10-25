
using System;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime 
{
	#region Class CommentCollection

	/// <summary>
	/// Implements a strongly typed collection of <see cref="Comment"/> elements.
	/// </summary>
	/// <remarks><para>
	/// <b>CommentCollection</b> provides an <see cref="List{T}"/>
	/// that is strongly typed for <see cref="Comment"/> elements.
	/// </para></remarks>

	[Serializable]
	public class CommentCollection:	List<Comment>, ICloneable 
	{
		#region Public Constructors
		#region CommentCollection()

		/// <overloads>
		/// Initializes a new instance of the <see cref="CommentCollection"/> class.
		/// </overloads>
		/// <summary>
		/// Initializes a new instance of the <see cref="CommentCollection"/> class
		/// that is empty and has the default initial capacity.
		/// </summary>
		/// <remarks>Please refer to <see cref="List{T}()"/> for details.</remarks>

		public CommentCollection() 
:base()		{
		//empty
		}

		#endregion
		#region CommentCollection(Int32)

		/// <summary>
		/// Initializes a new instance of the <see cref="CommentCollection"/> class
		/// that is empty and has the specified initial capacity.
		/// </summary>
		/// <param name="capacity">The number of elements that the new
		/// <see cref="CommentCollection"/> is initially capable of storing.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="capacity"/> is less than zero.</exception>
		/// <remarks>Please refer to <see cref="List{T}(Int32)"/> for details.</remarks>

		public CommentCollection(int capacity) 
		:base(capacity){
//empty
        }

		#endregion
		#region CommentCollection(CommentCollection)

		/// <summary>
		/// Initializes a new instance of the <see cref="CommentCollection"/> class
		/// that contains elements copied from the specified collection and
		/// that has the same initial capacity as the number of elements copied.
		/// </summary>
		/// <param name="collection">The <see cref="CommentCollection"/>
		/// whose elements are copied to the new collection.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="collection"/> is a null reference.</exception>
        /// <remarks>Please refer to <see cref="List{T}(IEnumerable{T})"/> for details.</remarks>

		public CommentCollection(IEnumerable<Comment> collection) 
		:base(collection){
//empty
        }

		#endregion
		
		#endregion
		
		#region IsUnique

		/// <summary>
		/// Gets a value indicating whether the <see cref="CommentCollection"/> 
		/// ensures that all elements are unique.
		/// </summary>
		/// <value>
		/// <c>true</c> if the <see cref="CommentCollection"/> ensures that all 
		/// elements are unique; otherwise, <c>false</c>. The default is <c>false</c>.
		/// </value>
		public virtual bool IsUnique 
		{
			get { return false; }
		}

		#endregion
		

		/// <summary>
		/// Gets the <see cref="Comment"/> with the specified comment id.
		/// </summary>
		/// <param name="commentId">The id of the <see cref="Comment" /> to get.</param>
		/// <value>
		/// The <see cref="Comment"/> element with the specified <paramref name="commentId"/>.
		/// </value>
		public virtual Comment this[ string commentId ] 
		{
			get 
			{

				return this.Find( delegate(Comment comment){
                    return String.Compare(comment.EntryId, commentId, StringComparison.InvariantCultureIgnoreCase)==0;
                });
			}
		}



		#region Public Methods

		#region Clone

		/// <summary>
		/// Creates a shallow copy of the <see cref="CommentCollection"/>.
		/// </summary>
		/// <returns>A shallow copy of the <see cref="CommentCollection"/>.</returns>
		/// <remarks>Please refer to <see cref="ICloneable.Clone"/> for details.</remarks>

		public virtual object Clone() 
		{
			CommentCollection collection = new CommentCollection(this.ToArray());

            return collection;
		}

		#endregion

		#endregion



        #region unused classes
/*
        #region Class ReadOnlyList

        [Serializable]
			private sealed class ReadOnlyList: CommentCollection 
		{
			#region Private Fields

			private CommentCollection _collection;

			#endregion
			#region Internal Constructors

			internal ReadOnlyList(CommentCollection collection):
				base(Tag.Default) 
			{
				this._collection = collection;
			}

			#endregion
			#region Protected Properties

			protected override Comment[] InnerArray 
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
						  "Read-only collections cannot be modified."); }
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

			public override Comment this[int index] 
			{
				get { return this._collection[index]; }
				set 
				{
					throw new NotSupportedException(
						  "Read-only collections cannot be modified."); }
			}

			public override object SyncRoot 
			{
				get { return this._collection.SyncRoot; }
			}

			#endregion
			#region Public Methods

			public override void Add(Comment value) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

			public override void AddRange(CommentCollection collection) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

			public override void AddRange(Comment[] array) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

			public override int BinarySearch(Comment value) 
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
				return new ReadOnlyList((CommentCollection) this._collection.Clone());
			}

			public override void CopyTo(Comment[] array) 
			{
				this._collection.CopyTo(array);
			}

			public override void CopyTo(Comment[] array, int arrayIndex) 
			{
				this._collection.CopyTo(array, arrayIndex);
			}

			public override IEnumerator<Comment> GetEnumerator() 
			{
				return this._collection.GetEnumerator();
			}

			public override int IndexOf(Comment value) 
			{
				return this._collection.IndexOf(value);
			}

			public override void Insert(int index, Comment value) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

			public override bool Remove(Comment value) 
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

            public override void Sort(IComparer<Comment> comparer) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

            public override void Sort(int index, int count, IComparer<Comment> comparer) 
			{
				throw new NotSupportedException(
					"Read-only collections cannot be modified.");
			}

			public override Comment[] ToArray() 
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
			private sealed class SyncList: CommentCollection 
		{
			#region Private Fields

			private CommentCollection _collection;
			private object _root;

			#endregion
			#region Internal Constructors

			internal SyncList(CommentCollection collection):
				base(Tag.Default) 
			{

				this._root = collection.SyncRoot;
				this._collection = collection;
			}

			#endregion
			#region Protected Properties

			protected override Comment[] InnerArray 
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

			public override Comment this[int index] 
			{
				get { lock (this._root) return this._collection[index]; }
				set { lock (this._root) this._collection[index] = value;  }
			}

			public override object SyncRoot 
			{
				get { return this._root; }
			}

			#endregion
			#region Public Methods

			public override void Add(Comment value) 
			{
                lock (this._root) { this._collection.Add(value); }
			}

			public override void AddRange(CommentCollection collection) 
			{
				lock (this._root) this._collection.AddRange(collection);
			}

			public override void AddRange(Comment[] array) 
			{
				lock (this._root) this._collection.AddRange(array);
			}

			public override int BinarySearch(Comment value) 
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
					return new SyncList((CommentCollection) this._collection.Clone());
			}

			public override void CopyTo(Comment[] array) 
			{
				lock (this._root) this._collection.CopyTo(array);
			}

			public override void CopyTo(Comment[] array, int arrayIndex) 
			{
				lock (this._root) this._collection.CopyTo(array, arrayIndex);
			}

			public override IEnumerator<Comment> GetEnumerator() 
			{
				lock (this._root) return this._collection.GetEnumerator();
			}

			public override int IndexOf(Comment value) 
			{
				lock (this._root) return this._collection.IndexOf(value);
			}

			public override void Insert(int index, Comment value) 
			{
				lock (this._root) this._collection.Insert(index, value);
			}

			public override bool Remove(Comment value) 
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

            public override void Sort(IComparer<Comment> comparer) 
			{
				lock (this._root) this._collection.Sort(comparer);
			}

            public override void Sort(int index, int count, IComparer<Comment> comparer) 
			{
				lock (this._root) this._collection.Sort(index, count, comparer);
			}

			public override Comment[] ToArray() 
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
			private sealed class UniqueList: CommentCollection 
		{
			#region Private Fields

			private CommentCollection _collection;

			#endregion
			#region Internal Constructors

			internal UniqueList(CommentCollection collection):
				base(Tag.Default) 
			{
				this._collection = collection;
			}

			#endregion
			#region Protected Properties

			protected override Comment[] InnerArray 
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

			public override Comment this[int index] 
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

			public override void Add(Comment value) 
			{
				CheckUnique(value);
				 this._collection.Add(value);
			}

			public override void AddRange(CommentCollection collection) 
			{
				foreach (Comment value in collection)
					CheckUnique(value);
            
				this._collection.AddRange(collection);
			}

			public override void AddRange(Comment[] array) 
			{
				foreach (Comment value in array)
					CheckUnique(value);
            
				this._collection.AddRange(array);
			}

			public override int BinarySearch(Comment value) 
			{
				return this._collection.BinarySearch(value);
			}

			public override void Clear() 
			{
				this._collection.Clear();
			}

			public override object Clone() 
			{
				return new UniqueList((CommentCollection) this._collection.Clone());
			}

			public override void CopyTo(Comment[] array) 
			{
				this._collection.CopyTo(array);
			}

			public override void CopyTo(Comment[] array, int arrayIndex) 
			{
				this._collection.CopyTo(array, arrayIndex);
			}

			public override IEnumerator<Comment> GetEnumerator() 
			{
				return this._collection.GetEnumerator();
			}

			public override int IndexOf(Comment value) 
			{
				return this._collection.IndexOf(value);
			}

			public override void Insert(int index, Comment value) 
			{
				CheckUnique(value);
				this._collection.Insert(index, value);
			}

			public override bool Remove(Comment value) 
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

            public override void Sort(IComparer<Comment> comparer) 
			{
				this._collection.Sort(comparer);
			}

            public override void Sort(int index, int count, IComparer<Comment> comparer) 
			{
				this._collection.Sort(index, count, comparer);
			}

			public override Comment[] ToArray() 
			{
				return this._collection.ToArray();
			}

			public override void TrimToSize() 
			{
				this._collection.TrimToSize();
			}

			#endregion
			#region Private Methods

			private void CheckUnique(Comment value) 
			{
				if (IndexOf(value) >= 0)
					throw new NotSupportedException(
						"Unique collections cannot contain duplicate elements.");
			}

			private void CheckUnique(int index, Comment value) 
			{
				int existing = IndexOf(value);
				if (existing >= 0 && existing != index)
					throw new NotSupportedException(
						"Unique collections cannot contain duplicate elements.");
			}

			#endregion
		}

		#endregion */
    }
        #endregion
    #endregion
}
