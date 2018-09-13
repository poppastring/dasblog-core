using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Core.Common.Comments
{
	[XmlRoot("validTags", Namespace = "urn:newtelligence-com:dasblog:config")]
	[XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
	public class ValidTagCollection : ICollection
	{

		// required for xml serializer
		public ValidTagCollection()
		{
			// ...
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidTagCollection"/> class.
		/// </summary>
		/// <param name="tagsDefinition">The tags definition, defined as 'tag1@att1@att2,tag2@att3@att4'.</param>
		public ValidTagCollection(string tagsDefinition)
		{
			if (tagsDefinition == null || tagsDefinition.Length == 0)
			{
				return;
			}

			this.tagsDefinition = tagsDefinition;

			string[] splitTags = tagsDefinition.Split(',');

			foreach (string tagDef in splitTags)
			{
				ValidTag tag = new ValidTag(tagDef);
				tags.Add(tag.Name, tag);
			}
		}


		public void Add(ValidTag tag)
		{

			if (tag == null)
			{
				throw new ArgumentNullException("tag");
			}

			tags.Add(tag.Name, tag);
		}

		/// <summary>
		/// Copies the ValidTags to the array, starting at the index.
		/// </summary>
		/// <param name="array">The array to copy the tags to.</param>
		/// <param name="index">The index to start at.</param>
		void ICollection.CopyTo(Array array, int index)
		{

			if (!(array is ValidTag[]))
			{
				throw new ArgumentException("Expected array of ValidTags", "array");
			}

			tags.Values.CopyTo((ValidTag[])array, index);
		}

		/// <summary>
		/// Gets the enumerator over the tags.
		/// </summary>
		/// <returns>The enumerator.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{

			// implement a ValidTagEnumerator when we actually need one
			return tags.Values.GetEnumerator();
		}

		/// <summary>
		/// Determines if the tagName is valid.
		/// </summary>
		/// <param name="tagName">The tag to validate.</param>
		/// <returns><see langword="true" /> if the tagName is valid; otherwise, <see langword="false" />.</returns>
		public bool IsValidTag(string tagName)
		{
			if (tags.ContainsKey(tagName) == false) return false;
			return tags[tagName] != null && ((ValidTag)tags[tagName]).IsAllowed;
		}

		/// <summary>
		/// Gets the number of tags in the collection.
		/// </summary>
		/// <value>The number of tags in the collection.</value>
		[XmlIgnore]
		public int Count
		{
			[DebuggerStepThrough]
			get
			{
				return tags.Count;
			}
		}



		/// <summary>
		/// Gets the number of allowed tags.
		/// </summary>
		/// <value>The allowed tags count.</value>
		public int AllowedTagsCount
		{
			get
			{
				int count = 0;
				foreach (ValidTag tag in this)
				{
					if (tag.IsAllowed)
					{
						count++;
					}
				}
				return count;
			}
		}

		/// <summary>
		/// Gets a value
		/// indicating whether access to the <see cref="T:System.Collections.ICollection"/> is synchronized
		/// (thread-safe).
		/// </summary>
		/// <value>Always returns <see langword="false" />.</value>
		[XmlIgnore]
		bool ICollection.IsSynchronized
		{
			[DebuggerStepThrough]
			get
			{
				return false;
			}
		}


		/// <summary>
		/// Gets an object that
		/// can be used to synchronize access to the <see cref="T:System.Collections.ICollection"/>.
		/// </summary>
		/// <value>An object to synchronize access to the collection.</value>
		[XmlIgnore]
		object ICollection.SyncRoot
		{
			[DebuggerStepThrough]
			get
			{
				return ((ICollection)tags).SyncRoot;
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="ValidTag"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="ValidTag"/>.
		/// </returns>
		public override string ToString()
		{

			StringBuilder sb = new StringBuilder();

			foreach (ValidTag tag in this)
			{
				if (tag.IsAllowed)
				{
					sb.Append(tag.ToString() + ", ");
				}
			}

			if (sb.Length > 0)
			{
				// remove trailing ", "
				sb.Remove(sb.Length - 2, 2);
			}

			return sb.ToString();
		}

		public string ToJavaScriptArray()
		{
			StringBuilder sb = new StringBuilder();

			foreach (ValidTag tag in this)
			{
				if (tag.IsAllowed)
				{
					sb.Append("'" + tag.Name + "', ");
				}
			}

			if (sb.Length > 0)
			{
				// remove trailing ", "
				sb.Remove(sb.Length - 2, 2);
			}

			return sb.ToString();
		}



		/// <summary>
		/// Gets the tags definition.
		/// </summary>
		/// <value>The tags definition.</value>
		[XmlIgnore]
		public string TagsDefinition
		{
			[DebuggerStepThrough]
			get
			{
				return this.tagsDefinition;
			}
		}

		/// <summary>
		/// Gets the <see cref="ValidTag"/> with the specified tag name.
		/// </summary>
		/// <value>The <see cref="ValidTag"/>.</value>
		public ValidTag this[string tagName]
		{
			get
			{
				return (ValidTag)tags[tagName];
			}
		}

		/// <summary>
		/// Gets the <see cref="ValidTag"/> at the specified index.
		/// </summary>
		/// <value>The <see cref="ValidTag"/>.</value>
		public ValidTag this[int index]
		{
			get
			{
				string key = tags.Keys[index];
				return tags[key];
			}
			set
			{
				string key = tags.Keys[index];
				tags[key] = value;
			}
		}

		// for versioning
		[XmlAnyElement]
		public XmlElement[] AnyElements;
		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttributes;

		// use invariant culture so case insensitive comparison 
		// behaves like we expect for tags
		private SortedList<string, ValidTag> tags = new SortedList<string, ValidTag>(StringComparer.InvariantCultureIgnoreCase);
		private string tagsDefinition;
	}
}
