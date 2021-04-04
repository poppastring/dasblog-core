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

using NodaTime;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
	[Serializable]
	[XmlRoot(Namespace=Data.NamespaceURI)]
	[XmlType(Namespace=Data.NamespaceURI)]
	public class Entry : EntryBase, IComparable, ITitledEntry
	{
		bool _allowComments = true;
		AttachmentCollection _attachments = new AttachmentCollection();
		string _author;
		string _categories;
		string _compressedTitle;
		CrosspostCollection _crossposts = new CrosspostCollection();
		string _description;
		bool _isPublic = true;
		Nullable<double> _latitude = new Nullable<double>();
		string _link; // A hyperlink that the entry is discussing or referring.
		Nullable<double> _longitude = new Nullable<double>();
		bool _showOnFrontPage = true;
		bool _syndicated = true;
		string _title;

		[XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;

		[XmlAnyElement]
		public XmlElement[] anyElements;

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string Title
		{
			get
			{
				if (_title == null || _title.Length == 0)
				{
					//Some imported/corrupted posts may not have a title...glean one in this last ditch effort
					_title = ClipString(Content, 128);
					_compressedTitle = InternalCompressTitle(_title);
					_title = _compressedTitle.Replace("+", " ");

					if (_compressedTitle == null || _compressedTitle.Length == 0)
					{
						_title = "No Title";
					}
				}
				return _title;
			}
			set
			{
				_title = value;
				_compressedTitle = InternalCompressTitle(_title);
			}
		}

		[XmlIgnore]
		public string CompressedTitle
		{
			get { return _compressedTitle; }
		}

		[XmlIgnore]
		public string CompressedTitleUnique
		{
			get
			{
				string date = CreatedUtc.ToString("yyyy/MM/dd/", CultureInfo.InvariantCulture);
				return date + _compressedTitle;
			}
		}

		public string Link
		{
			get { return _link; }
			set { _link = value; }
		}

		public string Categories
		{
			get { return _categories; }
			set { _categories = string.IsNullOrWhiteSpace(value) ? string.Empty : value.TrimEnd(';'); }
		}

		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public bool IsPublic
		{
			get { return _isPublic; }
			set { _isPublic = value; }
		}

		public bool Syndicated
		{
			get { return _syndicated; }
			set { _syndicated = value; }
		}

		public bool ShowOnFrontPage
		{
			get { return _showOnFrontPage; }
			set { _showOnFrontPage = value; }
		}

		public bool AllowComments
		{
			get { return _allowComments; }
			set { _allowComments = value; }
		}

		[XmlIgnore]
		public AttachmentCollection Attachments
		{
			get { return _attachments; }
		}

		[XmlArray("Attachments")]
		public Attachment[] AttachmentsArray
		{
			get { return new List<Attachment>(_attachments).ToArray(); }
			set
			{
				if (value == null)
				{
					_attachments = new AttachmentCollection();
				}
				else
				{
					_attachments = new AttachmentCollection(value);
				}
			}
		}

		[XmlIgnore]
		public Attachment Enclosure
		{
			get
			{
				foreach (Attachment attachment in _attachments)
				{
					if (attachment.AttachmentType == AttachmentType.Enclosure)
					{
						return attachment;
					}
				}

				return null;
			}
		}

		[XmlIgnore]
		public CrosspostCollection Crossposts
		{
			get { return _crossposts; }
		}

		[XmlArray("Crossposts")]
		public Crosspost[] CrosspostArray
		{
			get { return new List<Crosspost>(_crossposts).ToArray(); }
			set
			{
				if (value == null)
				{
					_crossposts = new CrosspostCollection();
				}
				else
				{
					_crossposts = new CrosspostCollection(value);
				}
			}
		}

		public Nullable<double> Latitude
		{
			get { return _latitude; }
			set { _latitude = value; }
		}

		public Nullable<double> Longitude
		{
			get { return _longitude; }
			set { _longitude = value; }
		}

		#region IComparable Members
		/// <summary>
		/// Returns true of all the simple properties are equal.
		/// </summary>
		/// <param name="entry">The <see cref="Entry"/>to compare to</param>
		/// <returns>0 if the entries contain equal simple properties</returns>
		/// <remarks>
		///		CompareTo will do a comparison on the following properties.
		///		<list type="bullet">
		///		<term><see cref="Entry.Title"/></term>
		///		<description>description</description>
		///		<item>
		///		<term><see cref="Entry.Description"/></term>
		///		</item>
		///		<item>
        ///		<term><see cref="EntryBase.Content"/></term>
		///		</item>
		///		<item>
		///		<term><see cref="Entry.Categories"/></term>
		///		</item>
		///		<item>
		///		<term><see cref="Entry.Author"/></term>
		///		</item>
		///		<item>
		///		<term><see cref="Entry.Link"/></term>
		///		</item>
		///		<item>
		///		<term><see cref="Entry.Attachments"/></term>
		///		</item>
		///		</list>
		/// </remarks>
		public int CompareTo(object entry)
		{
			bool different = false;
			if (entry is Entry)
			{
				Entry entry2 = entry as Entry;
				// we will only change the mod date if there has been a change to a few things
				if (Title != entry2.Title ||
				    Description != entry2.Description ||
				    Content != entry2.Content ||
				    Categories != entry2.Categories ||
				    Author != entry2.Author ||
				    Link != entry2.Link ||
				    Attachments.Count != entry2.Attachments.Count)
				{
					different = true;
				}
				else if (Attachments.Count == entry2.Attachments.Count)
				{
					// we need to also check the Attachments in calse they are modified
					for (int i = 0; i < Attachments.Count; i++)
					{
						if (Attachments[i].Url != entry2.Attachments[i].Url)
						{
							different = true;
						}
					}
				}

				return Convert.ToInt32(different);
			}
			else
			{
				throw new ArgumentException("entry is not an Entry");
			}
		}
		#endregion

		static string ClipString(string text, int length)
		{
			if (text != null && text.Length > 0)
			{
				if (text.Length > length)
				{
					text = text.Substring(0, length);
				}
			}
			else
			{
				text = "";
			}
			return text;
		}

		public static string InternalCompressTitle(string titleParam)
		{
			if (titleParam == null || titleParam.Length == 0)
			{
				return String.Empty;
			}

			StringBuilder retVal = new StringBuilder(titleParam.Length);

			bool pendingSpace = false;
			bool tag = false;

			for (int i = 0; i < titleParam.Length; ++i)
			{
				char c = titleParam[i];

				if (tag)
				{
					if (c == '>')
					{
						tag = false;
					}
				}
				else if (c == '<')
				{
					// Discard everything between '<' and '>', inclusive.
					// If no '>', just discard the '<'.
					tag = (titleParam.IndexOf('>', i) >= 0);
				}

					// Per RFC 2396 (URI syntax):
					//  delims   = "<" | ">" | "#" | "%" | <">
					//  reserved = ";" | "/" | "?" | ":" | "@" | "&" | "=" | "+" | "$" | ","
					// These characters should not appear in a URL
				else if ("#%\";/?:@&=$,".IndexOf(c) >= 0)
				{
					continue;
				}

				else if (char.IsWhiteSpace(c))
				{
					pendingSpace = true;
				}

					// The marks may appear in URLs
					//  mark = "-" | "_" | "." | "!" | "~" | "*" | "'" | "(" | ")"
					// as may all alphanumerics. (Tilde gets mangled by UrlEncode).
					// Here we are more lenient than RFC 2396, as we allow
					// all Unicode alphanumerics, not just the US-ASCII ones.
					// SDH: Update: it's really safer and maintains more permalinks if we stick with A-Za-z0-9.
				else if (char.IsLetterOrDigit(c) /* ||  "-_.!~*'()".IndexOf(c) >= 0 */)
				{
					if (pendingSpace)
					{
						// Discard leading spaces
						if (retVal.Length > 0)
						{
							// The caller will strip '+' if !siteConfig.EnableTitlePermaLinkSpaces
							retVal.Append("+");
						}

						pendingSpace = false;
					}

					retVal.Append(c);
				}
			}

			return WebUtility.UrlEncode(retVal.ToString()).Replace("%2b", "+", StringComparison.OrdinalIgnoreCase);
		}

		public static string InternalCompressTitle(string titleParam, string replacement)
		{
			return InternalCompressTitle(titleParam).Replace("+", replacement);
		}

		public string[] GetSplitCategories()
		{
			if (Categories == null || Categories.Length == 0)
			{
				return new string[0];
			}
			else
			{
				return Categories.Split(';');
			}
		}

		/// <summary>
		/// Returns a copy of the current entry
		/// </summary>
		/// <returns></returns>
		public Entry Clone()
		{
			Entry clone; //Not sure why the previous code created a new blank object just to overwrite it
			XmlSerializer ser = new XmlSerializer(typeof(Entry), Data.NamespaceURI);
			using (MemoryStream ms = new MemoryStream())
			{
				ser.Serialize(ms, this);
				ms.Position = 0;
				clone = ser.Deserialize(ms) as Entry;
			}
			return clone;
		}

		/// <summary>
		/// Delegate callback used to filter items that are not public.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public static bool IsPublicEntry(Entry entry)
		{
			return entry.IsPublic;
		}

		/// <summary>
		/// Delegate callback used to filter itmes that are not front page items.
		/// </summary>
		/// <param name="entry"></param>
		/// <returns></returns>
		public static bool ShowOnFrontPageEntry(Entry entry)
		{
			return entry.ShowOnFrontPage;
		}

		/// <summary>
		/// Returns true if the entry is in the categoryName specified.
		/// </summary>
		/// <param name="entry">The entry to check for category</param>
		/// <param name="categoryName">
		/// The name of the category to check.  If categoryName is null or empty check
		/// whether the entry has no categories,
		/// </param>
		/// <returns>
		/// A value of true indicates the entry is in the category specified or, if the categoryName
		/// is null or empty, return true if the entry is assigned no categories
		/// </returns>
		public static bool IsInCategory(Entry entry, string categoryName)
		{
			bool categoryFound = false;

			if (String.IsNullOrEmpty(categoryName))
			{
				categoryFound = String.IsNullOrEmpty(entry.Categories);
			}
			else
			{
				foreach (string entryCategory in entry.GetSplitCategories())
				{
					if (String.Equals(entryCategory, categoryName, StringComparison.InvariantCultureIgnoreCase))
					{
						categoryFound = true;
						break;
					}
				}
			}

			return categoryFound;
		}

		/// <summary>
		/// Returns true if the entry is in the categoryName specified.
		/// </summary>
		/// <param name="entry">The entry to check for category</param>
		/// <param name="timeZone">The time zone.</param>
		/// <param name="dateTime">The latest date and time the entry can occur.</param>
		/// <returns>
		/// A value of true indicates the entry is in the category specified or, if the categoryName
		/// is null or empty, return true if the entry is assigned no categories
		/// </returns>
		public static bool OccursBefore(Entry entry, DateTimeZone timeZone, DateTime dateTime)
		{
			var entryCreated = timeZone.AtStrictly(LocalDateTime.FromDateTime(entry.CreatedUtc)).LocalDateTime;
			var date = LocalDateTime.FromDateTime(dateTime);

			return (entryCreated <= date);
		}

		public static bool OccursBetween(Entry entry, DateTimeZone timeZone, 
												DateTime startDateTime, DateTime endDateTime)
		{
			var entryCreated = timeZone.AtStrictly(LocalDateTime.FromDateTime(entry.CreatedUtc)).LocalDateTime;
			var strartDate = LocalDateTime.FromDateTime(startDateTime);
			var endDate = LocalDateTime.FromDateTime(endDateTime);

			return ((entryCreated >= strartDate) && (entryCreated <= endDate));
		}

		/// <summary>
		/// Returns true if the specified Entry is within the same month as <c>month</c>;
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="timeZone"></param>
		/// <param name="month"></param>
		/// <returns></returns>
		public static bool OccursInMonth(Entry entry, DateTimeZone timeZone,
		                                 DateTime month)
		{
			DateTime startOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0);
			DateTime endOfMonth = new DateTime(month.Year, month.Month, 1, 0, 0, 0);
			endOfMonth = endOfMonth.AddMonths(1);
			endOfMonth = endOfMonth.AddSeconds(-1);

			return (OccursBetween(entry, timeZone, startOfMonth, endOfMonth));
		}

		/// <summary>
		/// Returns true if the specified entry has is an accepted language.
		/// </summary>
		/// <param name="entry">The entry whose Language property is checked.</param>
		/// <param name="acceptLanguages">A comma separated list of accepted languages.</param>
		/// <returns>
		/// Returns true if the entry's language is in the acceptedLanguages list or 
		/// else acceptedLanguages is null/empty and the entry's language is null/empty.
		/// </returns>
		public static bool IsInAcceptedLanguages(Entry entry, string acceptLanguages)
		{
			bool languageFound = false;

			// If acceptLanguages is empty or null then check to see
			// whether the entry has no languages.
			if (String.IsNullOrEmpty(acceptLanguages))
			{
				languageFound = String.IsNullOrEmpty(entry.Language);
			}
			else
			{
				foreach (string eachLanguageElement in acceptLanguages.Split(','))
				{
					string language = eachLanguageElement.Split(';')[0];
					if (language.ToUpper().StartsWith(entry.Language.ToUpper()))
					{
						languageFound = true;
						break;
					}
				}
			}

			return languageFound;
		}
	}
}
