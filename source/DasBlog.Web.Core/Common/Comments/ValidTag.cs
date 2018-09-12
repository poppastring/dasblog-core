using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Core.Common.Comments
{
	/// <summary>
	/// Represesents a tag, which can be used in the comments.
	/// </summary>
	[XmlRoot("tag", Namespace = "urn:newtelligence-com:dasblog:config")]
	[XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
	public class ValidTag
	{

		// required for xml Serializer
		public ValidTag()
		{
			//...
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidTag"/> class.
		/// </summary>
		/// <param name="tagDefinition">The tag definition, defined as 'tag@att1@att2'.</param>
		public ValidTag(string tagDefinition)
		{

			if (tagDefinition == null || tagDefinition.Length == 0)
			{
				throw new ArgumentNullException("tagDefinition");
			}

			// tags are defined as tag@att1@att2
			// so splitting on @ should give us what we need
			string[] splitDef = tagDefinition.Split('@');

			// first item is the name
			this.name = splitDef[0];

			// check for attributes and copy to collection
			if (splitDef.Length == 1)
			{
				attributes = new string[0];
			}
			else
			{
				attributes = new string[splitDef.Length - 1];
				Array.Copy(splitDef, 1, attributes, 0, attributes.Length);
				Array.Sort(attributes, StringComparer.InvariantCultureIgnoreCase);
			}
		}

		/// <summary>
		/// Determines whether attribute with the specified attribute name is valid.
		/// </summary>
		/// <param name="attributeName">Name of the attribute.</param>
		/// <returns>
		/// 	<see langword="true"/> if the attribute with the specified attribute name is valid; otherwise, <see langword="false"/>.
		/// </returns>
		public bool IsValidAttribute(string attributeName)
		{

			return Array.IndexOf(attributes, attributeName) >= 0;
		}

		public override string ToString()
		{
			return name + (attributes.Length > 0 ? "@" : "") + String.Join("@", attributes);
		}


		/// <summary>
		/// Gets the name of the tag.
		/// </summary>
		/// <value>The name of the tag.</value>
		[XmlAttribute("name", Namespace = "urn:newtelligence-com:dasblog:config")]
		public string Name
		{
			[DebuggerStepThrough]
			get
			{
				return this.name;
			}
			[DebuggerStepThrough]
			set
			{
				this.name = value;
			}
		}

		[XmlAttribute("attributes", Namespace = "urn:newtelligence-com:dasblog:config")]
		public string Attributes
		{
			[DebuggerStepThrough]
			get
			{
				return String.Join(",", attributes);
			}
			[DebuggerStepThrough]
			set
			{
				if (value == null || value.Length == 0)
				{
					attributes = new string[0];
				}
				else
				{
					attributes = value.Split(',');
				}
			}
		}

		[XmlAttribute("allowed", Namespace = "urn:newtelligence-com:dasblog:config")]
		public bool IsAllowed
		{
			[DebuggerStepThrough]
			get
			{
				return this.allowed;
			}
			[DebuggerStepThrough]
			set
			{
				this.allowed = value;
			}
		}

		// for versioning
		[XmlAnyElement]
		public XmlElement[] AnyElements;
		[XmlAnyAttribute]
		public XmlAttribute[] AnyAttributes;

		// FIELDS
		private bool allowed = true;
		private string name = "";
		private string[] attributes = new string[0];
	}
}
