using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DasBlog.Core.Common.Comments
{
	public class Tag
	{
		private string[] attributes = new string[0];

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "attributes")]
		public string Attributes { get; set; }

		[XmlAttribute(AttributeName = "allowed")]
		public bool Allowed { get; set; }

		[XmlIgnore]
		public string AttributesArray
		{
			get
			{
				return string.Join(",", attributes);
			}

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

		public Tag()
		{
		}

		public Tag(string tagdefinition)
		{

			if (tagdefinition == null || tagdefinition.Length == 0)
			{
				throw new ArgumentNullException("tagDefinition");
			}

			// tags are defined as tag@att1@att2
			// so splitting on @ should give us what we need
			var splitDef = tagdefinition.Split('@');

			// first item is the name
			Name = splitDef[0];

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

		public bool IsValidAttribute(string attributeName)
		{
			return Array.IndexOf(attributes, attributeName) >= 0;
		}

		public override string ToString()
		{
			return Name + (attributes.Length > 0 ? "@" : "") + string.Join("@", attributes);
		}

	}
}
