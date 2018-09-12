using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace DasBlog.Core.Common.Comments
{
	/// <summary>
	/// Represents a matched tag. 
	/// </summary>
	public class MatchedTag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MatchedTag"/> class.
		/// </summary>
		/// <param name="m">The match.</param>
		/// <param name="isValid">A valid tag if set to <see langword="true"/>.</param>
		public MatchedTag(Match m, bool isValid) : this(m, isValid, false)
		{
			// ...
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MatchedTag"/> class.
		/// </summary>
		/// <param name="m">The match.</param>
		/// <param name="isValid">A valid tag if set to <see langword="true"/>.</param>
		/// <param name="needsClosing">The tag needs to be self-closed before rendering.</param>
		public MatchedTag(Match m, bool isValid, bool needsClosing)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			this.match = m;
			this.isValid = isValid;
			this.needsClosing = needsClosing;
		}

		/// <summary>
		/// Filters the attributes in the current tag.
		/// </summary>
		/// <param name="validTag">The valid tag, which specifies which attributes are valid.</param>
		public void FilterAttributes(ValidTag validTag)
		{
			// nothing to check against, so no valid attributes
			if (validTag == null)
			{
				this.attributes = null;
				return;
			}

			CaptureCollection atts = match.Groups["attName"].Captures;
			CaptureCollection vals = match.Groups["attVal"].Captures;
			CaptureCollection attVal = match.Groups["attNameValue"].Captures;

			StringBuilder sb = new StringBuilder();

			for (int i = 0, j = 0; i < atts.Count && j < vals.Count; i++)
			{
				string attName = atts[i].Value.Trim();
				string attValue = vals[j].Value.Trim();

				if (validTag.IsValidAttribute(attName))
				{
					// should be a complete att.
					if (attName == attVal[i].Value.Trim())
					{
						continue;
					}
					sb.AppendFormat("{0}=\"{1}\" ", attName, attValue);
				}
				j++;
			}

			attributes = sb.ToString().Trim();
		}


		/// <summary>
		/// Gets the length of the captured tag.
		/// </summary>
		/// <value>The length of the captured tag.</value>
		public int Length
		{
			[DebuggerStepThrough]
			get
			{
				return match.Length;
			}
		}


		/// <summary>
		/// Gets the position in the string where the tag was found.
		/// </summary>
		/// <value>The position in the string where the tag was found.</value>
		public int Index
		{
			[DebuggerStepThrough]
			get
			{
				return match.Index;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current matched tag is valid.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if the current matched tag is valid; otherwise, <see langword="false"/>.
		/// </value>
		public bool IsValid
		{
			[DebuggerStepThrough]
			get
			{
				return this.isValid;
			}
		}


		/// <summary>
		/// Gets the cleaned value of the captured tag.
		/// </summary>
		/// <value>The cleaned value of the captured tag.</value>
		public string FilteredValue
		{
			get
			{
				if (this.isValid)
				{
					if (this.EndTag)
					{
						return String.Format("</{0}>", this.TagName);
					}

					if (this.needsClosing || this.SelfClosing)
					{
						return String.Format("<{0} {1}{2}/>", this.TagName, attributes, (attributes != null && attributes.Length != 0 ? " " : ""));
					}

					return String.Format("<{0}{1}{2}>", this.TagName, (attributes != null && attributes.Length != 0 ? " " : ""), attributes);
				}

				return WebUtility.HtmlEncode(match.Value);
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current captured tag.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current captured tag.
		/// </returns>
		public override string ToString()
		{
			return this.FilteredValue;
		}


		/// <summary>
		/// Gets the name of the tag.
		/// </summary>
		/// <value>The name of the tag.</value>
		public string TagName
		{
			[DebuggerStepThrough]
			get
			{
				return match.Groups["name"].Value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the tag is self closing.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if self closing; otherwise, <see langword="false"/>.
		/// </value>
		public bool SelfClosing
		{
			[DebuggerStepThrough]
			get
			{
				return (match.Groups["self"].Value.Length > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current tag is an end tag.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if it's an end tag; otherwise, <see langword="false"/>.
		/// </value>
		public bool EndTag
		{
			[DebuggerStepThrough]
			get
			{
				return (match.Groups["end"].Value.Length > 0);
			}
		}

		// FIELDS
		string attributes;
		bool isValid;
		bool needsClosing;
		Match match;
	}
}
