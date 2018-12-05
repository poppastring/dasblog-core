#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

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
